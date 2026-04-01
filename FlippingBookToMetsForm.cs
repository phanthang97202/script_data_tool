using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using MetsXmlParser;

namespace ScriptDataTool
{
    public partial class FlippingBookToMetsForm : Form
    {
        private Thread _workerThread;
        private volatile bool _isCancelled = false;

        public FlippingBookToMetsForm()
        {
            InitializeComponent();
        }

        #region ======== UI Helpers ========

        private void BrowseFolder(TextBox target)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.SelectedPath = target.Text;
                if (dlg.ShowDialog() == DialogResult.OK)
                    target.Text = dlg.SelectedPath;
            }
        }

        private void AppendLog(string message, Color color)
        {
            if (rtbLog.InvokeRequired)
            {
                rtbLog.Invoke(new Action(() => AppendLog(message, color)));
                return;
            }
            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.SelectionLength = 0;
            rtbLog.SelectionColor = color;
            rtbLog.AppendText(DateTime.Now.ToString("HH:mm:ss") + " " + message + "\n");
            rtbLog.ScrollToCaret();
        }

        private void SetProgress(int current, int total)
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke(new Action(() => SetProgress(current, total)));
                return;
            }
            progressBar.Maximum = total > 0 ? total : 1;
            progressBar.Value = Math.Min(current, progressBar.Maximum);
            lblProgress.Text = $"Đang xử lý: {current} / {total}";
        }

        private void SetStatus(string text)
        {
            if (lblStatus.InvokeRequired)
                lblStatus.Invoke(new Action(() => lblStatus.Text = text));
            else
                lblStatus.Text = text;
        }

        private void SetUIRunning(bool running)
        {
            if (btnStart.InvokeRequired)
            {
                btnStart.Invoke(new Action(() => SetUIRunning(running)));
                return;
            }
            btnStart.Enabled = !running;
            btnStop.Enabled = running;
            txtPhysicalRoot.Enabled = !running;
            txtVirtualRoot.Enabled = !running;
            txtOutputDir.Enabled = !running;
        }

        #endregion

        #region ======== Start / Stop ========

        private void BtnStart_Click(object sender, EventArgs e)
        {
            string sPhysicalRoot = txtPhysicalRoot.Text.Trim();
            string sVirtualRoot = txtVirtualRoot.Text.Trim();
            string sOutputDir = txtOutputDir.Text.Trim();

            if (string.IsNullOrEmpty(sPhysicalRoot) || string.IsNullOrEmpty(sVirtualRoot) || string.IsNullOrEmpty(sOutputDir))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ các tham số thư mục.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _isCancelled = false;
            rtbLog.Clear();
            SetUIRunning(true);
            SetStatus("Đang chạy...");

            _workerThread = new Thread(() => DoConvert(sPhysicalRoot, sVirtualRoot, sOutputDir));
            _workerThread.IsBackground = true;
            _workerThread.Start();
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn dừng quá trình convert?", "Xác nhận dừng",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _isCancelled = true;
                AppendLog("⚠ Người dùng yêu cầu dừng. Đang hoàn tất record hiện tại...", Color.Yellow);
                SetStatus("Đang dừng...");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_workerThread != null && _workerThread.IsAlive)
            {
                if (MessageBox.Show("Quá trình convert đang chạy. Bạn có muốn thoát không?",
                    "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
                _isCancelled = true;
            }
        }

        #endregion

        #region ======== Main Convert Worker ========

        private void DoConvert(string sPhysicalRoot, string sVirtualRoot, string sOutputDir)
        {
            try
            {
                AppendLog("▶ Bắt đầu lấy dữ liệu từ CSDL...", Color.Cyan);
                DataTable dtRecords = GetFlippingBookData();
                int iTotal = dtRecords.Rows.Count;
                AppendLog($"✔ Tổng số tài liệu cần xử lý: {iTotal}", Color.LightGreen);
                SetProgress(0, iTotal);

                int iDone = 0, iError = 0, iLine = 0;

                foreach (DataRow row in dtRecords.Rows)
                {
                    if (_isCancelled) break;

                    iLine++;
                    string sId = row["Id"].ToString();
                    string fileNameOldHPMU = (row["File_Name"] == DBNull.Value || string.IsNullOrWhiteSpace(row["File_Name"].ToString()))
                         ? "No name"
                         : row["File_Name"].ToString();
                    string sFilePathKps = row["File_Path_KPS"].ToString();
                    string sBibTypeName = row["Bib_Type_Name"].ToString();
                    string sBibXmlData = row["BIB_XMLDATA"] == DBNull.Value ? "" : row["BIB_XMLDATA"].ToString();

                    AppendLog($"[{iLine}/{iTotal}] Xử lý: {sFilePathKps}", Color.White);

                    try
                    {
                        string sSafeFolderName = MakeSafeDirectoryName(sBibTypeName);
                        if (string.IsNullOrEmpty(sSafeFolderName)) sSafeFolderName = "Khac";

                        string sOutputFolder = Path.Combine(sOutputDir, sSafeFolderName);
                        if (!Directory.Exists(sOutputFolder))
                            Directory.CreateDirectory(sOutputFolder);

                        string sBookFolderName = new DirectoryInfo(sFilePathKps).Name;
                        string sOutputMetsFile = GetUniqueFilePath(Path.Combine(sOutputFolder, sBookFolderName + ".xml"));


                        string sMetsXml = FlippingBookToMetsConverter.ConvertFlippingBookToMets(
                            fileNameOldHPMU,
                            sFilePathKps,
                            sPhysicalRoot,
                            sVirtualRoot,
                            sBibXmlData
                        );

                        if (string.IsNullOrEmpty(sMetsXml))
                            throw new Exception("Converter trả về METS XML rỗng");

                        sMetsXml = sMetsXml.Replace("&#xD;", "").Replace("&#xB;", "");
                        Cmets myMets = new Cmets();
                        myMets.load_Xml(sMetsXml);
                        myMets.ResolveMetsNamespaces();

                        myMets.Save(sOutputMetsFile);

                        UpdateMetsStatus(sId, 1, "OK", sOutputMetsFile);

                        iDone++;
                        AppendLog($"  ✔ Done → {sOutputMetsFile}", Color.LightGreen);
                    }
                    catch (Exception ex)
                    {
                        iError++;
                        UpdateMetsStatus(sId, -1, ex.Message, null);
                        AppendLog($"  ✗ Lỗi: {ex.Message}", Color.OrangeRed);
                    }

                    SetProgress(iLine, iTotal);
                }

                string sSummary = _isCancelled
                    ? $"⚠ Đã dừng giữa chừng. Hoàn thành: {iDone}, Lỗi: {iError}, Còn lại: {iTotal - iLine}"
                    : $"✔ Hoàn tất! Tổng: {iTotal} | Done: {iDone} | Lỗi: {iError}";

                AppendLog(sSummary, Color.Cyan);
                SetStatus(sSummary);
            }
            catch (Exception ex)
            {
                AppendLog($"✗ Lỗi nghiêm trọng: {ex.Message}", Color.Red);
                SetStatus("Lỗi: " + ex.Message);
            }
            finally
            {
                SetUIRunning(false);
            }
        }

        #endregion

        #region ======== Database Operations ========

        private DataTable GetFlippingBookData()
        {
            string sConnStr = ConfigurationManager.ConnectionStrings["MyDbConn"].ConnectionString;
            string sSql = @"
                SELECT 
                    s.Id,
                    s.File_Name,
                    s.File_Path_KPS,
                    ISNULL(s.Bib_Type_Name, 'Khac') AS Bib_Type_Name,
                    b.BIB_XMLDATA
                FROM ScriptMappingData s
                JOIN Dl_Bib b ON s.Bib_Id_ILib = b.ID
                WHERE (s.METS_Status IS NULL OR s.METS_Status = 0)
                  AND s.File_Path_KPS IS NOT NULL
                  AND s.File_Path_KPS <> ''
                ORDER BY s.Id ASC";

            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(sConnStr))
            {
                SqlCommand cmd = new SqlCommand(sSql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 120;
                SqlDataAdapter adap = new SqlDataAdapter(cmd);
                conn.Open();
                adap.Fill(ds);
                conn.Close();
            }
            return ds.Tables[0];
        }

        private void UpdateMetsStatus(string sId, short iStatus, string sLog, string sMetsPath)
        {
            string sConnStr = ConfigurationManager.ConnectionStrings["MyDbConn"].ConnectionString;
            string sSql = @"
                UPDATE ScriptMappingData
                SET METS_Status = @_Status,
                    METS_Log    = @_Log,
                    METS_Path   = @_MetsPath
                WHERE Id = @_Id";

            using (SqlConnection conn = new SqlConnection(sConnStr))
            {
                SqlCommand cmd = new SqlCommand(sSql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add(new SqlParameter("@_Id", SqlDbType.NVarChar, 50) { Value = sId });
                cmd.Parameters.Add(new SqlParameter("@_Status", SqlDbType.SmallInt) { Value = iStatus });

                cmd.Parameters.Add(new SqlParameter("@_Log", SqlDbType.NVarChar, 2000)
                {
                    Value = string.IsNullOrEmpty(sLog) ? (object)DBNull.Value : sLog
                });

                cmd.Parameters.Add(new SqlParameter("@_MetsPath", SqlDbType.NVarChar, 500)
                {
                    Value = string.IsNullOrEmpty(sMetsPath) ? (object)DBNull.Value : sMetsPath
                });

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        #endregion

        #region ======== Helpers ========

        private static string MakeSafeDirectoryName(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "Khac";
            foreach (char c in Path.GetInvalidFileNameChars())
                input = input.Replace(c.ToString(), "_");
            return input.Trim();
        }

        private static string GetUniqueFilePath(string filePath)
        {
            if (!File.Exists(filePath)) return filePath;

            string dir = Path.GetDirectoryName(filePath);
            string nameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
            string ext = Path.GetExtension(filePath);

            int counter = 2;
            string newPath;
            do
            {
                newPath = Path.Combine(dir, $"{nameWithoutExt}_{counter}{ext}");
                counter++;
            } while (File.Exists(newPath));

            return newPath;
        }

        #endregion
    }
}