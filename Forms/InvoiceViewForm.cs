using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using ClosedXML.Excel;
using iTextSharp.text.pdf;
//using Font = iTextSharp.text.Font;


namespace PetrochemicalSalesSystem.Forms
{
    public partial class InvoiceViewForm : Form
    {
        // کنترل‌های اصلی
        private DataGridView dgvInvoices;
        private Panel mainPanel;
        private TextBox txtSearch;
        private ComboBox cmbFilterBy;
        private ComboBox cmbStatusFilter;
        private DateTimePicker dtpFromDate;
        private DateTimePicker dtpToDate;
        private TextBox txtMinAmount;
        private TextBox txtMaxAmount;
        private Button btnSearch;
        private Button btnResetFilters;
        private Button btnExportExcel;
        private Button btnExportPDF;
        private Button btnPrintInvoice;
        private Button btnRefresh;
        private Label lblTotalAmount;
        private Label lblInvoiceCount;

        // داده‌ها
        private DataTable invoicesData;

        public InvoiceViewForm()
        {
            InitializeComponent();
            InitializeComponent2();
            InitializeInvoiceViewForm();
            //LoadInvoices();
        }

        private void InitializeComponent2()
        {
            this.SuspendLayout();
            this.Size = new Size(1300, 800);
            this.Text = "مشاهده فاکتورها";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.BackColor = Color.FromArgb(240, 242, 245);
            this.Font = new Font("IRANSans", 9.75f, FontStyle.Regular);
            this.ResumeLayout(false);
        }

        private void InitializeInvoiceViewForm()
        {
            this.Controls.Clear();

            // ایجاد پنل اصلی برای اسکرول
            mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.AutoScroll = true;
            mainPanel.AutoScrollMinSize = new Size(1280, 1200);
            this.Controls.Add(mainPanel);

            CreateHeader();
            CreateFilterSection();
            CreateStatisticsSection();
            CreateInvoicesGrid();
            CreateButtonsSection();
        }

        private void CreateHeader()
        {
            Panel headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 80;
            headerPanel.BackColor = Color.FromArgb(0, 102, 51);
            headerPanel.Padding = new Padding(20, 0, 20, 0);

            Label title = new Label();
            title.Text = "📋 مشاهده فاکتورها";
            title.Font = new Font("IRANSans", 18, FontStyle.Bold);
            title.ForeColor = Color.White;
            title.Dock = DockStyle.Fill;
            title.TextAlign = ContentAlignment.MiddleCenter;

            // دکمه بستن
            Button btnClose = new Button();
            btnClose.Text = "✕";
            btnClose.Font = new Font("Arial", 12, FontStyle.Bold);
            btnClose.Size = new Size(40, 40);
            btnClose.Location = new Point(20, 20);
            btnClose.BackColor = Color.Transparent;
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += (s, e) => this.Close();

            headerPanel.Controls.Add(title);
            headerPanel.Controls.Add(btnClose);
            mainPanel.Controls.Add(headerPanel);
        }

