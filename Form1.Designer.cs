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

            // Form
            this.Text = "Giải Nén Đệ Quy (ZIP/RAR lồng nhau) - Thư Viện Y Dược Hải Phòng";
            this.ClientSize = new System.Drawing.Size(700, 500);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F);

            // lblFolder
            this.lblFolder.Text = "Thư mục gốc:";
            this.lblFolder.Location = new System.Drawing.Point(12, 16);
            this.lblFolder.Size = new System.Drawing.Size(100, 22);
            this.lblFolder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // txtFolder
            this.txtFolder.Location = new System.Drawing.Point(118, 14);
            this.txtFolder.Size = new System.Drawing.Size(450, 24);

            // btnBrowse
            this.btnBrowse.Text = "Chọn...";
            this.btnBrowse.Location = new System.Drawing.Point(578, 13);
            this.btnBrowse.Size = new System.Drawing.Size(100, 26);
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);

            // lblNote — giải thích cách hoạt động
            this.lblNote.Text = "Tool sẽ giải nén tất cả ZIP/RAR trong thư mục gốc, sau đó tiếp tục giải nén các ZIP/RAR con vừa được giải nén ra (đệ quy nhiều lớp).";
            this.lblNote.Location = new System.Drawing.Point(12, 46);
            this.lblNote.Size = new System.Drawing.Size(666, 36);
            this.lblNote.ForeColor = System.Drawing.Color.FromArgb(80, 80, 200);

            // chkOverwrite
            this.chkOverwrite.Text = "Ghi đè file đã tồn tại";
            this.chkOverwrite.Location = new System.Drawing.Point(118, 88);
            this.chkOverwrite.Size = new System.Drawing.Size(200, 22);
            this.chkOverwrite.Checked = false;

            // chkDeleteZip
            this.chkDeleteZip.Text = "Xóa file ZIP/RAR sau khi giải nén";
            this.chkDeleteZip.Location = new System.Drawing.Point(330, 88);
            this.chkDeleteZip.Size = new System.Drawing.Size(240, 22);
            this.chkDeleteZip.Checked = false;

            // btnStart
            this.btnStart.Text = "Bắt đầu giải nén";
            this.btnStart.Location = new System.Drawing.Point(118, 118);
            this.btnStart.Size = new System.Drawing.Size(160, 30);
            this.btnStart.BackColor = System.Drawing.Color.FromArgb(26, 95, 168);
            this.btnStart.ForeColor = System.Drawing.Color.White;
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);

            // btnStop
            this.btnStop.Text = "Dừng";
            this.btnStop.Location = new System.Drawing.Point(288, 118);
            this.btnStop.Size = new System.Drawing.Size(90, 30);
            this.btnStop.Enabled = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);

            // btnClearLog
            this.btnClearLog.Text = "Xóa log";
            this.btnClearLog.Location = new System.Drawing.Point(388, 118);
            this.btnClearLog.Size = new System.Drawing.Size(90, 30);
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);

            // lblProgress
            this.lblProgress.Text = "Đã giải nén: 0 file";
            this.lblProgress.Location = new System.Drawing.Point(12, 160);
            this.lblProgress.Size = new System.Drawing.Size(200, 20);
            this.lblProgress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // progressBar (dùng làm spinner vì không biết tổng số)
            this.progressBar.Location = new System.Drawing.Point(220, 160);
            this.progressBar.Size = new System.Drawing.Size(458, 20);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.MarqueeAnimationSpeed = 0; // tắt, chỉ bật khi chạy

            // lblLog
            this.lblLog.Text = "Log:";
            this.lblLog.Location = new System.Drawing.Point(12, 190);
            this.lblLog.Size = new System.Drawing.Size(60, 20);

            // txtLog
            this.txtLog.Location = new System.Drawing.Point(12, 212);
            this.txtLog.Size = new System.Drawing.Size(672, 270);
            this.txtLog.Multiline = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.ReadOnly = true;
            this.txtLog.BackColor = System.Drawing.Color.FromArgb(20, 20, 20);
            this.txtLog.ForeColor = System.Drawing.Color.FromArgb(200, 230, 200);
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9F);

            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblFolder, txtFolder, btnBrowse,
                lblNote,
                chkOverwrite, chkDeleteZip,
                btnStart, btnStop, btnClearLog,
                lblProgress, progressBar,
                lblLog, txtLog
            });

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblFolder;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.Button btnBrowse;
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

