using System;
using System.Windows.Forms;
using PetrochemicalSalesSystem.Forms;
using PetrochemicalSalesSystem.Utilities;

namespace PetrochemicalSalesSystem
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // اجرای فرم لاگین
            LoginForm loginForm = new LoginForm();
            DialogResult loginResult = loginForm.ShowDialog();

            // بررسی نتیجه لاگین
            if (loginResult == DialogResult.OK)
            {
                // اگر لاگین موفق بود، بررسی نوع کاربر و باز کردن فرم مناسب
                if (SessionManager.IsAdmin)
                {
                    // مدیر به MainForm هدایت می‌شود
                    Application.Run(new MainForm());
                }
                else if (SessionManager.IsAccountant)
                {
                    // حسابدار به AccountantForm هدایت می‌شود
                    // اگر AccountantForm ندارید، می‌توانید از MainForm استفاده کنید
                    Application.Run(new AccountantForm()); // یا new MainForm()
                }
                else
                {
                    // نوع کاربر نامشخص
                    MessageBox.Show("نوع کاربر نامشخص است.", "خطا");
                    Application.Exit();
                }
            }
            else
            {
                // اگر کاربر لاگین نکرد یا فرم را بست
                Application.Exit();
            }
        }
    }
}