        private void CreateFilterSection()
        {
            Panel filterPanel = new Panel();
            filterPanel.Size = new Size(1450, 280);
            filterPanel.Location = new Point(25, 100);
            filterPanel.BackColor = Color.White;
            filterPanel.BorderStyle = BorderStyle.FixedSingle;
            filterPanel.Padding = new Padding(15);

            // عنوان بخش
            Label sectionTitle = new Label();
            sectionTitle.Text = "🔍 فیلتر و جستجوی پیشرفته";
            sectionTitle.Font = new Font("IRANSans", 12, FontStyle.Bold);
            sectionTitle.ForeColor = Color.FromArgb(0, 102, 51);
            sectionTitle.Size = new Size(250, 30);
            sectionTitle.Location = new Point(575, 30);
            filterPanel.Controls.Add(sectionTitle);

            // ردیف اول
            int y = 90;
            int x1 = 50, x2 = 390, x3 = 730, x4 = 1070, x5 = 1460;

            // جستجو
            CreateFilterField("جستجو", txtSearch = new TextBox(), x1, y, filterPanel, 200);
            txtSearch.Text = "شماره فاکتور، نام مشتری، کد ملی...";

            // فیلتر بر اساس
            CreateFilterField("فیلتر بر اساس", cmbFilterBy = new ComboBox(), x2, y, filterPanel, 200);
            cmbFilterBy.Items.AddRange(new string[] { "همه", "شماره فاکتور", "نام مشتری", "کد مشتری", "کد ملی", "توضیحات" });
            cmbFilterBy.SelectedIndex = 0;

            // وضعیت فاکتور
            CreateFilterField("وضعیت", cmbStatusFilter = new ComboBox(), x3, y, filterPanel, 200);
            cmbStatusFilter.Items.AddRange(new string[] { "همه", "پرداخت شده", "پرداخت نشده", "بخشی پرداخت شده", "لغو شده" });
            cmbStatusFilter.SelectedIndex = 0;

            // ردیف دوم
            y += 50;
            CreateFilterField("از تاریخ", dtpFromDate = new DateTimePicker(), x1, y, filterPanel, 200);
            dtpFromDate.Value = DateTime.Now.AddMonths(-1);
            dtpFromDate.Format = DateTimePickerFormat.Short;

            CreateFilterField("تا تاریخ", dtpToDate = new DateTimePicker(), x2, y, filterPanel, 200);
            dtpToDate.Value = DateTime.Now;
            dtpToDate.Format = DateTimePickerFormat.Short;

            CreateFilterField("حداقل مبلغ", txtMinAmount = new TextBox(), x3, y, filterPanel, 200);
            txtMinAmount.Text = "مثال: 1000000";

            CreateFilterField("حداکثر مبلغ", txtMaxAmount = new TextBox(), x4, y, filterPanel, 200);
            txtMaxAmount.Text = "مثال: 5000000";

            // دکمه‌های ردیف سوم
            y += 60;
            btnSearch = new Button();
            btnSearch.Text = "🔍 اعمال فیلتر";
            btnSearch.Size = new Size(150, 50);
            btnSearch.Location = new Point(450, y);
            btnSearch.Font = new Font("IRANSans", 11, FontStyle.Bold);
            btnSearch.BackColor = Color.FromArgb(52, 152, 219);
            btnSearch.ForeColor = Color.White;
            btnSearch.FlatStyle = FlatStyle.Flat;
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Cursor = Cursors.Hand;
            btnSearch.Click += BtnSearch_Click;
            filterPanel.Controls.Add(btnSearch);

            btnResetFilters = new Button();
            btnResetFilters.Text = "🔄 بازنشانی فیلترها";
            btnResetFilters.Size = new Size(150, 50);
            btnResetFilters.Location = new Point(650, y);
            btnResetFilters.Font = new Font("IRANSans", 11);
            btnResetFilters.BackColor = Color.FromArgb(241, 196, 15);
            btnResetFilters.ForeColor = Color.White;
            btnResetFilters.FlatStyle = FlatStyle.Flat;
            btnResetFilters.FlatAppearance.BorderSize = 0;
            btnResetFilters.Cursor = Cursors.Hand;
            btnResetFilters.Click += BtnResetFilters_Click;
            filterPanel.Controls.Add(btnResetFilters);

            btnRefresh = new Button();
            btnRefresh.Text = "🔄 بروزرسانی لیست";
            btnRefresh.Size = new Size(150, 50);
            btnRefresh.Location = new Point(850, y);
            btnRefresh.Font = new Font("IRANSans", 11);
            btnRefresh.BackColor = Color.FromArgb(155, 89, 182);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Cursor = Cursors.Hand;
            btnRefresh.Click += (s, e) => LoadInvoices();
            filterPanel.Controls.Add(btnRefresh);

            mainPanel.Controls.Add(filterPanel);
        }

