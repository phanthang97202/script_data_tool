using System.Drawing;
using System.Windows.Forms;

namespace ScriptDataTool
{
    partial class FlippingBookToMetsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region ======== UI CONTROLS (Designer code - khai báo thủ công) ========

        private System.Windows.Forms.Label lblPhysical;
        private System.Windows.Forms.TextBox txtPhysicalRoot;
        private System.Windows.Forms.Button btnBrowsePhysical;

        private System.Windows.Forms.Label lblVirtual;
        private System.Windows.Forms.TextBox txtVirtualRoot;

        private System.Windows.Forms.Label lblOutput;
        private System.Windows.Forms.TextBox txtOutputDir;
        private System.Windows.Forms.Button btnBrowseOutput;

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblProgress;

        private System.Windows.Forms.RichTextBox rtbLog;

        private System.Windows.Forms.Label lblStatus;

        private void InitializeComponent()
        {
            this.Text = "Flipping Book to METS XML Converter";
            this.Width = 820;
            this.Height = 620;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9f);

            int xLabel = 12, xInput = 200, xBtn = 700, wInput = 490, wLabel = 185;
            int yRow1 = 18, yRow2 = 52, yRow3 = 86;

            // --- Row 1: Physical Root ---
            lblPhysical = new Label { Text = "Root Physical Directory:", Left = xLabel, Top = yRow1 + 4, Width = wLabel };
            txtPhysicalRoot = new TextBox { Left = xInput, Top = yRow1, Width = wInput, Text = "E:\\NDUNEDATA" };
            btnBrowsePhysical = new Button { Text = "📁", Left = xBtn, Top = yRow1 - 1, Width = 32, Height = 24 };
            btnBrowsePhysical.Click += (s, e) => BrowseFolder(txtPhysicalRoot);

            // --- Row 2: Virtual Root ---
            lblVirtual = new Label { Text = "Root Virtual Directory:", Left = xLabel, Top = yRow2 + 4, Width = wLabel };
            txtVirtualRoot = new TextBox { Left = xInput, Top = yRow2, Width = wInput + 36, Text = "https://lib.yourhost.edu.vn/KIPOSDATA1" };

            // --- Row 3: Output METS Dir ---
            lblOutput = new Label { Text = "Output METS Directory:", Left = xLabel, Top = yRow3 + 4, Width = wLabel };
            txtOutputDir = new TextBox { Left = xInput, Top = yRow3, Width = wInput, Text = "E:\\NDUNEDATA_METS" };
            btnBrowseOutput = new Button { Text = "📁", Left = xBtn, Top = yRow3 - 1, Width = 32, Height = 24 };
            btnBrowseOutput.Click += (s, e) => BrowseFolder(txtOutputDir);

            // --- Separator ---
            var sep = new Panel { Left = 0, Top = 120, Width = 820, Height = 1, BackColor = Color.LightGray };

            // --- Buttons ---
            btnStart = new Button { Text = "▶ Start Convert", Left = 12, Top = 132, Width = 150, Height = 30 };
            btnStart.BackColor = Color.FromArgb(0, 120, 215);
            btnStart.ForeColor = Color.White;
            btnStart.FlatStyle = FlatStyle.Flat;
            btnStart.Click += BtnStart_Click;

            btnStop = new Button { Text = "✖ Stop", Left = 170, Top = 132, Width = 100, Height = 30, Enabled = false };
            btnStop.BackColor = Color.FromArgb(200, 60, 60);
            btnStop.ForeColor = Color.White;
            btnStop.FlatStyle = FlatStyle.Flat;
            btnStop.Click += BtnStop_Click;

            // --- Progress ---
            progressBar = new ProgressBar { Left = 12, Top = 175, Width = 770, Height = 20, Minimum = 0 };
            lblProgress = new Label { Text = "Sẵn sàng", Left = 12, Top = 200, Width = 770, ForeColor = Color.DimGray };

            // --- Log ---
            var lblLog = new Label { Text = "Log:", Left = 12, Top = 220, Width = 50 };
            rtbLog = new RichTextBox
            {
                Left = 12,
                Top = 238,
                Width = 774,
                Height = 310,
                ReadOnly = true,
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.LightGreen,
                Font = new Font("Consolas", 9f),
                ScrollBars = RichTextBoxScrollBars.Vertical
            };

            // --- Status bar ---
            lblStatus = new Label
            {
                Text = "Chưa chạy",
                Left = 12,
                Top = 556,
                Width = 774,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 8f)
            };

            this.Controls.AddRange(new Control[]
            {
                lblPhysical, txtPhysicalRoot, btnBrowsePhysical,
                lblVirtual, txtVirtualRoot,
                lblOutput, txtOutputDir, btnBrowseOutput,
                sep,
                btnStart, btnStop,
                progressBar, lblProgress,
                lblLog, rtbLog,
                lblStatus
            });

            this.FormClosing += Form1_FormClosing;
        }

        #endregion
    }
}