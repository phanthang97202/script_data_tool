namespace ScriptDataTool
{
    partial class ScanMissingSwfImageForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblSource = new System.Windows.Forms.Label();
            this.txtSourceFolder = new System.Windows.Forms.TextBox();
            this.btnChooseFolder = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();

            // ──────────────────────────────────────────
            // lblSource
            // ──────────────────────────────────────────
            this.lblSource.AutoSize = false;
            this.lblSource.Location = new System.Drawing.Point(12, 18);
            this.lblSource.Name = "lblSource";
            this.lblSource.Size = new System.Drawing.Size(118, 24);
            this.lblSource.TabIndex = 0;
            this.lblSource.Text = "Thư mục nguồn:";
            this.lblSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // ──────────────────────────────────────────
            // txtSourceFolder
            // ──────────────────────────────────────────
            this.txtSourceFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(
                                              System.Windows.Forms.AnchorStyles.Top |
                                              System.Windows.Forms.AnchorStyles.Left |
                                              System.Windows.Forms.AnchorStyles.Right));
            this.txtSourceFolder.Location = new System.Drawing.Point(136, 15);
            this.txtSourceFolder.Name = "txtSourceFolder";
            this.txtSourceFolder.Size = new System.Drawing.Size(690, 26);
            this.txtSourceFolder.TabIndex = 1;

            // ──────────────────────────────────────────
            // btnChooseFolder
            // ──────────────────────────────────────────
            this.btnChooseFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(
                                              System.Windows.Forms.AnchorStyles.Top |
                                              System.Windows.Forms.AnchorStyles.Right));
            this.btnChooseFolder.Location = new System.Drawing.Point(836, 14);
            this.btnChooseFolder.Name = "btnChooseFolder";
            this.btnChooseFolder.Size = new System.Drawing.Size(100, 28);
            this.btnChooseFolder.TabIndex = 2;
            this.btnChooseFolder.Text = "Chọn...";
            this.btnChooseFolder.UseVisualStyleBackColor = true;
            this.btnChooseFolder.Click += new System.EventHandler(this.btnChooseFolder_Click);

            // ──────────────────────────────────────────
            // lblStatus
            // ──────────────────────────────────────────
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(
                                         System.Windows.Forms.AnchorStyles.Top |
                                         System.Windows.Forms.AnchorStyles.Left |
                                         System.Windows.Forms.AnchorStyles.Right));
            this.lblStatus.ForeColor = System.Drawing.Color.DodgerBlue;
            this.lblStatus.Location = new System.Drawing.Point(12, 52);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(924, 22);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Sẵn sàng.";

            // ──────────────────────────────────────────
            // progressBar
            // ──────────────────────────────────────────
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(
                                         System.Windows.Forms.AnchorStyles.Top |
                                         System.Windows.Forms.AnchorStyles.Left |
                                         System.Windows.Forms.AnchorStyles.Right));
            this.progressBar.Location = new System.Drawing.Point(12, 78);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(924, 18);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 4;
            this.progressBar.Visible = false;

            // ──────────────────────────────────────────
            // btnStart
            // ──────────────────────────────────────────
            this.btnStart.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.ForeColor = System.Drawing.Color.White;
            this.btnStart.Location = new System.Drawing.Point(12, 106);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(140, 32);
            this.btnStart.TabIndex = 5;
            this.btnStart.Text = "Bắt đầu quét";
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);

            // ──────────────────────────────────────────
            // btnStop
            // ──────────────────────────────────────────
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(162, 106);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(100, 32);
            this.btnStop.TabIndex = 6;
            this.btnStop.Text = "Dừng";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);

            // ──────────────────────────────────────────
            // btnClearLog
            // ──────────────────────────────────────────
            this.btnClearLog.Location = new System.Drawing.Point(272, 106);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(100, 32);
            this.btnClearLog.TabIndex = 7;
            this.btnClearLog.Text = "Xóa log";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);

            // ──────────────────────────────────────────
            // rtbLog
            // ──────────────────────────────────────────
            this.rtbLog.Anchor = ((System.Windows.Forms.AnchorStyles)(
                                   System.Windows.Forms.AnchorStyles.Top |
                                   System.Windows.Forms.AnchorStyles.Bottom |
                                   System.Windows.Forms.AnchorStyles.Left |
                                   System.Windows.Forms.AnchorStyles.Right));
            this.rtbLog.BackColor = System.Drawing.Color.Black;
            this.rtbLog.ForeColor = System.Drawing.Color.Lime;
            this.rtbLog.Font = new System.Drawing.Font("Consolas", 9F,
                                          System.Drawing.FontStyle.Regular,
                                          System.Drawing.GraphicsUnit.Point, 0);
            this.rtbLog.Location = new System.Drawing.Point(12, 148);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbLog.Size = new System.Drawing.Size(924, 432);
            this.rtbLog.TabIndex = 8;
            this.rtbLog.Text = "";

            // ──────────────────────────────────────────
            // ScanMissingSwfImageForm
            // ──────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(948, 592);
            this.Controls.Add(this.lblSource);
            this.Controls.Add(this.txtSourceFolder);
            this.Controls.Add(this.btnChooseFolder);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnClearLog);
            this.Controls.Add(this.rtbLog);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F,
                                       System.Drawing.FontStyle.Regular,
                                       System.Drawing.GraphicsUnit.Point, 0);
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "ScanMissingSwfImageForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quét SWF Thiếu Ảnh Medium — Thư Viện Y Dược Hải Phòng";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        // ──────────────────────────────────────────────────────────────
        // Designer fields
        // ──────────────────────────────────────────────────────────────
        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.TextBox txtSourceFolder;
        private System.Windows.Forms.Button btnChooseFolder;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.RichTextBox rtbLog;
    }
}