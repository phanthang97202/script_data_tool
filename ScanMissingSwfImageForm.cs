using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ScriptDataTool
{
    public partial class ScanMissingSwfImageForm : Form
    {
        private CancellationTokenSource _cts;
        private bool _isRunning = false;

        public ScanMissingSwfImageForm()
        {
            InitializeComponent();
        }

        // ──────────────────────────────────────────────────────────────
        // SỰ KIỆN UI
        // ──────────────────────────────────────────────────────────────

        private void btnChooseFolder_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Chọn thư mục nguồn chứa dữ liệu Flipping Book đã giải nén";
                if (!string.IsNullOrEmpty(txtSourceFolder.Text))
                    dialog.SelectedPath = txtSourceFolder.Text;

                if (dialog.ShowDialog() == DialogResult.OK)
                    txtSourceFolder.Text = dialog.SelectedPath;
            }
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            if (_isRunning) return;

            string sourceFolder = txtSourceFolder.Text.Trim();
            if (string.IsNullOrEmpty(sourceFolder) || !Directory.Exists(sourceFolder))
            {
                MessageBox.Show("Vui lòng chọn thư mục nguồn hợp lệ.", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string connStr = System.Configuration.ConfigurationManager
                                   .ConnectionStrings["MyDbConn"]?.ConnectionString;
            if (string.IsNullOrEmpty(connStr))
            {
                MessageBox.Show("Không tìm thấy connection string 'DefaultConnection' trong App.config.",
                    "Lỗi cấu hình", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SetRunningState(true);
            rtbLog.Clear();
            _cts = new CancellationTokenSource();

            try
            {
                await Task.Run(() => RunScan(sourceFolder, connStr, _cts.Token));
            }
            catch (OperationCanceledException)
            {
                AppendLog("⚠ Đã dừng theo yêu cầu.", System.Drawing.Color.Yellow);
            }
            catch (Exception ex)
            {
                AppendLog($"❌ Lỗi không mong muốn: {ex.Message}", System.Drawing.Color.Red);
            }
            finally
            {
                SetRunningState(false);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _cts?.Cancel();
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            rtbLog.Clear();
        }

        // ──────────────────────────────────────────────────────────────
        // LOGIC CHÍNH
        // ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Xác định suffix của folder pages trong path.
        /// Flipping Book thực tế: \files\assets\pages
        /// Các biến thể khác:     \files\pages  hoặc  \pages
        /// </summary>
        private static string GetPagesSuffix(string dirPath)
        {
            if (dirPath.EndsWith(@"\files\assets\pages", StringComparison.OrdinalIgnoreCase))
                return @"\files\assets\pages";
            if (dirPath.EndsWith(@"\files\pages", StringComparison.OrdinalIgnoreCase))
                return @"\files\pages";
            if (dirPath.EndsWith(@"\pages", StringComparison.OrdinalIgnoreCase))
                return @"\pages";
            return null;
        }

        private void RunScan(string sourceFolder, string connStr, CancellationToken token)
        {
            AppendLog($"🔍 Bắt đầu quét từ: {sourceFolder}");
            AppendLog(new string('=', 90));

            var allSwfFiles = Directory.GetFiles(sourceFolder, "page*.swf", SearchOption.AllDirectories);
            AppendLog($"📄 Tổng số file page*.swf tìm thấy (toàn bộ): {allSwfFiles.Length}");
            AppendLog(new string('-', 90));

            int totalScanned = 0;
            int totalHasImage = 0;
            int totalMissing = 0;
            int totalInserted = 0;
            int totalError = 0;

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();

                foreach (var swfPath in allSwfFiles)
                {
                    token.ThrowIfCancellationRequested();

                    string parentDir = Path.GetDirectoryName(swfPath) ?? "";

                    // Xác định loại cấu trúc thư mục pages
                    string pagesSuffix = GetPagesSuffix(parentDir);
                    if (pagesSuffix == null)
                        continue;

                    totalScanned++;

                    // Kiểm tra file ảnh medium: pageXXXX.jpg (không có _l, _s)
                    string baseName = Path.GetFileNameWithoutExtension(swfPath);

                    if(parentDir.Contains("LuanVanBsckiiNguyenThiTuyet"))
                    {
                        Console.WriteLine("Debug");
                    }

                    string mediumJpg = Path.Combine(parentDir, baseName + ".jpg");

                    if (File.Exists(mediumJpg))
                    {
                        AppendLog($"=> CÓ ẢNH | {Path.GetFileName(swfPath)}", System.Drawing.Color.Green);
                        totalHasImage++;
                        continue;
                    }

                    // Thiếu ảnh medium
                    totalMissing++;

                    // docRoot = bỏ suffix \files\assets\pages
                    string docRoot = parentDir.Substring(0, parentDir.Length - pagesSuffix.Length);

                    string idILib = QueryIdILib(conn, docRoot);

                    if (idILib == null)
                    {
                        // Log chi tiết để dễ debug: in ra docRoot đang tìm
                        AppendLog($"⚠ Không tìm được Id_ILib | docRoot: [{docRoot}]", System.Drawing.Color.Yellow);
                        totalError++;
                        continue;
                    }

                    bool ok = InsertMissingRecord(conn, idILib, swfPath);
                    if (ok)
                    {
                        totalInserted++;
                        AppendLog(
                            $"✔ THIẾU ẢNH | Id: {idILib} | {Path.GetFileName(swfPath)} | {docRoot}",
                            System.Drawing.Color.Orange);
                    }
                    else
                    {
                        totalError++;
                    }

                    if (totalScanned % 100 == 0)
                        SetStatus($"Đang quét... {totalScanned} trang | Có ảnh: {totalHasImage} | Thiếu: {totalMissing}");
                }
            }

            AppendLog(new string('=', 90));
            AppendLog(
                $"✅ HOÀN THÀNH | Đã quét: {totalScanned} trang | Có ảnh: {totalHasImage} | Thiếu ảnh: {totalMissing} | Đã lưu DB: {totalInserted} | Lỗi: {totalError}",
                System.Drawing.Color.Cyan);
            SetStatus($"Hoàn thành. Có ảnh: {totalHasImage} | Thiếu ảnh: {totalMissing} | Đã lưu DB: {totalInserted} | Lỗi: {totalError}");
        }

        /// <summary>
        /// Tìm Id_ILib trong ScriptMappingData theo File_Path_KPS.
        /// Dùng LTRIM/RTRIM + so sánh cả 2 dạng (có và không có dấu \ cuối)
        /// để tránh lỗi do khoảng trắng hoặc dấu \ thừa trong DB.
        /// </summary>
        private string QueryIdILib(SqlConnection conn, string docRoot)
        {
            const string sql = @"
                SELECT TOP 1 Id_ILib
                FROM   dbo.ScriptMappingData
                WHERE  LTRIM(RTRIM(File_Path_KPS)) = @Path
                    OR LTRIM(RTRIM(File_Path_KPS)) = @PathSlash
                    OR LTRIM(RTRIM(File_Path_KPS)) = @PathLower
                    OR LTRIM(RTRIM(File_Path_KPS)) = @PathSlashLower";

            string clean = docRoot.TrimEnd('\\').Trim();
            string cleanSlash = clean + "\\";

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Path", clean);
                cmd.Parameters.AddWithValue("@PathSlash", cleanSlash);
                cmd.Parameters.AddWithValue("@PathLower", clean.ToLowerInvariant());
                cmd.Parameters.AddWithValue("@PathSlashLower", cleanSlash.ToLowerInvariant());
                var result = cmd.ExecuteScalar();
                return result?.ToString();
            }
        }

        /// <summary>
        /// Insert bản ghi vào MissingImgFromSwf. Bỏ qua nếu đã tồn tại.
        /// </summary>
        private bool InsertMissingRecord(SqlConnection conn, string idILib, string filePath)
        {
            const string sql = @"
                IF NOT EXISTS (
                    SELECT 1 FROM dbo.MissingImgFromSwf
                    WHERE Id = @Id AND FilePath = @FilePath
                )
                INSERT INTO dbo.MissingImgFromSwf (Id, FilePath)
                VALUES (@Id, @FilePath)";

            try
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", idILib);
                    cmd.Parameters.AddWithValue("@FilePath", filePath);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                AppendLog($"   → DB Error: {ex.Message}", System.Drawing.Color.Red);
                return false;
            }
        }

        // ──────────────────────────────────────────────────────────────
        // HELPERS — Thread-safe UI update
        // ──────────────────────────────────────────────────────────────

        private void AppendLog(string message, System.Drawing.Color? color = null)
        {
            if (rtbLog.InvokeRequired)
            {
                rtbLog.Invoke(new Action(() => AppendLog(message, color)));
                return;
            }
            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.SelectionLength = 0;
            rtbLog.SelectionColor = color ?? System.Drawing.Color.Lime;
            rtbLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n");
            rtbLog.ScrollToCaret();
        }

        private void SetStatus(string message)
        {
            if (lblStatus.InvokeRequired)
            {
                lblStatus.Invoke(new Action(() => SetStatus(message)));
                return;
            }
            lblStatus.Text = message;
        }

        private void SetRunningState(bool running)
        {
            if (InvokeRequired) { Invoke(new Action(() => SetRunningState(running))); return; }
            _isRunning = running;
            btnStart.Enabled = !running;
            btnStop.Enabled = running;
            btnChooseFolder.Enabled = !running;
            progressBar.Visible = running;
        }
    }
}