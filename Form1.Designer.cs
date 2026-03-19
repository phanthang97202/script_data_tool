namespace ScriptDataTool
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblFolder = new System.Windows.Forms.Label();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.lblOutputFolder = new System.Windows.Forms.Label();
            this.txtOutputFolder = new System.Windows.Forms.TextBox();
            this.btnBrowseOutput = new System.Windows.Forms.Button();
            this.chkOverwrite = new System.Windows.Forms.CheckBox();
            this.chkDeleteZip = new System.Windows.Forms.CheckBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.lblProgress = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblLog = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.lblNote = new System.Windows.Forms.Label();
            this.SuspendLayout();

            // ── Form ──────────────────────────────────────────────────────────
            this.Text = "Giải Nén Đệ Quy (ZIP/RAR lồng nhau) - Thư Viện Y Dược Hải Phòng";
            this.ClientSize = new System.Drawing.Size(700, 540);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F);

            // ── Row 1: Thư mục nguồn ─────────────────────────────────────────
            this.lblFolder.Text = "Thư mục nguồn:";
            this.lblFolder.Location = new System.Drawing.Point(12, 16);
            this.lblFolder.Size = new System.Drawing.Size(104, 22);
            this.lblFolder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.txtFolder.Location = new System.Drawing.Point(122, 14);
            this.txtFolder.Size = new System.Drawing.Size(446, 24);

            this.btnBrowse.Text = "Chọn...";
            this.btnBrowse.Location = new System.Drawing.Point(578, 13);
            this.btnBrowse.Size = new System.Drawing.Size(100, 26);
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);

            // ── Row 2: Thư mục đích (OUTPUT) ─────────────────────────────────
            this.lblOutputFolder.Text = "Thư mục đích:";
            this.lblOutputFolder.Location = new System.Drawing.Point(12, 48);
            this.lblOutputFolder.Size = new System.Drawing.Size(104, 22);
            this.lblOutputFolder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.txtOutputFolder.Location = new System.Drawing.Point(122, 46);
            this.txtOutputFolder.Size = new System.Drawing.Size(446, 24);

            this.btnBrowseOutput.Text = "Chọn...";
            this.btnBrowseOutput.Location = new System.Drawing.Point(578, 45);
            this.btnBrowseOutput.Size = new System.Drawing.Size(100, 26);
            this.btnBrowseOutput.Click += new System.EventHandler(this.btnBrowseOutput_Click);

            // ── Note ──────────────────────────────────────────────────────────
            this.lblNote.Text = "Tool sẽ giải nén ZIP/RAR từ thư mục nguồn sang thư mục đích, giữ nguyên cấu trúc thư mục con.";
            this.lblNote.Location = new System.Drawing.Point(12, 78);
            this.lblNote.Size = new System.Drawing.Size(666, 36);
            this.lblNote.ForeColor = System.Drawing.Color.FromArgb(80, 80, 200);

            // ── Checkboxes ────────────────────────────────────────────────────
            this.chkOverwrite.Text = "Ghi đè file đã tồn tại";
            this.chkOverwrite.Location = new System.Drawing.Point(122, 120);
            this.chkOverwrite.Size = new System.Drawing.Size(200, 22);
            this.chkOverwrite.Checked = false;

            this.chkDeleteZip.Text = "Xóa file ZIP/RAR sau khi giải nén";
            this.chkDeleteZip.Location = new System.Drawing.Point(334, 120);
            this.chkDeleteZip.Size = new System.Drawing.Size(240, 22);
            this.chkDeleteZip.Checked = false;

            // ── Buttons ───────────────────────────────────────────────────────
            this.btnStart.Text = "Bắt đầu giải nén";
            this.btnStart.Location = new System.Drawing.Point(122, 150);
            this.btnStart.Size = new System.Drawing.Size(160, 30);
            this.btnStart.BackColor = System.Drawing.Color.FromArgb(26, 95, 168);
            this.btnStart.ForeColor = System.Drawing.Color.White;
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);

            this.btnStop.Text = "Dừng";
            this.btnStop.Location = new System.Drawing.Point(292, 150);
            this.btnStop.Size = new System.Drawing.Size(90, 30);
            this.btnStop.Enabled = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);

            this.btnClearLog.Text = "Xóa log";
            this.btnClearLog.Location = new System.Drawing.Point(392, 150);
            this.btnClearLog.Size = new System.Drawing.Size(90, 30);
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);

            // ── Progress ──────────────────────────────────────────────────────
            this.lblProgress.Text = "Đã giải nén: 0 file";
            this.lblProgress.Location = new System.Drawing.Point(12, 194);
            this.lblProgress.Size = new System.Drawing.Size(220, 20);
            this.lblProgress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.progressBar.Location = new System.Drawing.Point(238, 194);
            this.progressBar.Size = new System.Drawing.Size(440, 20);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.MarqueeAnimationSpeed = 0;

            // ── Log ───────────────────────────────────────────────────────────
            this.lblLog.Text = "Log:";
            this.lblLog.Location = new System.Drawing.Point(12, 224);
            this.lblLog.Size = new System.Drawing.Size(60, 20);

            this.txtLog.Location = new System.Drawing.Point(12, 246);
            this.txtLog.Size = new System.Drawing.Size(672, 278);
            this.txtLog.Multiline = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.ReadOnly = true;
            this.txtLog.BackColor = System.Drawing.Color.FromArgb(20, 20, 20);
            this.txtLog.ForeColor = System.Drawing.Color.FromArgb(200, 230, 200);
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9F);

            // ── Add controls ─────────────────────────────────────────────────
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblFolder,       txtFolder,       btnBrowse,
                lblOutputFolder, txtOutputFolder, btnBrowseOutput,
                lblNote,
                chkOverwrite, chkDeleteZip,
                btnStart, btnStop, btnClearLog,
                lblProgress, progressBar,
                lblLog, txtLog
            });

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // ── Fields ────────────────────────────────────────────────────────────
        private System.Windows.Forms.Label lblFolder;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label lblOutputFolder;
        private System.Windows.Forms.TextBox txtOutputFolder;
        private System.Windows.Forms.Button btnBrowseOutput;
        private System.Windows.Forms.Label lblNote;
        private System.Windows.Forms.CheckBox chkOverwrite;
        private System.Windows.Forms.CheckBox chkDeleteZip;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblLog;
        private System.Windows.Forms.TextBox txtLog;
    }
}