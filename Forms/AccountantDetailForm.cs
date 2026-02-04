using PetrochemicalSalesSystem.Data;
using PetrochemicalSalesSystem.Forms;
using PetrochemicalSalesSystem.Models;
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace PetrochemicalAccountantSystem.Forms
{
    public partial class AccountantDetailForm : Form
    {
        private AccountantRepository _repository;
        private Accountant _accountant;

        private Font titleFont = new Font("B Nazanin", 14, FontStyle.Bold);
        private Font normalFont = new Font("B Nazanin", 11);
        private Color primaryColor = Color.FromArgb(0, 102, 51);

        public AccountantDetailForm(long accountantId)
        {
            InitializeComponent();
            _repository = new AccountantRepository();
            _accountant = _repository.GetAccountantById(accountantId);
            InitializeDesign();
            LoadAccountantDetails();
        }

        private void InitializeDesign()
        {
            this.Text = "جزئیات حسابدار";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.BackColor = Color.FromArgb(240, 242, 245);
            this.Font = normalFont;

            CreateHeaderPanel();
            CreateDetailPanel();
            CreateButtonPanel();
        }

        private void CreateHeaderPanel()
        {
            Panel headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 80;
            headerPanel.BackColor = primaryColor;

            Label titleLabel = new Label();
            titleLabel.Text = "👁️ جزئیات حسابدار";
            titleLabel.Font = titleFont;
            titleLabel.ForeColor = Color.White;
            titleLabel.Dock = DockStyle.Fill;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;

            Button closeButton = new Button();
            closeButton.Text = "✕";
            closeButton.Font = new Font("Arial", 12);
            closeButton.ForeColor = Color.White;
            closeButton.BackColor = Color.Transparent;
            closeButton.FlatStyle = FlatStyle.Flat;
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Size = new Size(40, 40);
            closeButton.Location = new Point(10, 20);
            closeButton.Cursor = Cursors.Hand;
            closeButton.Click += (s, e) => this.Close();

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(closeButton);

            this.Controls.Add(headerPanel);
        }

        private void CreateDetailPanel()
        {
            TabControl tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Location = new Point(0, 80);
            tabControl.Size = new Size(880, 520);
            tabControl.Font = normalFont;

            // تب اطلاعات پایه
            TabPage basicTab = new TabPage("اطلاعات پایه");
            basicTab.BackColor = Color.White;
            basicTab.Padding = new Padding(20);

            TableLayoutPanel basicTable = CreateBasicInfoTable();
            basicTab.Controls.Add(basicTable);

            // تب اطلاعات شغلی
            TabPage jobTab = new TabPage("اطلاعات شغلی");
            jobTab.BackColor = Color.White;
            jobTab.Padding = new Padding(20);

            TableLayoutPanel jobTable = CreateJobInfoTable();
            jobTab.Controls.Add(jobTable);

            // تب اطلاعات تماس
            TabPage contactTab = new TabPage("اطلاعات تماس");
            contactTab.BackColor = Color.White;
            contactTab.Padding = new Padding(20);

            TableLayoutPanel contactTable = CreateContactInfoTable();
            contactTab.Controls.Add(contactTable);

            tabControl.TabPages.Add(basicTab);
            tabControl.TabPages.Add(jobTab);
            tabControl.TabPages.Add(contactTab);

            this.Controls.Add(tabControl);
        }

        private TableLayoutPanel CreateBasicInfoTable()
        {
            TableLayoutPanel table = new TableLayoutPanel();
            table.Dock = DockStyle.Fill;
            table.ColumnCount = 2;
            table.RowCount = 10;
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));

            AddDetailRow(table, "کد پرسنلی:", _accountant?.EmployeeCode, 0);
            AddDetailRow(table, "کد ملی:", _accountant?.NationalID, 1);
            AddDetailRow(table, "نام کامل:", _accountant?.FullName, 2);
            AddDetailRow(table, "نام پدر:", _accountant?.FatherName, 3);
            AddDetailRow(table, "جنسیت:", GetGenderText(_accountant?.Gender), 4);
            AddDetailRow(table, "تاریخ تولد:", _accountant?.BirthDate.ToString("yyyy/MM/dd"), 5);
            AddDetailRow(table, "وضعیت تاهل:", GetMaritalStatusText(_accountant?.MaritalStatus), 6);
            AddDetailRow(table, "سطح تحصیلات:", _accountant?.EducationLevel, 7);
            AddDetailRow(table, "وضعیت:", GetStatusText(_accountant?.IsActive), 8);

            return table;
        }

        private TableLayoutPanel CreateJobInfoTable()
        {
            TableLayoutPanel table = new TableLayoutPanel();
            table.Dock = DockStyle.Fill;
            table.ColumnCount = 2;
            table.RowCount = 8;
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));

            AddDetailRow(table, "دپارتمان:", _accountant?.DepartmentName, 0);
            AddDetailRow(table, "سمت:", _accountant?.Position, 1);
            AddDetailRow(table, "عنوان شغلی:", _accountant?.JobTitle, 2);
            AddDetailRow(table, "سطح شغلی:", _accountant?.JobLevel.ToString(), 3);
            AddDetailRow(table, "نوع استخدام:", _accountant?.EmploymentType, 4);
            AddDetailRow(table, "تاریخ استخدام:", _accountant?.HireDate.ToString("yyyy/MM/dd"), 5);
            AddDetailRow(table, "حقوق پایه:", _accountant?.BaseSalary.ToString("N0") + " ریال", 6);
            AddDetailRow(table, "کد مرکز هزینه:", _accountant?.CostCenterCode, 7);

            return table;
        }

        private TableLayoutPanel CreateContactInfoTable()
        {
            TableLayoutPanel table = new TableLayoutPanel();
            table.Dock = DockStyle.Fill;
            table.ColumnCount = 2;
            table.RowCount = 6;
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));

            AddDetailRow(table, "موبایل:", _accountant?.Mobile, 0);
            AddDetailRow(table, "ایمیل کاری:", _accountant?.WorkEmail, 1);
            AddDetailRow(table, "ایمیل شخصی:", _accountant?.PersonalEmail, 2);
            AddDetailRow(table, "شماره حساب:", _accountant?.BankAccountNo, 3);
            AddDetailRow(table, "نام بانک:", _accountant?.BankName, 4);
            AddDetailRow(table, "شعبه بانک:", _accountant?.BankBranch, 5);

            return table;
        }

        private void AddDetailRow(TableLayoutPanel table, string label, string value, int row)
        {
            Label lbl = new Label();
            lbl.Text = label;
            lbl.Font = new Font("B Nazanin", 11, FontStyle.Bold);
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            lbl.Dock = DockStyle.Fill;
            lbl.Margin = new Padding(5);
            lbl.ForeColor = Color.FromArgb(64, 64, 64);

            Label val = new Label();
            val.Text = value ?? "---";
            val.Font = normalFont;
            val.TextAlign = ContentAlignment.MiddleLeft;
            val.Dock = DockStyle.Fill;
            val.Margin = new Padding(5);
            val.BorderStyle = BorderStyle.FixedSingle;
            val.BackColor = Color.White;
            val.Padding = new Padding(5);

            table.Controls.Add(lbl, 0, row);
            table.Controls.Add(val, 1, row);
        }

        private void CreateButtonPanel()
        {
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 70;
            buttonPanel.BackColor = Color.White;
            buttonPanel.BorderStyle = BorderStyle.FixedSingle;
            buttonPanel.Padding = new Padding(20);

            // دکمه ویرایش
            Button editButton = new Button();
            editButton.Text = "✏️ ویرایش اطلاعات";
            editButton.BackColor = Color.FromArgb(13, 110, 253);
            editButton.ForeColor = Color.White;
            editButton.Font = new Font("B Nazanin", 11, FontStyle.Bold);
            editButton.Size = new Size(150, 40);
            editButton.FlatStyle = FlatStyle.Flat;
            editButton.FlatAppearance.BorderSize = 0;
            editButton.Cursor = Cursors.Hand;
            editButton.Location = new Point(20, 15);
            editButton.Click += BtnEdit_Click;

            // دکمه چاپ
            Button printButton = new Button();
            printButton.Text = "🖨️ چاپ اطلاعات";
            printButton.BackColor = Color.FromArgb(108, 117, 125);
            printButton.ForeColor = Color.White;
            printButton.Font = normalFont;
            printButton.Size = new Size(130, 40);
            printButton.FlatStyle = FlatStyle.Flat;
            printButton.FlatAppearance.BorderSize = 0;
            printButton.Cursor = Cursors.Hand;
            printButton.Location = new Point(180, 15);
            printButton.Click += BtnPrint_Click;

            // دکمه بستن
            Button closeButton = new Button();
            closeButton.Text = "❌ بستن";
            closeButton.BackColor = Color.FromArgb(220, 53, 69);
            closeButton.ForeColor = Color.White;
            closeButton.Font = normalFont;
            closeButton.Size = new Size(100, 40);
            closeButton.FlatStyle = FlatStyle.Flat;
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Cursor = Cursors.Hand;
            closeButton.Location = new Point(720, 15);
            closeButton.Click += (s, e) => this.Close();

            buttonPanel.Controls.Add(editButton);
            buttonPanel.Controls.Add(printButton);
            buttonPanel.Controls.Add(closeButton);

            this.Controls.Add(buttonPanel);
        }

        private void LoadAccountantDetails()
        {
            if (_accountant != null)
            {
                this.Text = $"جزئیات حسابدار - {_accountant.FullName}";
            }
        }

        private string GetGenderText(char? gender)
        {
            if (!gender.HasValue)
                return "---";

            switch (gender.Value)
            {
                case 'M':
                    return "مرد";
                case 'F':
                    return "زن";
                default:
                    return "---";
            }
        }

        private string GetMaritalStatusText(char? status)
        {
            if (!status.HasValue)
                return "---";

            switch (status.Value)
            {
                case 'S':
                    return "مجرد";
                case 'M':
                    return "متاهل";
                case 'D':
                    return "مطلقه";
                case 'W':
                    return "همسر فوت شده";
                default:
                    return "---";
            }
        }

        private string GetStatusText(bool? isActive)
        {
            if (isActive == true) return "✅ فعال";
            if (isActive == false) return "❌ غیرفعال";
            return "---";
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (_accountant != null)
            {
                AccountantEditForm editForm = new AccountantEditForm(_accountant.AccountantID);
                editForm.ShowDialog();
                this.Close(); // بستن فرم جزئیات
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            PrintDocument printDoc = new PrintDocument();
            printDoc.DocumentName = $"جزئیات حسابدار - {_accountant?.FullName}";

            printDoc.PrintPage += (s, ev) =>
            {
                PrintAccountantDetails(ev);
            };

            printDialog.Document = printDoc;

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDoc.Print();
            }
        }

        private void PrintAccountantDetails(PrintPageEventArgs ev)
        {
            Font titleFont = new Font("B Nazanin", 16, FontStyle.Bold);
            Font normalFont = new Font("B Nazanin", 12);
            Brush brush = Brushes.Black;

            float yPos = 50;
            float leftMargin = ev.MarginBounds.Left;

            // چاپ عنوان
            ev.Graphics.DrawString($"جزئیات حسابدار - {_accountant?.FullName}", titleFont, brush, leftMargin, yPos);
            yPos += 40;

            // چاپ اطلاعات پایه
            ev.Graphics.DrawString("اطلاعات پایه:", new Font("B Nazanin", 14, FontStyle.Bold), brush, leftMargin, yPos);
            yPos += 30;

            string[] details = {
                $"کد پرسنلی: {_accountant?.EmployeeCode}",
                $"کد ملی: {_accountant?.NationalID}",
                $"نام کامل: {_accountant?.FullName}",
                $"جنسیت: {GetGenderText(_accountant?.Gender)}",
                $"تاریخ تولد: {_accountant?.BirthDate:yyyy/MM/dd}",
                $"وضعیت: {GetStatusText(_accountant?.IsActive)}"
            };

            foreach (string detail in details)
            {
                ev.Graphics.DrawString(detail, normalFont, brush, leftMargin + 20, yPos);
                yPos += 25;
            }

            ev.HasMorePages = false;
        }
    }
}