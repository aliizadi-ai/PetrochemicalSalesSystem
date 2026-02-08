using System;
using System.Drawing;
using System.Windows.Forms;
using PetrochemicalSalesSystem.Utilities;

namespace PetrochemicalSalesSystem.Forms
{
    public partial class AccountantForm : Form
    {
        private Panel sidebarPanel;
        private Panel contentPanel;
        private Button currentButton;

        // رنگ‌های تم
        private Color primaryColor = Color.FromArgb(0, 102, 51);
        private Color activeButtonColor = Color.FromArgb(0, 80, 40);

        public AccountantForm()
        {
            InitializeComponent();
            InitializeAccountantForm();
            LoadDashboard();
        }

        private void InitializeAccountantForm()
        {
            this.Text = $"سیستم فروش پتروشیمی - کاربر: حسابدار";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.WindowState = FormWindowState.Maximized;

            CreateSidebar();
            CreateContentPanel();
            CreateHeader();
        }

        private void CreateHeader()
        {
            Panel headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 60;
            headerPanel.BackColor = primaryColor;

            // عنوان سیستم
            Label lblTitle = new Label();
            lblTitle.Text = $"🏭 سیستم مدیریت حسابداری پتروشیمی";
            lblTitle.Font = new Font("IRANSans", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Dock = DockStyle.Left;
            lblTitle.Padding = new Padding(20, 0, 0, 0);
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;

            // اطلاعات کاربر
            Label lblUserInfo = new Label();
            lblUserInfo.Text = $"👤 حسابدار سیستم";
            lblUserInfo.Font = new Font("IRANSans", 12);
            lblUserInfo.ForeColor = Color.White;
            lblUserInfo.Dock = DockStyle.Right;
            lblUserInfo.Padding = new Padding(0, 0, 20, 0);
            lblUserInfo.TextAlign = ContentAlignment.MiddleRight;

            // دکمه خروج
            Button btnLogout = new Button();
            btnLogout.Text = "🚪 خروج";
            btnLogout.Font = new Font("IRANSans", 10, FontStyle.Bold);
            btnLogout.Size = new Size(100, 35);
            btnLogout.Location = new Point(this.Width - 120, 12);
            btnLogout.BackColor = Color.IndianRed;
            btnLogout.ForeColor = Color.White;
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Cursor = Cursors.Hand;
            btnLogout.Click += (s, e) =>
            {
                if (MessageBox.Show("آیا می‌خواهید از سیستم خارج شوید؟", "تأیید خروج",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // بستن فرم فعلی
                    this.Hide(); // یا this.Close()

                    // باز کردن فرم لاگین
                    LoginForm loginForm = new LoginForm();
                    loginForm.Show();

                    // یا اگر می‌خواهید فرم لاگین را به صورت مودال باز کنید:
                    // loginForm.ShowDialog();
                }
            };

            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(lblUserInfo);
            headerPanel.Controls.Add(btnLogout);
            this.Controls.Add(headerPanel);
        }

        private void CreateSidebar()
        {
            sidebarPanel = new Panel();
            sidebarPanel.Dock = DockStyle.Left;
            sidebarPanel.Width = 220;
            sidebarPanel.BackColor = Color.FromArgb(245, 245, 245);
            sidebarPanel.BorderStyle = BorderStyle.FixedSingle;

            // عنوان منو
            Label menuTitle = new Label();
            menuTitle.Text = "منوها";
            menuTitle.Font = new Font("IRANSans", 12, FontStyle.Bold);
            menuTitle.ForeColor = Color.FromArgb(0, 102, 51);
            menuTitle.Size = new Size(200, 40);
            menuTitle.Location = new Point(10, 10);
            menuTitle.TextAlign = ContentAlignment.MiddleCenter;
            sidebarPanel.Controls.Add(menuTitle);

            // لیست منوها
            string[] menuItems = {
                "📊 داشبورد",
                "🧾 ثبت فاکتور جدید",
                "📋 مشاهده فاکتورها",
                "📋 مشاهده پیشرفته فاکتورها",
                "🔍 جستجوی فاکتور",
                "📈 گزارش‌های فروش",
                "📊 سیستم گزارش‌گیری پیشرفته"
                /*
                "💰 آمارهای مالی",
                "👥 مدیریت مشتریان",
                "🏢 مدیریت محصولات"
                */
            };

            int buttonY = 60;
            foreach (var menuText in menuItems)
            {
                Button menuButton = new Button();
                menuButton.Text = menuText;
                menuButton.Font = new Font("IRANSans", 10);
                menuButton.Size = new Size(200, 45);
                menuButton.Location = new Point(10, buttonY);
                menuButton.BackColor = Color.Transparent;
                menuButton.ForeColor = Color.Black;
                menuButton.FlatStyle = FlatStyle.Flat;
                menuButton.FlatAppearance.BorderSize = 0;
                menuButton.TextAlign = ContentAlignment.MiddleLeft;
                menuButton.Padding = new Padding(15, 0, 0, 0);
                menuButton.Cursor = Cursors.Hand;
                menuButton.Tag = menuText;

                // رویدادها
                menuButton.Click += MenuButton_Click;
                menuButton.MouseEnter += (s, e) =>
                {
                    if (currentButton != menuButton)
                    {
                        menuButton.BackColor = Color.FromArgb(230, 230, 230);
                    }
                };
                menuButton.MouseLeave += (s, e) =>
                {
                    if (currentButton != menuButton)
                    {
                        menuButton.BackColor = Color.Transparent;
                    }
                };

                sidebarPanel.Controls.Add(menuButton);
                buttonY += 50;
            }

            this.Controls.Add(sidebarPanel);
        }

        private void CreateContentPanel()
        {
            contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.BackColor = Color.FromArgb(240, 242, 245);
            contentPanel.Padding = new Padding(20);

            this.Controls.Add(contentPanel);
            this.Controls.SetChildIndex(contentPanel, 0);
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            string menuText = clickedButton.Tag.ToString();

            // تغییر رنگ دکمه فعال
            if (currentButton != null)
            {
                currentButton.BackColor = Color.Transparent;
                currentButton.ForeColor = Color.Black;
            }

            clickedButton.BackColor = activeButtonColor;
            clickedButton.ForeColor = Color.White;
            currentButton = clickedButton;

            // بارگذاری محتوای مربوطه
            switch (menuText)
            {
                case "📊 داشبورد":
                    LoadDashboard();
                    break;
                case "🧾 ثبت فاکتور جدید":
                    LoadInvoiceForm();
                    break;
                case "📋 مشاهده فاکتورها":
                    LoadInvoicesList();
                    break;
                case "📋 مشاهده پیشرفته فاکتورها":
                    LoadInvoiceViewDetailList(); 
                    break;
                case "🔍 جستجوی فاکتور":
                    LoadInvoiceSearch();
                    break;
                case "📈 گزارش‌های فروش":
                    LoadSalesReports();
                    break;
                case "📊 سیستم گزارش‌گیری پیشرفته":
                    LoadReportForm();
                    break;
                    /*
                    case "💰 آمارهای مالی":
                        LoadFinancialStats();
                        break;
                    case "👥 مدیریت مشتریان":
                        LoadCustomerManagement();
                        break;
                    case "🏢 مدیریت محصولات":
                        LoadProductManagement();
                        break;
                    */
            }
        }

        private void LoadDashboard()
        {
            contentPanel.Controls.Clear();

            // عنوان
            Label title = new Label();
            title.Text = "📊 داشبورد مدیریتی";
            title.Font = new Font("IRANSans", 18, FontStyle.Bold);
            title.ForeColor = Color.FromArgb(0, 102, 51);
            title.Size = new Size(400, 50);
            title.Location = new Point(20, 20);
            contentPanel.Controls.Add(title);

            // کارت‌های آمار
            Panel statsPanel = new Panel();
            statsPanel.Size = new Size(1100, 120);
            statsPanel.Location = new Point(20, 80);
            statsPanel.BackColor = Color.Transparent;

            // ایجاد 4 کارت آمار
            CreateStatCard("فاکتورهای امروز", "15", "عدد", 0, statsPanel, Color.FromArgb(52, 152, 219));
            CreateStatCard("فروش امروز", "۲۵,۴۰۰,۰۰۰", "تومان", 280, statsPanel, Color.FromArgb(46, 204, 113));
            CreateStatCard("مشتریان فعال", "۸۷", "نفر", 560, statsPanel, Color.FromArgb(155, 89, 182));
            CreateStatCard("درآمد ماه", "۴۵۰,۰۰۰,۰۰۰", "تومان", 840, statsPanel, Color.FromArgb(241, 196, 15));

            contentPanel.Controls.Add(statsPanel);

            // نمودار فروش
            CreateSalesChart();
        }

        private void CreateStatCard(string title, string value, string unit, int x, Panel parent, Color color)
        {
            Panel card = new Panel();
            card.Size = new Size(260, 100);
            card.Location = new Point(x, 0);
            card.BackColor = Color.White;
            card.BorderStyle = BorderStyle.FixedSingle;

            // عنوان کارت
            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = new Font("IRANSans", 11, FontStyle.Bold);
            lblTitle.ForeColor = Color.DarkGray;
            lblTitle.Size = new Size(240, 25);
            lblTitle.Location = new Point(10, 10);
            lblTitle.TextAlign = ContentAlignment.MiddleRight;
            card.Controls.Add(lblTitle);

            // مقدار
            Label lblValue = new Label();
            lblValue.Text = value;
            lblValue.Font = new Font("IRANSans", 18, FontStyle.Bold);
            lblValue.ForeColor = color;
            lblValue.Size = new Size(240, 40);
            lblValue.Location = new Point(10, 35);
            lblValue.TextAlign = ContentAlignment.MiddleRight;
            card.Controls.Add(lblValue);

            // واحد
            Label lblUnit = new Label();
            lblUnit.Text = unit;
            lblUnit.Font = new Font("IRANSans", 10);
            lblUnit.ForeColor = Color.Gray;
            lblUnit.Size = new Size(240, 20);
            lblUnit.Location = new Point(10, 75);
            lblUnit.TextAlign = ContentAlignment.MiddleRight;
            card.Controls.Add(lblUnit);

            // خط رنگی پایین
            Panel colorLine = new Panel();
            colorLine.Size = new Size(260, 5);
            colorLine.Location = new Point(0, 95);
            colorLine.BackColor = color;
            card.Controls.Add(colorLine);

            parent.Controls.Add(card);
        }
        private void LoadInvoiceViewDetailList()
        {
            try
            {
                InvoiceViewForm invoiceViewForm = new InvoiceViewForm();
                invoiceViewForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در باز کردن فرم گزارش‌گیری: {ex.Message}", "خطا",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadReportForm()
        {
            try
            {
                ReportForm reportForm = new ReportForm();
                reportForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در باز کردن فرم گزارش‌گیری: {ex.Message}", "خطا",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CreateSalesChart()
        {
            Panel chartPanel = new Panel();
            chartPanel.Size = new Size(1100, 300);
            chartPanel.Location = new Point(20, 220);
            chartPanel.BackColor = Color.White;
            chartPanel.BorderStyle = BorderStyle.FixedSingle;

            // عنوان نمودار
            Label chartTitle = new Label();
            chartTitle.Text = "📈 آمار فروش ۷ روز اخیر";
            chartTitle.Font = new Font("IRANSans", 14, FontStyle.Bold);
            chartTitle.ForeColor = Color.FromArgb(0, 102, 51);
            chartTitle.Size = new Size(300, 40);
            chartTitle.Location = new Point(20, 15);
            chartTitle.TextAlign = ContentAlignment.MiddleRight;
            chartPanel.Controls.Add(chartTitle);

            // نمایش گراف ساده
            Panel graphContainer = new Panel();
            graphContainer.Size = new Size(1050, 200);
            graphContainer.Location = new Point(25, 70);
            graphContainer.BackColor = Color.FromArgb(250, 250, 250);
            graphContainer.BorderStyle = BorderStyle.FixedSingle;

            // خطوط گراف (نمایشی)
            int[] salesData = { 12000000, 15000000, 18000000, 14000000, 22000000, 19000000, 25000000 };
            int maxValue = 30000000;
            int width = 1000;
            int height = 180;

            for (int i = 0; i < salesData.Length; i++)
            {
                // نقطه روی نمودار
                int xPos = (i * (width / (salesData.Length - 1))) + 25;
                int yPos = height - (int)((salesData[i] / (double)maxValue) * height) + 10;

                // نقطه
                Panel point = new Panel();
                point.Size = new Size(10, 10);
                point.Location = new Point(xPos - 5, yPos - 5);
                point.BackColor = Color.FromArgb(46, 204, 113);
                graphContainer.Controls.Add(point);

                // مقدار بالای نقطه
                Label valueLabel = new Label();
                valueLabel.Text = (salesData[i] / 1000000).ToString() + "M";
                valueLabel.Font = new Font("IRANSans", 8);
                valueLabel.Size = new Size(50, 20);
                valueLabel.Location = new Point(xPos - 25, yPos - 25);
                valueLabel.TextAlign = ContentAlignment.MiddleCenter;
                graphContainer.Controls.Add(valueLabel);
            }

            chartPanel.Controls.Add(graphContainer);
            contentPanel.Controls.Add(chartPanel);
        }

        private void LoadInvoiceForm()
        {
            contentPanel.Controls.Clear();

            // بارگذاری فرم ثبت فاکتور
            try
            {
                InvoiceForm invoiceForm = new InvoiceForm();
                invoiceForm.TopLevel = false;
                invoiceForm.FormBorderStyle = FormBorderStyle.None;
                invoiceForm.Dock = DockStyle.Fill;
                contentPanel.Controls.Add(invoiceForm);
                invoiceForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بارگذاری فرم فاکتور: {ex.Message}", "خطا",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadInvoicesList()
        {
            contentPanel.Controls.Clear();

            // ایجاد فرم ساده برای نمایش لیست فاکتورها
            Panel listPanel = new Panel();
            listPanel.Dock = DockStyle.Fill;
            listPanel.BackColor = Color.White;

            // عنوان
            Label title = new Label();
            title.Text = "📋 لیست فاکتورها";
            title.Font = new Font("IRANSans", 16, FontStyle.Bold);
            title.ForeColor = Color.FromArgb(0, 102, 51);
            title.Size = new Size(300, 40);
            title.Location = new Point(20, 20);
            listPanel.Controls.Add(title);

            // دکمه‌های فیلتر
            Panel filterPanel = new Panel();
            filterPanel.Size = new Size(1100, 50);
            filterPanel.Location = new Point(20, 70);
            filterPanel.BackColor = Color.FromArgb(245, 245, 245);
            filterPanel.BorderStyle = BorderStyle.FixedSingle;

            // کنترل‌های فیلتر
            Label lblFromDate = new Label();
            lblFromDate.Text = "از تاریخ:";
            lblFromDate.Size = new Size(60, 30);
            lblFromDate.Location = new Point(20, 10);
            lblFromDate.TextAlign = ContentAlignment.MiddleRight;
            filterPanel.Controls.Add(lblFromDate);

            DateTimePicker dtpFrom = new DateTimePicker();
            dtpFrom.Size = new Size(120, 30);
            dtpFrom.Location = new Point(90, 10);
            dtpFrom.Value = DateTime.Now.AddDays(-30);
            filterPanel.Controls.Add(dtpFrom);

            Label lblToDate = new Label();
            lblToDate.Text = "تا تاریخ:";
            lblToDate.Size = new Size(60, 30);
            lblToDate.Location = new Point(230, 10);
            lblToDate.TextAlign = ContentAlignment.MiddleRight;
            filterPanel.Controls.Add(lblToDate);

            DateTimePicker dtpTo = new DateTimePicker();
            dtpTo.Size = new Size(120, 30);
            dtpTo.Location = new Point(300, 10);
            dtpTo.Value = DateTime.Now;
            filterPanel.Controls.Add(dtpTo);

            Button btnFilter = new Button();
            btnFilter.Text = "🔍 اعمال فیلتر";
            btnFilter.Size = new Size(120, 30);
            btnFilter.Location = new Point(440, 10);
            btnFilter.BackColor = Color.FromArgb(52, 152, 219);
            btnFilter.ForeColor = Color.White;
            btnFilter.FlatStyle = FlatStyle.Flat;
            filterPanel.Controls.Add(btnFilter);

            listPanel.Controls.Add(filterPanel);

            // DataGridView برای نمایش فاکتورها
            DataGridView dgvInvoices = new DataGridView();
            dgvInvoices.Size = new Size(1100, 400);
            dgvInvoices.Location = new Point(20, 140);
            dgvInvoices.BackgroundColor = Color.White;
            dgvInvoices.RowHeadersVisible = false;
            dgvInvoices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // ستون‌ها
            dgvInvoices.Columns.Add("InvoiceNo", "شماره فاکتور");
            dgvInvoices.Columns.Add("Date", "تاریخ");
            dgvInvoices.Columns.Add("Customer", "مشتری");
            dgvInvoices.Columns.Add("Amount", "مبلغ");
            dgvInvoices.Columns.Add("Status", "وضعیت");
            dgvInvoices.Columns.Add("Payment", "روش پرداخت");

            // استایل‌دهی
            dgvInvoices.Columns["Amount"].DefaultCellStyle.Format = "N0";
            dgvInvoices.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // داده‌های نمونه
            dgvInvoices.Rows.Add("INV-2024-001", "1402/12/15", "شرکت پتروشیمی الف", "۱۲,۵۰۰,۰۰۰", "پرداخت شده", "نقدی");
            dgvInvoices.Rows.Add("INV-2024-002", "1402/12/16", "شرکت صنعتی ب", "۸,۷۰۰,۰۰۰", "در انتظار", "چک");
            dgvInvoices.Rows.Add("INV-2024-003", "1402/12/17", "کارخانه ج", "۱۵,۲۰۰,۰۰۰", "پرداخت شده", "کارت بانکی");

            listPanel.Controls.Add(dgvInvoices);

            contentPanel.Controls.Add(listPanel);
        }

        private void LoadInvoiceSearch()
        {
            contentPanel.Controls.Clear();

            Panel searchPanel = new Panel();
            searchPanel.Dock = DockStyle.Fill;
            searchPanel.BackColor = Color.White;

            Label title = new Label();
            title.Text = "🔍 جستجوی فاکتور";
            title.Font = new Font("IRANSans", 16, FontStyle.Bold);
            title.ForeColor = Color.FromArgb(0, 102, 51);
            title.Size = new Size(300, 40);
            title.Location = new Point(20, 20);
            searchPanel.Controls.Add(title);

            // فیلد جستجو
            TextBox txtSearch = new TextBox();
            txtSearch.Size = new Size(500, 35);
            txtSearch.Location = new Point(20, 80);
            txtSearch.Font = new Font("IRANSans", 12);
            txtSearch.Text = "شماره فاکتور، نام مشتری، یا کد ملی...";
            searchPanel.Controls.Add(txtSearch);

            Button btnSearch = new Button();
            btnSearch.Text = "🔍 جستجو";
            btnSearch.Size = new Size(120, 35);
            btnSearch.Location = new Point(540, 80);
            btnSearch.BackColor = Color.FromArgb(52, 152, 219);
            btnSearch.ForeColor = Color.White;
            btnSearch.FlatStyle = FlatStyle.Flat;
            searchPanel.Controls.Add(btnSearch);

            contentPanel.Controls.Add(searchPanel);
        }

        private void LoadSalesReports()
        {
            contentPanel.Controls.Clear();

            Panel reportsPanel = new Panel();
            reportsPanel.Dock = DockStyle.Fill;
            reportsPanel.BackColor = Color.White;

            Label title = new Label();
            title.Text = "📈 گزارش‌های فروش";
            title.Font = new Font("IRANSans", 16, FontStyle.Bold);
            title.ForeColor = Color.FromArgb(0, 102, 51);
            title.Size = new Size(300, 40);
            title.Location = new Point(20, 20);
            reportsPanel.Controls.Add(title);

            // گزینه‌های گزارش
            string[] reportTypes = {
                "گزارش فروش روزانه",
                "گزارش فروش ماهانه",
                "گزارش فروش سالانه",
                "گزارش فروش بر اساس محصول",
                "گزارش فروش بر اساس مشتری",
                "گزارش فاکتورهای پرداخت نشده"
            };

            int yPos = 80;
            foreach (var report in reportTypes)
            {
                Button btnReport = new Button();
                btnReport.Text = $"📄 {report}";
                btnReport.Size = new Size(300, 45);
                btnReport.Location = new Point(20, yPos);
                btnReport.Font = new Font("IRANSans", 11);
                btnReport.BackColor = Color.FromArgb(240, 240, 240);
                btnReport.ForeColor = Color.Black;
                btnReport.FlatStyle = FlatStyle.Flat;
                btnReport.TextAlign = ContentAlignment.MiddleLeft;
                btnReport.Padding = new Padding(15, 0, 0, 0);
                reportsPanel.Controls.Add(btnReport);

                yPos += 55;
            }

            contentPanel.Controls.Add(reportsPanel);
        }

        private void LoadFinancialStats()
        {
            contentPanel.Controls.Clear();

            Panel statsPanel = new Panel();
            statsPanel.Dock = DockStyle.Fill;
            statsPanel.BackColor = Color.White;

            Label title = new Label();
            title.Text = "💰 آمارهای مالی";
            title.Font = new Font("IRANSans", 16, FontStyle.Bold);
            title.ForeColor = Color.FromArgb(0, 102, 51);
            title.Size = new Size(300, 40);
            title.Location = new Point(20, 20);
            statsPanel.Controls.Add(title);

            contentPanel.Controls.Add(statsPanel);
        }

        private void LoadCustomerManagement()
        {
            contentPanel.Controls.Clear();

            Panel customerPanel = new Panel();
            customerPanel.Dock = DockStyle.Fill;
            customerPanel.BackColor = Color.White;

            Label title = new Label();
            title.Text = "👥 مدیریت مشتریان";
            title.Font = new Font("IRANSans", 16, FontStyle.Bold);
            title.ForeColor = Color.FromArgb(0, 102, 51);
            title.Size = new Size(300, 40);
            title.Location = new Point(20, 20);
            customerPanel.Controls.Add(title);

            contentPanel.Controls.Add(customerPanel);
        }

        private void LoadProductManagement()
        {
            contentPanel.Controls.Clear();

            Panel productPanel = new Panel();
            productPanel.Dock = DockStyle.Fill;
            productPanel.BackColor = Color.White;

            Label title = new Label();
            title.Text = "🏢 مدیریت محصولات";
            title.Font = new Font("IRANSans", 16, FontStyle.Bold);
            title.ForeColor = Color.FromArgb(0, 102, 51);
            title.Size = new Size(300, 40);
            title.Location = new Point(20, 20);
            productPanel.Controls.Add(title);

            contentPanel.Controls.Add(productPanel);
        }

        // متد کمکی برای گرد کردن گوشه‌ها
        private void BorderRadius(Control control, int radius)
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(control.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(control.Width - radius, control.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, control.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();
            control.Region = new Region(path);
        }
    }
}