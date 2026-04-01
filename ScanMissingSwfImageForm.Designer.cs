namespace ScriptDataTool
{
    partial class ScanMissingSwfImageForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblSource = new System.Windows.Forms.Label();
            this.txtSourceFolder = new System.Windows.Forms.TextBox();
            this.btnChooseFolder = new System.Windows.Forms.Button();
            this.lblConverter = new System.Windows.Forms.Label();
            this.txtConverterPath = new System.Windows.Forms.TextBox();
            this.btnChooseConverter = new System.Windows.Forms.Button();
            this.lblFfdec = new System.Windows.Forms.Label();
            this.txtFfdecPath = new System.Windows.Forms.TextBox();
            this.btnChooseFfdec = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.btnCreateMets = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblSource
            // 
            this.lblSource.Location = new System.Drawing.Point(12, 16);
            this.lblSource.Name = "lblSource";
            this.lblSource.Size = new System.Drawing.Size(130, 24);
            this.lblSource.TabIndex = 0;
            this.lblSource.Text = "Thư mục nguồn:";
            this.lblSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtSourceFolder
            // 
            this.txtSourceFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSourceFolder.Location = new System.Drawing.Point(148, 13);
            this.txtSourceFolder.Name = "txtSourceFolder";
            this.txtSourceFolder.Size = new System.Drawing.Size(676, 29);
            this.txtSourceFolder.TabIndex = 0;
            // 
            // btnChooseFolder
            // 
            this.btnChooseFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChooseFolder.Location = new System.Drawing.Point(834, 12);
            this.btnChooseFolder.Name = "btnChooseFolder";
            this.btnChooseFolder.Size = new System.Drawing.Size(100, 28);
            this.btnChooseFolder.TabIndex = 1;
            this.btnChooseFolder.Text = "Chọn...";
            this.btnChooseFolder.UseVisualStyleBackColor = true;
            this.btnChooseFolder.Click += new System.EventHandler(this.btnChooseFolder_Click);
            // 
            // lblConverter
            // 
            this.lblConverter.Location = new System.Drawing.Point(12, 48);
            this.lblConverter.Name = "lblConverter";
            this.lblConverter.Size = new System.Drawing.Size(130, 24);
            this.lblConverter.TabIndex = 2;
            this.lblConverter.Text = "SwfToJpgConverter:";
            this.lblConverter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtConverterPath
            // 
            this.txtConverterPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConverterPath.Location = new System.Drawing.Point(148, 45);
            this.txtConverterPath.Name = "txtConverterPath";
            this.txtConverterPath.Size = new System.Drawing.Size(676, 29);
            this.txtConverterPath.TabIndex = 2;
            // 
            // btnChooseConverter
            // 
            this.btnChooseConverter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChooseConverter.Location = new System.Drawing.Point(834, 44);
            this.btnChooseConverter.Name = "btnChooseConverter";
            this.btnChooseConverter.Size = new System.Drawing.Size(100, 28);
            this.btnChooseConverter.TabIndex = 3;
            this.btnChooseConverter.Text = "Chọn...";
            this.btnChooseConverter.UseVisualStyleBackColor = true;
            this.btnChooseConverter.Click += new System.EventHandler(this.btnChooseConverter_Click);
            // 
            // lblFfdec
            // 
            this.lblFfdec.Location = new System.Drawing.Point(12, 80);
            this.lblFfdec.Name = "lblFfdec";
            this.lblFfdec.Size = new System.Drawing.Size(130, 24);
            this.lblFfdec.TabIndex = 4;
            this.lblFfdec.Text = "ffdec-cli.exe:";
            this.lblFfdec.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtFfdecPath
            // 
            this.txtFfdecPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFfdecPath.Location = new System.Drawing.Point(148, 77);
            this.txtFfdecPath.Name = "txtFfdecPath";
            this.txtFfdecPath.Size = new System.Drawing.Size(676, 29);
            this.txtFfdecPath.TabIndex = 4;
            // 
            // btnChooseFfdec
            // 
            this.btnChooseFfdec.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChooseFfdec.Location = new System.Drawing.Point(834, 76);
            this.btnChooseFfdec.Name = "btnChooseFfdec";
            this.btnChooseFfdec.Size = new System.Drawing.Size(100, 28);
            this.btnChooseFfdec.TabIndex = 5;
            this.btnChooseFfdec.Text = "Chọn...";
            this.btnChooseFfdec.UseVisualStyleBackColor = true;
            this.btnChooseFfdec.Click += new System.EventHandler(this.btnChooseFfdec_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.ForeColor = System.Drawing.Color.DodgerBlue;
            this.lblStatus.Location = new System.Drawing.Point(12, 112);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(924, 22);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "Sẵn sàng.";
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(12, 138);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(924, 16);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 7;
            this.progressBar.Visible = false;
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.ForeColor = System.Drawing.Color.White;
            this.btnStart.Location = new System.Drawing.Point(12, 164);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(200, 32);
            this.btnStart.TabIndex = 6;
            this.btnStart.Text = "Bắt đầu quét & Convert";
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(222, 164);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(90, 32);
            this.btnStop.TabIndex = 7;
            this.btnStop.Text = "Dừng";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnClearLog
            // 
            this.btnClearLog.Location = new System.Drawing.Point(322, 164);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(90, 32);
            this.btnClearLog.TabIndex = 8;
            this.btnClearLog.Text = "Xóa log";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // rtbLog
            // 
            this.rtbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbLog.BackColor = System.Drawing.Color.Black;
            this.rtbLog.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbLog.ForeColor = System.Drawing.Color.Lime;
            this.rtbLog.Location = new System.Drawing.Point(12, 206);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbLog.Size = new System.Drawing.Size(924, 400);
            this.rtbLog.TabIndex = 9;
            this.rtbLog.Text = "";
            // 
            // btnCreateMets
            // 
            this.btnCreateMets.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnCreateMets.Location = new System.Drawing.Point(418, 164);
            this.btnCreateMets.Name = "btnCreateMets";
            this.btnCreateMets.Size = new System.Drawing.Size(119, 32);
            this.btnCreateMets.TabIndex = 10;
            this.btnCreateMets.Text = "Tạo METS XML";
            this.btnCreateMets.UseVisualStyleBackColor = false;
            this.btnCreateMets.Click += new System.EventHandler(this.btnCreateMets_Click);
            // 
            // ScanMissingSwfImageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(948, 618);
            this.Controls.Add(this.btnCreateMets);
            this.Controls.Add(this.lblSource);
            this.Controls.Add(this.txtSourceFolder);
            this.Controls.Add(this.btnChooseFolder);
            this.Controls.Add(this.lblConverter);
            this.Controls.Add(this.txtConverterPath);
            this.Controls.Add(this.btnChooseConverter);
            this.Controls.Add(this.lblFfdec);
            this.Controls.Add(this.txtFfdecPath);
            this.Controls.Add(this.btnChooseFfdec);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnClearLog);
            this.Controls.Add(this.rtbLog);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(800, 560);
            this.Name = "ScanMissingSwfImageForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quét & Convert SWF → JPG — Thư Viện Y Dược Hải Phòng";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.TextBox txtSourceFolder;
        private System.Windows.Forms.Button btnChooseFolder;
        private System.Windows.Forms.Label lblConverter;
        private System.Windows.Forms.TextBox txtConverterPath;
        private System.Windows.Forms.Button btnChooseConverter;
        private System.Windows.Forms.Label lblFfdec;
        private System.Windows.Forms.TextBox txtFfdecPath;
        private System.Windows.Forms.Button btnChooseFfdec;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.Button btnCreateMets;
    }
}