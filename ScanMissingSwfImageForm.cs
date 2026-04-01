using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        private void btnChooseConverter_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Title = "Chọn SwfToJpgConverter.exe";
                dlg.Filter = "Executable|*.exe";
                if (dlg.ShowDialog() == DialogResult.OK)
                    txtConverterPath.Text = dlg.FileName;
            }
        }

        private void btnChooseFfdec_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Title = "Chọn ffdec-cli.exe";
                dlg.Filter = "Executable|*.exe";
                if (dlg.ShowDialog() == DialogResult.OK)
                    txtFfdecPath.Text = dlg.FileName;
            }
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            if (_isRunning) return;

            string sourceFolder = txtSourceFolder.Text.Trim();
            string converterPath = txtConverterPath.Text.Trim();
            string ffdecPath = txtFfdecPath.Text.Trim();

            if (string.IsNullOrEmpty(sourceFolder) || !Directory.Exists(sourceFolder))
            {
                MessageBox.Show("Vui lòng chọn thư mục nguồn hợp lệ.", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
            }
            if (string.IsNullOrEmpty(converterPath) || !File.Exists(converterPath))
            {
                MessageBox.Show("Vui lòng chọn đường dẫn SwfToJpgConverter.exe hợp lệ.", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
            }
            if (string.IsNullOrEmpty(ffdecPath) || !File.Exists(ffdecPath))
            {
                MessageBox.Show("Vui lòng chọn đường dẫn ffdec-cli.exe hợp lệ.", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
            }

            string connStr = GetConnStr();
            if (connStr == null) return;

            SetRunningState(true);
            rtbLog.Clear();
            _cts = new CancellationTokenSource();

            try
            {
                await Task.Run(() => RunScanAndConvert(sourceFolder, converterPath, ffdecPath, connStr, _cts.Token));
            }
            catch (OperationCanceledException) { AppendLog("⚠ Đã dừng theo yêu cầu.", Color.Yellow); }
            catch (Exception ex) { AppendLog($"❌ Lỗi: {ex.Message}", Color.Red); }
            finally { SetRunningState(false); }
        }

        private void btnStop_Click(object sender, EventArgs e) => _cts?.Cancel();
        private void btnClearLog_Click(object sender, EventArgs e) => rtbLog.Clear();

        // ──────────────────────────────────────────────────────────────
        // LOGIC CHÍNH — QUÉT + CONVERT TỪNG FILE + LƯU DB
        // ──────────────────────────────────────────────────────────────

        private static string GetPagesSuffix_Old(string dirPath)
        {
            if (dirPath.EndsWith(@"\files\assets\pages", StringComparison.OrdinalIgnoreCase))
                return @"\files\assets\pages";
            if (dirPath.EndsWith(@"\files\pages", StringComparison.OrdinalIgnoreCase))
                return @"\files\pages";
            if (dirPath.EndsWith(@"\pages", StringComparison.OrdinalIgnoreCase))
                return @"\pages";
            return null;
        }

        private static string GetPagesSuffix(string dirPath)
        {
            if (dirPath.EndsWith(@"\files\assets\flash\pages", StringComparison.OrdinalIgnoreCase))
                return @"\files\assets\flash\pages";
            if (dirPath.EndsWith(@"\files\assets\pages", StringComparison.OrdinalIgnoreCase))
                return @"\files\assets\pages";
            if (dirPath.EndsWith(@"\files\pages", StringComparison.OrdinalIgnoreCase))
                return @"\files\pages";
            if (dirPath.EndsWith(@"\pages", StringComparison.OrdinalIgnoreCase))
                return @"\pages";
            return null;
        }

        private void RunScanAndConvert(string sourceFolder, string converterPath,
                                       string ffdecPath, string connStr, CancellationToken token)
        {
            AppendLog($"🔍 Bắt đầu quét từ: {sourceFolder}");
            AppendLog(new string('=', 90));

            // ── Phase 1: Thu thập SWF thiếu JPG ──────────────────────
            var allSwfFiles = Directory.GetFiles(sourceFolder, "page*.swf", SearchOption.AllDirectories);
            AppendLog($"📄 Tổng số file page*.swf tìm thấy: {allSwfFiles.Length}");
            AppendLog(new string('-', 90));

            // Danh sách SWF thiếu JPG: (swfPath, pagesFolder, pagesSuffix)
            var missingList = new List<(string SwfPath, string PagesFolder, string PagesSuffix)>();

            int totalScanned = 0, totalHasImage = 0;

            foreach (var swfPath in allSwfFiles)
            {
                token.ThrowIfCancellationRequested();

                string parentDir = Path.GetDirectoryName(swfPath) ?? "";
                string pagesSuffix = GetPagesSuffix(parentDir);
                if (pagesSuffix == null) continue;

                totalScanned++;

                string baseName = Path.GetFileNameWithoutExtension(swfPath);
                string mediumJpg = Path.Combine(parentDir, baseName + ".jpg");

                if (File.Exists(mediumJpg))
                {
                    totalHasImage++;
                    AppendLog($"Có ảnh: {totalHasImage} ", Color.Green);
                    continue; // Đã có JPG → bỏ qua hoàn toàn, không convert, không lưu DB
                }

                missingList.Add((swfPath, parentDir, pagesSuffix));
            }

            AppendLog($"✅ Quét xong | Tổng SWF: {totalScanned} | " +
                      $"Có ảnh: {totalHasImage} | Thiếu JPG: {missingList.Count}");
            AppendLog(new string('-', 90));

            if (missingList.Count == 0)
            {
                AppendLog("🎉 Tất cả SWF đã có ảnh JPG tương ứng!", Color.Cyan);
                SetStatus("Hoàn thành. Không có SWF nào thiếu ảnh.");
                return;
            }

            // ── Phase 2: Convert từng file SWF thiếu + lưu DB ────────
            AppendLog($"🖼 Bắt đầu convert {missingList.Count} file SWF thiếu ảnh...");
            AppendLog(new string('-', 90));

            int processed = 0, fileOk = 0, fileFail = 0;

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();

                foreach (var (swfPath, pagesFolder, pagesSuffix) in missingList)
                {
                    token.ThrowIfCancellationRequested();
                    processed++;

                    // Lấy Id_ILib từ docRoot
                    string docRoot = pagesFolder.Substring(0, pagesFolder.Length - pagesSuffix.Length);
                    string idILib = QueryIdILib(conn, docRoot);

                    // Fallback: thử lên 1 cấp cha (trường hợp folder bị lồng thêm 1 cấp như \Buituananh45\Buituananh45)
                    if (idILib == null)
                    {
                        string parentDocRoot = Path.GetDirectoryName(docRoot);
                        if (!string.IsNullOrEmpty(parentDocRoot))
                            idILib = QueryIdILib(conn, parentDocRoot);
                    }

                    if (idILib == null)
                    {
                        AppendLog($"⚠ Không tìm được Id_ILib | [{Path.GetFileName(swfPath)}] | {docRoot}",
                                  Color.Yellow);
                        InsertRecord(conn, "UNKNOWN", swfPath, -1);
                        fileFail++;
                        continue;
                    }

                    // Gọi converter cho đúng 1 file SWF này:
                    // -i <swfPath> -o <pagesFolder> -f <ffdec> --overwrite
                    bool converterOk = RunConverterForFile(converterPath, swfPath, pagesFolder, ffdecPath);

                    // Kiểm tra JPG thực sự được tạo ra không
                    string baseName = Path.GetFileNameWithoutExtension(swfPath);
                    string outputJpg = Path.Combine(pagesFolder, baseName + ".jpg");

                    if (converterOk && File.Exists(outputJpg))
                    {
                        InsertRecord(conn, idILib, swfPath, 1); // Status = 1 Done
                        fileOk++;
                        AppendLog($"[{processed}/{missingList.Count}] ✔ {Path.GetFileName(swfPath)}",
                                  Color.LimeGreen);
                    }
                    else
                    {
                        InsertRecord(conn, idILib, swfPath, -1); // Status = -1 Error
                        fileFail++;
                        AppendLog($"[{processed}/{missingList.Count}] ❌ {Path.GetFileName(swfPath)} | {pagesFolder}",
                                  Color.Red);
                    }

                    if (processed % 20 == 0)
                        SetStatus($"Đang convert... {processed}/{missingList.Count} | OK: {fileOk} | Lỗi: {fileFail}");
                }
            }

            AppendLog(new string('=', 90));
            AppendLog($"✅ HOÀN THÀNH | Tổng xử lý: {processed} | Done: {fileOk} | Lỗi: {fileFail}",
                      Color.Cyan);
            SetStatus($"Xong. Done: {fileOk} | Lỗi: {fileFail}");
        }

        /// <summary>
        /// Gọi SwfToJpgConverter.exe cho đúng 1 file SWF:
        ///   -i "pageXXXX.swf"  -o "pagesFolder"  -f "ffdec"  --overwrite
        /// Trả về true nếu ExitCode = 0.
        /// </summary>
        private bool RunConverterForFile(string converterPath, string swfFilePath,
                                         string outputFolder, string ffdecPath)
        {
            try
            {
                // -i nhận file hoặc folder, --overwrite để ghi đè nếu tồn tại
                string args = $"-i \"{swfFilePath}\" -o \"{outputFolder}\" -f \"{ffdecPath}\" --overwrite";

                var psi = new ProcessStartInfo
                {
                    FileName = converterPath,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (var process = new Process { StartInfo = psi })
                {
                    // Chỉ log stderr (lỗi) để tránh log quá nhiều
                    process.ErrorDataReceived += (s, ev) =>
                    { if (!string.IsNullOrWhiteSpace(ev.Data)) AppendLog($"   ! {ev.Data}", Color.OrangeRed); };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();

                    return process.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                AppendLog($"   → Process error: {ex.Message}", Color.Red);
                return false;
            }
        }

        // ──────────────────────────────────────────────────────────────
        // DB HELPERS
        // ──────────────────────────────────────────────────────────────

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
                return cmd.ExecuteScalar()?.ToString();
            }
        }

        /// <summary>
        /// Insert record với Status chỉ định.
        /// Nếu đã tồn tại → UPDATE Status.
        /// Status: -1 Error | 1 Done
        /// </summary>
        private void InsertRecord(SqlConnection conn, string idILib, string filePath, short status)
        {
            const string sql = @"
                IF EXISTS (
                    SELECT 1 FROM dbo.MissingImgFromSwf
                    WHERE Id = @Id AND FilePath = @FilePath
                )
                    UPDATE dbo.MissingImgFromSwf
                    SET    Status = @Status
                    WHERE  Id = @Id AND FilePath = @FilePath
                ELSE
                    INSERT INTO dbo.MissingImgFromSwf (Id, FilePath, Status)
                    VALUES (@Id, @FilePath, @Status)";
            try
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", idILib);
                    cmd.Parameters.AddWithValue("@FilePath", filePath);
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                AppendLog($"   → DB Error: {ex.Message}", Color.Red);
            }
        }

        // ──────────────────────────────────────────────────────────────
        // HELPERS CHUNG
        // ──────────────────────────────────────────────────────────────

        private string GetConnStr()
        {
            string connStr = System.Configuration.ConfigurationManager
                                   .ConnectionStrings["MyDbConn"]?.ConnectionString;
            if (string.IsNullOrEmpty(connStr))
                MessageBox.Show("Không tìm thấy connection string 'MyDbConn' trong App.config.",
                    "Lỗi cấu hình", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return connStr;
        }

        private void AppendLog(string message, Color? color = null)
        {
            if (rtbLog.InvokeRequired) { rtbLog.Invoke(new Action(() => AppendLog(message, color))); return; }
            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.SelectionLength = 0;
            rtbLog.SelectionColor = color ?? Color.Lime;
            rtbLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n");
            rtbLog.ScrollToCaret();
        }

        private void SetStatus(string message)
        {
            if (lblStatus.InvokeRequired) { lblStatus.Invoke(new Action(() => SetStatus(message))); return; }
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

        private void btnCreateMets_Click(object sender, EventArgs e)
        {
            // Khởi tạo đối tượng Form mới
            FlippingBookToMetsForm scanForm = new FlippingBookToMetsForm();

            // CÁCH 1: Mở dạng cửa sổ độc lập (Người dùng vẫn có thể bấm vào Form cũ)
            scanForm.Show();

            // CÁCH 2: Mở dạng hội thoại (Bắt buộc xử lý xong Form này mới quay lại được Form cũ)
            // scanForm.ShowDialog(); 
        }
    }
}