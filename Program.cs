using System;
using System.Windows.Forms;
using PetrochemicalSalesSystem.Forms;

namespace PetrochemicalSalesSystem
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // ایجاد و نمایش فرم لاگین
            LoginForm loginForm = new LoginForm();

            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                // اگر لاگین موفق بود، فرم اصلی را اجرا کن
                Application.Run(new AccountantForm());
            }
            else
            {
                Application.Exit();
            }
        }
    }
}