        private void CreateStatisticsSection()
        {
            Panel statsPanel = new Panel();
            statsPanel.Size = new Size(1450, 80);
            statsPanel.Location = new Point(25, 315);
            statsPanel.BackColor = Color.FromArgb(230, 247, 255);
            statsPanel.BorderStyle = BorderStyle.FixedSingle;

            // آمار سریع
            Label statsTitle = new Label();
            statsTitle.Text = "📊 آمار فاکتورها";
            statsTitle.Font = new Font("IRANSans", 11, FontStyle.Bold);
            statsTitle.ForeColor = Color.FromArgb(0, 102, 51);
            statsTitle.Size = new Size(150, 30);
            statsTitle.Location = new Point(30, 25);
            statsTitle.TextAlign = ContentAlignment.MiddleRight;
            statsPanel.Controls.Add(statsTitle);

            // تعداد فاکتورها
            Label lblCountTitle = new Label();
            lblCountTitle.Text = "تعداد فاکتورها:";
            lblCountTitle.Font = new Font("IRANSans", 10);
            lblCountTitle.Size = new Size(100, 30);
            lblCountTitle.Location = new Point(200, 25);
            lblCountTitle.TextAlign = ContentAlignment.MiddleRight;
            statsPanel.Controls.Add(lblCountTitle);

            lblInvoiceCount = new Label();
            lblInvoiceCount.Text = "0";
            lblInvoiceCount.Font = new Font("IRANSans", 12, FontStyle.Bold);
            lblInvoiceCount.ForeColor = Color.FromArgb(231, 76, 60);
            lblInvoiceCount.Size = new Size(80, 30);
            lblInvoiceCount.Location = new Point(305, 25);
            lblInvoiceCount.TextAlign = ContentAlignment.MiddleCenter;
            lblInvoiceCount.BorderStyle = BorderStyle.FixedSingle;
            lblInvoiceCount.BackColor = Color.White;
            statsPanel.Controls.Add(lblInvoiceCount);

            // مجموع مبالغ
            Label lblAmountTitle = new Label();
            lblAmountTitle.Text = "مجموع مبالغ:";
            lblAmountTitle.Font = new Font("IRANSans", 10);
            lblAmountTitle.Size = new Size(100, 30);
            lblAmountTitle.Location = new Point(400, 25);
            lblAmountTitle.TextAlign = ContentAlignment.MiddleRight;
            statsPanel.Controls.Add(lblAmountTitle);

            lblTotalAmount = new Label();
            lblTotalAmount.Text = "0 تومان";
            lblTotalAmount.Font = new Font("IRANSans", 12, FontStyle.Bold);
            lblTotalAmount.ForeColor = Color.FromArgb(46, 204, 113);
            lblTotalAmount.Size = new Size(150, 30);
            lblTotalAmount.Location = new Point(505, 25);
            lblTotalAmount.TextAlign = ContentAlignment.MiddleCenter;
            lblTotalAmount.BorderStyle = BorderStyle.FixedSingle;
            lblTotalAmount.BackColor = Color.White;
            statsPanel.Controls.Add(lblTotalAmount);

            // میانگین مبلغ
            Label lblAvgTitle = new Label();
            lblAvgTitle.Text = "میانگین مبلغ:";
            lblAvgTitle.Font = new Font("IRANSans", 10);
            lblAvgTitle.Size = new Size(100, 30);
            lblAvgTitle.Location = new Point(670, 25);
            lblAvgTitle.TextAlign = ContentAlignment.MiddleRight;
            statsPanel.Controls.Add(lblAvgTitle);

            Label lblAvgAmount = new Label();
            lblAvgAmount.Text = "0 تومان";
            lblAvgAmount.Font = new Font("IRANSans", 11);
            lblAvgAmount.Size = new Size(150, 30);
            lblAvgAmount.Location = new Point(775, 25);
            lblAvgAmount.TextAlign = ContentAlignment.MiddleCenter;
            lblAvgAmount.BorderStyle = BorderStyle.FixedSingle;
            lblAvgAmount.BackColor = Color.White;
            lblAvgAmount.Tag = "avgAmount"; // برای به‌روزرسانی
            statsPanel.Controls.Add(lblAvgAmount);

            mainPanel.Controls.Add(statsPanel);
        }
        private void CreateInvoicesGrid()
        {
            Panel gridPanel = new Panel();
            gridPanel.Size = new Size(1450, 350);
            gridPanel.Location = new Point(25, 410);
            gridPanel.BackColor = Color.White;
            gridPanel.BorderStyle = BorderStyle.FixedSingle;
            gridPanel.Padding = new Padding(10);
            gridPanel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top; // اضافه کردن Anchor

            // عنوان بخش
            Label sectionTitle = new Label();
            sectionTitle.Text = "📄 لیست فاکتورها";
            sectionTitle.Font = new Font("IRANSans", 12, FontStyle.Bold);
            sectionTitle.ForeColor = Color.FromArgb(0, 102, 51);
            sectionTitle.Size = new Size(200, 30);
            sectionTitle.Location = new Point(575, 30);
            gridPanel.Controls.Add(sectionTitle);

            // DataGridView
            dgvInvoices = new DataGridView();
            dgvInvoices.Size = new Size(1410, 270);
            dgvInvoices.Location = new Point(10, 80);
            dgvInvoices.BackgroundColor = Color.White;
            dgvInvoices.BorderStyle = BorderStyle.FixedSingle;
            dgvInvoices.RowHeadersVisible = false;
            dgvInvoices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells; // تغییر این خط
            dgvInvoices.AllowUserToAddRows = false;
            dgvInvoices.AllowUserToDeleteRows = false;
            dgvInvoices.ReadOnly = true;
            dgvInvoices.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvInvoices.MultiSelect = true;
            dgvInvoices.ScrollBars = ScrollBars.Both;
            dgvInvoices.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right; // اضافه کردن Anchor

            // استایل‌دهی
            dgvInvoices.EnableHeadersVisualStyles = false;
            dgvInvoices.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 102, 51);
            dgvInvoices.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvInvoices.ColumnHeadersDefaultCellStyle.Font = new Font("IRANSans", 8, FontStyle.Bold);
            dgvInvoices.ColumnHeadersHeight = 60;
            dgvInvoices.RowTemplate.Height = 40;
            dgvInvoices.DefaultCellStyle.Font = new Font("IRANSans", 7);
            dgvInvoices.DefaultCellStyle.Padding = new Padding(5);

            // رویداد دو بار کلیک برای مشاهده جزئیات
            dgvInvoices.CellDoubleClick += DgvInvoices_CellDoubleClick;

            // اضافه کردن ستون‌ها و داده نمونه
            //SetupInvoicesColumns();
            //AddSampleData();

            gridPanel.Controls.Add(dgvInvoices);
            mainPanel.Controls.Add(gridPanel);
        }

        private void SetupInvoicesColumns()
        {
            dgvInvoices.Columns.Clear();

            DataGridViewColumn[] columns = {
        new DataGridViewTextBoxColumn() { Name = "InvoiceID", HeaderText = "شماره فاکتور", Width = 120 },
        new DataGridViewTextBoxColumn() { Name = "CompanyName", HeaderText = "نام مشتری", Width = 250 },
        new DataGridViewTextBoxColumn() { Name = "TotalAmount", HeaderText = "مبلغ (ریال)", Width = 150 },
        new DataGridViewTextBoxColumn() { Name = "InvoiceDate", HeaderText = "تاریخ", Width = 120 },
        new DataGridViewTextBoxColumn() { Name = "Status", HeaderText = "وضعیت", Width = 100 }
        //new DataGridViewTextBoxColumn() { Name = "Description", HeaderText = "توضیحات", Width = 300 } // عرض زیاد برای تست اسکرول
        };

            foreach (var column in columns)
            {
                dgvInvoices.Columns.Add(column);
            }
        }

