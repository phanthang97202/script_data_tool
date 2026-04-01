# 🖥️ MARCXMLEditorControl for WinForms

**MARCXMLEditorControl** là một thư viện giao diện người dùng dạng `UserControl` cho WinForms, được thiết kế để hỗ trợ trực quan việc tạo, chỉnh sửa và quản lý biểu ghi MARCXML (MARC21 dạng XML) trong các phần mềm biên mục, thư viện số hoặc lưu trữ.

Thư viện này sử dụng trực tiếp **MarcXmlParser** để thao tác dữ liệu MARCXML theo chuẩn MARC21 và cung cấp trải nghiệm nhập liệu thân thiện với người dùng, hỗ trợ tiếng Việt đầy đủ.

---

## 🧩 Tính năng nổi bật

- 🎛️ Giao diện trực quan giúp chỉnh sửa biểu ghi MARCXML với đầy đủ leader, controlfield, datafield, subfield.
- 🔁 Tự động xác định loại biểu ghi từ `Leader` vị trí 06–07 (ví dụ: Bibliographic, Authority, Holdings...).
- 🌐 Hỗ trợ đa ngôn ngữ qua `ResourceManager` – phù hợp cho ứng dụng thư viện tiếng Việt.
- 🎚️ Cho phép chọn loại biểu ghi thông qua combobox (ví dụ: sách, tạp chí, bản đồ, tài liệu số...).
- 🧩 Sử dụng lại thư viện **MarcXmlParser** để xử lý, làm sạch và chuyển đổi MARCXML.
- 🔌 Có thể nhúng vào các phần mềm WinForms hiện có như một thành phần độc lập.

---

## 🚀 Cách sử dụng

1. Thêm tham chiếu đến thư viện `MarcXmlParser`.
2. Thêm tệp `MARCXMLEditorControl.cs` và các thành phần phụ trợ (resource, WorkSheetControl...) vào dự án.
3. Kéo thả `MARCXMLEditorControl` vào `Form` hoặc khởi tạo bằng mã C#:

```csharp
var editor = new MARCXMLEditorControl();
editor.Dock = DockStyle.Fill;
this.Controls.Add(editor);

// Gán biểu ghi để hiển thị
editor.DataRecord = myCRecord;
editor.LabelRecord = myLabelRecord;
editor.Display();
```

4. Để xử lý sự kiện khi người dùng thay đổi nhãn tiêu đề hoặc yêu cầu nạp lại dữ liệu từ điển:

```csharp
editor.TitleUpdated += (s, e) => { Console.WriteLine("Tiêu đề mới: " + e.Title); };
editor.RefreshLookupData += (s, e) => { LoadMyLookupData(e.Tag); };
```

---

## 📦 Thành phần chính

| Thành phần               | Vai trò |
|---------------------------|--------|
| `MARCXMLEditorControl`    | Control chính để hiển thị và chỉnh sửa MARCXML |
| `WorkSheetControl`        | Thành phần con hiển thị các dòng MARC |
| `ComboBox` L06 / L07      | Chọn loại biểu ghi (theo MARC Leader) |
| `StatusBar`               | Hiển thị loại tài liệu theo chuẩn MARC |
| `MarcXmlParser` (tham chiếu ngoài) | Thư viện xử lý dữ liệu MARCXML |

---

## 🔧 Yêu cầu hệ thống

- .NET Framework 4.5 trở lên
- Visual Studio 2019 hoặc mới hơn
- Phù hợp với ứng dụng WinForms chạy trên Windows
- Cần tham chiếu thư viện `MarcXmlParser`

---

## 📝 Giấy phép và hỗ trợ

- **Bản quyền**: © 2004–2025 bởi CTCP Phần mềm Quản lý Hiện Đại (MMS.JSC)
- **Loại**: Phát hành nội bộ hoặc OEM có giới hạn
- **Liên hệ hỗ trợ**:  
  📧 support@hiendai.com.vn  
  🌐 [https://www.hiendai.com.vn](https://www.hiendai.com.vn)
