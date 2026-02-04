using PetrochemicalSalesSystem.Forms;
using System;
using System.Windows.Forms;

namespace PetrochemicalAccountantSystem
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // نمایش فرم لاگین
            MainForm loginForm = new MainForm();
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                Application.Run(new MainForm());
            }
        }
    }
}
