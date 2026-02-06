using PetrochemicalSalesSystem.Services;
using PetrochemicalSalesSystem.Utilities;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PetrochemicalSalesSystem.Forms
{
    public partial class LoginForm : Form
    {
        // تبدیل کنترل‌ها به فیلدهای کلاس
        private TextBox txtUser;
        private TextBox txtPass;
        private Button btnLogin;
        private CheckBox chkRemember;

        private Color primaryColor = Color.FromArgb(0, 102, 51);
        private Color secondaryColor = Color.FromArgb(34, 139, 34);

        // شیء سرویس احراز هویت
        private readonly AuthService _authService;

        public LoginForm()
        {
            InitializeComponent();
            _authService = new AuthService();
            InitializeDesign();
            AttachEventHandlers();
        }

        private void InitializeDesign()
        {
            this.Text = "ورود به سیستم - پتروشیمی";
            this.Size = new Size(500, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.BackColor = primaryColor;

            CreateLoginPanel();
        }

        private void CreateLoginPanel()
        {
            Panel mainPanel = new Panel();
            mainPanel.Size = new Size(440, 440);
            mainPanel.Location = new Point(22, 5);
            mainPanel.BackColor = Color.White;
            // mainPanel.BorderRadius(20); // این خط را کامنت کنید اگر متد BorderRadius ندارید

            // هدر
            Panel headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 50;
            headerPanel.Width = 550;
            headerPanel.BackColor = primaryColor;

            Label titleLabel = new Label();
            titleLabel.Text = "🔐 سیستم مدیریت حسابداران";
            titleLabel.Font = new Font("B Nazanin", 16, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.Dock = DockStyle.Fill;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            headerPanel.Controls.Add(titleLabel);

            // بدنه فرم
            Panel bodyPanel = new Panel();
            bodyPanel.Dock = DockStyle.Fill;
            bodyPanel.Padding = new Padding(40);

            // برچسب و فیلد نام کاربری
            Label lblUser = new Label();
            lblUser.Text = "👤 نام کاربری:";
            lblUser.Font = new Font("B Nazanin", 11);
            lblUser.Size = new Size(100, 30);
            lblUser.Location = new Point(300, 130);

            // تغییر: استفاده از TextBox معمولی
            txtUser = new TextBox();
            txtUser.Size = new Size(200, 35);
            txtUser.Location = new Point(70, 130);
            txtUser.Font = new Font("B Nazanin", 11);
            txtUser.Text = "";
            txtUser.BorderStyle = BorderStyle.FixedSingle;

            // برچسب و فیلد رمز عبور
            Label lblPass = new Label();
            lblPass.Text = "🔒 رمز عبور:";
            lblPass.Font = new Font("B Nazanin", 11);
            lblPass.Size = new Size(100, 30);
            lblPass.Location = new Point(300, 180);

            // تغییر: استفاده از TextBox معمولی
            txtPass = new TextBox();
            txtPass.Size = new Size(200, 35);
            txtPass.Location = new Point(70, 180);
            txtPass.Font = new Font("B Nazanin", 11);
            txtPass.PasswordChar = '•';
            txtPass.Text = "";
            txtPass.BorderStyle = BorderStyle.FixedSingle;

            // دکمه ورود
            btnLogin = new Button();
            btnLogin.Text = "🚪 ورود به سیستم";
            btnLogin.BackColor = secondaryColor;
            btnLogin.ForeColor = Color.White;
            btnLogin.Font = new Font("B Nazanin", 12, FontStyle.Bold);
            btnLogin.Size = new Size(200, 45);
            btnLogin.Location = new Point(70, 240);
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Cursor = Cursors.Hand;
            // btnLogin.BorderRadius(10); // این خط را حذف یا کامنت کنید

            // چک‌باکس یادآوری
            chkRemember = new CheckBox();
            chkRemember.Text = " مرا به خاطر بسپار";
            chkRemember.Font = new Font("B Nazanin", 10);
            chkRemember.Size = new Size(150, 25);
            chkRemember.Location = new Point(70, 300);

            // اضافه کردن کنترل‌ها به پنل
            bodyPanel.Controls.Add(lblUser);
            bodyPanel.Controls.Add(txtUser);
            bodyPanel.Controls.Add(lblPass);
            bodyPanel.Controls.Add(txtPass);
            bodyPanel.Controls.Add(btnLogin);
            bodyPanel.Controls.Add(chkRemember);

            mainPanel.Controls.Add(headerPanel);
            mainPanel.Controls.Add(bodyPanel);
            this.Controls.Add(mainPanel);
        }

        private void AttachEventHandlers()
        {
            btnLogin.Click += BtnLogin_Click;
            txtUser.KeyDown += TextBox_KeyDown;
            txtPass.KeyDown += TextBox_KeyDown;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            // غیرفعال کردن دکمه هنگام پردازش
            btnLogin.Enabled = false;
            btnLogin.Text = "در حال بررسی...";
            btnLogin.BackColor = Color.Gray;

            string username = txtUser.Text.Trim();
            string password = txtPass.Text;

            // اعتبارسنجی اولیه
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("لطفاً نام کاربری را وارد کنید.", "خطا",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ResetLoginButton();
                txtUser.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("لطفاً رمز عبور را وارد کنید.", "خطا",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ResetLoginButton();
                txtPass.Focus();
                return;
            }

            try
            {
                // فراخوانی سرویس احراز هویت
                var result = _authService.ValidateLogin(username, password);

                if (result.IsValid && result.Accountant != null)
                {
                    // ذخیره اطلاعات کاربر در Session
                    SessionManager.CurrentUser = result.Accountant;

                    /*
                    // ذخیره اطلاعات برای "مرا به خاطر بسپار"
                    if (chkRemember.Checked)
                    {
                        SaveRememberMe(username);
                    }
                    else
                    {
                        ClearRememberMe();
                    }
                    */

                    MessageBox.Show($"خوش آمدید {result.Accountant.FullName}!",
                        "ورود موفق", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // تنظیم DialogResult به OK برای بستن فرم
                    this.DialogResult = DialogResult.OK;
                    this.Close(); // فرم لاگین بسته می‌شود

                    /*
                    // باز کردن فرم اصلی
                    this.Hide();
                    var accountantEditForm = new AccountantEditForm(); // فرم اصلی برنامه
                    accountantEditForm.Closed += (s, args) => this.Close();
                    accountantEditForm.Show();
                    */
                }
                else
                {
                    MessageBox.Show(result.ErrorMessage, "خطای ورود",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ResetLoginButton();

                    // پاک کردن فیلد رمز عبور
                    txtPass.Text = "";
                    txtPass.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در اتصال به پایگاه داده:\n{ex.Message}",
                    "خطای سیستم", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetLoginButton();
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // ورود با کلید Enter
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                BtnLogin_Click(sender, e);
            }
        }

        private void ResetLoginButton()
        {
            btnLogin.Enabled = true;
            btnLogin.Text = "🚪 ورود به سیستم";
            btnLogin.BackColor = secondaryColor;
        }

        /*
        private void SaveRememberMe(string username)
        {
            Properties.Settings.Default.RememberedUsername = username;
            Properties.Settings.Default.RememberMe = true;
            Properties.Settings.Default.Save();
        }

        private void ClearRememberMe()
        {
            Properties.Settings.Default.RememberedUsername = "";
            Properties.Settings.Default.RememberMe = false;
            Properties.Settings.Default.Save();
        }

        // بارگذاری اطلاعات ذخیره شده در هنگام لود فرم
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Properties.Settings.Default.RememberMe)
            {
                txtUser.Text = Properties.Settings.Default.RememberedUsername;
                chkRemember.Checked = true;
                txtPass.Focus();
            }
            else
            {
                txtUser.Focus();
            }
        }
        */

    }
}