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
        private Color primaryColor = Color.FromArgb(0, 102, 51); // سبز پتروشیمی
        private Color secondaryColor = Color.FromArgb(34, 139, 34);
        private Color activeButtonColor = Color.FromArgb(0, 80, 40);

        public AccountantForm()
        {
            InitializeComponent();
            InitializeAccountantForm();
            LoadDashboard(); // پیش‌فرض داشبورد نمایش داده شود
        }

        private void InitializeAccountantForm()
        {
            this.Text = $"سیستم فروش پتروشیمی - کاربر: {SessionManager.CurrentUser?.FullName}";
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
            lblTitle.Font = new Font("B Nazanin", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Dock = DockStyle.Left;
            lblTitle.Padding = new Padding(20, 0, 0, 0);
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;

            // اطلاعات کاربر
            Label lblUserInfo = new Label();
            lblUserInfo.Text = $"👤 {SessionManager.CurrentUser?.FullName} | کد کارمندی: {SessionManager.CurrentUser?.EmployeeCode}";
            lblUserInfo.Font = new Font("B Nazanin", 12);
            lblUserInfo.ForeColor = Color.White;
            lblUserInfo.Dock = DockStyle.Right;
            lblUserInfo.Padding = new Padding(0, 0, 20, 0);
            lblUserInfo.TextAlign = ContentAlignment.MiddleRight;

            // دکمه خروج
            Button btnLogout = new Button();
            btnLogout.Text = "🚪 خروج";
            btnLogout.Font = new Font("B Nazanin", 10, FontStyle.Bold);
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
                    SessionManager.Logout();
                    Application.Restart();
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
            sidebarPanel.BackColor = Color.FromArgb(240, 240, 240);
            sidebarPanel.BorderStyle = BorderStyle.FixedSingle;

            // لیست منوها
            string[] menuItems = {
                "📊 داشبورد",
                "🧾 ثبت فاکتور جدید",
                "📋 مشاهده فاکتورها",
                "🔍 جستجوی فاکتور",
                "📈 گزارش‌های فروش",
                "💰 آمارهای مالی",
                "👥 مدیریت مشتریان",
                "🏢 مدیریت محصولات",
                "⚙️ تنظیمات سیستم"
            };

            int buttonY = 20;
            foreach (var menuText in menuItems)
            {
                Button menuButton = new Button();
                menuButton.Text = menuText;
                menuButton.Font = new Font("B Nazanin", 11);
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
                        menuButton.BackColor = Color.FromArgb(220, 220, 220);
                };
                menuButton.MouseLeave += (s, e) =>
                {
                    if (currentButton != menuButton)
                        menuButton.BackColor = Color.Transparent;
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
            contentPanel.BackColor = Color.White;
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
                case "📈 گزارش‌های فروش":
                    LoadSalesReports();
                    break;
                case "💰 آمارهای مالی":
                    LoadFinancialStats();
                    break;
                    // سایر موارد...
            }
        }

        private void LoadDashboard()
        {
            // پاک کردن محتوای قبلی
            contentPanel.Controls.Clear();

            // عنوان
            Label title = new Label();
            title.Text = "📊 داشبورد مدیریتی";
            title.Font = new Font("B Nazanin", 18, FontStyle.Bold);
            title.Size = new Size(400, 40);
            title.Location = new Point(20, 20);
            contentPanel.Controls.Add(title);

            // ایجاد کارت‌های آمار
            CreateStatCard("فاکتورهای امروز", "15", "عدد", new Point(20, 80), Color.FromArgb(52, 152, 219));
            CreateStatCard("فروش امروز", "۲۵,۴۰۰,۰۰۰", "تومان", new Point(250, 80), Color.FromArgb(46, 204, 113));
            CreateStatCard("مشتریان جدید", "۳", "نفر", new Point(480, 80), Color.FromArgb(155, 89, 182));
            CreateStatCard("موجودی انبار", "۱,۲۵۰", "عدد", new Point(710, 80), Color.FromArgb(241, 196, 15));

            // نمودار سریع (نمایش شبیه‌سازی)
            Panel chartPanel = new Panel();
            chartPanel.Size = new Size(800, 300);
            chartPanel.Location = new Point(20, 200);
            chartPanel.BackColor = Color.FromArgb(250, 250, 250);
            chartPanel.BorderStyle = BorderStyle.FixedSingle;

            Label chartTitle = new Label();
            chartTitle.Text = "📈 آمار فروش ۷ روز اخیر";
            chartTitle.Font = new Font("B Nazanin", 14, FontStyle.Bold);
            chartTitle.Location = new Point(20, 20);
            chartTitle.AutoSize = true;
            chartPanel.Controls.Add(chartTitle);

            // لیست فاکتورهای اخیر
            LoadRecentInvoices();
        }

        private void CreateStatCard(string title, string value, string unit, Point location, Color color)
        {
            Panel card = new Panel();
            card.Size = new Size(200, 100);
            card.Location = location;
            card.BackColor = color;
            card.BorderRadius(10);

            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = new Font("B Nazanin", 11, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(15, 15);
            lblTitle.AutoSize = true;

            Label lblValue = new Label();
            lblValue.Text = value;
            lblValue.Font = new Font("B Nazanin", 20, FontStyle.Bold);
            lblValue.ForeColor = Color.White;
            lblValue.Location = new Point(15, 40);
            lblValue.AutoSize = true;

            Label lblUnit = new Label();
            lblUnit.Text = unit;
            lblUnit.Font = new Font("B Nazanin", 10);
            lblUnit.ForeColor = Color.White;
            lblUnit.Location = new Point(15, 70);
            lblUnit.AutoSize = true;

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblValue);
            card.Controls.Add(lblUnit);
            contentPanel.Controls.Add(card);
        }

        private void LoadRecentInvoices()
        {
            Panel recentPanel = new Panel();
            recentPanel.Size = new Size(800, 200);
            recentPanel.Location = new Point(20, 520);
            recentPanel.BackColor = Color.White;
            recentPanel.BorderStyle = BorderStyle.FixedSingle;

            Label title = new Label();
            title.Text = "🧾 فاکتورهای اخیر";
            title.Font = new Font("B Nazanin", 14, FontStyle.Bold);
            title.Location = new Point(20, 15);
            title.AutoSize = true;
            recentPanel.Controls.Add(title);

            // دیتاگریوی ساده برای نمایش فاکتورها
            DataGridView dgv = new DataGridView();
            dgv.Size = new Size(760, 140);
            dgv.Location = new Point(20, 50);
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.None;
            dgv.RowHeadersVisible = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // ستون‌ها
            dgv.Columns.Add("InvoiceNo", "شماره فاکتور");
            dgv.Columns.Add("Date", "تاریخ");
            dgv.Columns.Add("Customer", "مشتری");
            dgv.Columns.Add("Amount", "مبلغ (تومان)");
            dgv.Columns.Add("Status", "وضعیت");

            // داده‌های نمونه
            dgv.Rows.Add("INV-001", "1402/12/15", "شرکت الف", "۱۲,۵۰۰,۰۰۰", "پرداخت شده");
            dgv.Rows.Add("INV-002", "1402/12/16", "شرکت ب", "۸,۷۰۰,۰۰۰", "در انتظار");
            dgv.Rows.Add("INV-003", "1402/12/17", "شرکت ج", "۱۵,۲۰۰,۰۰۰", "پرداخت شده");

            recentPanel.Controls.Add(dgv);
            contentPanel.Controls.Add(recentPanel);
        }

        private void LoadInvoiceForm()
        {
            contentPanel.Controls.Clear();

            // فرم ثبت فاکتور
            InvoiceForm invoiceForm = new InvoiceForm();
            invoiceForm.TopLevel = false;
            invoiceForm.FormBorderStyle = FormBorderStyle.None;
            invoiceForm.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(invoiceForm);
            invoiceForm.Show();
        }

        private void LoadInvoicesList()
        {
            contentPanel.Controls.Clear();

            // فرم لیست فاکتورها
            InvoicesListForm invoicesList = new InvoicesListForm();
            invoicesList.TopLevel = false;
            invoicesList.FormBorderStyle = FormBorderStyle.None;
            invoicesList.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(invoicesList);
            invoicesList.Show();
        }

        private void LoadSalesReports()
        {
            contentPanel.Controls.Clear();

            Label title = new Label();
            title.Text = "📈 گزارش‌های فروش";
            title.Font = new Font("B Nazanin", 18, FontStyle.Bold);
            title.Size = new Size(300, 40);
            title.Location = new Point(20, 20);
            contentPanel.Controls.Add(title);

            // افزودن کنترل‌های گزارش‌گیری
            // ...
        }

        private void LoadFinancialStats()
        {
            contentPanel.Controls.Clear();

            Label title = new Label();
            title.Text = "💰 آمارهای مالی";
            title.Font = new Font("B Nazanin", 18, FontStyle.Bold);
            title.Size = new Size(300, 40);
            title.Location = new Point(20, 20);
            contentPanel.Controls.Add(title);

            // آمارهای مالی
            // ...
        }
    }

    // متد اکستنشن برای گرد کردن گوشه‌ها
    public static class ControlExtensions
    {
        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect,
            int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        public static void BorderRadius(this Control control, int radius)
        {
            control.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, control.Width, control.Height, radius, radius));
        }
    }
}