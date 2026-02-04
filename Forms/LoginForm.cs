using System;
using System.Drawing;
using System.Windows.Forms;

namespace PetrochemicalSalesSystem.Forms
{
    public partial class LoginForm : Form
    {
        private Color primaryColor = Color.FromArgb(0, 102, 51);
        private Color secondaryColor = Color.FromArgb(34, 139, 34);

        public LoginForm()
        {
            InitializeComponent();
            InitializeDesign();
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
            mainPanel.BorderRadius(20); // نیاز به متد اکستنشن

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

            // فیلد نام کاربری
            Label lblUser = new Label();
            lblUser.Text = "👤 نام کاربری:";
            lblUser.Font = DefaultFont;
            lblUser.Size = new Size(100, 30);
            lblUser.Location = new Point(300, 130);


            TextBox txtUser = new RoundedTextBox();
            txtUser.Size = new Size(200, 35);
            txtUser.Location = new Point(70, 130);
            txtUser.Font = DefaultFont;
            txtUser.Text = "admin";

            // فیلد رمز عبور
            Label lblPass = new Label();
            lblPass.Text = "🔒 رمز عبور:";
            lblPass.Font = DefaultFont;
            lblPass.Size = new Size(100, 30);
            lblPass.Location = new Point(300, 180);

            TextBox txtPass = new RoundedTextBox();
            txtPass.Size = new Size(200, 35);
            txtPass.Location = new Point(70, 180);
            txtPass.Font = DefaultFont;
            txtPass.PasswordChar = '•';
            txtPass.Text = "********";

            // دکمه ورود
            Button btnLogin = new Button();
            btnLogin.Text = "🚪 ورود به سیستم";
            btnLogin.BackColor = secondaryColor;
            btnLogin.ForeColor = Color.White;
            btnLogin.Font = new Font("B Nazanin", 12, FontStyle.Bold);
            btnLogin.Size = new Size(200, 45);
            btnLogin.Location = new Point(70, 240);
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.BorderRadius(10);

            // چک‌باکس یادآوری
            CheckBox chkRemember = new CheckBox();
            chkRemember.Text = " مرا به خاطر بسپار";
            chkRemember.Font = default;
            chkRemember.Size = new Size(150, 25);
            chkRemember.Location = new Point(70, 300);

            // لینک فراموشی رمز
            LinkLabel lnkForgot = new LinkLabel();
            lnkForgot.Text = "🔗 رمز عبور را فراموش کرده‌ام";
            lnkForgot.Font = new Font("B Nazanin", 10);
            lnkForgot.Size = new Size(200, 25);
            lnkForgot.Location = new Point(70, 330);
            lnkForgot.LinkColor = primaryColor;

            bodyPanel.Controls.Add(lblUser);
            bodyPanel.Controls.Add(txtUser);
            bodyPanel.Controls.Add(lblPass);
            bodyPanel.Controls.Add(txtPass);
            bodyPanel.Controls.Add(btnLogin);
            bodyPanel.Controls.Add(chkRemember);
            bodyPanel.Controls.Add(lnkForgot);

            mainPanel.Controls.Add(headerPanel);
            mainPanel.Controls.Add(bodyPanel);

            this.Controls.Add(mainPanel);
        }
    }

    // کلاس‌های کمکی برای زیباسازی
    public static class ControlExtensions
    {
        public static void BorderRadius(this Control control, int radius)
        {
            control.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, control.Width, control.Height, radius, radius));
        }

        public static void BorderRadius(this Control control, int topLeft, int topRight, int bottomRight, int bottomLeft)
        {
            control.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, control.Width, control.Height,
                topLeft, topRight));
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect,
            int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);
    }

    public class RoundedTextBox : TextBox
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect,
            int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 10, 10));
        }

        public string PlaceholderText { get; set; }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == 0xF || m.Msg == 0x133)
            {
                if (!string.IsNullOrEmpty(this.PlaceholderText) && string.IsNullOrEmpty(this.Text))
                {
                    using (Graphics g = this.CreateGraphics())
                    {
                        g.DrawString(this.PlaceholderText, this.Font, Brushes.Gray,
                            new PointF(5, (this.Height - this.Font.Height) / 2));
                    }
                }
            }
        }
    }
}
