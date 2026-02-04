using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PetrochemicalSalesSystem.Forms
{
    public partial class ReportForm : Form
    {
        private Font titleFont = new Font("B Nazanin", 16, FontStyle.Bold);
        private Font normalFont = new Font("B Nazanin", 11);

        private Color primaryColor = Color.FromArgb(0, 102, 51);

        private Panel filterPanel;
        private Panel chartPanel;
        private DataGridView reportGrid;

        public ReportForm()
        {
            InitializeComponent();
            InitializeDesign();
        }

        private void InitializeDesign()
        {
            this.Text = "📊 سیستم گزارش‌گیری پیشرفته";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.BackColor = Color.FromArgb(240, 242, 245);

            CreateFilterPanel();
            CreateChartPanel();
            CreateReportGrid();
            //CreateExportPanel();
        }
        private void CreateReportGrid()
        {
            reportGrid = new DataGridView();
            reportGrid.Dock = DockStyle.Fill;
            reportGrid.Location = new Point(0, 420); // بعد از filterPanel و chartPanel
            reportGrid.Size = new Size(1180, 300);
            reportGrid.Name = "reportGrid";

            // تنظیمات ظاهری DataGridView
            reportGrid.BackgroundColor = Color.White;
            reportGrid.BorderStyle = BorderStyle.Fixed3D;
            reportGrid.Font = normalFont;
            reportGrid.RowHeadersVisible = false;
            reportGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            reportGrid.MultiSelect = false;
            reportGrid.AllowUserToAddRows = false;
            reportGrid.AllowUserToDeleteRows = false;
            reportGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            reportGrid.ReadOnly = true;

            // استایل هدر ستون‌ها
            reportGrid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle()
            {
                BackColor = primaryColor,
                ForeColor = Color.White,
                Font = new Font("B Nazanin", 11, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            // استایل ردیف‌های متناوب
            reportGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);

            // استایل سلول‌های عادی
            reportGrid.DefaultCellStyle = new DataGridViewCellStyle()
            {
                Font = normalFont,
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(5)
            };

            // اضافه کردن ستون‌های نمونه برای گزارش
            InitializeReportColumns();

            // رویدادهای DataGridView
            reportGrid.CellFormatting += ReportGrid_CellFormatting;
            reportGrid.CellDoubleClick += ReportGrid_CellDoubleClick;

            // ایجاد ContextMenu برای عملیات سریع
            CreateReportContextMenu();

            this.Controls.Add(reportGrid);
        }

        private void InitializeReportColumns()
        {
            // ستون‌های پیش‌فرض برای گزارش کلی
            reportGrid.Columns.Clear();

            // ستون شماره ردیف
            DataGridViewTextBoxColumn colIndex = new DataGridViewTextBoxColumn();
            colIndex.HeaderText = "ردیف";
            colIndex.Name = "colIndex";
            colIndex.Width = 60;
            colIndex.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            reportGrid.Columns.Add(colIndex);

            // ستون کد پرسنلی
            DataGridViewTextBoxColumn colEmpCode = new DataGridViewTextBoxColumn();
            colEmpCode.HeaderText = "کد پرسنلی";
            colEmpCode.Name = "colEmployeeCode";
            colEmpCode.Width = 120;
            reportGrid.Columns.Add(colEmpCode);

            // ستون نام کامل
            DataGridViewTextBoxColumn colFullName = new DataGridViewTextBoxColumn();
            colFullName.HeaderText = "نام و نام خانوادگی";
            colFullName.Name = "colFullName";
            colFullName.Width = 200;
            reportGrid.Columns.Add(colFullName);

            // ستون دپارتمان
            DataGridViewTextBoxColumn colDepartment = new DataGridViewTextBoxColumn();
            colDepartment.HeaderText = "دپارتمان";
            colDepartment.Name = "colDepartment";
            colDepartment.Width = 150;
            reportGrid.Columns.Add(colDepartment);

            // ستون سمت
            DataGridViewTextBoxColumn colPosition = new DataGridViewTextBoxColumn();
            colPosition.HeaderText = "سمت";
            colPosition.Name = "colPosition";
            colPosition.Width = 150;
            reportGrid.Columns.Add(colPosition);

            // ستون حقوق پایه
            DataGridViewTextBoxColumn colBaseSalary = new DataGridViewTextBoxColumn();
            colBaseSalary.HeaderText = "حقوق پایه";
            colBaseSalary.Name = "colBaseSalary";
            colBaseSalary.Width = 120;
            colBaseSalary.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            colBaseSalary.DefaultCellStyle.Format = "N0";
            reportGrid.Columns.Add(colBaseSalary);

            // ستون تاریخ استخدام
            DataGridViewTextBoxColumn colHireDate = new DataGridViewTextBoxColumn();
            colHireDate.HeaderText = "تاریخ استخدام";
            colHireDate.Name = "colHireDate";
            colHireDate.Width = 120;
            reportGrid.Columns.Add(colHireDate);

            // ستون وضعیت
            DataGridViewTextBoxColumn colStatus = new DataGridViewTextBoxColumn();
            colStatus.HeaderText = "وضعیت";
            colStatus.Name = "colStatus";
            colStatus.Width = 100;
            reportGrid.Columns.Add(colStatus);

            // ستون عملیات
            DataGridViewButtonColumn colAction = new DataGridViewButtonColumn();
            colAction.HeaderText = "عملیات";
            colAction.Name = "colAction";
            colAction.Text = "مشاهده جزئیات";
            colAction.UseColumnTextForButtonValue = true;
            colAction.Width = 120;
            reportGrid.Columns.Add(colAction);
        }

        private void ReportGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // رنگ‌آمیزی وضعیت فعال/غیرفعال
                if (reportGrid.Columns[e.ColumnIndex].Name == "colStatus")
                {
                    if (e.Value != null)
                    {
                        string status = e.Value.ToString();
                        if (status == "فعال")
                        {
                            e.CellStyle.BackColor = Color.FromArgb(212, 237, 218);
                            e.CellStyle.ForeColor = Color.FromArgb(21, 87, 36);
                        }
                        else if (status == "غیرفعال")
                        {
                            e.CellStyle.BackColor = Color.FromArgb(248, 215, 218);
                            e.CellStyle.ForeColor = Color.FromArgb(114, 28, 36);
                        }
                    }
                }

                // رنگ‌آمیزی ستون حقوق
                if (reportGrid.Columns[e.ColumnIndex].Name == "colBaseSalary")
                {
                    e.CellStyle.Font = new Font("B Nazanin", 10, FontStyle.Bold);
                    e.CellStyle.ForeColor = Color.FromArgb(0, 102, 51);
                }

                // شماره ردیف
                if (reportGrid.Columns[e.ColumnIndex].Name == "colIndex")
                {
                    e.Value = (e.RowIndex + 1).ToString();
                    e.CellStyle.BackColor = Color.FromArgb(240, 240, 240);
                }
            }
        }

        private void ReportGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // اگر روی ردیف دابل کلیک شد (به جز روی دکمه)
                if (reportGrid.Columns[e.ColumnIndex].Name != "colAction")
                {
                    DataGridViewRow row = reportGrid.Rows[e.RowIndex];

                    // گرفتن AccountantID (فرض می‌کنیم در Tag ردیف ذخیره شده)
                    if (row.Tag != null && row.Tag is long accountantId)
                    {
                        //AccountantDetailForm detailForm = new AccountantDetailForm(accountantId);
                        //detailForm.ShowDialog();
                    }
                    else
                    {
                        // یا از سلول‌های دیگر اطلاعات را بگیر
                        string employeeCode = row.Cells["colEmployeeCode"].Value?.ToString();
                        if (!string.IsNullOrEmpty(employeeCode))
                        {
                            MessageBox.Show($"انتخاب شد: {row.Cells["colFullName"].Value}", "اطلاعات",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
        }

        private void CreateReportContextMenu()
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();

            ToolStripMenuItem viewDetailsItem = new ToolStripMenuItem("👁️ مشاهده جزئیات");
            viewDetailsItem.Click += (s, e) =>
            {
                if (reportGrid.SelectedRows.Count > 0)
                {
                    DataGridViewRow row = reportGrid.SelectedRows[0];
                    // کد مشاهده جزئیات
                }
            };

            ToolStripMenuItem printItem = new ToolStripMenuItem("🖨️ چاپ ردیف انتخاب شده");
            printItem.Click += (s, e) =>
            {
                PrintSelectedRow();
            };

            ToolStripMenuItem copyItem = new ToolStripMenuItem("📋 کپی اطلاعات");
            copyItem.Click += (s, e) =>
            {
                CopyRowToClipboard();
            };

            ToolStripSeparator separator = new ToolStripSeparator();

            ToolStripMenuItem exportExcelItem = new ToolStripMenuItem("📊 خروجی Excel این ردیف");
            exportExcelItem.Click += (s, e) =>
            {
                ExportSelectedRowToExcel();
            };

            ToolStripMenuItem exportPDFItem = new ToolStripMenuItem("📄 خروجی PDF این ردیف");
            exportPDFItem.Click += (s, e) =>
            {
                ExportSelectedRowToPDF();
            };

            contextMenu.Items.Add(viewDetailsItem);
            contextMenu.Items.Add(printItem);
            contextMenu.Items.Add(copyItem);
            contextMenu.Items.Add(separator);
            contextMenu.Items.Add(exportExcelItem);
            contextMenu.Items.Add(exportPDFItem);

            reportGrid.ContextMenuStrip = contextMenu;
        }

        /*
        private void CreateExportPanel()
        {
            exportPanel = new Panel();
            exportPanel.Dock = DockStyle.Bottom;
            exportPanel.Height = 70;
            exportPanel.BackColor = Color.White;
            exportPanel.BorderStyle = BorderStyle.FixedSingle;
            exportPanel.Padding = new Padding(20, 10, 20, 10);

            // دکمه خروجی Excel
            Button btnExportExcel = CreateExportButton("📊 خروجی Excel", Color.FromArgb(25, 135, 84));
            btnExportExcel.Size = new Size(150, 45);
            btnExportExcel.Location = new Point(20, 10);
            btnExportExcel.Click += BtnExportExcel_Click;

            // دکمه خروجی PDF
            Button btnExportPDF = CreateExportButton("📄 خروجی PDF", Color.FromArgb(220, 53, 69));
            btnExportPDF.Size = new Size(150, 45);
            btnExportPDF.Location = new Point(185, 10);
            btnExportPDF.Click += BtnExportPDF_Click;

            // دکمه خروجی CSV
            Button btnExportCSV = CreateExportButton("📝 خروجی CSV", Color.FromArgb(13, 110, 253));
            btnExportCSV.Size = new Size(150, 45);
            btnExportCSV.Location = new Point(350, 10);
            btnExportCSV.Click += BtnExportCSV_Click;

            // دکمه چاپ
            Button btnPrint = CreateExportButton("🖨️ چاپ گزارش", Color.FromArgb(108, 117, 125));
            btnPrint.Size = new Size(150, 45);
            btnPrint.Location = new Point(515, 10);
            btnPrint.Click += BtnPrint_Click;

            // دکمه بارگذاری مجدد
            Button btnReload = CreateExportButton("🔄 بارگذاری مجدد", Color.FromArgb(255, 193, 7));
            btnReload.Size = new Size(150, 45);
            btnReload.Location = new Point(680, 10);
            btnReload.Click += BtnReload_Click;

            // برچسب تعداد رکوردها
            Label lblRecordCount = new Label();
            lblRecordCount.Name = "lblRecordCount";
            lblRecordCount.Text = "تعداد رکوردها: 0";
            lblRecordCount.Font = new Font("B Nazanin", 11, FontStyle.Bold);
            lblRecordCount.ForeColor = primaryColor;
            lblRecordCount.AutoSize = true;
            lblRecordCount.Location = new Point(1000, 20);
            lblRecordCount.TextAlign = ContentAlignment.MiddleRight;

            exportPanel.Controls.Add(btnExportExcel);
            exportPanel.Controls.Add(btnExportPDF);
            exportPanel.Controls.Add(btnExportCSV);
            exportPanel.Controls.Add(btnPrint);
            exportPanel.Controls.Add(btnReload);
            exportPanel.Controls.Add(lblRecordCount);

            this.Controls.Add(exportPanel);
        }
        */
        private Button CreateExportButton(string text, Color backColor)
        {
            Button button = new Button();
            button.Text = text;
            button.BackColor = backColor;
            button.ForeColor = Color.White;
            button.Font = new Font("B Nazanin", 10, FontStyle.Bold);
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;
            button.Padding = new Padding(10, 5, 10, 5);

            // افکت hover
            button.MouseEnter += (s, e) =>
            {
                button.BackColor = ControlPaint.Light(backColor, 0.2f);
            };

            button.MouseLeave += (s, e) =>
            {
                button.BackColor = backColor;
            };

            // افکت کلیک
            button.MouseDown += (s, e) =>
            {
                button.BackColor = ControlPaint.Dark(backColor, 0.1f);
            };

            button.MouseUp += (s, e) =>
            {
                button.BackColor = backColor;
            };

            return button;
        }

        // رویدادهای دکمه‌های خروجی
        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            if (reportGrid.Rows.Count == 0)
            {
                MessageBox.Show("داده‌ای برای خروجی وجود ندارد.", "هشدار",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "فایل Excel|*.xlsx";
            saveDialog.Title = "ذخیره به عنوان فایل Excel";
            saveDialog.FileName = $"گزارش_حسابداران_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ExportToExcel(saveDialog.FileName);
                    MessageBox.Show("خروجی Excel با موفقیت ایجاد شد.", "موفقیت",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"خطا در ایجاد خروجی Excel: {ex.Message}", "خطا",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnExportPDF_Click(object sender, EventArgs e)
        {
            if (reportGrid.Rows.Count == 0)
            {
                MessageBox.Show("داده‌ای برای خروجی وجود ندارد.", "هشدار",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "فایل PDF|*.pdf";
            saveDialog.Title = "ذخیره به عنوان فایل PDF";
            saveDialog.FileName = $"گزارش_حسابداران_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ExportToPDF(saveDialog.FileName);
                    MessageBox.Show("خروجی PDF با موفقیت ایجاد شد.", "موفقیت",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"خطا در ایجاد خروجی PDF: {ex.Message}", "خطا",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnExportCSV_Click(object sender, EventArgs e)
        {
            if (reportGrid.Rows.Count == 0)
            {
                MessageBox.Show("داده‌ای برای خروجی وجود ندارد.", "هشدار",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "فایل CSV|*.csv";
            saveDialog.Title = "ذخیره به عنوان فایل CSV";
            saveDialog.FileName = $"گزارش_حسابداران_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ExportToCSV(saveDialog.FileName);
                    MessageBox.Show("خروجی CSV با موفقیت ایجاد شد.", "موفقیت",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"خطا در ایجاد خروجی CSV: {ex.Message}", "خطا",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            if (reportGrid.Rows.Count == 0)
            {
                MessageBox.Show("داده‌ای برای چاپ وجود ندارد.", "هشدار",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PrintDialog printDialog = new PrintDialog();
            PrintDocument printDoc = new PrintDocument();
            printDoc.DocumentName = $"گزارش حسابداران - {DateTime.Now:yyyy/MM/dd}";

            printDoc.PrintPage += (s, ev) =>
            {
                // کد چاپ گزارش
                PrintReport(ev);
            };

            printDialog.Document = printDoc;

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDoc.Print();
            }
        }

        private void BtnReload_Click(object sender, EventArgs e)
        {
            // بارگذاری مجدد داده‌ها
            LoadReportData();
        }

        // متدهای کمکی برای خروجی‌ها
        private void ExportToExcel(string filePath)
        {
            try
            {
                // برای استفاده از Excel باید Microsoft.Office.Interop.Excel را نصب کنید
                // یا از کتابخانه‌هایی مثل EPPlus استفاده کنید

                // در اینجا یک نمونه ساده با CSV ارائه می‌دهیم:
                StringBuilder sb = new StringBuilder();

                // هدرها
                List<string> headers = new List<string>();
                foreach (DataGridViewColumn column in reportGrid.Columns)
                {
                    if (column.Visible && column.Name != "colAction")
                        headers.Add(column.HeaderText);
                }
                sb.AppendLine(string.Join(",", headers));

                // داده‌ها
                foreach (DataGridViewRow row in reportGrid.Rows)
                {
                    if (row.IsNewRow) continue;

                    List<string> cells = new List<string>();
                    foreach (DataGridViewColumn column in reportGrid.Columns)
                    {
                        if (column.Visible && column.Name != "colAction")
                        {
                            object value = row.Cells[column.Index].Value;
                            cells.Add(value?.ToString() ?? "");
                        }
                    }
                    sb.AppendLine(string.Join(",", cells));
                }

                //File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception($"خطا در ذخیره فایل: {ex.Message}");
            }
        }

        private void ExportToPDF(string filePath)
        {
            // نیاز به نصب کتابخانه iTextSharp یا مشابه
            MessageBox.Show("برای خروجی PDF نیاز به نصب کتابخانه iTextSharp دارید.", "اطلاعات",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ExportToCSV(string filePath)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                // هدرها
                List<string> headers = new List<string>();
                foreach (DataGridViewColumn column in reportGrid.Columns)
                {
                    if (column.Visible && column.Name != "colAction")
                        headers.Add($"\"{column.HeaderText}\"");
                }
                sb.AppendLine(string.Join(",", headers));

                // داده‌ها
                foreach (DataGridViewRow row in reportGrid.Rows)
                {
                    if (row.IsNewRow) continue;

                    List<string> cells = new List<string>();
                    foreach (DataGridViewColumn column in reportGrid.Columns)
                    {
                        if (column.Visible && column.Name != "colAction")
                        {
                            object value = row.Cells[column.Index].Value;
                            // فرار از کاما و نقل قول
                            string cellValue = value?.ToString() ?? "";
                            cellValue = cellValue.Replace("\"", "\"\"");
                            cells.Add($"\"{cellValue}\"");
                        }
                    }
                    sb.AppendLine(string.Join(",", cells));
                }

                //File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception($"خطا در ذخیره فایل CSV: {ex.Message}");
            }
        }

        private void PrintReport(PrintPageEventArgs ev)
        {
            Font printFont = new Font("B Nazanin", 12);
            Font headerFont = new Font("B Nazanin", 14, FontStyle.Bold);
            Brush brush = Brushes.Black;

            float yPos = 50;
            float leftMargin = ev.MarginBounds.Left;

            // چاپ هدر
            ev.Graphics.DrawString("گزارش حسابداران پتروشیمی", headerFont, brush, leftMargin, yPos);
            yPos += 40;

            // چاپ تاریخ
            ev.Graphics.DrawString($"تاریخ گزارش: {DateTime.Now:yyyy/MM/dd}", printFont, brush, leftMargin, yPos);
            yPos += 30;

            // چاپ جدول
            foreach (DataGridViewRow row in reportGrid.Rows)
            {
                if (row.IsNewRow) continue;

                string line = "";
                foreach (DataGridViewColumn column in reportGrid.Columns)
                {
                    if (column.Visible && column.Name != "colAction")
                    {
                        line += $"{column.HeaderText}: {row.Cells[column.Index].Value}   ";
                    }
                }

                ev.Graphics.DrawString(line, printFont, brush, leftMargin, yPos);
                yPos += 25;

                // اگر به انتهای صفحه رسیدیم
                if (yPos >= ev.MarginBounds.Bottom)
                {
                    ev.HasMorePages = true;
                    return;
                }
            }

            ev.HasMorePages = false;
        }

        // متدهای کمکی برای ContextMenu
        private void PrintSelectedRow()
        {
            if (reportGrid.SelectedRows.Count > 0)
            {
                DataGridViewRow row = reportGrid.SelectedRows[0];
                // کد چاپ ردیف انتخاب شده
            }
        }

        private void CopyRowToClipboard()
        {
            if (reportGrid.SelectedRows.Count > 0)
            {
                DataGridViewRow row = reportGrid.SelectedRows[0];
                StringBuilder sb = new StringBuilder();

                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Visible && reportGrid.Columns[cell.ColumnIndex].Name != "colAction")
                    {
                        sb.Append($"{reportGrid.Columns[cell.ColumnIndex].HeaderText}: ");
                        sb.Append($"{cell.Value}\t");
                    }
                }

                Clipboard.SetText(sb.ToString());
                MessageBox.Show("اطلاعات ردیف کپی شد.", "موفقیت",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ExportSelectedRowToExcel()
        {
            // مشابه ExportToExcel اما فقط برای ردیف انتخاب شده
        }

        private void ExportSelectedRowToPDF()
        {
            // مشابه ExportToPDF اما فقط برای ردیف انتخاب شده
        }

        // متد بارگذاری داده‌های گزارش
        private void LoadReportData()
        {
            try
            {
                // شبیه‌سازی داده‌های نمونه
                var sampleData = GetSampleReportData();

                // پاک کردن داده‌های قبلی
                reportGrid.Rows.Clear();

                // اضافه کردن داده‌های جدید
                foreach (var data in sampleData)
                {
                    int rowIndex = reportGrid.Rows.Add();
                    DataGridViewRow row = reportGrid.Rows[rowIndex];

                    row.Cells["colEmployeeCode"].Value = data.EmployeeCode;
                    row.Cells["colFullName"].Value = data.FullName;
                    row.Cells["colDepartment"].Value = data.Department;
                    row.Cells["colPosition"].Value = data.Position;
                    row.Cells["colBaseSalary"].Value = data.BaseSalary;
                    row.Cells["colHireDate"].Value = data.HireDate.ToString("yyyy/MM/dd");
                    row.Cells["colStatus"].Value = data.Status;

                    // ذخیره AccountantID در Tag ردیف
                    row.Tag = data.AccountantID;
                }

                // به‌روزرسانی تعداد رکوردها
                //UpdateRecordCount();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بارگذاری داده‌ها: {ex.Message}", "خطا",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /*
        private void UpdateRecordCount()
        {
            Control control = FindControlRecursive(this, "lblRecordCount");
            if (control is Label label)
            {
                label.Text = $"تعداد رکوردها: {reportGrid.Rows.Count}";
            }
        }
        */
        // کلاس نمونه برای داده‌های گزارش
        private class ReportData
        {
            public long AccountantID { get; set; }
            public string EmployeeCode { get; set; }
            public string FullName { get; set; }
            public string Department { get; set; }
            public string Position { get; set; }
            public decimal BaseSalary { get; set; }
            public DateTime HireDate { get; set; }
            public string Status { get; set; }
        }

        private List<ReportData> GetSampleReportData()
        {
            return new List<ReportData>
    {
        new ReportData { AccountantID = 1, EmployeeCode = "1001", FullName = "علی رضایی", Department = "حسابداری", Position = "حسابدار ارشد", BaseSalary = 15000000, HireDate = new DateTime(2020, 1, 15), Status = "فعال" },
        new ReportData { AccountantID = 2, EmployeeCode = "1002", FullName = "مریم محمدی", Department = "مالی", Position = "مدیر مالی", BaseSalary = 25000000, HireDate = new DateTime(2019, 5, 20), Status = "فعال" },
        new ReportData { AccountantID = 3, EmployeeCode = "1003", FullName = "حسین کریمی", Department = "حسابداری", Position = "حسابدار", BaseSalary = 12000000, HireDate = new DateTime(2021, 3, 10), Status = "فعال" },
        new ReportData { AccountantID = 4, EmployeeCode = "1004", FullName = "فاطمه احمدی", Department = "منابع انسانی", Position = "حسابدار حقوق", BaseSalary = 13000000, HireDate = new DateTime(2020, 8, 5), Status = "مرخصی" },
        new ReportData { AccountantID = 5, EmployeeCode = "1005", FullName = "محمد حسینی", Department = "مالی", Position = "کارشناس مالی", BaseSalary = 14000000, HireDate = new DateTime(2018, 11, 30), Status = "غیرفعال" }
    };
        }
        private Button CreateStyledButton(string text, Color backColor, Color foreColor)
        {
            Button button = new Button();
            button.Text = text;
            button.BackColor = backColor;
            button.ForeColor = foreColor;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = normalFont;
            button.Cursor = Cursors.Hand;
            button.Padding = new Padding(10, 5, 10, 5);

            // افکت hover
            button.MouseEnter += (s, e) =>
            {
                button.BackColor = ControlPaint.Light(backColor, 0.1f);
            };

            button.MouseLeave += (s, e) =>
            {
                button.BackColor = backColor;
            };

            return button;
        }
        private void CreateFilterPanel()
        {
            filterPanel = new Panel();
            filterPanel.Dock = DockStyle.Top;
            filterPanel.Height = 120;
            filterPanel.BackColor = Color.White;
            filterPanel.BorderStyle = BorderStyle.FixedSingle;
            filterPanel.Padding = new Padding(20);

            // عنوان
            Label titleLabel = new Label();
            titleLabel.Text = "⚙️ فیلترهای گزارش";
            titleLabel.Font = titleFont;
            titleLabel.ForeColor = primaryColor;
            titleLabel.AutoSize = true;
            titleLabel.Location = new Point(1000, 20);

            // نوع گزارش
            ComboBox reportType = new ComboBox();
            reportType.Font = normalFont;
            reportType.DropDownStyle = ComboBoxStyle.DropDownList;
            reportType.Items.AddRange(new string[] {
            "📈 گزارش حقوق و دستمزد",
            "👥 گزارش بر اساس دپارتمان",
            "📅 گزارش استخدام ماهانه",
            "🎓 گزارش سطح تحصیلات",
            "💰 گزارش مالیات",
            "🏥 گزارش بیمه",
            "📊 گزارش جامع عملکرد"
        });
            reportType.SelectedIndex = 0;
            reportType.Size = new Size(250, 30);
            reportType.Location = new Point(700, 20);

            // تاریخ از
            Label lblFrom = new Label();
            lblFrom.Text = "از تاریخ:";
            lblFrom.Font = normalFont;
            lblFrom.Size = new Size(70, 30);
            lblFrom.Location = new Point(620, 20);
            lblFrom.TextAlign = ContentAlignment.MiddleLeft;

            DateTimePicker dtpFrom = new DateTimePicker();
            dtpFrom.Font = normalFont;
            dtpFrom.Size = new Size(150, 30);
            dtpFrom.Location = new Point(460, 20);
            dtpFrom.Format = DateTimePickerFormat.Short;

            // تاریخ تا
            Label lblTo = new Label();
            lblTo.Text = "تا تاریخ:";
            lblTo.Font = normalFont;
            lblTo.Size = new Size(70, 30);
            lblTo.Location = new Point(380, 20);
            lblTo.TextAlign = ContentAlignment.MiddleLeft;

            DateTimePicker dtpTo = new DateTimePicker();
            dtpTo.Font = normalFont;
            dtpTo.Size = new Size(150, 30);
            dtpTo.Location = new Point(220, 20);
            dtpTo.Format = DateTimePickerFormat.Short;

            // دکمه تولید گزارش
            Button generateBtn = CreateStyledButton("🚀 تولید گزارش", primaryColor, Color.White);
            generateBtn.Size = new Size(150, 35);
            generateBtn.Location = new Point(40, 15);

            // ردیف دوم فیلترها
            Label lblDept = new Label();
            lblDept.Text = "دپارتمان:";
            lblDept.Font = normalFont;
            lblDept.Size = new Size(80, 30);
            lblDept.Location = new Point(1000, 70);
            lblDept.TextAlign = ContentAlignment.MiddleLeft;

            ComboBox cmbDept = new ComboBox();
            cmbDept.Font = normalFont;
            cmbDept.Size = new Size(180, 30);
            cmbDept.Location = new Point(800, 70);

            Label lblStatus = new Label();
            lblStatus.Text = "وضعیت:";
            lblStatus.Font = normalFont;
            lblStatus.Size = new Size(60, 30);
            lblStatus.Location = new Point(720, 70);
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;

            ComboBox cmbStatus = new ComboBox();
            cmbStatus.Font = normalFont;
            cmbStatus.Size = new Size(120, 30);
            cmbStatus.Location = new Point(590, 70);
            cmbStatus.Items.AddRange(new string[] { "همه", "فعال", "غیرفعال" });
            cmbStatus.SelectedIndex = 0;

            // اضافه کردن کنترل‌ها
            filterPanel.Controls.Add(titleLabel);
            filterPanel.Controls.Add(reportType);
            filterPanel.Controls.Add(lblFrom);
            filterPanel.Controls.Add(dtpFrom);
            filterPanel.Controls.Add(lblTo);
            filterPanel.Controls.Add(dtpTo);
            filterPanel.Controls.Add(generateBtn);
            filterPanel.Controls.Add(lblDept);
            filterPanel.Controls.Add(cmbDept);
            filterPanel.Controls.Add(lblStatus);
            filterPanel.Controls.Add(cmbStatus);

            this.Controls.Add(filterPanel);
        }

        private void CreateChartPanel()
        {
            chartPanel = new Panel();
            chartPanel.Dock = DockStyle.Top;
            chartPanel.Height = 300;
            chartPanel.BackColor = Color.White;
            chartPanel.BorderStyle = BorderStyle.FixedSingle;
            chartPanel.Padding = new Padding(10);

            // TabControl برای انواع نمودارها
            TabControl chartTabs = new TabControl();
            chartTabs.Dock = DockStyle.Fill;
            chartTabs.Appearance = TabAppearance.FlatButtons;

            TabPage tab1 = new TabPage("📊 نمودار ستونی");
            TabPage tab2 = new TabPage("📈 نمودار خطی");
            TabPage tab3 = new TabPage("🥧 نمودار دایره‌ای");

            // شبیه‌سازی نمودار با Panel
            Panel chart1 = CreateSampleChart(Color.FromArgb(54, 162, 235));
            Panel chart2 = CreateSampleChart(Color.FromArgb(255, 99, 132));
            Panel chart3 = CreateSampleChart(Color.FromArgb(75, 192, 192));

            tab1.Controls.Add(chart1);
            tab2.Controls.Add(chart2);
            tab3.Controls.Add(chart3);

            chartTabs.TabPages.Add(tab1);
            chartTabs.TabPages.Add(tab2);
            chartTabs.TabPages.Add(tab3);

            chartPanel.Controls.Add(chartTabs);
            this.Controls.Add(chartPanel);
        }

        private Panel CreateSampleChart(Color chartColor)
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.BackColor = Color.White;
            panel.Paint += (sender, e) =>
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // عنوان نمودار
                Font titleFont = new Font("B Nazanin", 12, FontStyle.Bold);
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                g.DrawString("توزیع حقوق بر اساس دپارتمان", titleFont,
                    new SolidBrush(Color.Black), panel.Width / 2, 20, sf);

                // رسم نمودار ساده
                int[] data = { 120, 80, 150, 90, 60 };
                string[] labels = { "مالی", "حسابداری", "منابع انسانی", "فناوری", "عملیات" };
                int barWidth = 50;
                int spacing = 20;
                int startX = 100;
                int maxHeight = 150;
                int maxValue = data.Max();

                for (int i = 0; i < data.Length; i++)
                {
                    int barHeight = (int)((double)data[i] / maxValue * maxHeight);
                    int x = startX + i * (barWidth + spacing);
                    int y = panel.Height - barHeight - 50;

                    // رسم ستون
                    g.FillRectangle(new SolidBrush(chartColor), x, y, barWidth, barHeight);
                    g.DrawRectangle(Pens.Black, x, y, barWidth, barHeight);

                    // مقدار
                    g.DrawString(data[i].ToString("N0"), normalFont, Brushes.Black,
                        x + barWidth / 2 - 10, y - 20);

                    // برچسب
                    g.DrawString(labels[i], normalFont, Brushes.Black,
                        x + barWidth / 2 - 15, panel.Height - 30);
                }
            };

            return panel;
        }
    }
}