        private void AddSampleData()
        {
            for (int i = 1; i <= 100; i++) // تعداد زیاد ردیف برای تست اسکرول عمودی
            {
                dgvInvoices.Rows.Add(
                    i.ToString("00000"),
                    $"مشتری شرکت نمونه شماره {i}",
                    (i * 1250000).ToString("N0"),
                    $"1403/{i % 12 + 1:00}/{i % 28 + 1:00}",
                    i % 3 == 0 ? "پرداخت شده" : "در انتظار",
                    $"توضیحات کامل فاکتور شماره {i} با جزئیات کامل و متن طولانی برای تست نمایش اسکرول بار"
                );
            }
        }
        /*
        private void CreateInvoicesGrid()
        {
            Panel gridPanel = new Panel();
            gridPanel.Size = new Size(1250, 400);
            gridPanel.Location = new Point(25, 410);
            gridPanel.BackColor = Color.White;
            gridPanel.BorderStyle = BorderStyle.FixedSingle;
            gridPanel.Padding = new Padding(10);

            // عنوان بخش
            Label sectionTitle = new Label();
            sectionTitle.Text = "📄 لیست فاکتورها";
            sectionTitle.Font = new Font("IRANSans", 12, FontStyle.Bold);
            sectionTitle.ForeColor = Color.FromArgb(0, 102, 51);
            sectionTitle.Size = new Size(200, 30);
            sectionTitle.Location = new Point(20, 15);
            gridPanel.Controls.Add(sectionTitle);

            // DataGridView
            dgvInvoices = new DataGridView();
            dgvInvoices.Size = new Size(1220, 340);
            dgvInvoices.Location = new Point(10, 50);
            dgvInvoices.BackgroundColor = Color.White;
            dgvInvoices.BorderStyle = BorderStyle.FixedSingle;
            dgvInvoices.RowHeadersVisible = false;
            dgvInvoices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvInvoices.AllowUserToAddRows = false;
            dgvInvoices.AllowUserToDeleteRows = false;
            dgvInvoices.ReadOnly = true;
            dgvInvoices.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvInvoices.MultiSelect = true;
            dgvInvoices.ScrollBars = ScrollBars.Both;

            // استایل‌دهی
            dgvInvoices.EnableHeadersVisualStyles = false;
            dgvInvoices.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 102, 51);
            dgvInvoices.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvInvoices.ColumnHeadersDefaultCellStyle.Font = new Font("IRANSans", 8, FontStyle.Bold);
            dgvInvoices.ColumnHeadersHeight = 60;
            dgvInvoices.RowTemplate.Height = 40;
            dgvInvoices.DefaultCellStyle.Font = new Font("IRANSans", 7);
            dgvInvoices.DefaultCellStyle.Padding = new Padding(5);

            // رویداد دو بار کلیک برای مشاهده جزئیات
            dgvInvoices.CellDoubleClick += DgvInvoices_CellDoubleClick;

            gridPanel.Controls.Add(dgvInvoices);
            mainPanel.Controls.Add(gridPanel);
        }
        */

        private void CreateButtonsSection()
        {
            Panel buttonPanel = new Panel();
            buttonPanel.Size = new Size(1450, 70);
            buttonPanel.Location = new Point(10, 800);
            buttonPanel.BackColor = Color.Transparent;

            int x = 950;
            int spacing = 160;

            // دکمه مشاهده جزئیات
            Button btnViewDetails = new Button();
            btnViewDetails.Text = "👁️ مشاهده جزئیات";
            btnViewDetails.Size = new Size(150, 60);
            btnViewDetails.Location = new Point(x, 10);
            btnViewDetails.Font = new Font("IRANSans", 11);
            btnViewDetails.BackColor = Color.FromArgb(52, 152, 219);
            btnViewDetails.ForeColor = Color.White;
            btnViewDetails.FlatStyle = FlatStyle.Flat;
            btnViewDetails.FlatAppearance.BorderSize = 0;
            btnViewDetails.Cursor = Cursors.Hand;
            btnViewDetails.Click += BtnViewDetails_Click;
            buttonPanel.Controls.Add(btnViewDetails);

            x -= spacing;

            // دکمه چاپ
            btnPrintInvoice = new Button();
            btnPrintInvoice.Text = "🖨️ چاپ فاکتور";
            btnPrintInvoice.Size = new Size(150, 60);
            btnPrintInvoice.Location = new Point(x, 10);
            btnPrintInvoice.Font = new Font("IRANSans", 11);
            btnPrintInvoice.BackColor = Color.FromArgb(155, 89, 182);
            btnPrintInvoice.ForeColor = Color.White;
            btnPrintInvoice.FlatStyle = FlatStyle.Flat;
            btnPrintInvoice.FlatAppearance.BorderSize = 0;
            btnPrintInvoice.Cursor = Cursors.Hand;
            btnPrintInvoice.Click += BtnPrintInvoice_Click;
            buttonPanel.Controls.Add(btnPrintInvoice);

            x -= spacing;

            // دکمه خروجی Excel
            btnExportExcel = new Button();
            btnExportExcel.Text = "📊 خروجی Excel";
            btnExportExcel.Size = new Size(150, 60);
            btnExportExcel.Location = new Point(x, 10);
            btnExportExcel.Font = new Font("IRANSans", 11);
            btnExportExcel.BackColor = Color.FromArgb(46, 204, 113);
            btnExportExcel.ForeColor = Color.White;
            btnExportExcel.FlatStyle = FlatStyle.Flat;
            btnExportExcel.FlatAppearance.BorderSize = 0;
            btnExportExcel.Cursor = Cursors.Hand;
            btnExportExcel.Click += BtnExportExcel_Click;
            buttonPanel.Controls.Add(btnExportExcel);

            x -= spacing;

            // دکمه خروجی PDF
            btnExportPDF = new Button();
            btnExportPDF.Text = "📄 خروجی PDF";
            btnExportPDF.Size = new Size(150, 60);
            btnExportPDF.Location = new Point(x, 10);
            btnExportPDF.Font = new Font("IRANSans", 11);
            btnExportPDF.BackColor = Color.FromArgb(231, 76, 60);
            btnExportPDF.ForeColor = Color.White;
            btnExportPDF.FlatStyle = FlatStyle.Flat;
            btnExportPDF.FlatAppearance.BorderSize = 0;
            btnExportPDF.Cursor = Cursors.Hand;
            //btnExportPDF.Click += BtnExportPDF_Click;
            buttonPanel.Controls.Add(btnExportPDF);

            mainPanel.Controls.Add(buttonPanel);
        }

