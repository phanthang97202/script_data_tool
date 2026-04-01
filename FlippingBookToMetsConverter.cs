using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MetsXmlParser;
using MarcXmlParser;

namespace ScriptDataTool
{
    public static class FlippingBookToMetsConverter
    {
        /// <summary>
        /// Convert Flipping Book (đã giải nén) sang METS XML
        /// - Scan thư mục pages để lấy ảnh trang (hỗ trợ jpg + png)
        /// - Ưu tiên: _l > medium > _s
        /// - Tự động tìm thư mục pages trong các cấu trúc thư mục khác nhau
        /// </summary>
        public static string ConvertFlippingBookToMets(
            string fileNameOldHPMU,
            string sPhysicalBookDirectory,
            string sRootPhysicalDirectory,
            string sRootVirtualDirectory,
            string sDmdMarcXml)
        {
            // --- 1. Xác định thư mục chứa ảnh trang ---
            string sPagesDirectory = null;
            string sEffectiveBookDirectory = sPhysicalBookDirectory;

            string[] suffixes = new[]
            {
                Path.Combine("files", "assets", "pages"),
                Path.Combine("files", "assets", "flash", "pages"),
                Path.Combine("files", "pages"),
                "pages"
            };

            // Hàm tìm pages trong 1 base directory
            Func<string, string> findPagesIn = (baseDir) =>
            {
                foreach (string suffix in suffixes)
                {
                    string candidate = Path.Combine(baseDir, suffix);
                    if (Directory.Exists(candidate)) return candidate;
                }
                return null;
            };

            // Bước 1: Thử tìm trực tiếp trong sPhysicalBookDirectory
            sPagesDirectory = findPagesIn(sPhysicalBookDirectory);

            // Bước 2: Nếu không thấy → duyệt tất cả thư mục con 1 cấp
            if (sPagesDirectory == null && Directory.Exists(sPhysicalBookDirectory))
            {
                foreach (string subDir in Directory.GetDirectories(sPhysicalBookDirectory))
                {
                    sPagesDirectory = findPagesIn(subDir);
                    if (sPagesDirectory != null)
                    {
                        sEffectiveBookDirectory = subDir;
                        break;
                    }
                }
            }

            // Bước 3: Không có pages → kiểm tra index.html
            bool bNoPagesButHasIndex = false;
            if (sPagesDirectory == null)
            {
                string indexCheck = Path.Combine(sPhysicalBookDirectory, "index.html");
                if (File.Exists(indexCheck))
                {
                    bNoPagesButHasIndex = true;
                    // Không throw, tiếp tục tạo METS chỉ có index.html
                }
                else
                {
                    throw new DirectoryNotFoundException(
                        $"Không tìm thấy thư mục pages trong: {sPhysicalBookDirectory}");
                }
            }

            // --- 2. Scan và lấy danh sách ảnh theo thứ tự trang ---
            List<PageImageInfo> pageImages = new List<PageImageInfo>();
            if (!bNoPagesButHasIndex)
            {
                pageImages = ScanPageImages(sPagesDirectory);
                if (pageImages.Count == 0)
                    throw new FileNotFoundException(
                        $"Không tìm thấy ảnh trang nào trong: {sPagesDirectory}");
            }

            // --- 3. Tạo METS record ---
            Cmets docMets = new Cmets();
            docMets.metsHdr.RECORDSTATUS = 1;
            docMets.metsHdr.CREATEDUSER = "KIPOS";
            docMets.metsHdr.CREATEDATE = DateTime.Now.ToString("yyyyMMddHHmmss");
            docMets.metsHdr.agent.name = "KIPOS";
            docMets.metsHdr.agent.ROLE = "CREATOR";
            docMets.metsHdr.agent.TYPE = "ORGANIZATION";
            docMets.PROFILE = "COMMON";
            docMets.LABEL = "UnTitle";

            // --- 4. dmdSec: nhúng MARC XML ---
            if (!string.IsNullOrEmpty(sDmdMarcXml))
            {
                try
                {
                    CRecord myRec = new CRecord();
                    myRec.load_Xml(sDmdMarcXml);
                    myRec.RemoveAllNamespaces();

                    CdmdSec myDmdSec = new CdmdSec();
                    myDmdSec.GROUPID = "0"; // Đánh dấu dmdSec này thuộc nhóm 0 // nếu không có thuộc tính này thì mets sau khi tạo import vào kmetnavi sẽ không đọc được metadata để hiển thị thông tin khi click btn Infor
                    CmdWrap myMdWrap = new CmdWrap();
                    myMdWrap.MDTYPE = "MARC";
                    myMdWrap.xmlData = myRec.OuterXml;
                    myDmdSec.mdWrap = myMdWrap;

                    docMets.dmdSecsGroup0.Add(myDmdSec);

                    try
                    {
                        string sTitle = fileNameOldHPMU;
                        if (!string.IsNullOrEmpty(sTitle))
                            docMets.LABEL = sTitle;
                    }
                    catch { /* Bỏ qua nếu không có field 245 */ }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi parse MARC XML: {ex.Message}", ex);
                }
            }

            // --- 5. Kiểm tra file index.html ---
            string sIndexHtmlPath = Path.Combine(sEffectiveBookDirectory, "index.html");
            bool bHasIndexHtml = File.Exists(sIndexHtmlPath);

            // --- 6. fileSec: tạo fileGrp từ danh sách ảnh trang ---
            CfileGrp myFileGrp = new CfileGrp();

            for (int i = 0; i < pageImages.Count; i++)
            {
                PageImageInfo pageInfo = pageImages[i];
                string sFID = "FID" + (i + 1);

                string sVirtualFileURL = pageInfo.PhysicalPath
                    .Replace(sRootPhysicalDirectory, sRootVirtualDirectory)
                    .Replace("\\", "/");

                Cfile pageFile = new Cfile();
                pageFile.ID = sFID;
                pageFile.MIMETYPE = pageInfo.MimeType;

                FileInfo fInfo = new FileInfo(pageInfo.PhysicalPath);
                pageFile.SIZE = (int)(fInfo.Length);

                CFLocat fileLocat = new CFLocat();
                fileLocat.LOCTYPE = "URL";
                fileLocat.xlink_href = sVirtualFileURL;
                pageFile.FLocat = fileLocat;

                myFileGrp.files.Add(pageFile);
            }

            // --- Thêm trang "Xem FlippingBook" (index.html) vào cuối fileSec ---
            string sFlippingBookFID = null;
            if (bHasIndexHtml)
            {
                sFlippingBookFID = "FID" + (pageImages.Count + 1);

                string sIndexVirtualURL = sIndexHtmlPath
                    .Replace(sRootPhysicalDirectory, sRootVirtualDirectory)
                    .Replace("\\", "/");

                Cfile indexFile = new Cfile();
                indexFile.ID = sFlippingBookFID;
                indexFile.MIMETYPE = "text/html";
                indexFile.ADMID = "1";

                FileInfo fIndexInfo = new FileInfo(sIndexHtmlPath);
                indexFile.SIZE = (int)(fIndexInfo.Length);

                CFLocat indexLocat = new CFLocat();
                indexLocat.LOCTYPE = "URL";
                indexLocat.xlink_href = sIndexVirtualURL;
                indexFile.FLocat = indexLocat;

                myFileGrp.files.Add(indexFile);
            }

            CfileSec myFileSec = new CfileSec();
            docMets.fileSec = myFileSec;
            docMets.fileSec.fileGrps.Add(myFileGrp);

            // --- 7. Physical StructMap ---
            CstructMap physicalStructMap = new CstructMap();
            physicalStructMap.LABEL = "Cấu trúc vật lý";
            physicalStructMap.TYPE = "physical";

            for (int i = 1; i <= pageImages.Count; i++)
            {
                Cdiv pDiv = new Cdiv();
                pDiv.LABEL = "Trang " + i;
                pDiv.TYPE = "page";
                pDiv.ID = i.ToString();

                string sFID = "FID" + i;
                Cfile myFile = new Cfile();
                if (docMets.fileSec.get_file(sFID, ref myFile))
                {
                    Cfptr nFPTR = new Cfptr();
                    nFPTR.FILEID = sFID;
                    pDiv.fptrs.Add(nFPTR);
                }
                physicalStructMap.divs.Add(pDiv);
            }

            if (bHasIndexHtml && sFlippingBookFID != null)
            {
                Cdiv flipDiv = new Cdiv();
                flipDiv.LABEL = "Xem FlippingBook";
                flipDiv.TYPE = "file";
                flipDiv.ID = (pageImages.Count + 1).ToString();

                Cfile flipFile = new Cfile();
                if (docMets.fileSec.get_file(sFlippingBookFID, ref flipFile))
                {
                    Cfptr nFPTR = new Cfptr();
                    nFPTR.FILEID = sFlippingBookFID;
                    flipDiv.fptrs.Add(nFPTR);
                }
                physicalStructMap.divs.Add(flipDiv);
            }

            docMets.structMaps.Add(physicalStructMap);

            // --- 8. Không có Logical StructMap ---

            return docMets.OuterXml;
        }

