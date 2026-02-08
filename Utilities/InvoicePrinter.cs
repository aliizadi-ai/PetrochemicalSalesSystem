using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace PetrochemicalSalesSystem.Utilities
{
    public class InvoicePrinter
    {
        private dynamic _invoice; // یا از مدل Invoice استفاده کنید
        private DataTable _itemsData;
        private PrintDocument _printDoc;

        public InvoicePrinter(dynamic invoice, DataTable itemsData)
        {
            _invoice = invoice;
            _itemsData = itemsData;
            InitializePrintDocument();
        }

        private void InitializePrintDocument()
        {
            _printDoc = new PrintDocument();
            //_printDoc.PrintPage += PrintDocument_PrintPag;
            _printDoc.DocumentName = $"فاکتور {_invoice.InvoiceNo}";
        }

        public void PrintPreview()
        {
            PrintPreviewDialog previewDialog = new PrintPreviewDialog();
            previewDialog.Document = _printDoc;
            previewDialog.WindowState = FormWindowState.Maximized;
            previewDialog.Text = $"پیش‌نمایش فاکتور {_invoice.InvoiceNo}";
            previewDialog.ShowDialog();
        }

        public void PrintDirect()
        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = _printDoc;

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                _printDoc.Print();
            }
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font titleFont = new Font("B Nazanin", 16, FontStyle.Bold);
            Font headerFont = new Font("B Nazanin", 11, FontStyle.Bold);
            Font normalFont = new Font("B Nazanin", 10);
            Font smallFont = new Font("B Nazanin", 9);

            int yPos = 50;
            int leftMargin = 50;

            // هدر
            g.DrawString("فاکتور فروش", titleFont, Brushes.Black, leftMargin, yPos);
            yPos += 40;

            // اطلاعات فاکتور
            g.DrawString($"شماره فاکتور: {_invoice.InvoiceNo}", normalFont, Brushes.Black, leftMargin, yPos);
            yPos += 25;

            if (_invoice.CustomerName != null)
            {
                g.DrawString($"مشتری: {_invoice.CustomerName}", normalFont, Brushes.Black, leftMargin, yPos);
                yPos += 25;
            }

            if (_invoice.TotalAmount != null)
            {
                g.DrawString($"مبلغ: {_invoice.TotalAmount:N0} تومان", normalFont, Brushes.Black, leftMargin, yPos);
                yPos += 25;
            }

            // آیتم‌ها
            if (_itemsData != null && _itemsData.Rows.Count > 0)
            {
                yPos += 20;
                g.DrawString("لیست کالاها:", headerFont, Brushes.Black, leftMargin, yPos);
                yPos += 30;

                // هدر جدول
                g.DrawLine(Pens.Black, leftMargin, yPos, 750, yPos);
                yPos += 10;

                // داده‌های جدول
                foreach (DataRow row in _itemsData.Rows)
                {
                    string rowText = $"{row["نام محصول"]} - {row["تعداد"]} {row["واحد"]} = {row["مبلغ"]}";
                    g.DrawString(rowText, normalFont, Brushes.Black, leftMargin, yPos);
                    yPos += 25;
                }
            }

            // پاورقی
            yPos += 30;
            g.DrawString("با تشکر از خرید شما", normalFont, Brushes.Black, leftMargin, yPos);
            yPos += 20;
            g.DrawString($"تاریخ چاپ: {DateTime.Now:yyyy/MM/dd HH:mm}", smallFont, Brushes.Gray, leftMargin, yPos);

            e.HasMorePages = false;
        }
    }
}