        private void CreateFilterField(string labelText, Control control, int x, int y, Panel parent, int width = 200)
        {
            Label lbl = new Label();
            lbl.Text = labelText;
            lbl.Font = new Font("IRANSans", 10);
            lbl.Size = new Size(80, 25);
            lbl.Location = new Point(x, y);
            lbl.TextAlign = ContentAlignment.MiddleRight;
            parent.Controls.Add(lbl);

            control.Size = new Size(width, 35);
            control.Location = new Point(x + 85, y - 5);
            control.Font = new Font("IRANSans", 10);

            if (control is ComboBox)
                ((ComboBox)control).DropDownStyle = ComboBoxStyle.DropDownList;

            parent.Controls.Add(control);
        }

        // ==================== متدهای منطقی ====================

        private string GetConnectionString()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["PetrochemicalSalesDB"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                return "Server=localhost;Database=PetrochemicalSalesDB;Integrated Security=true;TrustServerCertificate=true;";
            }
            return connectionString;
        }

        /*
        private void LoadInvoices()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    string query = @"SELECT * FROM Invoices";
                    string q = @"SELECT * FROM Invoices i
                                    LEFT JOIN Customers c ON i.CustomerID = c.CustomerID
                                    LEFT JOIN InvoiceItems t ON t.InvoiceID = i.InvoiceID";
                    
                            i.InvoiceID,
                            i.InvoiceNo,
                            i.InvoiceDate,
                            c.CompanyName AS CustomerName,
                            i.TotalAmount,
                            i.SubTotal,
                            i.DiscountAmount,
                            i.TaxAmount,
                            CASE 
                                WHEN i.Status = 1 THEN 'پرداخت شده'
                                WHEN i.Status = 0 THEN 'پرداخت نشده'
                                WHEN i.Status = 2 THEN 'بخشی پرداخت شده'
                                WHEN i.Status = 3 THEN 'لغو شده'
                                ELSE 'نامشخص'
                            END AS StatusText,
                            i.PaymentMethod,
                            i.PaymentDate,
                            i.Notes,
                            i.CreatedDate,
                            u.FullName AS CreatedBy
                        FROM Invoices i
                        LEFT JOIN Customers c ON i.CustomerID = c.CustomerID
                        LEFT JOIN Users u ON i.CreatedBy = u.UserID
                        ORDER BY i.InvoiceDate DESC, i.InvoiceID DESC";
                    
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        invoicesData = new DataTable();
                        invoicesData.Load(cmd.ExecuteReader());

                        // نمایش در DataGridView
                        DisplayInvoices(invoicesData);

                        // به‌روزرسانی آمار
                        UpdateStatistics(invoicesData);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بارگذاری فاکتورها: {ex.Message}", "خطا",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        */
        private void LoadInvoices()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                SELECT 
                    i.InvoiceID,
                    i.InvoiceDate,
                    c.CompanyName,
                    i.TotalAmount,
                    CASE 
                        WHEN i.Status = 1 THEN 'پرداخت شده'
                        WHEN i.Status = 0 THEN 'پرداخت نشده'
                        WHEN i.Status = 2 THEN 'بخشی پرداخت شده'
                        WHEN i.Status = 3 THEN 'لغو شده'
                        ELSE 'نامشخص'
                    END AS Status
                FROM Invoices i
                LEFT JOIN Customers c ON i.CustomerID = c.CustomerID
                ORDER BY i.InvoiceID DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        invoicesData = new DataTable();
                        invoicesData.Load(cmd.ExecuteReader());

                        // نمایش در DataGridView
                        DisplayInvoices(invoicesData);

                        // به‌روزرسانی آمار
                        UpdateStatistics(invoicesData);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بارگذاری فاکتورها: {ex.Message}", "خطا",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                // در صورت خطا، داده‌های نمونه نمایش ندهید
                // AddSampleData(); // این خط را کامنت یا حذف کنید
            }
        }
        private void DisplayInvoices(DataTable data)
        {
            dgvInvoices.DataSource = data;

            // تنظیم نام ستون‌های فارسی
            if (dgvInvoices.Columns.Count > 0)
            {
                string[] persianColumns = {
                    "شناسه", "شماره فاکتور", "تاریخ فاکتور", "نام مشتری", "کد مشتری",
                    "کد ملی", "مبلغ کل", "جمع کل", "تخفیف", "مالیات",
                    "وضعیت", "روش پرداخت", "تاریخ پرداخت", "یادداشت", "تاریخ ایجاد", "ایجاد کننده"
                };

                for (int i = 0; i < dgvInvoices.Columns.Count && i < persianColumns.Length; i++)
                {
                    dgvInvoices.Columns[i].HeaderText = persianColumns[i];
                }

                // فرمت‌دهی ستون‌های عددی
                string[] numericColumns = { "مبلغ کل", "جمع کل", "تخفیف", "مالیات" };
                foreach (DataGridViewColumn column in dgvInvoices.Columns)
                {
                    if (numericColumns.Contains(column.HeaderText))
                    {
                        column.DefaultCellStyle.Format = "N0";
                        column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                }

                // ستون تاریخ
                string[] dateColumns = { "تاریخ فاکتور", "تاریخ پرداخت", "تاریخ ایجاد" };
                foreach (DataGridViewColumn column in dgvInvoices.Columns)
                {
                    if (dateColumns.Contains(column.HeaderText))
                    {
                        column.DefaultCellStyle.Format = "yyyy/MM/dd";
                        column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }
                }

                // رنگ‌آمیزی وضعیت
                /*
                foreach (DataGridViewRow row in dgvInvoices.Rows)
                {
                    if (row.Cells["StatusText"].Value?.ToString() == "پرداخت شده")
                        row.DefaultCellStyle.BackColor = Color.FromArgb(220, 255, 220);
                    else if (row.Cells["StatusText"].Value?.ToString() == "پرداخت نشده")
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 220);
                    else if (row.Cells["StatusText"].Value?.ToString() == "بخشی پرداخت شده")
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 200);
                }
                */
            }
        }

        private void UpdateStatistics(DataTable data)
        {
            try
            {
                lblInvoiceCount.Text = data.Rows.Count.ToString("N0");

                decimal totalAmount = 0;
                foreach (DataRow row in data.Rows)
                {
                    if (row["TotalAmount"] != DBNull.Value)
                        totalAmount += Convert.ToDecimal(row["TotalAmount"]);
                }

                lblTotalAmount.Text = totalAmount.ToString("N0") + " تومان";

                // به‌روزرسانی میانگین
                decimal avgAmount = data.Rows.Count > 0 ? totalAmount / data.Rows.Count : 0;
                foreach (Control ctrl in mainPanel.Controls)
                {
                    if (ctrl is Panel panel)
                    {
                        foreach (Control child in panel.Controls)
                        {
                            if (child is Label lbl && lbl.Tag?.ToString() == "avgAmount")
                            {
                                lbl.Text = avgAmount.ToString("N0") + " تومان";
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // در صورت خطا، مقدار پیش‌فرض قرار بده
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            if (invoicesData == null) return;

            DataTable filteredData = invoicesData.Clone();

            foreach (DataRow row in invoicesData.Rows)
            {
                bool includeRow = true;

                // فیلتر تاریخ
                DateTime invoiceDate = Convert.ToDateTime(row["InvoiceDate"]);
                if (invoiceDate < dtpFromDate.Value.Date || invoiceDate > dtpToDate.Value.Date)
                    includeRow = false;

                // فیلتر مبلغ
                if (includeRow && !string.IsNullOrEmpty(txtMinAmount.Text))
                {
                    decimal amount = Convert.ToDecimal(row["TotalAmount"]);
                    decimal minAmount = Convert.ToDecimal(txtMinAmount.Text);
                    if (amount < minAmount)
                        includeRow = false;
                }

                if (includeRow && !string.IsNullOrEmpty(txtMaxAmount.Text))
                {
                    decimal amount = Convert.ToDecimal(row["TotalAmount"]);
                    decimal maxAmount = Convert.ToDecimal(txtMaxAmount.Text);
                    if (amount > maxAmount)
                        includeRow = false;
                }

                // فیلتر وضعیت
                if (includeRow && cmbStatusFilter.SelectedIndex > 0)
                {
                    string status = cmbStatusFilter.SelectedItem.ToString();
                    if (row["StatusText"].ToString() != status)
                        includeRow = false;
                }

                // فیلتر جستجو
                if (includeRow && !string.IsNullOrEmpty(txtSearch.Text))
                {
                    string searchText = txtSearch.Text.ToLower();
                    string filterType = cmbFilterBy.SelectedItem.ToString();

                    bool found = false;
                    switch (filterType)
                    {
                        case "همه":
                            foreach (DataColumn col in invoicesData.Columns)
                            {
                                if (row[col].ToString().ToLower().Contains(searchText))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            break;
                        case "شماره فاکتور":
                            found = row["InvoiceNo"].ToString().ToLower().Contains(searchText);
                            break;
                        case "نام مشتری":
                            found = row["CustomerName"].ToString().ToLower().Contains(searchText);
                            break;
                        case "کد مشتری":
                            found = row["CustomerCode"].ToString().ToLower().Contains(searchText);
                            break;
                        case "کد ملی":
                            found = row["NationalCode"].ToString().ToLower().Contains(searchText);
                            break;
                        case "توضیحات":
                            found = row["Notes"].ToString().ToLower().Contains(searchText);
                            break;
                    }

                    if (!found)
                        includeRow = false;
                }

                if (includeRow)
                    filteredData.ImportRow(row);
            }

            DisplayInvoices(filteredData);
            UpdateStatistics(filteredData);
        }

        private void BtnResetFilters_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            cmbFilterBy.SelectedIndex = 0;
            cmbStatusFilter.SelectedIndex = 0;
            dtpFromDate.Value = DateTime.Now.AddMonths(-1);
            dtpToDate.Value = DateTime.Now;
            txtMinAmount.Clear();
            txtMaxAmount.Clear();

            if (invoicesData != null)
            {
                DisplayInvoices(invoicesData);
                UpdateStatistics(invoicesData);
            }
        }

        /*
        private void BtnViewDetails_Click(object sender, EventArgs e)
        {
            if (dgvInvoices.SelectedRows.Count == 0)
            {
                MessageBox.Show("لطفاً یک فاکتور را انتخاب کنید", "اطلاع",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // روش درست 1: مستقیماً از SelectedRows[0] استفاده کنید
                int invoiceId = Convert.ToInt32(dgvInvoices.SelectedRows[0].Cells["InvoiceID"].Value);

                // روش درست 2: اگر حتماً می‌خواهید از Rows استفاده کنید
                // int rowIndex = dgvInvoices.SelectedRows[0].Index;
                // int invoiceId = Convert.ToInt32(dgvInvoices.Rows[rowIndex].Cells["InvoiceID"].Value);

                // باز کردن فرم جزئیات
                InvoiceDetailForm detailForm = new InvoiceDetailForm(invoiceId);
                detailForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در نمایش جزئیات: {ex.Message}", "خطا",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        */
        private void BtnViewDetails_Click(object sender, EventArgs e)
        {
            if (dgvInvoices.SelectedRows.Count == 0)
            {
                MessageBox.Show("لطفاً یک فاکتور را انتخاب کنید", "اطلاع",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // روش درست 1: مستقیماً از SelectedRows[0] استفاده کنید
                int invoiceIdNumber = Convert.ToInt32(dgvInvoices.SelectedRows[0].Cells["InvoiceID"].Value);
                MessageBox.Show(invoiceIdNumber.ToString());
                // روش درست 2: اگر حتماً می‌خواهید از Rows استفاده کنید
                // int rowIndex = dgvInvoices.SelectedRows[0].Index;
                // int invoiceId = Convert.ToInt32(dgvInvoices.Rows[rowIndex].Cells["InvoiceID"].Value);

                // باز کردن فرم جزئیات
                InvoiceDetailForm detailForm = new InvoiceDetailForm(invoiceIdNumber);
                detailForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در نمایش جزئیات: {ex.Message}", "خطا",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvInvoices_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int invoiceIdNumber = Convert.ToInt32(dgvInvoices.Rows[e.RowIndex].Cells["InvoiceID"].Value);
                ShowInvoiceDetails(invoiceIdNumber);
            }
        }

        /*
        private void ShowInvoiceDetails(int invoiceId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    string q = @"SELECT 
                            i.*,
                            c.CompanyName,
                            c.CustomerID,
                            c.NationalID,
                            c.Phone,
                            c.Address,
                        FROM Invoices i
                        LEFT JOIN Customers c ON i.CustomerID = c.CustomerID
                        LEFT JOIN InvoiceItems t ON t.InvoiceID = i.CustomerID
                        WHERE i.InvoiceID = @InvoiceID";


                    string query = @"SELECT *
                                    FROM 
                                    Invoices";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@InvoiceID", invoiceId);
                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                InvoiceDetailForm detailForm = new InvoiceDetailForm(Convert.ToInt32(reader));
                                detailForm.ShowDialog();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در نمایش جزئیات: {ex.Message}", "خطا",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        */
        private void ShowInvoiceDetails(int invoiceId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                SELECT 
                    i.*,
                    c.CompanyName,
                    c.CustomerID,
                    c.NationalID,
                    c.Phone,
                    c.Address
                FROM Invoices i
                LEFT JOIN Customers c ON i.CustomerID = c.CustomerID
                WHERE i.InvoiceID = @InvoiceID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@InvoiceID", invoiceId);
                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // باید مقادیر مورد نیاز را از reader بخوانید
                                // نه کل reader را تبدیل به int کنید!

                                // روش 1: ارسال invoiceId به فرم
                                InvoiceDetailForm detailForm = new InvoiceDetailForm(invoiceId);
                                detailForm.ShowDialog();

                                // یا روش 2: ارسال کل reader یا DataTable
                                // InvoiceDetailForm detailForm = new InvoiceDetailForm(reader);
                                // detailForm.ShowDialog();
                            }
                            else
                            {
                                MessageBox.Show("فاکتور مورد نظر یافت نشد.", "اطلاع",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در نمایش جزئیات: {ex.Message}", "خطا",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnPrintInvoice_Click(object sender, EventArgs e)
        {
            if (dgvInvoices.SelectedRows.Count == 0)
            {
                MessageBox.Show("لطفاً حداقل یک فاکتور را انتخاب کنید", "اطلاع",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // در اینجا می‌توانید کد چاپ فاکتور را اضافه کنید
            MessageBox.Show("امکان چاپ در نسخه بعدی اضافه خواهد شد", "اطلاع",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            if (dgvInvoices.Rows.Count == 0)
            {
                MessageBox.Show("هیچ داده‌ای برای ذخیره وجود ندارد", "اطلاع",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "فایل اکسل (*.xlsx)|*.xlsx";
                saveFileDialog.FileName = $"فاکتورها_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                saveFileDialog.Title = "ذخیره فایل اکسل";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var workbook = new XLWorkbook())
                        {
                            var worksheet = workbook.Worksheets.Add("فاکتورها");

                            // نوشتن عنوان
                            worksheet.Cell(1, 1).Value = "گزارش فاکتورها";
                            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
                            worksheet.Cell(1, 1).Style.Font.Bold = true;
                            worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            worksheet.Range(1, 1, 1, dgvInvoices.Columns.Count).Merge();

                            // نوشتن تاریخ تولید گزارش
                            worksheet.Cell(2, 1).Value = $"تاریخ تولید گزارش: {DateTime.Now:yyyy/MM/dd HH:mm}";
                            worksheet.Cell(2, 1).Style.Font.FontSize = 10;
                            worksheet.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            // نوشتن سرستون‌ها
                            for (int i = 0; i < dgvInvoices.Columns.Count; i++)
                            {
                                worksheet.Cell(4, i + 1).Value = dgvInvoices.Columns[i].HeaderText;
                                worksheet.Cell(4, i + 1).Style.Font.Bold = true;
                                worksheet.Cell(4, i + 1).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 102, 51);
                                worksheet.Cell(4, i + 1).Style.Font.FontColor = XLColor.White;
                                worksheet.Cell(4, i + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            }

                            // نوشتن داده‌ها
                            for (int i = 0; i < dgvInvoices.Rows.Count; i++)
                            {
                                for (int j = 0; j < dgvInvoices.Columns.Count; j++)
                                {
                                    object value = dgvInvoices.Rows[i].Cells[j].Value;
                                    worksheet.Cell(i + 5, j + 1).Value = value?.ToString() ?? "";

                                    // فرمت‌دهی اعداد
                                    if (dgvInvoices.Columns[j].DefaultCellStyle.Format == "N0")
                                    {
                                        worksheet.Cell(i + 5, j + 1).Style.NumberFormat.Format = "#,##0";
                                        worksheet.Cell(i + 5, j + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                    }
                                }
                            }

                            // تنظیم عرض ستون‌ها
                            worksheet.Columns().AdjustToContents();

                            // ذخیره فایل
                            workbook.SaveAs(saveFileDialog.FileName);

                            MessageBox.Show($"فایل اکسل با موفقیت ذخیره شد\n{Path.GetFileName(saveFileDialog.FileName)}",
                                "موفقیت", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"خطا در ذخیره فایل اکسل: {ex.Message}", "خطا",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /*
        private void BtnExportPDF_Click(object sender, EventArgs e)
        {
            if (dgvInvoices.Rows.Count == 0)
            {
                MessageBox.Show("هیچ داده‌ای برای ذخیره وجود ندارد", "اطلاع",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "فایل PDF (*.pdf)|*.pdf";
                saveFileDialog.FileName = $"فاکتورها_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                saveFileDialog.Title = "ذخیره فایل PDF";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Document document = new Document(PageSize.A4.Rotate(), 20, 20, 20, 20);
                        PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create));
                        document.Open();

                        // فونت فارسی
                        string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "tahoma.ttf");
                        BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                        Font fontNormal = new Font(baseFont, 12);
                        Font fontBold = new Font(baseFont, 14, Font.BOLD);
                        Font fontHeader = new Font(baseFont, 16, Font.BOLD);

                        // عنوان
                        Paragraph title = new Paragraph("گزارش فاکتورها", fontHeader);
                        title.Alignment = Element.ALIGN_CENTER;
                        document.Add(title);

                        // تاریخ گزارش
                        Paragraph date = new Paragraph($"تاریخ تولید گزارش: {DateTime.Now:yyyy/MM/dd HH:mm}", fontNormal);
                        date.Alignment = Element.ALIGN_LEFT;
                        document.Add(date);

                        document.Add(new Paragraph(" "));

                        // ایجاد جدول
                        PdfPTable table = new PdfPTable(dgvInvoices.Columns.Count);
                        table.WidthPercentage = 100;

                        // سرستون‌ها
                        foreach (DataGridViewColumn column in dgvInvoices.Columns)
                        {
                            PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText, fontBold));
                            cell.BackgroundColor = new BaseColor(0, 102, 51);
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Padding = 5;
                            table.AddCell(cell);
                        }

                        // داده‌ها
                        foreach (DataGridViewRow row in dgvInvoices.Rows)
                        {
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                string value = cell.Value?.ToString() ?? "";
                                PdfPCell pdfCell = new PdfPCell(new Phrase(value, fontNormal));
                                pdfCell.Padding = 3;
                                table.AddCell(pdfCell);
                            }
                        }

                        document.Add(table);

                        // آمار
                        document.Add(new Paragraph(" "));
                        Paragraph stats = new Paragraph(
                            $"تعداد فاکتورها: {lblInvoiceCount.Text} | مجموع مبالغ: {lblTotalAmount.Text}",
                            fontNormal);
                        stats.Alignment = Element.ALIGN_LEFT;
                        document.Add(stats);

                        document.Close();

                        MessageBox.Show($"فایل PDF با موفقیت ذخیره شد\n{Path.GetFileName(saveFileDialog.FileName)}",
                            "موفقیت", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"خطا در ذخیره فایل PDF: {ex.Message}", "خطا",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        */
    }
}