        /// <summary>
        /// Scan thư mục pages, lấy ảnh chất lượng cao nhất cho từng trang.
        /// Hỗ trợ cả JPG và PNG.
        /// Ưu tiên: _l > medium > _s
        /// </summary>
        private static List<PageImageInfo> ScanPageImages(string sPagesDirectory)
        {
            // Lấy tất cả file jpg + png
            var allImageFiles = Directory.GetFiles(sPagesDirectory, "*.jpg", SearchOption.TopDirectoryOnly)
                .Concat(Directory.GetFiles(sPagesDirectory, "*.png", SearchOption.TopDirectoryOnly))
                .ToArray();

            // Regex match cả jpg lẫn png
            Regex rxPage = new Regex(@"^page(\d{4})(_l|_s)?\.(?:jpg|png)$", RegexOptions.IgnoreCase);

            var pageGroups = new Dictionary<int, Dictionary<string, string>>();

            foreach (string sFile in allImageFiles)
            {
                string sFileName = Path.GetFileName(sFile);
                Match m = rxPage.Match(sFileName);
                if (!m.Success) continue;

                int iPageNum = int.Parse(m.Groups[1].Value);
                string sSuffix = m.Groups[2].Value.ToLower(); // "", "_l", "_s"

                if (!pageGroups.ContainsKey(iPageNum))
                    pageGroups[iPageNum] = new Dictionary<string, string>();

                // JPG ưu tiên hơn PNG cùng suffix
                if (!pageGroups[iPageNum].ContainsKey(sSuffix) ||
                    sFile.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
                {
                    pageGroups[iPageNum][sSuffix] = sFile;
                }
            }

            List<PageImageInfo> result = new List<PageImageInfo>();

            foreach (int iPageNum in pageGroups.Keys.OrderBy(k => k))
            {
                var variants = pageGroups[iPageNum];
                string sChosenPath = null;
                string sChosenType = null;

                if (variants.ContainsKey("_l"))
                {
                    sChosenPath = variants["_l"];
                    sChosenType = "high";
                }
                else if (variants.ContainsKey(""))
                {
                    sChosenPath = variants[""];
                    sChosenType = "medium";
                }
                else if (variants.ContainsKey("_s"))
                {
                    sChosenPath = variants["_s"];
                    sChosenType = "small";
                }

                if (sChosenPath != null)
                {
                    result.Add(new PageImageInfo
                    {
                        PageNumber = iPageNum,
                        PhysicalPath = sChosenPath,
                        ImageType = sChosenType,
                        MimeType = sChosenPath.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                                   ? "image/png"
                                   : "image/jpeg"
                    });
                }
            }

            return result;
        }
    }

