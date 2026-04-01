using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace ScriptDataTool
{
    public partial class UnZipForm : Form
    {
        private bool _isRunning = false;
        private int _totalExtracted = 0;
        private int _totalErrors = 0;

        // Lưu lại thư mục gốc input để tính đường dẫn tương đối
        private string _rootInputFolder = "";
        // Thư mục output gốc
        private string _rootOutputFolder = "";

        public UnZipForm()
        {
            InitializeComponent();
        }

        // =====================================================================
        // CHỌN THƯ MỤC INPUT
        // =====================================================================
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Chọn thư mục gốc chứa các file ZIP/RAR";
                if (dlg.ShowDialog() == DialogResult.OK)
                    txtFolder.Text = dlg.SelectedPath;
            }
        }

        // =====================================================================
        // CHỌN THƯ MỤC OUTPUT
        // =====================================================================
        private void btnBrowseOutput_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Chọn thư mục đích để giải nén ra";
                if (dlg.ShowDialog() == DialogResult.OK)
                    txtOutputFolder.Text = dlg.SelectedPath;
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
                MessageBox.Show("Vui lòng chọn thư mục nguồn hợp lệ.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtOutputFolder.Text))
            {
                MessageBox.Show("Vui lòng chọn thư mục đích (Output).", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _isRunning = true;
            _totalExtracted = 0;
            _totalErrors = 0;
            _rootInputFolder = txtFolder.Text.TrimEnd(Path.DirectorySeparatorChar);
            _rootOutputFolder = txtOutputFolder.Text.TrimEnd(Path.DirectorySeparatorChar);

            btnStart.Enabled = false;
            btnStop.Enabled = true;
            progressBar.MarqueeAnimationSpeed = 30;
            txtLog.Clear();

            // Tạo thư mục output nếu chưa có
            try { Directory.CreateDirectory(_rootOutputFolder); }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tạo thư mục output: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _isRunning = false;
                btnStart.Enabled = true;
                btnStop.Enabled = false;
                progressBar.MarqueeAnimationSpeed = 0;
                return;
            }

            AppendLog("Input : " + _rootInputFolder);
            AppendLog("Output: " + _rootOutputFolder);
            AppendLog(new string('=', 60));

            string rootFolder = txtFolder.Text;
            await Task.Run(() => ProcessFolder(rootFolder));

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
        // TÍNH THƯ MỤC OUTPUT TƯƠNG ỨNG VỚI MỘT FOLDER INPUT
        // Ví dụ:
        //   inputFolder  = G:\zip\2023\05\23
        //   _rootInput   = G:\zip
        //   _rootOutput  = G:\OutputUnZip
        //   → return      G:\OutputUnZip\2023\05\23
        // =====================================================================
        private string GetMirroredOutputPath(string inputFolder)
        {
            // Nếu inputFolder chính là root hoặc không phải con của root
            // thì trả về _rootOutputFolder luôn
            if (string.Equals(inputFolder, _rootInputFolder, StringComparison.OrdinalIgnoreCase))
                return _rootOutputFolder;

            string relative = inputFolder.Substring(_rootInputFolder.Length)
                                         .TrimStart(Path.DirectorySeparatorChar);
            return Path.Combine(_rootOutputFolder, relative);
        }

        // =====================================================================
        // HÀM CHÍNH: xử lý 1 folder
        // =====================================================================
        private void ProcessFolder(string folder)
        {
            if (!_isRunning) return;

            string folderName = Path.GetFileName(folder);

            // --- TRƯỜNG HỢP 1: Đây là folder Bib_Id ---
            if (IsBibIdFolder(folderName))
            {
                ProcessBibIdFolder(folder, folderName);
                return;
            }

            // --- TRƯỜNG HỢP 2: Giải nén ZIP/RAR ở cấp này ---
            var archiveFiles = Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly);
            foreach (var filePath in archiveFiles)
            {
                if (!_isRunning) break;

                string ext = Path.GetExtension(filePath).ToLower();
                if (ext != ".zip" && ext != ".rar") continue;

                string nameNoExt = Path.GetFileNameWithoutExtension(filePath);

                // Thư mục output tương ứng với folder hiện tại
                // 2019.rar nằm trực tiếp trong _rootInputFolder → giải nén thẳng ra output
                //            G:\OutputUnZip\
                //  ├── 2019\01\03\BibIdFolder\...  ✔
                //  ├── 2019\02\15\BibIdFolder\...  ✔
                //  └── 2020\04\22\BibIdFolder\...  ✔

                //// ZIP con bên trong BibId → giữ nguyên logic cũ
                //    G:\OutputUnZip\2019\01\03\BibIdFolder\
                //      └── TenFile\...  ✔
                string mirroredFolder = GetMirroredOutputPath(folder);
                bool isTopLevelArchive = string.Equals(
                                        Path.GetDirectoryName(filePath),
                                        _rootInputFolder,
                                        StringComparison.OrdinalIgnoreCase);

                string extractDir = isTopLevelArchive
                    ? _rootOutputFolder
                    : Path.Combine(mirroredFolder, nameNoExt);

                AppendLog("[ZIP/RAR] " + Path.GetFileName(filePath));
                AppendLog("  → " + extractDir);

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

                            var options = new ExtractionOptions
                            {
                                ExtractFullPath = true,
                                Overwrite = chkOverwrite.Checked
                            };
                            reader.WriteEntryToDirectory(extractDir, options);
                        }
                    }

                    AppendLog("  OK");
                    // File ZIP gốc bên InputZip — KHÔNG xóa, giữ nguyên input

                    // Đệ quy vào folder vừa giải nén để xử lý BibId và ZIP con bên trong
                    int extractedBefore = _totalExtracted;
                    ProcessExtractedFolder(extractDir);

                    // Nếu không có file nào thực sự được giải nén bên trong
                    // → xóa toàn bộ folder output rỗng để không để lại folder trung gian vô nghĩa
                    if (_totalExtracted == extractedBefore)
                    {
                        TryDeleteEmptyDirectory(extractDir);
                        AppendLog("  Không có nội dung hợp lệ, đã dọn folder output: " + extractDir);
                    }
                }
                catch (Exception ex)
                {
                    _totalErrors++;
                    UpdateProgress();
                    AppendLog("  LỖI: " + ex.Message);
                }
            }

            // --- TRƯỜNG HỢP 3: Đệ quy vào các subfolder input ---
            foreach (var subDir in Directory.GetDirectories(folder, "*", SearchOption.TopDirectoryOnly))
            {
                if (!_isRunning) break;
                ProcessFolder(subDir);
            }
        }

        // =====================================================================
        // Giải nén tiếp các ZIP/RAR nằm trong folder đã được giải nén ra (output)
        // Lúc này không cần mirror nữa, giải nén ngay tại chỗ trong output
        // =====================================================================
        private void ProcessExtractedFolder(string folder)
        {
            if (!_isRunning) return;

            string folderName = Path.GetFileName(folder);

            if (IsBibIdFolder(folderName))
            {
                // BibId nằm trong output (đã giải nén từ ZIP cấp 1)
                // Sau khi xử lý: nếu không có file nào được giải nén
                // → xóa folder BibId này khỏi output
                int extractedBefore = _totalExtracted;
                ProcessBibIdFolder(folder, folderName);
                if (_totalExtracted == extractedBefore)
                    TryDeleteEmptyDirectory(folder);
                return;
            }

            var archiveFiles = Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly);
            foreach (var filePath in archiveFiles)
            {
                if (!_isRunning) break;
                string ext = Path.GetExtension(filePath).ToLower();
                if (ext != ".zip" && ext != ".rar") continue;

                string nameNoExt = Path.GetFileNameWithoutExtension(filePath);
                string extractDir = Path.Combine(folder, nameNoExt);

                AppendLog("  [ZIP con] " + Path.GetFileName(filePath) + " → " + extractDir);

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
                            var options = new ExtractionOptions
                            {
                                ExtractFullPath = true,
                                Overwrite = chkOverwrite.Checked
                            };
                            reader.WriteEntryToDirectory(extractDir, options);
                        }
                    }

                    if (chkDeleteZip.Checked && _isRunning)
                    {
                        File.Delete(filePath);
                        AppendLog("    Đã xóa: " + Path.GetFileName(filePath));
                    }

                    ProcessExtractedFolder(extractDir);
                }
                catch (Exception ex)
                {
                    _totalErrors++;
                    UpdateProgress();
                    AppendLog("    LỖI: " + ex.Message);
                }
            }

            foreach (var subDir in Directory.GetDirectories(folder, "*", SearchOption.TopDirectoryOnly))
            {
                if (!_isRunning) break;
                ProcessExtractedFolder(subDir);
            }
        }

        // =====================================================================
        // XỬ LÝ FOLDER BIB_ID
        // Flow: check mapping trước → chỉ khi có file cần làm mới tạo folder
        //       output, đổi tên folder input, rồi giải nén
        // =====================================================================
        private void ProcessBibIdFolder(string bibFolder, string bibIdOriginal)
        {
            string bibId = bibIdOriginal;

            // ── BƯỚC 1: Quét tất cả ZIP/RAR và check mapping trước ───────────
            var allFiles = Directory.GetFiles(bibFolder, "*.*", SearchOption.TopDirectoryOnly);
            var workList = new List<(string FilePath, MappingInfo Mapping)>();

            foreach (var filePath in allFiles)
            {
                string ext = Path.GetExtension(filePath).ToLower();
                if (ext != ".zip" && ext != ".rar") continue;

                string idILib = Path.GetFileNameWithoutExtension(filePath);
                MappingInfo mapping = CheckMapping(idILib, bibId);

                if (mapping == null)
                {
                    // Không có trong ScriptMappingData → bỏ qua hoàn toàn, không log BIB
                    continue;
                }
                if (mapping.Status == 1)
                {
                    // Đã Done → bỏ qua
                    continue;
                }
                // Status = 0 (Pending) hoặc -1 (Error) → đưa vào danh sách cần xử lý
                workList.Add((filePath, mapping));
            }

            // ── BƯỚC 2: Không có gì cần làm → thoát, không tạo folder output ─
            if (workList.Count == 0)
            {
                AppendLog("[BIB] " + bibIdOriginal + " — Không có file nào cần giải nén, bỏ qua");
                return;
            }

            // ── BƯỚC 3: Có việc làm → mới query tên, đổi tên folder input ────
            AppendLog("[BIB] " + bibIdOriginal);

            BibInfo info = QueryBibInfo(bibId);
            string parentDir = Path.GetDirectoryName(bibFolder);
            string newBibFolderName;
            string newBibFolderPath;

            if (info != null)
            {
                string slugTitle = ToSlug(info.TitlSort, 15);

                // Kiểm tra trùng: nếu có BibId khác cùng ShortPath có cùng Titl_Sort
                // → nối thêm BibIndexAutor để đảm bảo unique
                bool hasDup = HasDuplicateSlugInSameFolder(bibId, info.TitlSort);
                newBibFolderName = hasDup
                    ? slugTitle + "_" + info.BibIndexAutor
                    : slugTitle;
                AppendLog("  Tên mới: " + newBibFolderName + (hasDup ? " (có trùng, đã nối BibIndexAutor)" : ""));
            }
            else
            {
                newBibFolderName = bibIdOriginal;
                AppendLog("  Không tìm thấy trong DB, giữ nguyên tên: " + bibIdOriginal);
            }

            newBibFolderPath = Path.Combine(parentDir, newBibFolderName);

            if (!string.Equals(bibFolder, newBibFolderPath, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    newBibFolderPath = GetUniqueFolderPath(newBibFolderPath);
                    newBibFolderName = Path.GetFileName(newBibFolderPath);
                    Directory.Move(bibFolder, newBibFolderPath);
                    AppendLog("  Đổi tên: " + bibIdOriginal + " → " + newBibFolderName);

                    // workList chứa đường dẫn cũ → cập nhật lại sang đường dẫn mới
                    for (int i = 0; i < workList.Count; i++)
                    {
                        string newFilePath = Path.Combine(newBibFolderPath,
                                                Path.GetFileName(workList[i].FilePath));
                        workList[i] = (newFilePath, workList[i].Mapping);
                    }
                }
                catch (Exception ex)
                {
                    AppendLog("  LỖI đổi tên folder: " + ex.Message + " — giữ tên cũ.");
                    newBibFolderPath = bibFolder;
                    newBibFolderName = bibIdOriginal;
                }
            }

            // ── BƯỚC 4: Tạo folder output (mirror) rồi giải nén ──────────────
            string bibOutputFolder;
            if (IsInsideOutputTree(newBibFolderPath))
            {
                bibOutputFolder = newBibFolderPath;
            }
            else
            {
                bibOutputFolder = GetMirroredOutputPath(newBibFolderPath);
                if (!Directory.Exists(bibOutputFolder))
                    Directory.CreateDirectory(bibOutputFolder);
            }

            int counter = 1;
            foreach (var (filePath, mapping) in workList)
            {
                if (!_isRunning) break;

                string ext = Path.GetExtension(filePath).ToLower();

                // Ưu tiên dùng FileName từ DB (không có extension) làm tên subfolder
                // Fallback: newBibFolderName + _001 nếu FileName rỗng
                string baseName;

                if (!string.IsNullOrWhiteSpace(mapping.FileName))
                {
                    baseName = mapping.FileName;
                }
                else
                {
                    baseName = $"{newBibFolderName}_{counter:D3}";
                    counter++;   // ⭐ chỉ tăng ở đây
                }

                string slug = ToSlug(baseName, 15);

                string uniqueFullPath = GetUniqueFolderPath(
                    Path.Combine(bibOutputFolder, slug)
                );

                string subFolderName = Path.GetFileName(uniqueFullPath);
                string subFolderPath = uniqueFullPath;

                AppendLog("  [" + ext.TrimStart('.').ToUpper() + "] "
                          + Path.GetFileName(filePath) + " → " + subFolderPath);

                try
                {
                    if (!Directory.Exists(subFolderPath))
                        Directory.CreateDirectory(subFolderPath);

                    using (var stream = File.OpenRead(filePath))
                    using (var reader = ReaderFactory.Open(stream))
                    {
                        while (reader.MoveToNextEntry())
                        {
                            if (!_isRunning) break;
                            if (reader.Entry.IsDirectory) continue;

                            var options = new ExtractionOptions
                            {
                                ExtractFullPath = true,
                                Overwrite = chkOverwrite.Checked
                            };
                            reader.WriteEntryToDirectory(subFolderPath, options);
                        }
                    }

                    // File ZIP con trong BibId folder bên InputZip — KHÔNG xóa, giữ nguyên input

                    // Nếu bibOutputFolder nằm trong Output và checkbox xóa được tick
                    // → xóa file gốc tương ứng trong bibOutputFolder (bản copy từ ZIP cấp 1)
                    if (chkDeleteZip.Checked && IsInsideOutputTree(bibOutputFolder))
                    {
                        string outputCopy = Path.Combine(bibOutputFolder, Path.GetFileName(filePath));
                        if (File.Exists(outputCopy))
                        {
                            try { File.Delete(outputCopy); AppendLog("    Đã xóa (output): " + Path.GetFileName(outputCopy)); }
                            catch { /* bỏ qua */ }
                        }
                        // Xóa luôn file .info cùng tên nếu có
                        string infoFile = Path.Combine(bibOutputFolder,
                                              Path.GetFileNameWithoutExtension(filePath) + ".info");
                        if (File.Exists(infoFile))
                        {
                            try { File.Delete(infoFile); AppendLog("    Đã xóa (output): " + Path.GetFileName(infoFile)); }
                            catch { /* bỏ qua */ }
                        }
                    }

                    UpdateMappingInDb(mapping.Id, subFolderPath, 1, "OK");
                    _totalExtracted++;
                    UpdateProgress();
                    AppendLog("    DONE → " + subFolderPath);
                }
                catch (Exception ex)
                {
                    _totalErrors++;
                    UpdateProgress();
                    UpdateMappingInDb(mapping.Id, subFolderPath, -1, ex.Message);
                    AppendLog("    ERR: " + ex.Message);
                }
            }
        }

        // Kiểm tra xem một path có nằm trong cây output không
        private bool IsInsideOutputTree(string path)
        {
            return path.StartsWith(_rootOutputFolder, StringComparison.OrdinalIgnoreCase);
        }

        // =====================================================================
        // QUERY DB
        // =====================================================================
        private BibInfo QueryBibInfo(string bibId)
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["MyDbConn"].ConnectionString;
                string sql = @"
                    SELECT TOP 1 b.BibIndexAutor, b.Titl_Sort
                    FROM Dl_DigitalFile f
                    INNER JOIN Dl_Bib b ON f.Bib_Id = b.ID
                    WHERE f.Bib_Id = @BibId AND f.Bib_Id IS NOT NULL";

                using (var conn = new SqlConnection(connString))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@BibId", bibId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new BibInfo
                            {
                                BibIndexAutor = reader["BibIndexAutor"]?.ToString()?.Trim() ?? "",
                                TitlSort = reader["Titl_Sort"]?.ToString()?.Trim() ?? ""
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppendLog("  [DB ERROR] QueryBibInfo: " + ex.Message);
            }
            return null;
        }

        // =====================================================================
        // KIỂM TRA TRÙNG TÊN SLUG TRONG CÙNG THƯ MỤC (ShortPath = 6 cấp đầu)
        // Dùng cùng logic PARTITION BY Titl_Sort, ShortPath như SQL báo cáo
        // =====================================================================
        private bool HasDuplicateSlugInSameFolder(string bibId, string titlSort)
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["MyDbConn"].ConnectionString;
                // ShortPath = 6 cấp đầu của FilePath (E:\THUVIEN\FileContent\year\mm\dd\)
                // Đếm xem có BibId khác (khác @BibId hiện tại) nào cùng ShortPath
                // mà có Titl_Sort giống nhau không
                string sql = @"
                    WITH ShortPaths AS (
                        SELECT f.Bib_Id,
                               LEFT(f.FilePath,
                                    CHARINDEX('\', f.FilePath,
                                    CHARINDEX('\', f.FilePath,
                                    CHARINDEX('\', f.FilePath,
                                    CHARINDEX('\', f.FilePath,
                                    CHARINDEX('\', f.FilePath,
                                    CHARINDEX('\', f.FilePath) + 1) + 1) + 1) + 1) + 1)
                               ) AS ShortPath
                        FROM Dl_DigitalFile f
                        INNER JOIN Dl_Bib b ON f.Bib_Id = b.ID
                        WHERE b.Titl_Sort = @TitlSort
                    ),
                    MySP AS (
                        SELECT ShortPath FROM ShortPaths WHERE Bib_Id = @BibId
                    )
                    SELECT COUNT(*)
                    FROM ShortPaths sp
                    INNER JOIN MySP m ON sp.ShortPath = m.ShortPath
                    WHERE sp.Bib_Id <> @BibId";

                using (var conn = new SqlConnection(connString))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@BibId", bibId);
                    cmd.Parameters.AddWithValue("@TitlSort", titlSort);
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                AppendLog("  [DB ERROR] HasDuplicateSlug: " + ex.Message);
                return true; // An toàn: nếu lỗi thì nối BibIndexAutor để chắc chắn unique
            }
        }

        // =====================================================================
        // KIỂM TRA MAPPING — trả về null nếu không tìm thấy
        // =====================================================================
        private MappingInfo CheckMapping(string idILib, string bibIdILib)
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["MyDbConn"].ConnectionString;
                string sql = @"
                    SELECT TOP 1 m.Id, m.[Status],
                           ISNULL(d.FileName, '') AS FileName
                    FROM ScriptMappingData m
                    LEFT JOIN Dl_DigitalFile d ON d.Id = m.Id_ILib
                    WHERE m.Id_ILib = @Id_ILib AND m.Bib_Id_ILib = @Bib_Id_ILib";

                using (var conn = new SqlConnection(connString))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id_ILib", (object)idILib ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Bib_Id_ILib", (object)bibIdILib ?? DBNull.Value);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string rawFileName = reader["FileName"]?.ToString()?.Trim() ?? "";
                            string fileNameNoExt = string.IsNullOrEmpty(rawFileName)
                                ? ""
                                : Path.GetFileNameWithoutExtension(rawFileName);
                            return new MappingInfo
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Status = Convert.ToInt16(reader["Status"]),
                                FileName = fileNameNoExt
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppendLog("  [DB ERROR] CheckMapping: " + ex.Message);
            }
            return null;
        }

        // =====================================================================
        // CẬP NHẬT MAPPING SAU KHI GIẢI NÉN (UPDATE, không INSERT)
        // =====================================================================
        private void UpdateMappingInDb(int mappingId, string filePathKps, short status, string msgLog)
        {
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["MyDbConn"].ConnectionString;
                string sql = @"
                    UPDATE ScriptMappingData
                    SET File_Path_KPS = @File_Path_KPS,
                        [Status]      = @Status,
                        MessageLog    = @MessageLog,
                        CreatedDTime  = @CreatedDTime
                    WHERE Id = @Id";

                using (var conn = new SqlConnection(connString))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@File_Path_KPS", (object)filePathKps ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@MessageLog", (object)msgLog ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedDTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Id", mappingId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                AppendLog("  [DB ERROR] UpdateMapping: " + ex.Message);
            }
        }

        // =====================================================================
        // HELPERS
        // =====================================================================
        private static readonly Regex BibIdPattern = new Regex(
            @"^\d{12}-[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$",
            RegexOptions.Compiled);

        private static bool IsBibIdFolder(string folderName)
        {
            return BibIdPattern.IsMatch(folderName);
        }

        private static string ToSlug(string input, int maxWords)
        {
            if (string.IsNullOrWhiteSpace(input)) return "Unknown";

            // 1. Khử dấu tiếng Việt
            string noDiacritics = RemoveVietnameseDiacritics(input);

            // 2. Xóa ký tự đặc biệt, chỉ giữ lại chữ cái và số
            string cleanText = Regex.Replace(noDiacritics, @"[^a-zA-Z0-9\s]", "");

            // 3. Tách chuỗi thành mảng các từ (Dựa vào khoảng trắng)
            // "Nghien cuu thuc trang" -> ["Nghien", "cuu", "thuc", "trang"]
            string[] words = cleanText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0) return "Unknown";

            // Kiểm tra xem tổng số từ có nhiều hơn số lượng mong muốn không
            bool isTruncated = words.Length > maxWords;

            // 4. Lấy ĐÚNG số lượng từ (maxWords) và viết hoa chữ đầu từng từ
            var selectedWords = words.Take(maxWords).Select(word =>
            {
                if (string.IsNullOrEmpty(word)) return "";
                // PascalCase: Chữ đầu Hoa, còn lại thường
                return char.ToUpper(word[0]) + word.Substring(1).ToLower();
            });

            // 5. Ghép lại viết liền nhau (Không tốn thêm ký tự cho dấu gạch ngang)
            string result = string.Concat(selectedWords);

            // 6. Nếu tiêu đề gốc dài hơn số từ đã lấy, thêm "_" ở cuối để đánh dấu "còn nữa"
            if (isTruncated)
            {
                result += "";
            }

            return result;
        }

        private static string RemoveVietnameseDiacritics(string text)
        {
            var map = new Dictionary<string, string>
            {
                {"à","a"},{"á","a"},{"ả","a"},{"ã","a"},{"ạ","a"},
                {"ă","a"},{"ằ","a"},{"ắ","a"},{"ẳ","a"},{"ẵ","a"},{"ặ","a"},
                {"â","a"},{"ầ","a"},{"ấ","a"},{"ẩ","a"},{"ẫ","a"},{"ậ","a"},
                {"è","e"},{"é","e"},{"ẻ","e"},{"ẽ","e"},{"ẹ","e"},
                {"ê","e"},{"ề","e"},{"ế","e"},{"ể","e"},{"ễ","e"},{"ệ","e"},
                {"ì","i"},{"í","i"},{"ỉ","i"},{"ĩ","i"},{"ị","i"},
                {"ò","o"},{"ó","o"},{"ỏ","o"},{"õ","o"},{"ọ","o"},
                {"ô","o"},{"ồ","o"},{"ố","o"},{"ổ","o"},{"ỗ","o"},{"ộ","o"},
                {"ơ","o"},{"ờ","o"},{"ớ","o"},{"ở","o"},{"ỡ","o"},{"ợ","o"},
                {"ù","u"},{"ú","u"},{"ủ","u"},{"ũ","u"},{"ụ","u"},
                {"ư","u"},{"ừ","u"},{"ứ","u"},{"ử","u"},{"ữ","u"},{"ự","u"},
                {"ỳ","y"},{"ý","y"},{"ỷ","y"},{"ỹ","y"},{"ỵ","y"},{"đ","d"},
                {"À","A"},{"Á","A"},{"Ả","A"},{"Ã","A"},{"Ạ","A"},
                {"Ă","A"},{"Ằ","A"},{"Ắ","A"},{"Ẳ","A"},{"Ẵ","A"},{"Ặ","A"},
                {"Â","A"},{"Ầ","A"},{"Ấ","A"},{"Ẩ","A"},{"Ẫ","A"},{"Ậ","A"},
                {"È","E"},{"É","E"},{"Ẻ","E"},{"Ẽ","E"},{"Ẹ","E"},
                {"Ê","E"},{"Ề","E"},{"Ế","E"},{"Ể","E"},{"Ễ","E"},{"Ệ","E"},
                {"Ì","I"},{"Í","I"},{"Ỉ","I"},{"Ĩ","I"},{"Ị","I"},
                {"Ò","O"},{"Ó","O"},{"Ỏ","O"},{"Õ","O"},{"Ọ","O"},
                {"Ô","O"},{"Ồ","O"},{"Ố","O"},{"Ổ","O"},{"Ỗ","O"},{"Ộ","O"},
                {"Ơ","O"},{"Ờ","O"},{"Ớ","O"},{"Ở","O"},{"Ỡ","O"},{"Ợ","O"},
                {"Ù","U"},{"Ú","U"},{"Ủ","U"},{"Ũ","U"},{"Ụ","U"},
                {"Ư","U"},{"Ừ","U"},{"Ứ","U"},{"Ử","U"},{"Ữ","U"},{"Ự","U"},
                {"Ỳ","Y"},{"Ý","Y"},{"Ỷ","Y"},{"Ỹ","Y"},{"Ỵ","Y"},{"Đ","D"}
            };
            var sb = new StringBuilder(text.Length);
            foreach (char c in text)
            {
                string s = c.ToString();
                sb.Append(map.ContainsKey(s) ? map[s] : s);
            }
            return sb.ToString();
        }

        // Xóa folder output (kể cả khi còn file gốc như .zip/.info)
        // Gọi khi xác định folder này không có nội dung hợp lệ cần giữ lại
        private static void TryDeleteEmptyDirectory(string path)
        {
            if (!Directory.Exists(path)) return;
            try { Directory.Delete(path, recursive: true); }
            catch { /* bỏ qua lỗi xóa */ }
        }

        private static string GetUniqueFolderPath(string path)
        {
            if (!Directory.Exists(path)) return path;
            int i = 1;
            string newPath;
            do { newPath = path + "_" + i++; }
            while (Directory.Exists(newPath));
            return newPath;
        }

        // =====================================================================
        // TIỆN ÍCH UI
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

        private void btnScanSwf_Click(object sender, EventArgs e)
        {
            // Khởi tạo đối tượng Form mới
            ScanMissingSwfImageForm scanForm = new ScanMissingSwfImageForm();

            // CÁCH 1: Mở dạng cửa sổ độc lập (Người dùng vẫn có thể bấm vào Form cũ)
            scanForm.Show();

            // CÁCH 2: Mở dạng hội thoại (Bắt buộc xử lý xong Form này mới quay lại được Form cũ)
            // scanForm.ShowDialog(); 
        }
    }

    // =========================================================================
    // MODELS
    // =========================================================================
    public class BibInfo
    {
        public string BibIndexAutor { get; set; }
        public string TitlSort { get; set; }
    }

    public class MappingInfo
    {
        public int Id { get; set; }
        public short Status { get; set; }  // 0 Pending | 1 Done | -1 Error
        public string FileName { get; set; }  // FileName từ Dl_DigitalFile (không có extension)
    }
}