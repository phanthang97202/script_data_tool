namespace ScriptDataTool
{
    partial class UnZipForm
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
            this.btnScanSwf = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblFolder
            // 
            this.lblFolder.Location = new System.Drawing.Point(12, 16);
            this.lblFolder.Name = "lblFolder";
            this.lblFolder.Size = new System.Drawing.Size(104, 22);
            this.lblFolder.TabIndex = 0;
            this.lblFolder.Text = "Thư mục nguồn:";
            this.lblFolder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtFolder
            // 
            this.txtFolder.Location = new System.Drawing.Point(122, 14);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.Size = new System.Drawing.Size(446, 29);
            this.txtFolder.TabIndex = 1;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(578, 13);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(100, 26);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Chọn...";
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // lblOutputFolder
            // 
            this.lblOutputFolder.Location = new System.Drawing.Point(12, 48);
            this.lblOutputFolder.Name = "lblOutputFolder";
            this.lblOutputFolder.Size = new System.Drawing.Size(104, 22);
            this.lblOutputFolder.TabIndex = 3;
            this.lblOutputFolder.Text = "Thư mục đích:";
            this.lblOutputFolder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtOutputFolder
            // 
            this.txtOutputFolder.Location = new System.Drawing.Point(122, 46);
            this.txtOutputFolder.Name = "txtOutputFolder";
            this.txtOutputFolder.Size = new System.Drawing.Size(446, 29);
            this.txtOutputFolder.TabIndex = 4;
            // 
            // btnBrowseOutput
            // 
            this.btnBrowseOutput.Location = new System.Drawing.Point(578, 45);
            this.btnBrowseOutput.Name = "btnBrowseOutput";
            this.btnBrowseOutput.Size = new System.Drawing.Size(100, 26);
            this.btnBrowseOutput.TabIndex = 5;
            this.btnBrowseOutput.Text = "Chọn...";
            this.btnBrowseOutput.Click += new System.EventHandler(this.btnBrowseOutput_Click);
            // 
            // chkOverwrite
            // 
            this.chkOverwrite.Location = new System.Drawing.Point(122, 120);
            this.chkOverwrite.Name = "chkOverwrite";
            this.chkOverwrite.Size = new System.Drawing.Size(200, 22);
            this.chkOverwrite.TabIndex = 7;
            this.chkOverwrite.Text = "Ghi đè file đã tồn tại";
            // 
            // chkDeleteZip
            // 
            this.chkDeleteZip.Location = new System.Drawing.Point(334, 120);
            this.chkDeleteZip.Name = "chkDeleteZip";
            this.chkDeleteZip.Size = new System.Drawing.Size(240, 22);
            this.chkDeleteZip.TabIndex = 8;
            this.chkDeleteZip.Text = "Xóa file ZIP/RAR sau khi giải nén";
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(95)))), ((int)(((byte)(168)))));
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.ForeColor = System.Drawing.Color.White;
            this.btnStart.Location = new System.Drawing.Point(122, 150);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(160, 30);
            this.btnStart.TabIndex = 9;
            this.btnStart.Text = "Bắt đầu giải nén";
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(292, 150);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(90, 30);
            this.btnStop.TabIndex = 10;
            this.btnStop.Text = "Dừng";
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnClearLog
            // 
            this.btnClearLog.Location = new System.Drawing.Point(392, 150);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(90, 30);
            this.btnClearLog.TabIndex = 11;
            this.btnClearLog.Text = "Xóa log";
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // lblProgress
            // 
            this.lblProgress.Location = new System.Drawing.Point(12, 194);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(220, 20);
            this.lblProgress.TabIndex = 12;
            this.lblProgress.Text = "Đã giải nén: 0 file";
            this.lblProgress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(238, 194);
            this.progressBar.MarqueeAnimationSpeed = 0;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(440, 20);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 13;
            // 
            // lblLog
            // 
            this.lblLog.Location = new System.Drawing.Point(12, 224);
            this.lblLog.Name = "lblLog";
            this.lblLog.Size = new System.Drawing.Size(60, 20);
            this.lblLog.TabIndex = 14;
            this.lblLog.Text = "Log:";
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(230)))), ((int)(((byte)(200)))));
            this.txtLog.Location = new System.Drawing.Point(12, 246);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(672, 278);
            this.txtLog.TabIndex = 15;
            // 
            // lblNote
            // 
            this.lblNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(200)))));
            this.lblNote.Location = new System.Drawing.Point(12, 78);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(666, 36);
            this.lblNote.TabIndex = 6;
            this.lblNote.Text = "Tool sẽ giải nén ZIP/RAR từ thư mục nguồn sang thư mục đích, giữ nguyên cấu trúc " +
    "thư mục con.";
            // 
            // btnScanSwf
            // 
            this.btnScanSwf.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(95)))), ((int)(((byte)(168)))));
            this.btnScanSwf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnScanSwf.ForeColor = System.Drawing.Color.White;
            this.btnScanSwf.Location = new System.Drawing.Point(502, 150);
            this.btnScanSwf.Name = "btnScanSwf";
            this.btnScanSwf.Size = new System.Drawing.Size(160, 30);
            this.btnScanSwf.TabIndex = 16;
            this.btnScanSwf.Text = "Quét *.swf";
            this.btnScanSwf.UseVisualStyleBackColor = false;
            this.btnScanSwf.Click += new System.EventHandler(this.btnScanSwf_Click);
            // 
            // UnZipForm
            // 
            this.ClientSize = new System.Drawing.Size(700, 540);
            this.Controls.Add(this.btnScanSwf);
            this.Controls.Add(this.lblFolder);
            this.Controls.Add(this.txtFolder);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.lblOutputFolder);
            this.Controls.Add(this.txtOutputFolder);
            this.Controls.Add(this.btnBrowseOutput);
            this.Controls.Add(this.lblNote);
            this.Controls.Add(this.chkOverwrite);
            this.Controls.Add(this.chkDeleteZip);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnClearLog);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lblLog);
            this.Controls.Add(this.txtLog);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "UnZipForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Giải Nén Đệ Quy (ZIP/RAR lồng nhau) - Thư Viện Y Dược Hải Phòng";
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
        private System.Windows.Forms.Button btnScanSwf;
    }
}