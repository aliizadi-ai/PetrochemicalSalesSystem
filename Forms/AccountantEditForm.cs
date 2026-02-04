using PetrochemicalSalesSystem.Data;
using PetrochemicalSalesSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PetrochemicalSalesSystem.Forms
{
    public partial class AccountantEditForm : Form
    {
        private void AddComboBoxToTable(TableLayoutPanel table, string name, int column, int row, List<Department> departments)
        {
            ComboBox comboBox = new ComboBox();
            comboBox.Name = name;
            comboBox.Font = normalFont;
            comboBox.Dock = DockStyle.Fill;
            comboBox.Margin = new Padding(5);
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            // اضافه کردن آیتم‌ها
            comboBox.Items.Add("--- انتخاب کنید ---");
            foreach (var dept in departments)
            {
                comboBox.Items.Add(new { ID = dept.DepartmentID, Name = dept.DepartmentName });
            }

            // تنظیم DisplayMember و ValueMember
            comboBox.DisplayMember = "Name";
            comboBox.ValueMember = "ID";

            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;

            table.Controls.Add(comboBox, column, row);
        }

        private void AddComboBoxToTable(TableLayoutPanel table, string name, int column, int row, List<Accountant> accountants)
        {
            ComboBox comboBox = new ComboBox();
            comboBox.Name = name;
            comboBox.Font = normalFont;
            comboBox.Dock = DockStyle.Fill;
            comboBox.Margin = new Padding(5);
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            // اضافه کردن آیتم‌ها
            comboBox.Items.Add("--- انتخاب کنید ---");
            foreach (var acc in accountants)
            {
                comboBox.Items.Add(new { ID = acc.AccountantID, Name = $"{acc.FirstName} {acc.LastName} ({acc.EmployeeCode})" });
            }

            // تنظیم DisplayMember و ValueMember
            comboBox.DisplayMember = "Name";
            comboBox.ValueMember = "ID";

            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;

            table.Controls.Add(comboBox, column, row);
        }
        // متغیرهای طراحی
        private Font titleFont = new Font("B Nazanin", 14, FontStyle.Bold);
        private Font groupFont = new Font("B Nazanin", 12, FontStyle.Bold);
        private Font normalFont = new Font("B Nazanin", 11);

        private Color primaryColor = Color.FromArgb(0, 102, 51);
        private Color secondaryColor = Color.FromArgb(34, 139, 34);

        private TabControl tabControl;
        private Panel headerPanel;
        private FlowLayoutPanel buttonPanel;

        // متغیرهای داده‌ای
        private AccountantRepository _accountantRepository;
        private LookupRepository _lookupRepository;
        private Accountant _currentAccountant;
        private bool _isEditMode = false;
        private string _photoPath = string.Empty;

        public AccountantEditForm()
        {
            InitializeComponent();
            _accountantRepository = new AccountantRepository();
            _lookupRepository = new LookupRepository();
            InitializeDesign();
            InitializeTabs();
            InitializeEvents();
        }

        // سازنده برای حالت ویرایش
        public AccountantEditForm(long accountantId) : this()
        {
            _currentAccountant = _accountantRepository.GetAccountantById(accountantId);
            if (_currentAccountant != null)
            {
                _isEditMode = true;
                LoadAccountantData();
                this.Text = $"ویرایش حسابدار - {_currentAccountant.FullName}";
            }
        }

        private void InitializeDesign()
        {
            this.Text = "فرم ثبت حسابدار جدید";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.BackColor = Color.FromArgb(240, 242, 245);
            this.Font = normalFont;

            CreateHeaderPanel();
            CreateTabControl();
            CreateButtonPanel();
        }

        private void CreateHeaderPanel()
        {
            headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 80;
            headerPanel.BackColor = primaryColor;

            Label titleLabel = new Label();
            titleLabel.Text = "📋 فرم اطلاعات حسابدار";
            titleLabel.Font = titleFont;
            titleLabel.ForeColor = Color.White;
            titleLabel.Dock = DockStyle.Fill;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;

            // آیکون بستن
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

        private void CreateTabControl()
        {
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Location = new Point(0, 80);
            tabControl.Size = new Size(980, 520);
            tabControl.Font = normalFont;
            tabControl.Appearance = TabAppearance.FlatButtons;
            tabControl.ItemSize = new Size(120, 35);
            tabControl.SizeMode = TabSizeMode.Fixed;

            // استایل تب‌ها
            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl.DrawItem += (sender, e) =>
            {
                e.Graphics.FillRectangle(new SolidBrush(primaryColor), e.Bounds);

                Rectangle paddedBounds = e.Bounds;
                paddedBounds.Inflate(-2, -2);

                using (StringFormat sf = new StringFormat())
                {
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;

                    if (e.Index == tabControl.SelectedIndex)
                    {
                        e.Graphics.FillRectangle(new SolidBrush(Color.White), paddedBounds);
                        e.Graphics.DrawString(tabControl.TabPages[e.Index].Text,
                            new Font("B Nazanin", 11, FontStyle.Bold),
                            new SolidBrush(primaryColor),
                            paddedBounds,
                            sf);
                    }
                    else
                    {
                        e.Graphics.FillRectangle(new SolidBrush(primaryColor), paddedBounds);
                        e.Graphics.DrawString(tabControl.TabPages[e.Index].Text,
                            normalFont,
                            new SolidBrush(Color.White),
                            paddedBounds,
                            sf);
                    }
                }
            };

            this.Controls.Add(tabControl);
        }

        private void InitializeTabs()
        {
            // تب 1: اطلاعات شخصی
            TabPage personalTab = CreatePersonalTab();
            tabControl.TabPages.Add(personalTab);

            // تب 2: اطلاعات شغلی
            TabPage jobTab = CreateJobTab();
            tabControl.TabPages.Add(jobTab);

            // تب 3: اطلاعات مالی
            TabPage financialTab = CreateFinancialTab();
            tabControl.TabPages.Add(financialTab);

            // تب 4: اطلاعات تماس
            TabPage contactTab = CreateContactTab();
            tabControl.TabPages.Add(contactTab);

            // تب 5: اطلاعات سیستم
            TabPage systemTab = CreateSystemTab();
            tabControl.TabPages.Add(systemTab);

            // تب 6: اطلاعات تکمیلی
            TabPage additionalTab = CreateAdditionalTab();
            tabControl.TabPages.Add(additionalTab);
        }

        private TabPage CreatePersonalTab()
        {
            TabPage tab = new TabPage("👤 اطلاعات شخصی");
            tab.BackColor = Color.White;
            tab.Padding = new Padding(20);

            TableLayoutPanel table = new TableLayoutPanel();
            table.Dock = DockStyle.Fill;
            table.ColumnCount = 4;
            table.RowCount = 8;
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

            // ردیف 1: کد پرسنلی و کد ملی
            AddLabelToTable(table, "کد پرسنلی *", 0, 0);
            AddTextBoxToTable(table, "txtEmployeeCode", 1, 0);

            AddLabelToTable(table, "کد ملی *", 2, 0);
            AddTextBoxToTable(table, "txtNationalID", 3, 0);

            // ردیف 2: نام و نام خانوادگی
            AddLabelToTable(table, "نام *", 0, 1);
            AddTextBoxToTable(table, "txtFirstName", 1, 1);

            AddLabelToTable(table, "نام خانوادگی *", 2, 1);
            AddTextBoxToTable(table, "txtLastName", 3, 1);

            // ردیف 3: نام پدر و جنسیت
            AddLabelToTable(table, "نام پدر", 0, 2);
            AddTextBoxToTable(table, "txtFatherName", 1, 2);

            AddLabelToTable(table, "جنسیت *", 2, 2);
            AddComboBoxToTable(table, "cmbGender", 3, 2, new string[] { "مرد", "زن" });

            // ردیف 4: تاریخ تولد و وضعیت تاهل
            AddLabelToTable(table, "تاریخ تولد *", 0, 3);
            AddDateTimePickerToTable(table, "dtpBirthDate", 1, 3);

            AddLabelToTable(table, "وضعیت تاهل *", 2, 3);
            AddComboBoxToTable(table, "cmbMaritalStatus", 3, 3,
                new string[] { "مجرد", "متاهل", "مطلقه", "همسر فوت شده" });

            // ردیف 5: تعداد فرزندان و افراد تحت تکفل
            AddLabelToTable(table, "تعداد فرزندان", 0, 4);
            AddNumericUpDownToTable(table, "nudChildren", 1, 4, 0, 10);

            AddLabelToTable(table, "تعداد تحت تکفل", 2, 4);
            AddNumericUpDownToTable(table, "nudDependents", 3, 4, 0, 10);

            // ردیف 6: استان و شهر محل تولد
            AddLabelToTable(table, "استان محل تولد", 0, 5);
            AddComboBoxToTable(table, "cmbBirthProvince", 1, 5, GetProvinces());

            AddLabelToTable(table, "شهر محل تولد", 2, 5);
            AddTextBoxToTable(table, "txtBirthCity", 3, 5);

            // ردیف 7: شماره شناسنامه و سریال
            AddLabelToTable(table, "شماره شناسنامه", 0, 6);
            AddTextBoxToTable(table, "txtBirthCertNo", 1, 6);

            AddLabelToTable(table, "سریال شناسنامه", 2, 6);
            AddTextBoxToTable(table, "txtBirthCertSerial", 3, 6);

            // ردیف 8: کد بیمه
            AddLabelToTable(table, "کد بیمه", 0, 7);
            AddTextBoxToTable(table, "txtInsuranceID", 1, 7);

            tab.Controls.Add(table);
            return tab;
        }

        private TabPage CreateJobTab()
        {
            TabPage tab = new TabPage("💼 اطلاعات شغلی");
            tab.BackColor = Color.White;
            tab.Padding = new Padding(20);

            TableLayoutPanel table = new TableLayoutPanel();
            table.Dock = DockStyle.Fill;
            table.ColumnCount = 4;
            table.RowCount = 6;

            // ردیف 1: دپارتمان و سمت
            AddLabelToTable(table, "دپارتمان *", 0, 0);
            AddComboBoxToTable(table, "cmbDepartment", 1, 0, GetDepartments());

            AddLabelToTable(table, "سمت *", 2, 0);
            AddTextBoxToTable(table, "txtPosition", 3, 0);

            // ردیف 2: عنوان شغلی و سطح شغلی
            AddLabelToTable(table, "عنوان شغلی", 0, 1);
            AddTextBoxToTable(table, "txtJobTitle", 1, 1);

            AddLabelToTable(table, "سطح شغلی *", 2, 1);
            AddComboBoxToTable(table, "cmbJobLevel", 3, 1,
                Enumerable.Range(1, 15).Select(x => x.ToString()).ToArray());

            // ردیف 3: نوع استخدام و تاریخ استخدام
            AddLabelToTable(table, "نوع استخدام *", 0, 2);
            AddComboBoxToTable(table, "cmbEmploymentType", 1, 2,
                new string[] { "دائمی", "پیمانی", "قراردادی", "پروژه‌ای", "ساعتی" });

            AddLabelToTable(table, "تاریخ استخدام *", 2, 2);
            AddDateTimePickerToTable(table, "dtpHireDate", 3, 2);

            // ردیف 4: مدیر مستقیم و فرد پشتیبان
            AddLabelToTable(table, "مدیر مستقیم", 0, 3);
            AddComboBoxToTable(table, "cmbManager", 1, 3, GetManagers());

            AddLabelToTable(table, "فرد پشتیبان", 2, 3);
            AddComboBoxToTable(table, "cmbBackupPerson", 3, 3, GetAccountants());

            // ردیف 5: کد مرکز هزینه و کد مرکز سود
            AddLabelToTable(table, "کد مرکز هزینه *", 0, 4);
            AddTextBoxToTable(table, "txtCostCenter", 1, 4);

            AddLabelToTable(table, "کد مرکز سود", 2, 4);
            AddTextBoxToTable(table, "txtProfitCenter", 3, 4);

            // ردیف 6: کد پروژه و کد شرکت
            AddLabelToTable(table, "کد پروژه", 0, 5);
            AddTextBoxToTable(table, "txtProjectCode", 1, 5);

            AddLabelToTable(table, "کد شرکت", 2, 5);
            AddTextBoxToTable(table, "txtCompanyCode", 3, 5);

            tab.Controls.Add(table);
            return tab;
        }

        private TabPage CreateFinancialTab()
        {
            TabPage tab = new TabPage("💰 اطلاعات مالی");
            tab.BackColor = Color.White;
            tab.Padding = new Padding(20);

            TableLayoutPanel table = new TableLayoutPanel();
            table.Dock = DockStyle.Fill;
            table.ColumnCount = 4;
            table.RowCount = 5;

            // ردیف 1: حقوق پایه و حقوق خالص
            AddLabelToTable(table, "حقوق پایه *", 0, 0);
            AddTextBoxToTable(table, "txtBaseSalary", 1, 0);

            AddLabelToTable(table, "حقوق خالص", 2, 0);
            AddTextBoxToTable(table, "txtNetSalary", 3, 0);

            // ردیف 2: حقوق ناخالص و ارز حقوق
            AddLabelToTable(table, "حقوق ناخالص", 0, 1);
            AddTextBoxToTable(table, "txtGrossSalary", 1, 1);

            AddLabelToTable(table, "ارز حقوق", 2, 1);
            AddComboBoxToTable(table, "cmbSalaryCurrency", 3, 1,
                new string[] { "ریال (IRR)", "دلار (USD)", "یورو (EUR)" });

            // ردیف 3: پایه بیمه و پایه مالیات
            AddLabelToTable(table, "پایه بیمه", 0, 2);
            AddTextBoxToTable(table, "txtInsuranceBase", 1, 2);

            AddLabelToTable(table, "پایه مالیات", 2, 2);
            AddTextBoxToTable(table, "txtTaxBase", 3, 2);

            // ردیف 4: شماره حساب بانکی و نام بانک
            AddLabelToTable(table, "شماره حساب *", 0, 3);
            AddTextBoxToTable(table, "txtBankAccountNo", 1, 3);

            AddLabelToTable(table, "نام بانک *", 2, 3);
            AddTextBoxToTable(table, "txtBankName", 3, 3);

            // ردیف 5: شعبه بانک و نوع حساب
            AddLabelToTable(table, "شعبه بانک *", 0, 4);
            AddTextBoxToTable(table, "txtBankBranch", 1, 4);

            AddLabelToTable(table, "نوع حساب", 2, 4);
            AddComboBoxToTable(table, "cmbAccountType", 3, 4,
                new string[] { "جاری", "پس‌انداز", "قرض‌الحسنه" });

            // GroupBox برای وام‌ها
            GroupBox loanGroup = new GroupBox();
            loanGroup.Text = " 💰 اطلاعات وام‌ها ";
            loanGroup.Font = groupFont;
            loanGroup.ForeColor = primaryColor;
            loanGroup.Dock = DockStyle.Bottom;
            loanGroup.Height = 100;

            TableLayoutPanel loanTable = new TableLayoutPanel();
            loanTable.Dock = DockStyle.Fill;
            loanTable.ColumnCount = 4;
            loanTable.RowCount = 1;

            AddLabelToTable(loanTable, "مبلغ وام مسکن", 0, 0);
            AddTextBoxToTable(loanTable, "txtHousingLoan", 1, 0);

            AddLabelToTable(loanTable, "سایر وام‌ها", 2, 0);
            AddTextBoxToTable(loanTable, "txtOtherLoans", 3, 0);

            loanGroup.Controls.Add(loanTable);

            tab.Controls.Add(table);
            tab.Controls.Add(loanGroup);

            return tab;
        }

        private TabPage CreateContactTab()
        {
            TabPage tab = new TabPage("📞 اطلاعات تماس");
            tab.BackColor = Color.White;
            tab.Padding = new Padding(20);

            TableLayoutPanel table = new TableLayoutPanel();
            table.Dock = DockStyle.Fill;
            table.ColumnCount = 4;
            table.RowCount = 6;

            // ردیف 1: موبایل اصلی و موبایل دوم
            AddLabelToTable(table, "موبایل *", 0, 0);
            AddTextBoxToTable(table, "txtMobile", 1, 0);

            AddLabelToTable(table, "موبایل دوم", 2, 0);
            AddTextBoxToTable(table, "txtMobile2", 3, 0);

            // ردیف 2: تلفن محل کار و داخلی
            AddLabelToTable(table, "تلفن محل کار", 0, 1);
            AddTextBoxToTable(table, "txtWorkPhone", 1, 1);

            AddLabelToTable(table, "داخلی", 2, 1);
            AddTextBoxToTable(table, "txtWorkExtension", 3, 1);

            // ردیف 3: ایمیل کاری و ایمیل شخصی
            AddLabelToTable(table, "ایمیل کاری *", 0, 2);
            AddTextBoxToTable(table, "txtWorkEmail", 1, 2);

            AddLabelToTable(table, "ایمیل شخصی", 2, 2);
            AddTextBoxToTable(table, "txtPersonalEmail", 3, 2);

            // ردیف 4: آدرس محل کار
            AddLabelToTable(table, "آدرس محل کار", 0, 3);
            AddMultiLineTextBoxToTable(table, "txtWorkAddress", 3, 3, 1, 60);

            // ردیف 5: آدرس منزل
            AddLabelToTable(table, "آدرس منزل", 0, 4);
            AddMultiLineTextBoxToTable(table, "txtHomeAddress", 3, 4, 1, 60);

            // ردیف 6: استان و شهر محل سکونت
            AddLabelToTable(table, "استان محل سکونت", 0, 5);
            AddComboBoxToTable(table, "cmbHomeProvince", 1, 5, GetProvinces());

            AddLabelToTable(table, "شهر محل سکونت", 2, 5);
            AddTextBoxToTable(table, "txtHomeCity", 3, 5);

            // GroupBox برای تماس اضطراری
            GroupBox emergencyGroup = new GroupBox();
            emergencyGroup.Text = " 🆘 تماس اضطراری ";
            emergencyGroup.Font = groupFont;
            emergencyGroup.ForeColor = Color.FromArgb(220, 53, 69);
            emergencyGroup.Dock = DockStyle.Bottom;
            emergencyGroup.Height = 120;

            TableLayoutPanel emergencyTable = new TableLayoutPanel();
            emergencyTable.Dock = DockStyle.Fill;
            emergencyTable.ColumnCount = 4;
            emergencyTable.RowCount = 2;

            AddLabelToTable(emergencyTable, "نام تماس اضطراری", 0, 0);
            AddTextBoxToTable(emergencyTable, "txtEmergencyContact", 1, 0);

            AddLabelToTable(emergencyTable, "نسبت", 2, 0);
            AddTextBoxToTable(emergencyTable, "txtEmergencyRelation", 3, 0);

            AddLabelToTable(emergencyTable, "تلفن تماس اضطراری", 0, 1);
            AddTextBoxToTable(emergencyTable, "txtEmergencyPhone", 1, 1);

            emergencyGroup.Controls.Add(emergencyTable);

            tab.Controls.Add(table);
            tab.Controls.Add(emergencyGroup);

            return tab;
        }

        private TabPage CreateSystemTab()
        {
            TabPage tab = new TabPage("🔐 اطلاعات سیستم");
            tab.BackColor = Color.White;
            tab.Padding = new Padding(20);

            TableLayoutPanel table = new TableLayoutPanel();
            table.Dock = DockStyle.Fill;
            table.ColumnCount = 4;
            table.RowCount = 6;

            // ردیف 1: نام کاربری سیستم و نام کاربری AD
            AddLabelToTable(table, "نام کاربری سیستم *", 0, 0);
            AddTextBoxToTable(table, "txtSystemUsername", 1, 0);

            AddLabelToTable(table, "نام کاربری AD", 2, 0);
            AddTextBoxToTable(table, "txtADUsername", 3, 0);

            // ردیف 2: دامنه و کد کاربر در ERP
            AddLabelToTable(table, "دامنه", 0, 1);
            AddTextBoxToTable(table, "txtDomain", 1, 1);

            AddLabelToTable(table, "کد کاربر در ERP", 2, 1);
            AddTextBoxToTable(table, "txtERPUserID", 3, 1);

            // ردیف 3: CheckBox های دسترسی
            AddLabelToTable(table, "دسترسی‌ها", 0, 2);
            Panel accessPanel = new Panel();
            accessPanel.Dock = DockStyle.Fill;
            accessPanel.Margin = new Padding(5);

            CheckBox chkCanApprove = new CheckBox();
            chkCanApprove.Text = "تایید پرداخت";
            chkCanApprove.Font = normalFont;
            chkCanApprove.Location = new Point(10, 10);
            chkCanApprove.Name = "chkCanApprove";

            CheckBox chkCanPost = new CheckBox();
            chkCanPost.Text = "ثبت سند";
            chkCanPost.Font = normalFont;
            chkCanPost.Location = new Point(150, 10);
            chkCanPost.Name = "chkCanPost";

            CheckBox chkCanViewSensitive = new CheckBox();
            chkCanViewSensitive.Text = "مشاهده اطلاعات حساس";
            chkCanViewSensitive.Font = normalFont;
            chkCanViewSensitive.Location = new Point(290, 10);
            chkCanViewSensitive.Name = "chkCanViewSensitive";

            accessPanel.Controls.Add(chkCanApprove);
            accessPanel.Controls.Add(chkCanPost);
            accessPanel.Controls.Add(chkCanViewSensitive);

            table.Controls.Add(accessPanel, 1, 2);
            table.SetColumnSpan(accessPanel, 3);

            // ردیف 4: سطح دسترسی مالی
            AddLabelToTable(table, "سطح دسترسی مالی", 0, 3);
            AddComboBoxToTable(table, "cmbFinancialAccess", 1, 3,
                new string[] { "بدون دسترسی", "مشاهده", "ثبت", "تایید", "ادمین" });

            // ردیف 5: مبلغ تایید حداکثر
            AddLabelToTable(table, "حداکثر مبلغ تایید", 0, 4);
            AddTextBoxToTable(table, "txtMaxApprovalAmount", 1, 4);

            AddLabelToTable(table, "ارز تایید", 2, 4);
            AddComboBoxToTable(table, "cmbApprovalCurrency", 3, 4,
                new string[] { "IRR", "USD", "EUR" });

            // ردیف 6: تاریخ آخرین دسترسی
            AddLabelToTable(table, "تاریخ آخرین دسترسی", 0, 5);
            AddDateTimePickerToTable(table, "dtpLastAccess", 1, 5);

            tab.Controls.Add(table);
            return tab;
        }

        private TabPage CreateAdditionalTab()
        {
            TabPage tab = new TabPage("📋 اطلاعات تکمیلی");
            tab.BackColor = Color.White;
            tab.Padding = new Padding(20);

            TableLayoutPanel table = new TableLayoutPanel();
            table.Dock = DockStyle.Fill;
            table.ColumnCount = 4;
            table.RowCount = 6;

            // ردیف 1: وضعیت نظام وظیفه و گروه خونی
            AddLabelToTable(table, "وضعیت نظام وظیفه", 0, 0);
            AddComboBoxToTable(table, "cmbMilitaryStatus", 1, 0,
                new string[] { "معاف", "مشمول", "پایان خدمت", "در حال خدمت", "معاف پزشکی" });

            AddLabelToTable(table, "گروه خونی", 2, 0);
            AddComboBoxToTable(table, "cmbBloodType", 3, 0,
                new string[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" });

            // ردیف 2: تخصص‌ها
            AddLabelToTable(table, "تخصص‌ها", 0, 1);
            AddMultiLineTextBoxToTable(table, "txtSpecializations", 1, 1, 3, 40);

            // ردیف 3: زبان‌ها
            AddLabelToTable(table, "زبان‌ها", 0, 2);
            AddMultiLineTextBoxToTable(table, "txtLanguages", 1, 2, 3, 40);

            // ردیف 4: مهارت‌های نرم‌افزاری
            AddLabelToTable(table, "مهارت‌های نرم‌افزاری", 0, 3);
            AddMultiLineTextBoxToTable(table, "txtSoftwareSkills", 1, 3, 3, 40);

            // ردیف 5: مدارک و گواهینامه‌ها
            AddLabelToTable(table, "مدارک و گواهینامه‌ها", 0, 4);
            AddMultiLineTextBoxToTable(table, "txtCertifications", 1, 4, 3, 40);

            // ردیف 6: وضعیت فعال و CheckBox های اضافی
            AddLabelToTable(table, "وضعیت", 0, 5);

            Panel statusPanel = new Panel();
            statusPanel.Dock = DockStyle.Fill;
            statusPanel.Margin = new Padding(5);

            CheckBox chkIsActive = new CheckBox();
            chkIsActive.Text = "فعال";
            chkIsActive.Font = normalFont;
            chkIsActive.Location = new Point(10, 10);
            chkIsActive.Name = "chkIsActive";
            chkIsActive.Checked = true;

            CheckBox chkIsOnLeave = new CheckBox();
            chkIsOnLeave.Text = "مرخصی";
            chkIsOnLeave.Font = normalFont;
            chkIsOnLeave.Location = new Point(100, 10);
            chkIsOnLeave.Name = "chkIsOnLeave";

            CheckBox chkHasSecurityClearance = new CheckBox();
            chkHasSecurityClearance.Text = "گواهی امنیتی";
            chkHasSecurityClearance.Font = normalFont;
            chkHasSecurityClearance.Location = new Point(190, 10);
            chkHasSecurityClearance.Name = "chkHasSecurityClearance";

            statusPanel.Controls.Add(chkIsActive);
            statusPanel.Controls.Add(chkIsOnLeave);
            statusPanel.Controls.Add(chkHasSecurityClearance);

            table.Controls.Add(statusPanel, 1, 5);
            table.SetColumnSpan(statusPanel, 3);

            tab.Controls.Add(table);
            return tab;
        }

        private void CreateButtonPanel()
        {
            buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 70;
            buttonPanel.BackColor = Color.White;
            buttonPanel.BorderStyle = BorderStyle.FixedSingle;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonPanel.Padding = new Padding(20);

            // دکمه ذخیره
            Button saveButton = new Button();
            saveButton.Text = "💾 ذخیره اطلاعات";
            saveButton.BackColor = secondaryColor;
            saveButton.ForeColor = Color.White;
            saveButton.Font = new Font("B Nazanin", 11, FontStyle.Bold);
            saveButton.Size = new Size(150, 40);
            saveButton.FlatStyle = FlatStyle.Flat;
            saveButton.FlatAppearance.BorderSize = 0;
            saveButton.Cursor = Cursors.Hand;
            saveButton.Click += BtnSave_Click;

            // دکمه انصراف
            Button cancelButton = new Button();
            cancelButton.Text = "❌ انصراف";
            cancelButton.BackColor = Color.FromArgb(108, 117, 125);
            cancelButton.ForeColor = Color.White;
            cancelButton.Font = normalFont;
            cancelButton.Size = new Size(120, 40);
            cancelButton.FlatStyle = FlatStyle.Flat;
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.Cursor = Cursors.Hand;
            cancelButton.Click += BtnCancel_Click;

            // دکمه پاک کردن فرم
            Button clearButton = new Button();
            clearButton.Text = "🧹 پاک کردن فرم";
            clearButton.BackColor = Color.FromArgb(255, 193, 7);
            clearButton.ForeColor = Color.Black;
            clearButton.Font = normalFont;
            clearButton.Size = new Size(130, 40);
            clearButton.FlatStyle = FlatStyle.Flat;
            clearButton.FlatAppearance.BorderSize = 0;
            clearButton.Cursor = Cursors.Hand;
            clearButton.Click += BtnClear_Click;

            // دکمه بارگذاری عکس
            Button uploadButton = new Button();
            uploadButton.Text = "📷 بارگذاری عکس";
            uploadButton.BackColor = Color.FromArgb(13, 110, 253);
            uploadButton.ForeColor = Color.White;
            uploadButton.Font = normalFont;
            uploadButton.Size = new Size(130, 40);
            uploadButton.FlatStyle = FlatStyle.Flat;
            uploadButton.FlatAppearance.BorderSize = 0;
            uploadButton.Cursor = Cursors.Hand;
            uploadButton.Click += BtnUpload_Click;

            buttonPanel.Controls.Add(saveButton);
            buttonPanel.Controls.Add(cancelButton);
            buttonPanel.Controls.Add(clearButton);
            buttonPanel.Controls.Add(uploadButton);

            this.Controls.Add(buttonPanel);
        }

        // متدهای کمکی برای ایجاد کنترل‌ها
        private void AddLabelToTable(TableLayoutPanel table, string text, int column, int row)
        {
            Label label = new Label();
            label.Text = text;
            label.Font = normalFont;
            label.TextAlign = ContentAlignment.MiddleLeft;
            label.Dock = DockStyle.Fill;
            label.Margin = new Padding(5);
            table.Controls.Add(label, column, row);
        }

        private void AddTextBoxToTable(TableLayoutPanel table, string name, int column, int row)
        {
            TextBox textBox = new TextBox();
            textBox.Name = name;
            textBox.Font = normalFont;
            textBox.Dock = DockStyle.Fill;
            textBox.Margin = new Padding(5);
            textBox.BorderStyle = BorderStyle.FixedSingle;
            table.Controls.Add(textBox, column, row);
        }

        private void AddMultiLineTextBoxToTable(TableLayoutPanel table, string name, int column, int row, int columnSpan, int height)
        {
            TextBox textBox = new TextBox();
            textBox.Name = name;
            textBox.Font = normalFont;
            textBox.Dock = DockStyle.Fill;
            textBox.Margin = new Padding(5);
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Multiline = true;
            textBox.ScrollBars = ScrollBars.Vertical;
            textBox.Height = height;
            table.Controls.Add(textBox, column, row);
            table.SetColumnSpan(textBox, columnSpan);
        }

        private void AddComboBoxToTable(TableLayoutPanel table, string name, int column, int row, string[] items)
        {
            ComboBox comboBox = new ComboBox();
            comboBox.Name = name;
            comboBox.Font = normalFont;
            comboBox.Dock = DockStyle.Fill;
            comboBox.Margin = new Padding(5);
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.Items.AddRange(items);
            if (items.Length > 0)
                comboBox.SelectedIndex = 0;
            table.Controls.Add(comboBox, column, row);
        }

        private void AddDateTimePickerToTable(TableLayoutPanel table, string name, int column, int row)
        {
            DateTimePicker dateTimePicker = new DateTimePicker();
            dateTimePicker.Name = name;
            dateTimePicker.Font = normalFont;
            dateTimePicker.Dock = DockStyle.Fill;
            dateTimePicker.Margin = new Padding(5);
            dateTimePicker.Format = DateTimePickerFormat.Short;
            dateTimePicker.RightToLeftLayout = true;
            table.Controls.Add(dateTimePicker, column, row);
        }

        private void AddNumericUpDownToTable(TableLayoutPanel table, string name, int column, int row, int min, int max)
        {
            NumericUpDown numericUpDown = new NumericUpDown();
            numericUpDown.Name = name;
            numericUpDown.Font = normalFont;
            numericUpDown.Dock = DockStyle.Fill;
            numericUpDown.Margin = new Padding(5);
            numericUpDown.Minimum = min;
            numericUpDown.Maximum = max;
            table.Controls.Add(numericUpDown, column, row);
        }

        private string[] GetProvinces()
        {
            return new string[]
            {
                "آذربایجان شرقی", "آذربایجان غربی", "اردبیل", "اصفهان",
                "البرز", "ایلام", "بوشهر", "تهران", "چهارمحال و بختیاری",
                "خراسان جنوبی", "خراسان رضوی", "خراسان شمالی", "خوزستان",
                "زنجان", "سمنان", "سیستان و بلوچستان", "فارس", "قزوین",
                "قم", "کردستان", "کرمان", "کرمانشاه", "کهگیلویه و بویراحمد",
                "گلستان", "گیلان", "لرستان", "مازندران", "مرکزی",
                "هرمزگان", "همدان", "یزد"
            };
        }

        private List<Department> GetDepartments()
        {
            return _lookupRepository.GetDepartments();
        }

        private List<Accountant> GetManagers()
        {
            return _lookupRepository.GetManagers();
        }

        private List<Accountant> GetAccountants()
        {
            return _lookupRepository.GetAllAccountants();
        }

        private void InitializeEvents()
        {
            // رویدادهای TextBox های مالی برای فرمت کردن
            AttachCurrencyFormatting("txtBaseSalary");
            AttachCurrencyFormatting("txtNetSalary");
            AttachCurrencyFormatting("txtGrossSalary");
            AttachCurrencyFormatting("txtInsuranceBase");
            AttachCurrencyFormatting("txtTaxBase");
            AttachCurrencyFormatting("txtHousingLoan");
            AttachCurrencyFormatting("txtOtherLoans");
            AttachCurrencyFormatting("txtMaxApprovalAmount");
        }

        private void AttachCurrencyFormatting(string controlName)
        {
            Control control = FindControlRecursive(this, controlName);
            if (control is TextBox textBox)
            {
                textBox.TextChanged += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(textBox.Text))
                    {
                        string text = textBox.Text.Replace(",", "");
                        if (long.TryParse(text, out long value))
                        {
                            textBox.Text = value.ToString("N0");
                            textBox.SelectionStart = textBox.Text.Length;
                        }
                    }
                };
            }
        }

        private Control FindControlRecursive(Control parent, string name)
        {
            if (parent.Name == name)
                return parent;

            foreach (Control child in parent.Controls)
            {
                Control found = FindControlRecursive(child, name);
                if (found != null)
                    return found;
            }

            return null;
        }

        private void LoadAccountantData()
        {
            if (_currentAccountant == null) return;

            try
            {
                // اطلاعات شخصی
                SetControlValue("txtEmployeeCode", _currentAccountant.EmployeeCode);
                SetControlValue("txtNationalID", _currentAccountant.NationalID);
                SetControlValue("txtFirstName", _currentAccountant.FirstName);
                SetControlValue("txtLastName", _currentAccountant.LastName);
                SetControlValue("txtFatherName", _currentAccountant.FatherName);
                SetComboBoxValue("cmbGender", _currentAccountant.Gender == 'M' ? "مرد" : "زن");
                SetDateTimePickerValue("dtpBirthDate", _currentAccountant.BirthDate);
                SetComboBoxValue("cmbMaritalStatus", GetMaritalStatusText(_currentAccountant.MaritalStatus));
                SetNumericUpDownValue("nudChildren", _currentAccountant.NumberOfChildren ?? 0);
                SetNumericUpDownValue("nudDependents", _currentAccountant.DependentsCount ?? 0);
                SetControlValue("txtInsuranceID", _currentAccountant.InsuranceID);

                // اطلاعات شغلی
                SetComboBoxValue("cmbDepartment", _currentAccountant.DepartmentID.ToString());
                SetControlValue("txtPosition", _currentAccountant.Position);
                SetControlValue("txtJobTitle", _currentAccountant.JobTitle);
                SetComboBoxValue("cmbJobLevel", _currentAccountant.JobLevel.ToString());
                SetComboBoxValue("cmbEmploymentType", _currentAccountant.EmploymentType);
                SetDateTimePickerValue("dtpHireDate", _currentAccountant.HireDate);
                SetControlValue("txtCostCenter", _currentAccountant.CostCenterCode);

                // اطلاعات مالی
                SetControlValue("txtBaseSalary", _currentAccountant.BaseSalary.ToString("N0"));
                SetControlValue("txtNetSalary", _currentAccountant.NetSalary?.ToString("N0"));
                SetControlValue("txtGrossSalary", _currentAccountant.GrossSalary?.ToString("N0"));
                SetComboBoxValue("cmbSalaryCurrency", _currentAccountant.SalaryCurrency ?? "IRR");
                SetControlValue("txtBankAccountNo", _currentAccountant.BankAccountNo);
                SetControlValue("txtBankName", _currentAccountant.BankName);
                SetControlValue("txtBankBranch", _currentAccountant.BankBranch);
                SetComboBoxValue("cmbAccountType", _currentAccountant.BankAccountType ?? "جاری");

                // اطلاعات تماس
                SetControlValue("txtMobile", _currentAccountant.Mobile);
                SetControlValue("txtWorkEmail", _currentAccountant.WorkEmail);
                SetControlValue("txtPersonalEmail", _currentAccountant.PersonalEmail);
                SetControlValue("txtWorkAddress", _currentAccountant.WorkAddress);
                SetControlValue("txtHomeAddress", _currentAccountant.HomeAddress);

                // اطلاعات سیستم
                SetControlValue("txtSystemUsername", _currentAccountant.SystemUsername);
                SetCheckBoxValue("chkIsActive", _currentAccountant.IsActive ?? true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بارگیری اطلاعات: {ex.Message}", "خطا",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetControlValue(string controlName, string value)
        {
            Control control = FindControlRecursive(this, controlName);
            if (control != null && value != null)
            {
                if (control is TextBox textBox)
                    textBox.Text = value;
                else if (control is ComboBox comboBox)
                    comboBox.Text = value;
            }
        }

        private void SetComboBoxValue(string controlName, string valueStr)
        {
            try
            {
                Int32 value = Convert.ToInt32(valueStr);
                Control control = FindControlRecursive(this, controlName);
                if (control is ComboBox comboBox)
                {
                    // جستجو بر اساس Value (ID)
                    for (int i = 0; i < comboBox.Items.Count; i++)
                    {
                        dynamic item = comboBox.Items[i];
                        if (item.GetType().GetProperty("ID") != null)
                        {
                            if ((int)item.ID == value)
                            {
                                comboBox.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void SetDateTimePickerValue(string controlName, DateTime value)
        {
            Control control = FindControlRecursive(this, controlName);
            if (control is DateTimePicker dateTimePicker)
                dateTimePicker.Value = value;
        }

        private void SetNumericUpDownValue(string controlName, decimal value)
        {
            Control control = FindControlRecursive(this, controlName);
            if (control is NumericUpDown numericUpDown)
                numericUpDown.Value = value;
        }

        private void SetCheckBoxValue(string controlName, bool value)
        {
            Control control = FindControlRecursive(this, controlName);
            if (control is CheckBox checkBox)
                checkBox.Checked = value;
        }

        private string GetMaritalStatusText(char status)
        {
            switch (status)
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
                    return "مجرد";
            }
        }

        private char GetMaritalStatusChar(string text)
        {
            switch (text)
            {
                case "مجرد":
                    return 'S';
                case "متاهل":
                    return 'M';
                case "مطلقه":
                    return 'D';
                case "همسر فوت شده":
                    return 'W';
                default:
                    return 'S';
            }
        }

        // رویدادهای دکمه‌ها
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    Accountant accountant = CollectFormData();

                    bool result;
                    if (_isEditMode)
                    {
                        result = _accountantRepository.UpdateAccountant(accountant);
                        MessageBox.Show("اطلاعات حسابدار با موفقیت به‌روزرسانی شد.", "موفقیت",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        result = _accountantRepository.InsertAccountant(accountant);
                        MessageBox.Show("حسابدار جدید با موفقیت ثبت شد.", "موفقیت",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    if (result)
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"خطا در ذخیره اطلاعات: {ex.Message}", "خطا",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("آیا از پاک کردن تمام فیلدهای فرم مطمئن هستید؟", "تأیید",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ClearAllControls(this);
            }
        }

        private void BtnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "تصاویر|*.jpg;*.jpeg;*.png|همه فایل‌ها|*.*";
            openFileDialog.Title = "انتخاب عکس حسابدار";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _photoPath = openFileDialog.FileName;
                MessageBox.Show("عکس با موفقیت انتخاب شد.", "موفقیت",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private bool ValidateForm()
        {
            // اعتبارسنجی فیلدهای ضروری
            if (string.IsNullOrWhiteSpace(GetControlValue("txtEmployeeCode")))
            {
                MessageBox.Show("کد پرسنلی الزامی است.", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tabControl.SelectedIndex = 0;
                return false;
            }

            if (string.IsNullOrWhiteSpace(GetControlValue("txtNationalID")))
            {
                MessageBox.Show("کد ملی الزامی است.", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tabControl.SelectedIndex = 0;
                return false;
            }

            if (string.IsNullOrWhiteSpace(GetControlValue("txtFirstName")) ||
                string.IsNullOrWhiteSpace(GetControlValue("txtLastName")))
            {
                MessageBox.Show("نام و نام خانوادگی الزامی است.", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tabControl.SelectedIndex = 0;
                return false;
            }

            if (string.IsNullOrWhiteSpace(GetControlValue("txtMobile")))
            {
                MessageBox.Show("شماره موبایل الزامی است.", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tabControl.SelectedIndex = 3;
                return false;
            }

            if (string.IsNullOrWhiteSpace(GetControlValue("txtWorkEmail")))
            {
                MessageBox.Show("ایمیل کاری الزامی است.", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tabControl.SelectedIndex = 3;
                return false;
            }

            if (string.IsNullOrWhiteSpace(GetControlValue("txtBankAccountNo")))
            {
                MessageBox.Show("شماره حساب بانکی الزامی است.", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tabControl.SelectedIndex = 2;
                return false;
            }

            return true;
        }

        private string GetControlValue(string controlName)
        {
            Control control = FindControlRecursive(this, controlName);
            if (control != null)
            {
                if (control is TextBox textBox)
                    return textBox.Text;
                else if (control is ComboBox comboBox)
                    return comboBox.Text;
                else if (control is DateTimePicker dateTimePicker)
                    return dateTimePicker.Value.ToString();
                else if (control is NumericUpDown numericUpDown)
                    return numericUpDown.Value.ToString();
                else if (control is CheckBox checkBox)
                    return checkBox.Checked.ToString();
            }
            return string.Empty;
        }

        private Accountant CollectFormData()
        {
            Accountant accountant = new Accountant();

            if (_isEditMode)
            {
                accountant.AccountantID = _currentAccountant.AccountantID;
            }

            // اطلاعات شخصی
            accountant.EmployeeCode = GetControlValue("txtEmployeeCode");
            accountant.NationalID = GetControlValue("txtNationalID");
            accountant.FirstName = GetControlValue("txtFirstName");
            accountant.LastName = GetControlValue("txtLastName");
            accountant.FatherName = GetControlValue("txtFatherName");
            accountant.Gender = GetComboBoxValue("cmbGender") == "مرد" ? 'M' : 'F';
            accountant.BirthDate = GetDateTimePickerValue("dtpBirthDate");
            accountant.MaritalStatus = GetMaritalStatusChar(GetComboBoxValue("cmbMaritalStatus"));
            accountant.NumberOfChildren = (byte?)GetNumericUpDownValue("nudChildren");
            accountant.DependentsCount = (byte?)GetNumericUpDownValue("nudDependents");
            accountant.InsuranceID = GetControlValue("txtInsuranceID");

            // اطلاعات شغلی
            accountant.DepartmentID = int.Parse(GetComboBoxValue("cmbDepartment"));
            accountant.Position = GetControlValue("txtPosition");
            accountant.JobTitle = GetControlValue("txtJobTitle");
            accountant.JobLevel = byte.Parse(GetComboBoxValue("cmbJobLevel"));
            accountant.EmploymentType = GetComboBoxValue("cmbEmploymentType");
            accountant.HireDate = GetDateTimePickerValue("dtpHireDate");
            accountant.CostCenterCode = GetControlValue("txtCostCenter");

            // اطلاعات مالی
            accountant.BaseSalary = decimal.Parse(GetControlValue("txtBaseSalary").Replace(",", ""));
            accountant.NetSalary = !string.IsNullOrWhiteSpace(GetControlValue("txtNetSalary")) ?
                decimal.Parse(GetControlValue("txtNetSalary").Replace(",", "")) : (decimal?)null;
            accountant.GrossSalary = !string.IsNullOrWhiteSpace(GetControlValue("txtGrossSalary")) ?
                decimal.Parse(GetControlValue("txtGrossSalary").Replace(",", "")) : (decimal?)null;
            accountant.SalaryCurrency = GetComboBoxValue("cmbSalaryCurrency").Split(' ')[0];
            accountant.BankAccountNo = GetControlValue("txtBankAccountNo");
            accountant.BankName = GetControlValue("txtBankName");
            accountant.BankBranch = GetControlValue("txtBankBranch");
            accountant.BankAccountType = GetComboBoxValue("cmbAccountType");

            // اطلاعات تماس
            accountant.Mobile = GetControlValue("txtMobile");
            accountant.WorkEmail = GetControlValue("txtWorkEmail");
            accountant.PersonalEmail = GetControlValue("txtPersonalEmail");
            accountant.WorkAddress = GetControlValue("txtWorkAddress");
            accountant.HomeAddress = GetControlValue("txtHomeAddress");

            // اطلاعات سیستم
            accountant.SystemUsername = GetControlValue("txtSystemUsername");
            accountant.IsActive = GetCheckBoxValue("chkIsActive");

            // تاریخ‌های ایجاد و ویرایش
            if (_isEditMode)
            {
                accountant.ModifiedDate = DateTime.Now;
            }
            else
            {
                accountant.CreatedDate = DateTime.Now;
                accountant.ModifiedDate = DateTime.Now;
                accountant.GUID = Guid.NewGuid();
            }

            return accountant;
        }

        private string GetComboBoxValue(string controlName)
        {
            Control control = FindControlRecursive(this, controlName);
            if (control is ComboBox comboBox)
                return comboBox.Text;
            return string.Empty;
        }

        private DateTime GetDateTimePickerValue(string controlName)
        {
            Control control = FindControlRecursive(this, controlName);
            if (control is DateTimePicker dateTimePicker)
                return dateTimePicker.Value;
            return DateTime.Now;
        }

        private decimal GetNumericUpDownValue(string controlName)
        {
            Control control = FindControlRecursive(this, controlName);
            if (control is NumericUpDown numericUpDown)
                return numericUpDown.Value;
            return 0;
        }

        private bool GetCheckBoxValue(string controlName)
        {
            Control control = FindControlRecursive(this, controlName);
            if (control is CheckBox checkBox)
                return checkBox.Checked;
            return false;
        }

        private void ClearAllControls(Control control)
        {
            foreach (Control c in control.Controls)
            {
                if (c is TextBox textBox)
                    textBox.Clear();
                else if (c is ComboBox comboBox)
                    comboBox.SelectedIndex = -1;
                else if (c is DateTimePicker dateTimePicker)
                    dateTimePicker.Value = DateTime.Now;
                else if (c is NumericUpDown numericUpDown)
                    numericUpDown.Value = numericUpDown.Minimum;
                else if (c is CheckBox checkBox)
                    checkBox.Checked = false;
                else if (c.HasChildren)
                    ClearAllControls(c);
            }
        }

    }
}