//version này giải quyets được zip cùng cấp nhưng chưa hỗ trợ cả zip lẫn rar

    //public partial class Form1 : Form
    //{
    //    private bool _isRunning = false;
    //    private int _totalExtracted = 0;
    //    private int _totalErrors = 0;

    //    public Form1()
    //    {
    //        InitializeComponent();
    //    }

    //    // =====================================================================
    //    // CHỌN THƯ MỤC
    //    // =====================================================================
    //    private void btnBrowse_Click(object sender, EventArgs e)
    //    {
    //        using (var dlg = new FolderBrowserDialog())
    //        {
    //            dlg.Description = "Chọn thư mục gốc (sẽ tìm và giải nén tất cả ZIP, kể cả lồng nhau)";
    //            if (dlg.ShowDialog() == DialogResult.OK)
    //                txtFolder.Text = dlg.SelectedPath;
    //        }
    //    }

    //    // =====================================================================
    //    // BẮT ĐẦU
    //    // =====================================================================
    //    private async void btnStart_Click(object sender, EventArgs e)
    //    {
    //        if (_isRunning) return;

    //        if (string.IsNullOrWhiteSpace(txtFolder.Text) || !Directory.Exists(txtFolder.Text))
    //        {
    //            MessageBox.Show("Vui lòng chọn thư mục hợp lệ.", "Lỗi",
    //                MessageBoxButtons.OK, MessageBoxIcon.Warning);
    //            return;
    //        }

    //        _isRunning = true;
    //        _totalExtracted = 0;
    //        _totalErrors = 0;
    //        btnStart.Enabled = false;
    //        btnStop.Enabled = true;
    //        progressBar.MarqueeAnimationSpeed = 30;
    //        txtLog.Clear();

    //        string rootFolder = txtFolder.Text;
    //        await Task.Run(() => ExtractAllInFolder(rootFolder, 0));

    //        _isRunning = false;
    //        btnStart.Enabled = true;
    //        btnStop.Enabled = false;
    //        progressBar.MarqueeAnimationSpeed = 0;
    //        UpdateProgress();
    //        AppendLog(new string('=', 60));
    //        AppendLog($"HOÀN THÀNH — Đã giải nén: {_totalExtracted} file | Lỗi: {_totalErrors}");
    //    }

    //    private void btnStop_Click(object sender, EventArgs e)
    //    {
    //        _isRunning = false;
    //        AppendLog("[Người dùng dừng]");
    //    }

    //    private void btnClearLog_Click(object sender, EventArgs e)
    //    {
    //        txtLog.Clear();
    //    }

    //    // =====================================================================
    //    // ĐỆ QUY: tìm tất cả ZIP trong folder, mỗi ZIP giải nén vào thư mục
    //    //         riêng theo tên UUID của nó (tránh ghi đè khi 2 ZIP cùng cấp)
    //    // =====================================================================
    //    private void ExtractAllInFolder(string folder, int depth)
    //    {
    //        if (!_isRunning) return;
    //        if (depth > 10) return;

    //        string indent = new string(' ', depth * 2);

    //        var zipFiles = Directory.GetFiles(folder, "*.zip", SearchOption.TopDirectoryOnly);

    //        foreach (var zipPath in zipFiles)
    //        {
    //            if (!_isRunning) break;

    //            // Thư mục đích = cùng cấp với ZIP, đặt tên theo UUID (tên file không có đuôi)
    //            // Ví dụ: 50c8cba5-3366-4eab-8735-ecffea6de906.zip
    //            //     → giải nén vào: 50c8cba5-3366-4eab-8735-ecffea6de906\
    //            string zipNameNoExt = Path.GetFileNameWithoutExtension(zipPath);
    //            string parentDir = Path.GetDirectoryName(zipPath);
    //            string extractDir = Path.Combine(parentDir, zipNameNoExt);

    //            AppendLog($"{indent}[Lớp {depth}] {Path.GetFileName(zipPath)}");
    //            AppendLog($"{indent}  → Giải nén vào: {extractDir}");

    //            try
    //            {
    //                if (!Directory.Exists(extractDir))
    //                    Directory.CreateDirectory(extractDir);

    //                using (var archive = ZipFile.OpenRead(zipPath))
    //                {
    //                    foreach (var entry in archive.Entries)
    //                    {
    //                        if (!_isRunning) break;
    //                        if (string.IsNullOrEmpty(entry.Name)) continue;

    //                        string destPath = Path.Combine(extractDir,
    //                            entry.FullName.Replace('/', Path.DirectorySeparatorChar));
    //                        string destFileDir = Path.GetDirectoryName(destPath);

    //                        if (!Directory.Exists(destFileDir))
    //                            Directory.CreateDirectory(destFileDir);

    //                        if (File.Exists(destPath) && !chkOverwrite.Checked)
    //                        {
    //                            AppendLog($"{indent}  Bỏ qua (đã tồn tại): {entry.Name}");
    //                            continue;
    //                        }

    //                        entry.ExtractToFile(destPath, overwrite: chkOverwrite.Checked);
    //                        _totalExtracted++;
    //                        UpdateProgress();
    //                        AppendLog($"{indent}  OK: {entry.FullName}");
    //                    }
    //                }

    //                // Xóa ZIP sau khi giải nén nếu được chọn
    //                if (chkDeleteZip.Checked && _isRunning)
    //                {
    //                    File.Delete(zipPath);
    //                    AppendLog($"{indent}  Đã xóa: {Path.GetFileName(zipPath)}");
    //                }

    //                // Tiếp tục tìm ZIP con trong thư mục vừa giải nén ra
    //                ExtractAllInFolder(extractDir, depth + 1);
    //            }
    //            catch (Exception ex)
    //            {
    //                _totalErrors++;
    //                AppendLog($"{indent}  LỖI: {ex.Message}");
    //            }
    //        }

    //        // Đệ quy vào các thư mục con có sẵn
    //        foreach (var subDir in Directory.GetDirectories(folder, "*", SearchOption.TopDirectoryOnly))
    //        {
    //            if (!_isRunning) break;
    //            ExtractAllInFolder(subDir, depth);
    //        }
    //    }

    //    // =====================================================================
    //    // TIỆN ÍCH
    //    // =====================================================================
    //    private void AppendLog(string message)
    //    {
    //        if (txtLog.InvokeRequired)
    //            txtLog.Invoke(new Action(() =>
    //            {
    //                txtLog.AppendText(message + Environment.NewLine);
    //                txtLog.ScrollToCaret();
    //            }));
    //        else
    //        {
    //            txtLog.AppendText(message + Environment.NewLine);
    //            txtLog.ScrollToCaret();
    //        }
    //    }

    //    private void UpdateProgress()
    //    {
    //        if (lblProgress.InvokeRequired)
    //            lblProgress.Invoke(new Action(() =>
    //                lblProgress.Text = $"Đã giải nén: {_totalExtracted} file | Lỗi: {_totalErrors}"));
    //        else
    //            lblProgress.Text = $"Đã giải nén: {_totalExtracted} file | Lỗi: {_totalErrors}";
    //    }
    //}

    // version này sẽ bị overwrite khi 2 zip cùng cấp

    //public partial class Form1 : Form
    //{
    //    private bool _isRunning = false;
    //    private int _totalExtracted = 0;
    //    private int _totalErrors = 0;

    //    public Form1()
    //    {
    //        InitializeComponent();
    //    }

    //    // =====================================================================
    //    // CHỌN THƯ MỤC
    //    // =====================================================================
    //    private void btnBrowse_Click(object sender, EventArgs e)
    //    {
    //        using (var dlg = new FolderBrowserDialog())
    //        {
    //            dlg.Description = "Chọn thư mục gốc (sẽ tìm và giải nén tất cả ZIP, kể cả lồng nhau)";
    //            if (dlg.ShowDialog() == DialogResult.OK)
    //                txtFolder.Text = dlg.SelectedPath;
    //        }
    //    }

    //    // =====================================================================
    //    // BẮT ĐẦU
    //    // =====================================================================
    //    private async void btnStart_Click(object sender, EventArgs e)
    //    {
    //        if (_isRunning) return;

    //        if (string.IsNullOrWhiteSpace(txtFolder.Text) || !Directory.Exists(txtFolder.Text))
    //        {
    //            MessageBox.Show("Vui lòng chọn thư mục hợp lệ.", "Lỗi",
    //                MessageBoxButtons.OK, MessageBoxIcon.Warning);
    //            return;
    //        }

    //        _isRunning = true;
    //        _totalExtracted = 0;
    //        _totalErrors = 0;
    //        btnStart.Enabled = false;
    //        btnStop.Enabled = true;
    //        progressBar.MarqueeAnimationSpeed = 30;
    //        txtLog.Clear();

    //        string rootFolder = txtFolder.Text;
    //        await Task.Run(() => ExtractAllInFolder(rootFolder, 0));

    //        _isRunning = false;
    //        btnStart.Enabled = true;
    //        btnStop.Enabled = false;
    //        progressBar.MarqueeAnimationSpeed = 0;
    //        UpdateProgress();
    //        AppendLog(new string('=', 60));
    //        AppendLog($"HOÀN THÀNH — Đã giải nén: {_totalExtracted} file | Lỗi: {_totalErrors}");
    //    }

    //    private void btnStop_Click(object sender, EventArgs e)
    //    {
    //        _isRunning = false;
    //        AppendLog("[Người dùng dừng]");
    //    }

    //    private void btnClearLog_Click(object sender, EventArgs e)
    //    {
    //        txtLog.Clear();
    //    }

    //    // =====================================================================
    //    // ĐỆ QUY: giải nén tất cả ZIP trong folder, rồi tiếp tục giải nén
    //    //         các ZIP con vừa được giải nén ra
    //    // =====================================================================
    //    private void ExtractAllInFolder(string folder, int depth)
    //    {
    //        if (!_isRunning) return;
    //        if (depth > 10) return; // giới hạn tránh vòng lặp vô hạn

    //        string indent = new string(' ', depth * 2);

    //        // Xử lý tất cả ZIP trong thư mục hiện tại (không đệ quy vào subdir ở bước này)
    //        var zipFiles = Directory.GetFiles(folder, "*.zip", SearchOption.TopDirectoryOnly);

    //        foreach (var zipPath in zipFiles)
    //        {
    //            if (!_isRunning) break;

    //            AppendLog($"{indent}[Lớp {depth}] {Path.GetFileName(zipPath)}");

    //            try
    //            {
    //                // Giải nén vào cùng thư mục chứa file ZIP
    //                string extractDir = Path.GetDirectoryName(zipPath);

    //                using (var archive = ZipFile.OpenRead(zipPath))
    //                {
    //                    foreach (var entry in archive.Entries)
    //                    {
    //                        if (!_isRunning) break;
    //                        if (string.IsNullOrEmpty(entry.Name)) continue; // bỏ qua entry thư mục

    //                        string destPath = Path.Combine(extractDir,
    //                            entry.FullName.Replace('/', Path.DirectorySeparatorChar));
    //                        string destDir = Path.GetDirectoryName(destPath);

    //                        if (!Directory.Exists(destDir))
    //                            Directory.CreateDirectory(destDir);

    //                        if (File.Exists(destPath) && !chkOverwrite.Checked)
    //                        {
    //                            AppendLog($"{indent}  Bỏ qua (đã tồn tại): {entry.Name}");
    //                            continue;
    //                        }

    //                        entry.ExtractToFile(destPath, overwrite: chkOverwrite.Checked);
    //                        _totalExtracted++;
    //                        UpdateProgress();
    //                        AppendLog($"{indent}  OK: {entry.FullName}");
    //                    }
    //                }

    //                // Xóa ZIP cha nếu được chọn
    //                if (chkDeleteZip.Checked && _isRunning)
    //                {
    //                    File.Delete(zipPath);
    //                    AppendLog($"{indent}  Đã xóa: {Path.GetFileName(zipPath)}");
    //                }

    //                // Sau khi giải nén xong, tiếp tục tìm ZIP con trong cùng thư mục
    //                ExtractAllInFolder(extractDir, depth + 1);
    //            }
    //            catch (Exception ex)
    //            {
    //                _totalErrors++;
    //                AppendLog($"{indent}  LỖI: {ex.Message}");
    //            }
    //        }

    //        // Đệ quy vào các thư mục con có sẵn (không phải từ giải nén)
    //        foreach (var subDir in Directory.GetDirectories(folder, "*", SearchOption.TopDirectoryOnly))
    //        {
    //            if (!_isRunning) break;
    //            ExtractAllInFolder(subDir, depth);
    //        }
    //    }

    //    // =====================================================================
    //    // TIỆN ÍCH
    //    // =====================================================================
    //    private void AppendLog(string message)
    //    {
    //        if (txtLog.InvokeRequired)
    //            txtLog.Invoke(new Action(() =>
    //            {
    //                txtLog.AppendText(message + Environment.NewLine);
    //                txtLog.ScrollToCaret();
    //            }));
    //        else
    //        {
    //            txtLog.AppendText(message + Environment.NewLine);
    //            txtLog.ScrollToCaret();
    //        }
    //    }

    //    private void UpdateProgress()
    //    {
    //        if (lblProgress.InvokeRequired)
    //            lblProgress.Invoke(new Action(() =>
    //                lblProgress.Text = $"Đã giải nén: {_totalExtracted} file | Lỗi: {_totalErrors}"));
    //        else
    //            lblProgress.Text = $"Đã giải nén: {_totalExtracted} file | Lỗi: {_totalErrors}";
    //    }
    //}