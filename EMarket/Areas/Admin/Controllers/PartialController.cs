using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace EMarket.Areas.Admin.Controllers
{
    public class PartialController : Controller
    {
        // Action nhận data và điều hướng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Generate(string type, string jsonData, string title)
        {
            // BẮT BUỘC DÙNG ExpandoObjectConverter để map JSON lồng nhau
            var data = JsonConvert.DeserializeObject<List<ExpandoObject>>(jsonData, new ExpandoObjectConverter());

            switch (type.ToLower())
            {
                case "print":
                case "pdf":
                    ViewBag.Title = title;
                    return PartialView("~/Areas/Admin/Views/Partial/_PrintTemplate.cshtml", data);

                case "excel":
                    return ExportToExcel(data, title);

                default:
                    return HttpNotFound();
            }
        }

        private ActionResult ExportToExcel(List<ExpandoObject> data, string title)
        {
            var flatDataList = new List<Dictionary<string, object>>();
            var allHeaders = new HashSet<string>(); // Dùng HashSet để tự động loại bỏ cột trùng

            // 1. Làm phẳng (Flatten) từng dòng dữ liệu và gom tên cột (Header)
            foreach (var row in data)
            {
                var flatRow = new Dictionary<string, object>();
                FlattenExpando(row, flatRow, "");
                flatDataList.Add(flatRow);

                foreach (var key in flatRow.Keys)
                {
                    allHeaders.Add(key);
                }
            }

            var headers = allHeaders.ToList();

            // 2. Tạo file Excel bằng ClosedXML
            using (var workbook = new XLWorkbook())
            {
                // Tên sheet không được quá 31 ký tự và không chứa ký tự đặc biệt
                string sheetName = string.IsNullOrWhiteSpace(title) ? "Export_Data" :
                    new string(title.Take(30).ToArray()).Replace(":", "").Replace("/", "");

                var ws = workbook.Worksheets.Add(sheetName);

                // 2.1 In Dòng Header (Tiêu đề cột)
                for (int i = 0; i < headers.Count; i++)
                {
                    ws.Cell(1, i + 1).Value = headers[i];
                    ws.Cell(1, i + 1).Style.Font.Bold = true;
                    ws.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                }

                // 2.2 In Data vào các ô
                for (int rowIndex = 0; rowIndex < flatDataList.Count; rowIndex++)
                {
                    var rowData = flatDataList[rowIndex];
                    for (int colIndex = 0; colIndex < headers.Count; colIndex++)
                    {
                        var colName = headers[colIndex];
                        if (rowData.ContainsKey(colName) && rowData[colName] != null)
                        {
                            ws.Cell(rowIndex + 2, colIndex + 1).Value = rowData[colName].ToString();
                        }
                    }
                }

                // Tự động căn chỉnh độ rộng cột cho đẹp
                ws.Columns().AdjustToContents();

                // 3. Trả file về cho trình duyệt tải xuống
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    string fileName = $"Export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        // ==========================================
        // HÀM ĐỆ QUY: Trái tim của hệ thống
        // ==========================================
        private void FlattenExpando(IDictionary<string, object> dict, Dictionary<string, object> flatDict, string prefix)
        {
            foreach (var kvp in dict)
            {
                string newKey = string.IsNullOrEmpty(prefix) ? kvp.Key : $"{prefix}_{kvp.Key}";

                if (kvp.Value == null)
                {
                    flatDict[newKey] = string.Empty;
                }
                else if (kvp.Value is IDictionary<string, object> nestedDict)
                {
                    // Trường hợp 1: Object lồng Object -> Đệ quy đi sâu vào trong
                    FlattenExpando(nestedDict, flatDict, newKey);
                }
                else if (kvp.Value is System.Collections.IEnumerable enumerable && !(kvp.Value is string))
                {
                    // Trường hợp 2: Là một Mảng (List/Array)
                    var list = enumerable.Cast<object>().ToList();

                    if (list.Count == 0)
                    {
                        flatDict[newKey] = string.Empty;
                    }
                    else if (list.First() is IDictionary<string, object>)
                    {
                        // 🔥 ĐÂY CHÍNH LÀ THẰNG GÂY LỖI CỦA BRO: Mảng chứa Object lồng nhau

                        // --- TRƯỜNG PHÁI 1: GOM VÀO 1 Ô EXCEL (Khuyên dùng) ---
                        // Chuyển mảng Object thành chuỗi JSON (hoặc chuỗi đọc được) để nhét gọn vào 1 ô Excel.
                        // Giúp file Excel không bị đẻ ra hàng chục cột mới làm loạn file.
                        // flatDict[newKey] = JsonConvert.SerializeObject(list, Formatting.Indented);

                        // --- TRƯỜNG PHÁI 2: ĐẺ CỘT THEO INDEX (Bỏ comment nếu sếp thích kiểu này) ---
                        // Ví dụ: details_0_Ten, details_0_Gia, details_1_Ten, details_1_Gia
                        for (int i = 0; i < list.Count; i++)
                        {
                            var listItem = list[i] as IDictionary<string, object>;
                            FlattenExpando(listItem, flatDict, $"{newKey}_{i}");
                        }
                        
                    }
                    else
                    {
                        // Mảng chứa giá trị đơn (Ví dụ: tags: ["Apple", "Mới"]) -> Ghép dấu phẩy
                        flatDict[newKey] = string.Join(", ", list);
                    }
                }
                else
                {
                    // Trường hợp 3: Các giá trị cơ bản bình thường (string, int, bool, datetime...)
                    flatDict[newKey] = kvp.Value;
                }
            }
        }
    }
}