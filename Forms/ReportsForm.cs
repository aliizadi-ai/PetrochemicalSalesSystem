using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PetrochemicalSalesSystem.Forms
{
    public partial class ReportsForm : Form
    {
        private Chart salesChart;
        private DataGridView dgvStats;

        public ReportsForm()
        {
            InitializeComponent();
            InitializeReportsForm();
            LoadSalesReport();
            LoadFinancialStats();
        }

        private void InitializeReportsForm()
        {
            this.Size = new Size(1200, 700);
            this.BackColor = Color.White;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;

            // هدر
            Panel headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 60;
            headerPanel.BackColor = Color.FromArgb(0, 102, 51);

            Label title = new Label();
            title.Text = "📊 گزارش‌گیری و آمار پیشرفته";
            title.Font = new Font("B Nazanin", 16, FontStyle.Bold);
            title.ForeColor = Color.White;
            title.Dock = DockStyle.Fill;
            title.TextAlign = ContentAlignment.MiddleCenter;
            headerPanel.Controls.Add(title);
            this.Controls.Add(headerPanel);

            // پنل فیلترها
            CreateFilterPanel();

            // تب‌های مختلف
            CreateTabControl();
        }

        private void CreateFilterPanel()
        {
            Panel filterPanel = new Panel();
            filterPanel.Dock = DockStyle.Top;
            filterPanel.Height = 80;
            filterPanel.BackColor = Color.FromArgb(245, 245, 245);
            filterPanel.BorderStyle = BorderStyle.FixedSingle;

            // فیلتر تاریخ
            Label lblDateRange = new Label();
            lblDateRange.Text = "بازه زمانی:";
            lblDateRange.Font = new Font("B Nazanin", 10);
            lblDateRange.Size = new Size(80, 25);
            lblDateRange.Location = new Point(20, 25);

            ComboBox cmbPeriod = new ComboBox();
            cmbPeriod.Size = new Size(150, 25);
            cmbPeriod.Location = new Point(105, 25);
            cmbPeriod.Font = new Font("B Nazanin", 10);
            cmbPeriod.Items.AddRange(new string[] {
                "امروز", "دیروز", "هفته جاری", "ماه جاری",
                "سه ماهه اخیر", "شش ماهه اخیر", "سال جاری", "سفارشی"
            });
            cmbPeriod.SelectedIndex = 3;
            cmbPeriod.SelectedIndexChanged += CmbPeriod_SelectedIndexChanged;

            DateTimePicker dtpFrom = new DateTimePicker();
            dtpFrom.Size = new Size(120, 25);
            dtpFrom.Location = new Point(270, 25);
            dtpFrom.Font = new Font("B Nazanin", 9);
            dtpFrom.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            Label lblTo = new Label();
            lblTo.Text = "تا";
            lblTo.Font = new Font("B Nazanin", 10);
            lblTo.Size = new Size(30, 25);
            lblTo.Location = new Point(395, 25);
            lblTo.TextAlign = ContentAlignment.MiddleCenter;

            DateTimePicker dtpTo = new DateTimePicker();
            dtpTo.Size = new Size(120, 25);
            dtpTo.Location = new Point(430, 25);
            dtpTo.Font = new Font("B Nazanin", 9);
            dtpTo.Value = DateTime.Now;

            // دکمه‌ها
            Button btnGenerate = new Button();
            btnGenerate.Text = "🔍 تولید گزارش";
            btnGenerate.Size = new Size(120, 35);
            btnGenerate.Location = new Point(570, 20);
            btnGenerate.Font = new Font("B Nazanin", 10, FontStyle.Bold);
            btnGenerate.BackColor = Color.FromArgb(52, 152, 219);
            btnGenerate.ForeColor = Color.White;
            btnGenerate.Click += BtnGenerate_Click;

            Button btnExport = new Button();
            btnExport.Text = "📥 خروجی گزارش";
            btnExport.Size = new Size(120, 35);
            btnExport.Location = new Point(700, 20);
            btnExport.Font = new Font("B Nazanin", 10);
            btnExport.BackColor = Color.FromArgb(46, 204, 113);
            btnExport.ForeColor = Color.White;
            btnExport.Click += BtnExportReport_Click;

            filterPanel.Controls.AddRange(new Control[] {
                lblDateRange, cmbPeriod, dtpFrom, lblTo, dtpTo, btnGenerate, btnExport
            });
            this.Controls.Add(filterPanel);
        }

        private void CreateTabControl()
        {
            TabControl tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Location = new Point(0, 140);
            tabControl.Size = new Size(1200, 560);
            tabControl.Font = new Font("B Nazanin", 10);

            // تب آمار فروش
            TabPage tabSales = new TabPage("📈 آمار فروش");
            CreateSalesTab(tabSales);

            // تب آمار مالی
            TabPage tabFinancial = new TabPage("💰 آمار مالی");
            CreateFinancialTab(tabFinancial);

            // تب گزارش مشتریان
            TabPage tabCustomers = new TabPage("👥 گزارش مشتریان");
            //CreateCustomersTab(tabCustomers);

            // تب گزارش محصولات
            TabPage tabProducts = new TabPage("🏢 گزارش محصولات");
            //CreateProductsTab(tabProducts);

            tabControl.TabPages.Add(tabSales);
            tabControl.TabPages.Add(tabFinancial);
            tabControl.TabPages.Add(tabCustomers);
            tabControl.TabPages.Add(tabProducts);

            this.Controls.Add(tabControl);
        }

        private void CreateSalesTab(TabPage tab)
        {
            tab.BackColor = Color.White;

            // نمودار فروش
            salesChart = new Chart();
            salesChart.Size = new Size(800, 350);
            salesChart.Location = new Point(20, 20);

            // تنظیمات نمودار
            ChartArea chartArea = new ChartArea();
            chartArea.AxisX.LabelStyle.Font = new Font("B Nazanin", 9);
            chartArea.AxisY.LabelStyle.Font = new Font("B Nazanin", 9);
            chartArea.AxisX.LabelStyle.Format = "MM/dd";
            chartArea.AxisY.LabelStyle.Format = "#,##0";

            salesChart.ChartAreas.Add(chartArea);

            Series series = new Series("فروش روزانه");
            series.ChartType = SeriesChartType.Column;
            series.Color = Color.FromArgb(52, 152, 219);
            salesChart.Series.Add(series);

            // آمار خلاصه
            Panel summaryPanel = new Panel();
            summaryPanel.Size = new Size(800, 120);
            summaryPanel.Location = new Point(20, 390);
            summaryPanel.BackColor = Color.FromArgb(250, 250, 250);
            summaryPanel.BorderStyle = BorderStyle.FixedSingle;

            CreateSummaryCards(summaryPanel);

            tab.Controls.Add(salesChart);
            tab.Controls.Add(summaryPanel);
        }

        private void CreateFinancialTab(TabPage tab)
        {
            // DataGridView برای آمار مالی
            dgvStats = new DataGridView();
            dgvStats.Size = new Size(1000, 400);
            dgvStats.Location = new Point(20, 20);
            dgvStats.BackgroundColor = Color.White;
            dgvStats.RowHeadersVisible = false;
            dgvStats.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // ستون‌ها
            dgvStats.Columns.Add("Month", "ماه");
            dgvStats.Columns.Add("TotalInvoices", "تعداد فاکتور");
            dgvStats.Columns.Add("TotalSales", "مجموع فروش");
            dgvStats.Columns.Add("AverageInvoice", "میانگین فاکتور");
            dgvStats.Columns.Add("Growth", "رشد (%)");

            tab.Controls.Add(dgvStats);
        }

        private void CreateSummaryCards(Panel panel)
        {
            int x = 20;
            int cardWidth = 180;

            string[] titles = { "فروش کل", "میانگین فاکتور", "پرفروش‌ترین روز", "بیشترین مبلغ" };
            string[] values = { "۰ تومان", "۰ تومان", "-", "۰ تومان" };
            Color[] colors = {
                Color.FromArgb(52, 152, 219),
                Color.FromArgb(46, 204, 113),
                Color.FromArgb(155, 89, 182),
                Color.FromArgb(241, 196, 15)
            };

            for (int i = 0; i < 4; i++)
            {
                Panel card = new Panel();
                card.Size = new Size(cardWidth, 90);
                card.Location = new Point(x, 15);
                card.BackColor = colors[i];

                Label lblTitle = new Label();
                lblTitle.Text = titles[i];
                lblTitle.Font = new Font("B Nazanin", 10, FontStyle.Bold);
                lblTitle.ForeColor = Color.White;
                lblTitle.Location = new Point(10, 10);
                lblTitle.AutoSize = true;

                Label lblValue = new Label();
                lblValue.Text = values[i];
                lblValue.Font = new Font("B Nazanin", 14, FontStyle.Bold);
                lblValue.ForeColor = Color.White;
                lblValue.Location = new Point(10, 40);
                lblValue.AutoSize = true;

                card.Controls.Add(lblTitle);
                card.Controls.Add(lblValue);
                panel.Controls.Add(card);

                x += cardWidth + 20;
            }
        }

        private void LoadSalesReport()
        {
            try
            {
                // دریافت داده‌های فروش از دیتابیس
                string query = @"
                    SELECT 
                        CONVERT(VARCHAR(10), InvoiceDate, 120) as SaleDate,
                        COUNT(*) as InvoiceCount,
                        SUM(TotalAmount) as DailySales
                    FROM Invoices
                    WHERE InvoiceDate >= DATEADD(DAY, -30, GETDATE())
                    GROUP BY CONVERT(VARCHAR(10), InvoiceDate, 120)
                    ORDER BY SaleDate";

                DataTable dt = Data.DatabaseHelper.ExecuteQuery(query);

                // آپدیت نمودار
                salesChart.Series[0].Points.Clear();

                foreach (DataRow row in dt.Rows)
                {
                    DateTime date = DateTime.Parse(row["SaleDate"].ToString());
                    decimal sales = Convert.ToDecimal(row["DailySales"]);

                    salesChart.Series[0].Points.AddXY(date.ToString("MM/dd"), sales);
                }

                // آپدیت کارت‌های خلاصه
                UpdateSummaryCards(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بارگذاری گزارش: {ex.Message}", "خطا");
            }
        }

        private void LoadFinancialStats()
        {
            try
            {
                string query = @"
                    SELECT 
                        FORMAT(InvoiceDate, 'yyyy-MM') as Month,
                        COUNT(*) as TotalInvoices,
                        SUM(TotalAmount) as TotalSales,
                        AVG(TotalAmount) as AverageInvoice,
                        LAG(SUM(TotalAmount)) OVER (ORDER BY FORMAT(InvoiceDate, 'yyyy-MM')) as PreviousSales
                    FROM Invoices
                    WHERE InvoiceDate >= DATEADD(MONTH, -6, GETDATE())
                    GROUP BY FORMAT(InvoiceDate, 'yyyy-MM')
                    ORDER BY Month DESC";

                DataTable dt = Data.DatabaseHelper.ExecuteQuery(query);

                dgvStats.Rows.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    decimal currentSales = Convert.ToDecimal(row["TotalSales"]);
                    decimal previousSales = row["PreviousSales"] != DBNull.Value ?
                        Convert.ToDecimal(row["PreviousSales"]) : 0;

                    decimal growth = previousSales > 0 ?
                        ((currentSales - previousSales) / previousSales) * 100 : 0;

                    dgvStats.Rows.Add(
                        row["Month"],
                        row["TotalInvoices"],
                        string.Format("{0:N0} تومان", row["TotalSales"]),
                        string.Format("{0:N0} تومان", row["AverageInvoice"]),
                        string.Format("{0:F1}%", growth)
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بارگذاری آمار مالی: {ex.Message}", "خطا");
            }
        }

        private void UpdateSummaryCards(DataTable dt)
        {
            // محاسبات برای کارت‌های خلاصه
            // ...
        }

        private void CmbPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            // تغییر تاریخ‌ها بر اساس انتخاب کاربر
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            LoadSalesReport();
            LoadFinancialStats();
        }

        private void BtnExportReport_Click(object sender, EventArgs e)
        {
            // کد خروجی گزارش
        }

        // بقیه متدها...
    }
}