    /// <summary>
    /// Thông tin ảnh đại diện cho một trang
    /// </summary>
    public class PageImageInfo
    {
        public int PageNumber { get; set; }
        public string PhysicalPath { get; set; }
        public string ImageType { get; set; } // "medium", "high", "small"
        public string MimeType { get; set; }  // "image/jpeg" hoặc "image/png"
    }
}









//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text.RegularExpressions;
//using MetsXmlParser;
//using MarcXmlParser;

//namespace ScriptDataTool
//{
//    public static class FlippingBookToMetsConverter
//    {
//        /// <summary>
//        /// Convert Flipping Book (đã giải nén) sang METS XML
//        /// - Scan thư mục files\assets\pages\ để lấy ảnh trang
//        /// - Ưu tiên: medium (page0001.jpg) > _l (page0001_l.jpg) > _s (page0001_s.jpg)
//        /// - Không có Logical StructMap vì Flipping Book không có bookmark/TOC
//        /// </summary>
//        /// <param name="sPhysicalBookDirectory">Đường dẫn vật lý thư mục gốc của flipping book đã giải nén</param>
//        /// <param name="sRootPhysicalDirectory">Root physical directory (để map sang virtual URL)</param>
//        /// <param name="sRootVirtualDirectory">Root virtual directory (URL gốc)</param>
//        /// <param name="sDmdMarcXml">MARC XML biểu ghi thư mục</param>
//        /// <returns>METS XML string hoặc null nếu không tìm thấy ảnh trang</returns>
//        public static string ConvertFlippingBookToMets(
//            string fileNameOldHPMU,
//            string sPhysicalBookDirectory,
//            string sRootPhysicalDirectory,
//            string sRootVirtualDirectory,
//            string sDmdMarcXml)
//        {
//            // --- 1. Xác định thư mục chứa ảnh trang ---
//            string sPagesDirectory = null;

//            // Tạo danh sách các base directory cần thử
//            // Case 1: bình thường     → sPhysicalBookDirectory\files\assets\pages
//            // Case 2: lồng 1 cấp      → sPhysicalBookDirectory\<tên thư mục>\files\assets\pages
//            var baseDirectories = new List<string> { sPhysicalBookDirectory };

//            // Thử thêm thư mục con cùng tên (trường hợp bị lồng cấp như \Buituananh45\Buituananh45)
//            string folderName = new DirectoryInfo(sPhysicalBookDirectory).Name;
//            string nestedPath = Path.Combine(sPhysicalBookDirectory, folderName);
//            if (Directory.Exists(nestedPath))
//                baseDirectories.Add(nestedPath);

//            string[] suffixes = new[]
//            {
//                Path.Combine("files", "assets", "pages"),
//                Path.Combine("files", "assets", "flash", "pages"),
//                Path.Combine("files", "pages"),
//                "pages"
//            };

//            foreach (string baseDir in baseDirectories)
//            {
//                foreach (string suffix in suffixes)
//                {
//                    string candidate = Path.Combine(baseDir, suffix);
//                    if (Directory.Exists(candidate))
//                    {
//                        sPagesDirectory = candidate;
//                        break;
//                    }
//                }
//                if (sPagesDirectory != null) break;
//            }

//            if (sPagesDirectory == null)
//            {
//                throw new DirectoryNotFoundException(
//                    $"Không tìm thấy thư mục pages trong: {sPhysicalBookDirectory}");
//            }

//            // --- 2. Scan và lấy danh sách ảnh theo thứ tự trang ---
//            List<PageImageInfo> pageImages = ScanPageImages(sPagesDirectory);
//            if (pageImages.Count == 0)
//            {
//                throw new FileNotFoundException($"Không tìm thấy ảnh trang nào trong: {sPagesDirectory}");
//            }

//            // --- 3. Tạo METS record ---
//            Cmets docMets = new Cmets();
//            docMets.metsHdr.RECORDSTATUS = 1;
//            docMets.metsHdr.CREATEDUSER = "KIPOS";
//            docMets.metsHdr.CREATEDATE = DateTime.Now.ToString("yyyyMMddHHmmss");
//            docMets.metsHdr.agent.name = "KIPOS";
//            docMets.metsHdr.agent.ROLE = "CREATOR";
//            docMets.metsHdr.agent.TYPE = "ORGANIZATION";
//            docMets.PROFILE = "COMMON";
//            docMets.LABEL = "UnTitle";

//            // --- 4. dmdSec: nhúng MARC XML ---
//            if (!string.IsNullOrEmpty(sDmdMarcXml))
//            {
//                try
//                {
//                    CRecord myRec = new CRecord();
//                    myRec.load_Xml(sDmdMarcXml);
//                    myRec.RemoveAllNamespaces();

//                    CdmdSec myDmdSec = new CdmdSec();
//                    CmdWrap myMdWrap = new CmdWrap();
//                    myMdWrap.MDTYPE = "MARC";
//                    myMdWrap.xmlData = myRec.OuterXml;
//                    myDmdSec.mdWrap = myMdWrap;

//                    docMets.dmdSecsGroup0.Add(myDmdSec);

//                    // Lấy tiêu đề từ MARC field 245 subfield a
//                    try
//                    {
//                        string sTitle = fileNameOldHPMU; // dùng tên file gốc theo hệ thống cũ HPMU // myRec.Datafields.Datafield("245").Subfields.Subfield("a").Value;
//                        if (!string.IsNullOrEmpty(sTitle))
//                            docMets.LABEL = sTitle;
//                    }
//                    catch { /* Bỏ qua nếu không có field 245 */ }
//                }
//                catch (Exception ex)
//                {
//                    throw new Exception($"Lỗi parse MARC XML: {ex.Message}", ex);
//                }
//            }

//            // --- 5. Kiểm tra file index.html (FlippingBook viewer) ---
//            // index.html nằm cùng cấp với thư mục files, tức là trong sPhysicalBookDirectory
//            string sIndexHtmlPath = Path.Combine(sPhysicalBookDirectory, "index.html");
//            bool bHasIndexHtml = File.Exists(sIndexHtmlPath);

//            // --- 6. fileSec: tạo fileGrp từ danh sách ảnh trang ---
//            CfileGrp myFileGrp = new CfileGrp();

//            for (int i = 0; i < pageImages.Count; i++)
//            {
//                PageImageInfo pageInfo = pageImages[i];
//                string sFID = "FID" + (i + 1);

//                // Map physical path sang virtual URL
//                string sVirtualFileURL = pageInfo.PhysicalPath
//                    .Replace(sRootPhysicalDirectory, sRootVirtualDirectory)
//                    .Replace("\\", "/");

//                Cfile pageFile = new Cfile();
//                pageFile.ID = sFID;
//                pageFile.MIMETYPE = "image/jpeg";

//                FileInfo fInfo = new FileInfo(pageInfo.PhysicalPath);
//                pageFile.SIZE = (int)(fInfo.Length);

//                CFLocat fileLocat = new CFLocat();
//                fileLocat.LOCTYPE = "URL";
//                fileLocat.xlink_href = sVirtualFileURL;
//                pageFile.FLocat = fileLocat;

//                myFileGrp.files.Add(pageFile);
//            }

//            // --- Thêm trang "Xem FlippingBook" (index.html) vào cuối fileSec ---
//            string sFlippingBookFID = null;
//            if (bHasIndexHtml)
//            {
//                sFlippingBookFID = "FID" + (pageImages.Count + 1);

//                string sIndexVirtualURL = sIndexHtmlPath
//                    .Replace(sRootPhysicalDirectory, sRootVirtualDirectory)
//                    .Replace("\\", "/");

//                Cfile indexFile = new Cfile();
//                indexFile.ID = sFlippingBookFID;
//                indexFile.MIMETYPE = "text/html";
//                indexFile.ADMID = "1"; // với flipping book thì phải để admid = 1 để bảo vệ
//                FileInfo fIndexInfo = new FileInfo(sIndexHtmlPath);
//                indexFile.SIZE = (int)(fIndexInfo.Length);

//                CFLocat indexLocat = new CFLocat();
//                indexLocat.LOCTYPE = "URL";
//                indexLocat.xlink_href = sIndexVirtualURL;
//                indexFile.FLocat = indexLocat;

//                myFileGrp.files.Add(indexFile);
//            }

//            CfileSec myFileSec = new CfileSec();
//            docMets.fileSec = myFileSec;
//            docMets.fileSec.fileGrps.Add(myFileGrp);

//            // --- 7. Physical StructMap ---
//            CstructMap physicalStructMap = new CstructMap();
//            physicalStructMap.LABEL = "Cấu trúc vật lý";
//            physicalStructMap.TYPE = "physical";

//            // Các trang ảnh thật
//            for (int i = 1; i <= pageImages.Count; i++)
//            {
//                Cdiv pDiv = new Cdiv();
//                pDiv.LABEL = "Trang " + i;
//                pDiv.TYPE = "page";
//                pDiv.ID = i.ToString();

//                string sFID = "FID" + i;
//                Cfile myFile = new Cfile();
//                if (docMets.fileSec.get_file(sFID, ref myFile))
//                {
//                    Cfptr nFPTR = new Cfptr();
//                    nFPTR.FILEID = sFID;
//                    pDiv.fptrs.Add(nFPTR);
//                }
//                physicalStructMap.divs.Add(pDiv);
//            }

//            // Trang cuối: "Xem FlippingBook" → index.html
//            if (bHasIndexHtml && sFlippingBookFID != null)
//            {
//                Cdiv flipDiv = new Cdiv();
//                flipDiv.LABEL = "Xem FlippingBook";
//                flipDiv.TYPE = "file";  // type "file" để phân biệt với các trang ảnh
//                flipDiv.ID = (pageImages.Count + 1).ToString();

//                Cfile flipFile = new Cfile();
//                if (docMets.fileSec.get_file(sFlippingBookFID, ref flipFile))
//                {
//                    Cfptr nFPTR = new Cfptr();
//                    nFPTR.FILEID = sFlippingBookFID;
//                    flipDiv.fptrs.Add(nFPTR);
//                }
//                physicalStructMap.divs.Add(flipDiv);
//            }

//            docMets.structMaps.Add(physicalStructMap);

//            // --- 8. Không có Logical StructMap (Flipping Book không có bookmark/TOC) ---

//            return docMets.OuterXml;
//        }

//        /// <summary>
//        /// Scan thư mục pages, lấy ảnh chất lượng cao nhất ưu tiên cho từng trang.
//        /// Quy tắc đặt tên FlippingBook: page0001.jpg, page0001_l.jpg, page0001_s.jpg
//        /// Ưu tiên: _l (high quality) > medium (không hậu tố) > _s (thumbnail)
//        /// </summary>
//        private static List<PageImageInfo> ScanPageImages(string sPagesDirectory)
//        {
//            // Lấy tất cả file jpg trong thư mục
//            string[] allJpgFiles = Directory.GetFiles(sPagesDirectory, "*.jpg", SearchOption.TopDirectoryOnly);

//            // Regex: tên file dạng page<4 số><optional hậu tố>.jpg
//            Regex rxPage = new Regex(@"^page(\d{4})(_l|_s)?\.jpg$", RegexOptions.IgnoreCase);

//            // Group theo số trang
//            var pageGroups = new Dictionary<int, Dictionary<string, string>>();
//            // pageGroups[pageNum]["_l"] = high quality path
//            // pageGroups[pageNum][""]   = medium path
//            // pageGroups[pageNum]["_s"] = thumbnail path

//            foreach (string sFile in allJpgFiles)
//            {
//                string sFileName = Path.GetFileName(sFile);
//                Match m = rxPage.Match(sFileName);
//                if (!m.Success) continue;

//                int iPageNum = int.Parse(m.Groups[1].Value);
//                string sSuffix = m.Groups[2].Value.ToLower(); // "", "_l", "_s"

//                if (!pageGroups.ContainsKey(iPageNum))
//                    pageGroups[iPageNum] = new Dictionary<string, string>();

//                pageGroups[iPageNum][sSuffix] = sFile;
//            }

//            // Build danh sách ảnh theo thứ tự trang, chọn ảnh ưu tiên
//            List<PageImageInfo> result = new List<PageImageInfo>();

//            foreach (int iPageNum in pageGroups.Keys.OrderBy(k => k))
//            {
//                var variants = pageGroups[iPageNum];
//                string sChosenPath = null;
//                string sChosenType = null;

//                if (variants.ContainsKey("_l"))
//                {
//                    sChosenPath = variants["_l"];
//                    sChosenType = "high";
//                }
//                else if (variants.ContainsKey(""))
//                {
//                    sChosenPath = variants[""];
//                    sChosenType = "medium";
//                }
//                else if (variants.ContainsKey("_s"))
//                {
//                    sChosenPath = variants["_s"];
//                    sChosenType = "small";
//                }

//                if (sChosenPath != null)
//                {
//                    result.Add(new PageImageInfo
//                    {
//                        PageNumber = iPageNum,
//                        PhysicalPath = sChosenPath,
//                        ImageType = sChosenType
//                    });
//                }
//            }

//            return result;
//        }
//    }

//    /// <summary>
//    /// Thông tin ảnh đại diện cho một trang
//    /// </summary>
//    public class PageImageInfo
//    {
//        public int PageNumber { get; set; }
//        public string PhysicalPath { get; set; }
//        public string ImageType { get; set; } // "medium", "high", "small"
//    }
//}
