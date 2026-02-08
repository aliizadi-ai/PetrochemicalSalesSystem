using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PetrochemicalSalesSystem.Services
{
    public class ExportService
    {
        /// <summary>
        /// خروجی Excel از لیست فاکتورها
        /// </summary>
        public bool ExportToExcel(DataTable data, string fileName)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("فاکتورها");

                    // تنظیمات راست به چپ
                    worksheet.RightToLeft = true;

                    // هدرها
                    for (int i = 0; i < data.Columns.Count; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = data.Columns[i].ColumnName;
                        worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                        worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
                        worksheet.Cell(1, i + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }

                    // داده‌ها
                    for (int row = 0; row < data.Rows.Count; row++)
                    {
                        for (int col = 0; col < data.Columns.Count; col++)
                        {
                            worksheet.Cell(row + 2, col + 1).Value = data.Rows[row][col].ToString();
                        }
                    }

                    // تنظیم عرض ستون‌ها
                    worksheet.Columns().AdjustToContents();

                    // ذخیره فایل
                    workbook.SaveAs(fileName);
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در ایجاد فایل Excel: {ex.Message}", "خطا");
                return false;
            }
        }

        /// <summary>
        /// خروجی PDF از لیست فاکتورها (نسخه ساده‌شده)
        /// </summary>
        /// 
        /// 
        /*
        public bool ExportToPdf(DataTable data, string fileName, string title)
        {
            try
            {
                // اگر داده‌ای وجود ندارد
                if (data == null || data.Rows.Count == 0)
                {
                    MessageBox.Show("داده‌ای برای خروجی وجود ندارد.", "خطا");
                    return false;
                }

                // ایجاد داکیومنت
                Document document = new Document(PageSize.A4.Rotate());
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(fileName, FileMode.Create));

                document.Open();

                // عنوان
                Font titleFont = FontFactory.GetFont("Tahoma", 16, Font.BOLD, BaseColor.Black);
                Paragraph titleParagraph = new Paragraph(title, titleFont);
                titleParagraph.Alignment = Element.ALIGN_CENTER;
                titleParagraph.SpacingAfter = 20f;
                document.Add(titleParagraph);

                // تاریخ ایجاد
                Font dateFont = FontFactory.GetFont("Tahoma", 10, Font.NORMAL, BaseColor.Black);
                Paragraph dateParagraph = new Paragraph($"تاریخ ایجاد: {DateTime.Now:yyyy/MM/dd HH:mm}", dateFont);
                dateParagraph.Alignment = Element.ALIGN_LEFT;
                dateParagraph.SpacingAfter = 10f;
                document.Add(dateParagraph);

                // ایجاد جدول
                PdfPTable table = new PdfPTable(data.Columns.Count);
                table.WidthPercentage = 100;

                // تنظیم عرض ستون‌ها
                float[] columnWidths = new float[data.Columns.Count];
                for (int i = 0; i < data.Columns.Count; i++)
                {
                    columnWidths[i] = 100f / data.Columns.Count;
                }
                table.SetWidths(columnWidths);

                // هدر جدول
                Font headerFont = FontFactory.GetFont("Tahoma", 10, Font.BOLD, BaseColor.White);
                foreach (DataColumn column in data.Columns)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(column.ColumnName, headerFont));
                    cell.BackgroundColor = new BaseColor(0, 102, 51); // سبز پتروشیمی
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Padding = 5;
                    table.AddCell(cell);
                }

                // داده‌های جدول
                Font cellFont = FontFactory.GetFont("Tahoma", 9, Font.NORMAL, BaseColor.Black);
                foreach (DataRow row in data.Rows)
                {
                    foreach (DataColumn column in data.Columns)
                    {
                        string cellValue = row[column].ToString();
                        PdfPCell cell = new PdfPCell(new Phrase(cellValue, cellFont));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.Padding = 5;
                        table.AddCell(cell);
                    }
                }

                document.Add(table);

                // جمع‌کل
                if (data.Columns.Contains("مبلغ") || data.Columns.Contains("قیمت"))
                {
                    decimal total = 0;
                    int amountColumnIndex = -1;

                    // پیدا کردن ستون مبلغ
                    for (int i = 0; i < data.Columns.Count; i++)
                    {
                        if (data.Columns[i].ColumnName.Contains("مبلغ") ||
                            data.Columns[i].ColumnName.Contains("قیمت"))
                        {
                            amountColumnIndex = i;
                            break;
                        }
                    }

                    if (amountColumnIndex >= 0)
                    {
                        foreach (DataRow row in data.Rows)
                        {
                            if (decimal.TryParse(row[amountColumnIndex].ToString().Replace("تومان", "").Trim(), out decimal amount))
                            {
                                total += amount;
                            }
                        }

                        Font totalFont = FontFactory.GetFont("Tahoma", 12, Font.BOLD, BaseColor.Black);
                        Paragraph totalParagraph = new Paragraph($"\n\nجمع کل: {total:N0} تومان", totalFont);
                        totalParagraph.Alignment = Element.ALIGN_LEFT;
                        document.Add(totalParagraph);
                    }
                }

                document.Close();
                writer.Close();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در ایجاد فایل PDF: {ex.Message}", "خطا");
                return false;
            }
        }

        /// <summary>
        /// خروجی PDF از یک فاکتور خاص
        /// </summary>
        public bool ExportInvoiceToPdf(DataTable invoiceData, string customerName, string invoiceNo, string fileName)
        {
            try
            {
                Document document = new Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(fileName, FileMode.Create));

                document.Open();

                // هدر فاکتور
                Font headerFont = FontFactory.GetFont("Tahoma", 18, Font.BOLD, BaseColor.Black);
                Paragraph header = new Paragraph("فاکتور فروش", headerFont);
                header.Alignment = Element.ALIGN_CENTER;
                header.SpacingAfter = 20f;
                document.Add(header);

                // اطلاعات فاکتور
                Font infoFont = FontFactory.GetFont("Tahoma", 11, Font.NORMAL, BaseColor.Black);

                Paragraph invoiceInfo = new Paragraph();
                invoiceInfo.Add(new Chunk($"شماره فاکتور: {invoiceNo}\n", infoFont));
                invoiceInfo.Add(new Chunk($"تاریخ: {DateTime.Now:yyyy/MM/dd}\n", infoFont));
                invoiceInfo.Add(new Chunk($"مشتری: {customerName}\n\n", infoFont));
                invoiceInfo.SpacingAfter = 10f;
                document.Add(invoiceInfo);

                // جدول آیتم‌ها
                if (invoiceData != null && invoiceData.Rows.Count > 0)
                {
                    PdfPTable table = new PdfPTable(invoiceData.Columns.Count);
                    table.WidthPercentage = 100;

                    // هدر جدول
                    Font tableHeaderFont = FontFactory.GetFont("Tahoma", 10, Font.BOLD, BaseColor.White);
                    foreach (DataColumn column in invoiceData.Columns)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(column.ColumnName, tableHeaderFont));
                        cell.BackgroundColor = new BaseColor(0, 102, 51);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.Padding = 5;
                        table.AddCell(cell);
                    }

                    // داده‌های جدول
                    Font tableCellFont = FontFactory.GetFont("Tahoma", 9, Font.NORMAL, BaseColor.Black);
                    decimal totalAmount = 0;

                    foreach (DataRow row in invoiceData.Rows)
                    {
                        foreach (DataColumn column in invoiceData.Columns)
                        {
                            string cellValue = row[column].ToString();
                            PdfPCell cell = new PdfPCell(new Phrase(cellValue, tableCellFont));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.Padding = 5;
                            table.AddCell(cell);

                            // محاسبه جمع کل
                            if (column.ColumnName.Contains("مبلغ") && decimal.TryParse(cellValue, out decimal amount))
                            {
                                totalAmount += amount;
                            }
                        }
                    }

                    document.Add(table);

                    // جمع‌کل
                    Font totalFont = FontFactory.GetFont("Tahoma", 12, Font.BOLD, BaseColor.Black);
                    Paragraph totalParagraph = new Paragraph($"\n\nمبلغ قابل پرداخت: {totalAmount:N0} تومان", totalFont);
                    totalParagraph.Alignment = Element.ALIGN_LEFT;
                    document.Add(totalParagraph);
                }

                // پاورقی
                Font footerFont = FontFactory.GetFont("Tahoma", 9, Font.ITALIC, BaseColor.Gray);
                Paragraph footer = new Paragraph("\n\nبا تشکر از خرید شما\nشرکت پتروشیمی نمونه\nتلفن: ۰۲۱-۸۸۸۸۸۸۸۸", footerFont);
                footer.Alignment = Element.ALIGN_CENTER;
                document.Add(footer);

                document.Close();
                writer.Close();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در ایجاد فاکتور PDF: {ex.Message}", "خطا");
                return false;
            }
        }
        */

        /// <summary>
        /// خروجی Excel از یک فاکتور خاص
        /// </summary>
        public bool ExportInvoiceToExcel(DataTable invoiceData, string customerName, string invoiceNo, string fileName)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add($"فاکتور {invoiceNo}");
                    worksheet.RightToLeft = true;

                    // اطلاعات فاکتور
                    worksheet.Cell(1, 1).Value = "فاکتور فروش";
                    worksheet.Cell(1, 1).Style.Font.Bold = true;
                    worksheet.Cell(1, 1).Style.Font.FontSize = 16;

                    worksheet.Cell(2, 1).Value = $"شماره فاکتور: {invoiceNo}";
                    worksheet.Cell(3, 1).Value = $"تاریخ: {DateTime.Now:yyyy/MM/dd}";
                    worksheet.Cell(4, 1).Value = $"مشتری: {customerName}";

                    // فاصله
                    int startRow = 6;

                    // اگر داده‌ای وجود دارد
                    if (invoiceData != null && invoiceData.Rows.Count > 0)
                    {
                        // هدرها
                        for (int i = 0; i < invoiceData.Columns.Count; i++)
                        {
                            worksheet.Cell(startRow, i + 1).Value = invoiceData.Columns[i].ColumnName;
                            worksheet.Cell(startRow, i + 1).Style.Font.Bold = true;
                            worksheet.Cell(startRow, i + 1).Style.Fill.BackgroundColor = XLColor.LightGreen;
                        }

                        // داده‌ها
                        for (int row = 0; row < invoiceData.Rows.Count; row++)
                        {
                            for (int col = 0; col < invoiceData.Columns.Count; col++)
                            {
                                worksheet.Cell(startRow + row + 1, col + 1).Value = invoiceData.Rows[row][col].ToString();
                            }
                        }

                        // جمع‌کل
                        int lastRow = startRow + invoiceData.Rows.Count + 2;
                        worksheet.Cell(lastRow, invoiceData.Columns.Count - 1).Value = "جمع کل:";
                        worksheet.Cell(lastRow, invoiceData.Columns.Count - 1).Style.Font.Bold = true;

                        // محاسبه جمع (فرض می‌کنیم آخرین ستون مبلغ است)
                        if (invoiceData.Columns.Count > 0)
                        {
                            decimal total = 0;
                            for (int row = 0; row < invoiceData.Rows.Count; row++)
                            {
                                if (decimal.TryParse(invoiceData.Rows[row][invoiceData.Columns.Count - 1].ToString(), out decimal amount))
                                {
                                    total += amount;
                                }
                            }

                            worksheet.Cell(lastRow, invoiceData.Columns.Count).Value = total;
                            worksheet.Cell(lastRow, invoiceData.Columns.Count).Style.NumberFormat.Format = "#,##0";
                        }
                    }

                    // تنظیم عرض ستون‌ها
                    worksheet.Columns().AdjustToContents();

                    // ذخیره فایل
                    workbook.SaveAs(fileName);
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در ایجاد فاکتور Excel: {ex.Message}", "خطا");
                return false;
            }
        }
    }
}