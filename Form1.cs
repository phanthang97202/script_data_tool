using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ScriptDataTool
{

    public partial class Form1 : Form
    {
        private bool _isRunning = false;
        private int _totalExtracted = 0;
        private int _totalErrors = 0;

        public Form1()
        {
            InitializeComponent();
        }

        // =====================================================================
        // CHỌN THƯ MỤC
        // =====================================================================
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Chọn thư mục gốc (sẽ tìm và giải nén tất cả ZIP/RAR, kể cả lồng nhau)";
                if (dlg.ShowDialog() == DialogResult.OK)
                    txtFolder.Text = dlg.SelectedPath;
            }
        }

        // =====================================================================
        // BẮT ĐẦU
        // =====================================================================
        private async void btnStart_Click(object sender, EventArgs e)
        {
            if (_isRunning) return;

            if (string.IsNullOrWhiteSpace(txtFolder.Text) || !Directory.Exists(txtFolder.Text))
            {
                MessageBox.Show("Vui lòng chọn thư mục hợp lệ.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _isRunning = true;
            _totalExtracted = 0;
            _totalErrors = 0;
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            progressBar.MarqueeAnimationSpeed = 30;
            txtLog.Clear();

            string rootFolder = txtFolder.Text;
            await Task.Run(() => ExtractAllInFolder(rootFolder, 0));

            _isRunning = false;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            progressBar.MarqueeAnimationSpeed = 0;
            UpdateProgress();
            AppendLog(new string('=', 60));
            AppendLog("HOÀN THÀNH — Đã giải nén: " + _totalExtracted + " file | Lỗi: " + _totalErrors);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _isRunning = false;
            AppendLog("[Người dùng dừng]");
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
        }

        // =====================================================================
        // ĐỆ QUY: tìm tất cả ZIP + RAR trong folder
        //         mỗi archive giải nén vào thư mục riêng theo tên UUID
        // =====================================================================
        //private void ExtractAllInFolder(string folder, int depth)
        //{
        //    if (!_isRunning) return;
        //    if (depth > 10) return;

        //    string indent = new string(' ', depth * 2);

        //    var allFiles = Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly);

        //    foreach (var filePath in allFiles)
        //    {
        //        if (!_isRunning) break;

        //        string ext = Path.GetExtension(filePath).ToLower();
        //        if (ext != ".zip" && ext != ".rar") continue;

        //        string nameNoExt = Path.GetFileNameWithoutExtension(filePath);
        //        string parentDir = Path.GetDirectoryName(filePath);
        //        string extractDir = Path.Combine(parentDir, nameNoExt);

        //        AppendLog(indent + "[Lớp " + depth + "] [" + ext.ToUpper().TrimStart('.') + "] " + Path.GetFileName(filePath));
        //        AppendLog(indent + "  → Giải nén vào: " + nameNoExt + "\\");

        //        try
        //        {
        //            if (!Directory.Exists(extractDir))
        //                Directory.CreateDirectory(extractDir);

        //            // Dùng IReader của SharpCompress — tương thích mọi version
        //            using (var stream = File.OpenRead(filePath))
        //            using (var reader = ReaderFactory.Open(stream))
        //            {
        //                while (reader.MoveToNextEntry())
        //                {
        //                    if (!_isRunning) break;
        //                    if (reader.Entry.IsDirectory) continue;

        //                    string entryKey = reader.Entry.Key.Replace('/', Path.DirectorySeparatorChar);
        //                    string destPath = Path.Combine(extractDir, entryKey);
        //                    string destDir = Path.GetDirectoryName(destPath);

        //                    if (!Directory.Exists(destDir))
        //                        Directory.CreateDirectory(destDir);

        //                    if (File.Exists(destPath) && !chkOverwrite.Checked)
        //                    {
        //                        AppendLog(indent + "  Bỏ qua (đã tồn tại): " + reader.Entry.Key);
        //                        continue;
        //                    }

        //                    // Tạo ExtractionOptions theo cách tương thích C# 7.3
        //                    var options = new ExtractionOptions();
        //                    options.ExtractFullPath = true;
        //                    options.Overwrite = chkOverwrite.Checked;

        //                    reader.WriteEntryToDirectory(extractDir, options);

        //                    _totalExtracted++;
        //                    UpdateProgress();
        //                    AppendLog(indent + "  OK: " + reader.Entry.Key);
        //                }
        //            }

        //            // Xóa archive sau khi giải nén nếu được chọn
        //            if (chkDeleteZip.Checked && _isRunning)
        //            {
        //                File.Delete(filePath);
        //                AppendLog(indent + "  Đã xóa: " + Path.GetFileName(filePath));
        //            }

        //            // Tiếp tục tìm archive con trong thư mục vừa giải nén ra
        //            ExtractAllInFolder(extractDir, depth + 1);
        //        }
        //        catch (Exception ex)
        //        {
        //            _totalErrors++;
        //            AppendLog(indent + "  LỖI: " + ex.Message);
        //        }
        //    }

        //    // Đệ quy vào các thư mục con có sẵn
        //    foreach (var subDir in Directory.GetDirectories(folder, "*", SearchOption.TopDirectoryOnly))
        //    {
        //        if (!_isRunning) break;
        //        ExtractAllInFolder(subDir, depth);
        //    }
        //}

        private void ExtractAllInFolder(string folder, int depth)
        {
            if (!_isRunning) return;
            if (depth > 10) return;

            string indent = new string(' ', depth * 2);
            string currentFolderName = Path.GetFileName(folder);

            // Kiểm tra nếu folder hiện tại là cấp Bib_Id (Ví dụ: 202403200933-4b8fb5f6...)
            bool isLastLevel = currentFolderName.Contains("-") && currentFolderName.Length > 20;

            var allFiles = Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly);

            foreach (var filePath in allFiles)
            {
                if (!_isRunning) break;

                string ext = Path.GetExtension(filePath).ToLower();
                if (ext != ".zip" && ext != ".rar") continue;

                // Thông tin ID cũ
                string nameNoExt = Path.GetFileNameWithoutExtension(filePath);
                string parentDir = Path.GetDirectoryName(filePath);

                // NÂNG CẤP: Đổi tên folder giải nén thành Tai_lieu_so
                string targetFolderName = "Tai_lieu_so";
                string extractDir = Path.Combine(parentDir, targetFolderName);

                string idILib = nameNoExt;
                string bibIdILib = currentFolderName;

                try
                {
                    if (!Directory.Exists(extractDir))
                        Directory.CreateDirectory(extractDir);

                    using (var stream = File.OpenRead(filePath))
                    using (var reader = ReaderFactory.Open(stream))
                    {
                        while (reader.MoveToNextEntry())
                        {
                            if (!_isRunning) break;
                            if (reader.Entry.IsDirectory) continue;

                            // Giải nén vào thư mục Tai_lieu_so
                            var options = new ExtractionOptions
                            {
                                ExtractFullPath = true,
                                Overwrite = chkOverwrite.Checked // Khuyến nghị luôn tích Overwrite trên UI
                            };
                            reader.WriteEntryToDirectory(extractDir, options);
                        }
                    }

                    // GHI LOG THÀNH CÔNG (1 file Zip = 1 dòng)
                    SaveMappingToDb(idILib, bibIdILib, filePath, extractDir, 1, "OK");

                    _totalExtracted++;
                    UpdateProgress();
                    AppendLog(indent + "  DONE: " + Path.GetFileName(filePath) + " -> " + targetFolderName);

                    // Xóa file nén nếu được yêu cầu
                    if (chkDeleteZip.Checked && _isRunning)
                    {
                        File.Delete(filePath);
                    }

                    // Chỉ đệ quy tiếp nếu chưa phải cấp Bib_Id
                    if (!isLastLevel)
                    {
                        ExtractAllInFolder(extractDir, depth + 1);
                    }
                }
                catch (Exception ex)
                {
                    _totalErrors++;
                    UpdateProgress();
                    SaveMappingToDb(idILib, bibIdILib, filePath, "", -1, ex.Message);
                    AppendLog(indent + "  ERR: " + Path.GetFileName(filePath) + " - " + ex.Message);
                }
            }

            // Tiếp tục duyệt các thư mục con nếu chưa đạt cấp cuối
            if (!isLastLevel)
            {
                foreach (var subDir in Directory.GetDirectories(folder, "*", SearchOption.TopDirectoryOnly))
                {
                    if (!_isRunning) break;
                    ExtractAllInFolder(subDir, depth);
                }
            }
        }

        // version OK nhất tuy nhiên chưa có đổi tên folder sau khi giải nén theo chuẩn quy ước
        //private void ExtractAllInFolder(string folder, int depth)
        //{
        //    if (!_isRunning) return;
        //    if (depth > 10) return;

        //    string indent = new string(' ', depth * 2);
        //    string currentFolderName = Path.GetFileName(folder);
        //    bool isLastLevel = currentFolderName.Contains("-") && currentFolderName.Length > 20;

        //    var allFiles = Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly);

        //    foreach (var filePath in allFiles)
        //    {
        //        if (!_isRunning) break;

        //        string ext = Path.GetExtension(filePath).ToLower();
        //        if (ext != ".zip" && ext != ".rar") continue;

        //        string nameNoExt = Path.GetFileNameWithoutExtension(filePath);
        //        string parentDir = Path.GetDirectoryName(filePath);
        //        string extractDir = Path.Combine(parentDir, nameNoExt);

        //        string idILib = nameNoExt;
        //        string bibIdILib = currentFolderName;

        //        try
        //        {
        //            if (!Directory.Exists(extractDir))
        //                Directory.CreateDirectory(extractDir);

        //            using (var stream = File.OpenRead(filePath))
        //            using (var reader = ReaderFactory.Open(stream))
        //            {
        //                while (reader.MoveToNextEntry())
        //                {
        //                    if (!_isRunning) break;
        //                    if (reader.Entry.IsDirectory) continue;

        //                    string entryKey = reader.Entry.Key.Replace('/', Path.DirectorySeparatorChar);
        //                    string destPath = Path.Combine(extractDir, entryKey);

        //                    var options = new ExtractionOptions { ExtractFullPath = true, Overwrite = chkOverwrite.Checked };
        //                    reader.WriteEntryToDirectory(extractDir, options);

        //                    // KHÔNG ĐẶT SaveMappingToDb Ở ĐÂY
        //                }
        //            }

        //            // GHI LOG VÀO DB SAU KHI ĐÃ GIẢI NÉN XONG TOÀN BỘ FILE ZIP
        //            // File_Path_KPS lúc này lưu đường dẫn thư mục đã giải nén xong
        //            SaveMappingToDb(idILib, bibIdILib, filePath, extractDir, 1, "OK");
        //            AppendLog(indent + "  DONE: " + Path.GetFileName(filePath));

        //            if (chkDeleteZip.Checked && _isRunning)
        //            {
        //                File.Delete(filePath);
        //            }

        //            if (!isLastLevel)
        //            {
        //                ExtractAllInFolder(extractDir, depth + 1);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            _totalErrors++;
        //            SaveMappingToDb(idILib, bibIdILib, filePath, "", -1, ex.Message);
        //            AppendLog(indent + "  Err: " + ex.Message);
        //        }
        //    }

        //    if (!isLastLevel)
        //    {
        //        foreach (var subDir in Directory.GetDirectories(folder, "*", SearchOption.TopDirectoryOnly))
        //        {
        //            if (!_isRunning) break;
        //            ExtractAllInFolder(subDir, depth);
        //        }
        //    }
        //}

        // version lưu log nhưng quá tốn tài nguyên do đệ quy quá sâu không cần thiết
        //private void ExtractAllInFolder(string folder, int depth)
        //{
        //    if (!_isRunning) return;
        //    if (depth > 10) return;

        //    string indent = new string(' ', depth * 2);
        //    var allFiles = Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly);

        //    foreach (var filePath in allFiles)
        //    {
        //        if (!_isRunning) break;

        //        string ext = Path.GetExtension(filePath).ToLower();
        //        if (ext != ".zip" && ext != ".rar") continue;

        //        string nameNoExt = Path.GetFileNameWithoutExtension(filePath);
        //        string parentDir = Path.GetDirectoryName(filePath);
        //        string extractDir = Path.Combine(parentDir, nameNoExt);

        //        // --- LOGIC TRÍCH XUẤT THÔNG TIN MAPPING ---
        //        // Id_ILib là tên file (a41e9bdb...)
        //        string idILib = nameNoExt;
        //        // Bib_Id_ILib là tên thư mục cha (202403200930-a14f6a59...)
        //        string bibIdILib = Path.GetFileName(parentDir);
        //        // ------------------------------------------

        //        AppendLog(indent + "[Lớp " + depth + "] [" + ext.ToUpper().TrimStart('.') + "] " + Path.GetFileName(filePath));

        //        try
        //        {
        //            if (!Directory.Exists(extractDir))
        //                Directory.CreateDirectory(extractDir);

        //            using (var stream = File.OpenRead(filePath))
        //            using (var reader = ReaderFactory.Open(stream))
        //            {
        //                while (reader.MoveToNextEntry())
        //                {
        //                    if (!_isRunning) break;
        //                    if (reader.Entry.IsDirectory) continue;

        //                    string entryKey = reader.Entry.Key.Replace('/', Path.DirectorySeparatorChar);
        //                    string destPath = Path.Combine(extractDir, entryKey);
        //                    string destDir = Path.GetDirectoryName(destPath);

        //                    if (!Directory.Exists(destDir))
        //                        Directory.CreateDirectory(destDir);

        //                    var options = new ExtractionOptions { ExtractFullPath = true, Overwrite = chkOverwrite.Checked };
        //                    reader.WriteEntryToDirectory(extractDir, options);

        //                    _totalExtracted++;
        //                    UpdateProgress();
        //                    AppendLog(indent + "  OK: " + reader.Entry.Key);

        //                    // --- LƯU TRUY VẾT VÀO DB ---
        //                    // Chúng ta lưu mỗi khi giải nén xong một file con trong archive
        //                    SaveMappingToDb(idILib, bibIdILib, filePath, destPath, 1, "OK");
        //                }
        //            }

        //            if (chkDeleteZip.Checked && _isRunning)
        //            {
        //                File.Delete(filePath);
        //                AppendLog(indent + "  Đã xóa: " + Path.GetFileName(filePath));
        //            }

        //            ExtractAllInFolder(extractDir, depth + 1);
        //        }
        //        catch (Exception ex)
        //        {
        //            _totalErrors++;
        //            AppendLog(indent + "  LỖI: " + ex.Message);

        //            // Lưu trạng thái lỗi vào DB
        //            SaveMappingToDb(idILib, bibIdILib, filePath, "", -1, ex.Message);
        //        }
        //    }

        //    foreach (var subDir in Directory.GetDirectories(folder, "*", SearchOption.TopDirectoryOnly))
        //    {
        //        if (!_isRunning) break;
        //        ExtractAllInFolder(subDir, depth);
        //    }
        //}

        // =====================================================================
        // TIỆN ÍCH
        // =====================================================================
        private void AppendLog(string message)
        {
            if (txtLog.InvokeRequired)
                txtLog.Invoke(new Action(() =>
                {
                    txtLog.AppendText(message + Environment.NewLine);
                    txtLog.ScrollToCaret();
                }));
            else
            {
                txtLog.AppendText(message + Environment.NewLine);
                txtLog.ScrollToCaret();
            }
        }

        private void UpdateProgress()
        {
            if (lblProgress.InvokeRequired)
                lblProgress.Invoke(new Action(() =>
                    lblProgress.Text = "Đã giải nén: " + _totalExtracted + " file | Lỗi: " + _totalErrors));
            else
                lblProgress.Text = "Đã giải nén: " + _totalExtracted + " file | Lỗi: " + _totalErrors;
        }


        private void SaveMappingToDb(string idILib, string bibIdILib, string filePathILib, string filePathKps, short status, string msgLog)
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["MyDbConn"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string sql = @"INSERT INTO ScriptMappingData (Id_ILib, Bib_Id_ILib, File_Path_ILib, File_Path_KPS, Status, MessageLog) 
                           VALUES (@Id_ILib, @Bib_Id_ILib, @File_Path_ILib, @File_Path_KPS, @Status, @MessageLog)";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id_ILib", idILib ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Bib_Id_ILib", bibIdILib ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@File_Path_ILib", filePathILib ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@File_Path_KPS", filePathKps ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Status", status);
                        cmd.Parameters.AddWithValue("@MessageLog", msgLog);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                AppendLog("  [DB ERROR]: " + ex.Message);
            }
        }

    }
} 