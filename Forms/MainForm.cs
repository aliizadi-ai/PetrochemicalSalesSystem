using PetrochemicalAccountantSystem.Forms;
using PetrochemicalSalesSystem.Data;
using PetrochemicalSalesSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;


namespace PetrochemicalSalesSystem.Forms
{
    public partial class MainForm : Form
    {
        // متغیرهای طراحی
        private Font titleFont = new Font("B Nazanin", 16, FontStyle.Bold);
        private Font normalFont = new Font("B Nazanin", 11);
        private Font smallFont = new Font("B Nazanin", 10);

        private Color primaryColor = Color.FromArgb(0, 102, 51); // سبز پتروشیمی
        private Color secondaryColor = Color.FromArgb(34, 139, 34);
        private Color backgroundColor = Color.FromArgb(240, 242, 245);

        private Panel topPanel;
        private Panel searchPanel;
        private Panel buttonPanel;
        private DataGridView dgvAccountants;
        private StatusStrip statusStrip;
        // متغیرهای قبلی...
        private AccountantRepository _accountantRepository;
        private List<Accountant> _accountants;
        public MainForm()
        {
            InitializeComponent();
            _accountantRepository = new AccountantRepository();
            InitializeDesign();
            LoadAccountants();
            InitializeEvents();
        }

        // ========== رویدادهای دکمه‌ها ==========

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            OpenAccountantEditForm(0); // 0 به معنای ایجاد جدید
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvAccountants.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvAccountants.SelectedRows[0];
                long accountantId = Convert.ToInt64(selectedRow.Cells["AccountantID"].Value);
                OpenAccountantEditForm(accountantId);
            }
            else
            {
                MessageBox.Show("لطفاً یک حسابدار را از لیست انتخاب کنید.", "هشدار",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void OpenAccountantEditForm(long accountantId)
        {
            try
            {
                if (accountantId == 0)
                {
                    // حالت ایجاد جدید
                    AccountantEditForm editForm = new AccountantEditForm();
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadAccountants(); // بارگذاری مجدد لیست
                    }
                }
                else
                {
                    // حالت ویرایش
                    AccountantEditForm editForm = new AccountantEditForm(accountantId);
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadAccountants(); // بارگذاری مجدد لیست
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در باز کردن فرم: {ex.Message}", "خطا",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvAccountants.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvAccountants.SelectedRows[0];
                long accountantId = Convert.ToInt64(selectedRow.Cells["AccountantID"].Value);
                string fullName = selectedRow.Cells["FullName"].Value?.ToString() ?? "ناشناس";

                // تأیید حذف
                DialogResult result = MessageBox.Show(
                    $"آیا از حذف حسابدار '{fullName}' مطمئن هستید؟\nاین عمل غیرقابل بازگشت است!",
                    "تأیید حذف",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    DeleteAccountant(accountantId);
                }
            }
            else
            {
                MessageBox.Show("لطفاً یک حسابدار را از لیست انتخاب کنید.", "هشدار",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DeleteAccountant(long accountantId)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                bool success = _accountantRepository.DeleteAccountant(accountantId);

                if (success)
                {
                    MessageBox.Show("حسابدار با موفقیت حذف شد.", "موفقیت",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadAccountants(); // بارگذاری مجدد لیست
                }
                else
                {
                    MessageBox.Show("خطا در حذف حسابدار.", "خطا",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در حذف حسابدار: {ex.Message}", "خطا",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadAccountants();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            ExportToExcel();
        }

        private void BtnReport_Click(object sender, EventArgs e)
        {
            OpenReportForm();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void TxtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                ApplyFilters();
                e.Handled = true;
            }
        }

        private void CmbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void DgvAccountants_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // اگر روی دکمه "مشاهده جزئیات" کلیک شد
                if (dgvAccountants.Columns[e.ColumnIndex].HeaderText == "عملیات" ||
                    dgvAccountants.Columns[e.ColumnIndex].HeaderText == "مشاهده جزئیات")
                {
                    long accountantId = Convert.ToInt64(dgvAccountants.Rows[e.RowIndex].Cells["AccountantID"].Value);
                    OpenAccountantDetailForm(accountantId);
                }
            }
        }

        private void DgvAccountants_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                long accountantId = Convert.ToInt64(dgvAccountants.Rows[e.RowIndex].Cells["AccountantID"].Value);
                OpenAccountantEditForm(accountantId);
            }
        }

        private void DgvAccountants_SelectionChanged(object sender, EventArgs e)
        {
            // به‌روزرسانی وضعیت دکمه‌ها بر اساس انتخاب
            bool hasSelection = dgvAccountants.SelectedRows.Count > 0;

            Button editButton = buttonPanel.Controls.OfType<Button>()
                .FirstOrDefault(b => b.Text.Contains("ویرایش"));
            Button deleteButton = buttonPanel.Controls.OfType<Button>()
                .FirstOrDefault(b => b.Text.Contains("حذف"));

            if (editButton != null) editButton.Enabled = hasSelection;
            if (deleteButton != null) deleteButton.Enabled = hasSelection;
        }

        private void OpenAccountantDetailForm(long accountantId)
        {
            try
            {
                AccountantDetailForm detailForm = new AccountantDetailForm(accountantId);
                detailForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در باز کردن فرم جزئیات: {ex.Message}", "خطا",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenReportForm()
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

        private void ExportToExcel()
        {
            if (dgvAccountants.Rows.Count == 0)
            {
                MessageBox.Show("داده‌ای برای خروجی وجود ندارد.", "هشدار",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "فایل Excel|*.xlsx";
            saveDialog.Title = "ذخیره به عنوان فایل Excel";
            saveDialog.FileName = $"حسابداران_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;

                    // استفاده از EPPlus برای ایجاد فایل Excel
                    // ابتدا باید NuGet Package EPPlus را نصب کنید
                    ExportToExcelUsingEPPlus(saveDialog.FileName);

                    MessageBox.Show("خروجی Excel با موفقیت ایجاد شد.", "موفقیت",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"خطا در ایجاد خروجی Excel: {ex.Message}", "خطا",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void ExportToExcelUsingEPPlus(string filePath)
        {
            // اگر EPPlus نصب نیست، از روش ساده‌تر استفاده کنید
            ExportToExcelSimple(filePath);
        }

        private void ExportToExcelSimple(string filePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
                {
                    // نوشتن هدرها
                    List<string> headers = new List<string>();
                    foreach (DataGridViewColumn column in dgvAccountants.Columns)
                    {
                        if (column.Visible && !column.HeaderText.Contains("عملیات"))
                        {
                            headers.Add(column.HeaderText);
                        }
                    }
                    writer.WriteLine(string.Join(",", headers));

                    // نوشتن داده‌ها
                    foreach (DataGridViewRow row in dgvAccountants.Rows)
                    {
                        if (row.IsNewRow) continue;

                        List<string> cells = new List<string>();
                        foreach (DataGridViewColumn column in dgvAccountants.Columns)
                        {
                            if (column.Visible && !column.HeaderText.Contains("عملیات"))
                            {
                                object value = row.Cells[column.Index].Value;
                                string cellValue = value?.ToString() ?? "";
                                // فرار از کاما
                                if (cellValue.Contains(","))
                                {
                                    cellValue = $"\"{cellValue}\"";
                                }
                                cells.Add(cellValue);
                            }
                        }
                        writer.WriteLine(string.Join(",", cells));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"خطا در ذخیره فایل: {ex.Message}");
            }
        }




        // تابع InitializeDesign را قبلاً داشتید، حالا رویدادهای دکمه‌ها را اضافه می‌کنیم

        private void InitializeEvents()
        {
            // اگر دکمه‌ها را در Designer ایجاد کرده‌اید، رویدادها را به آنها متصل کنید
            // یا اگر کد زیر را دارید:

            // دکمه افزودن
            Button addButton = buttonPanel.Controls.OfType<Button>()
                .FirstOrDefault(b => b.Text.Contains("افزودن"));
            if (addButton != null)
            {
                addButton.Click += BtnAdd_Click;
            }

            // دکمه ویرایش
            Button editButton = buttonPanel.Controls.OfType<Button>()
                .FirstOrDefault(b => b.Text.Contains("ویرایش"));
            if (editButton != null)
            {
                editButton.Click += BtnEdit_Click;
            }

            // دکمه حذف
            Button deleteButton = buttonPanel.Controls.OfType<Button>()
                .FirstOrDefault(b => b.Text.Contains("حذف"));
            if (deleteButton != null)
            {
                deleteButton.Click += BtnDelete_Click;
            }


            // دکمه خروجی Excel
            Button exportButton = buttonPanel.Controls.OfType<Button>()
                .FirstOrDefault(b => b.Text.Contains("Excel"));
            if (exportButton != null)
            {
                exportButton.Click += BtnExport_Click;
            }

            // دکمه گزارش‌گیری
            Button reportButton = buttonPanel.Controls.OfType<Button>()
                .FirstOrDefault(b => b.Text.Contains("گزارش"));
            if (reportButton != null)
            {
                reportButton.Click += BtnReport_Click;
            }

            // دکمه جستجو
            Button searchButton = searchPanel.Controls.OfType<Button>()
                .FirstOrDefault(b => b.Text.Contains("جستجو"));
            if (searchButton != null)
            {
                searchButton.Click += BtnSearch_Click;
            }

            // TextBox جستجو
            TextBox searchTextBox = searchPanel.Controls.OfType<TextBox>().FirstOrDefault();
            if (searchTextBox != null)
            {
                searchTextBox.KeyPress += TxtSearch_KeyPress;
            }

            // ComboBox فیلتر وضعیت
            ComboBox filterCombo = searchPanel.Controls.OfType<ComboBox>().FirstOrDefault();
            if (filterCombo != null)
            {
                filterCombo.SelectedIndexChanged += CmbFilter_SelectedIndexChanged;
            }

            // دکمه بروزرسانی
            Button refreshButton = buttonPanel.Controls.OfType<Button>()
                .FirstOrDefault(b => b.Text.Contains("بروزرسانی"));
            if (refreshButton != null)
            {
                refreshButton.Click += (s, e) =>
                {
                    // پاک کردن TextBox جستجو
                    if (searchTextBox != null)
                        searchTextBox.Text = "";

                    // بازنشانی ComboBox وضعیت
                    if (filterCombo != null)
                        filterCombo.SelectedIndex = 0;

                    // بارگذاری مجدد
                    LoadAccountants();
                };
            }



            // رویداد DataGridView
            dgvAccountants.CellContentClick += DgvAccountants_CellContentClick;
            dgvAccountants.CellDoubleClick += DgvAccountants_CellDoubleClick;
            dgvAccountants.SelectionChanged += DgvAccountants_SelectionChanged;
        }

        private void DataGripShowAllRows()
        {

            InitialDataGridView();
        }

        private void ApplyFilters()
        {
            if (_accountants == null) return;

            var filteredList = _accountants.AsEnumerable();

            // فیلتر جستجو
            TextBox searchTextBox = searchPanel.Controls.OfType<TextBox>().FirstOrDefault();
            if (searchTextBox != null && !string.IsNullOrWhiteSpace(searchTextBox.Text))
            {
                string searchText = searchTextBox.Text.ToLower();
                filteredList = filteredList.Where(a =>
                    (a.EmployeeCode?.ToLower().Contains(searchText) ?? false) ||
                    (a.NationalID?.Contains(searchText) ?? false) ||
                    (a.FirstName?.ToLower().Contains(searchText) ?? false) ||
                    (a.LastName?.ToLower().Contains(searchText) ?? false) ||
                    (a.FullName?.ToLower().Contains(searchText) ?? false) ||
                    (a.Mobile?.Contains(searchText) ?? false) ||
                    (a.Position?.ToLower().Contains(searchText) ?? false));
            }

            // فیلتر وضعیت
            ComboBox filterCombo = searchPanel.Controls.OfType<ComboBox>().FirstOrDefault();
            if (filterCombo != null && filterCombo.SelectedIndex > 0)
            {
                bool isActive = filterCombo.SelectedIndex == 1;
                filteredList = filteredList.Where(a => a.IsActive == isActive);
            }

            // نمایش در DataGridView
            dgvAccountants.DataSource = filteredList.ToList();
            FormatDataGridView();

            // به‌روزرسانی نوار وضعیت
            UpdateStatusBar(filteredList.Count());
        }

        private void InitialDataGridView()
        {

            // تنظیم فرمت ستون‌ها
            foreach (DataGridViewColumn column in dgvAccountants.Columns)
            {

                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }


        }

        private void FormatDataGridView()
        {
            if (dgvAccountants.Columns.Count == 0) return;

            // تنظیمات ظاهری
            dgvAccountants.ColumnHeadersDefaultCellStyle.BackColor = primaryColor;
            dgvAccountants.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvAccountants.ColumnHeadersDefaultCellStyle.Font = new Font("B Nazanin", 11, FontStyle.Bold);
            dgvAccountants.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // راست‌چین کردن متن برای فارسی
            dgvAccountants.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvAccountants.DefaultCellStyle.Font = new Font("B Nazanin", 10);

            // سطرهای متناوب
            dgvAccountants.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);

            // تنظیم AutoSizeColumnsMode برای نمایش بهتر
            dgvAccountants.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvAccountants.AllowUserToOrderColumns = true;
            dgvAccountants.AllowUserToResizeColumns = true;
        }

        private void LoadAccountants()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // دریافت همه حسابداران
                _accountants = _accountantRepository.GetAllAccountants();

                // *** تغییر: ابتدا همه داده‌ها را نمایش بده، سپس فیلترها را اعمال کن ***
                DisplayAllAccountants();

                // بعداً اگر جستجو انجام شد، ApplyFilters() فراخوانی می‌شود
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بارگیری اطلاعات: {ex.Message}", "خطا",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void DisplayAllAccountants()
        {
            if (_accountants == null) return;

            // ابتدا ستون‌ها را تنظیم کنید
            ConfigureGridViewColumns();

            // نمایش تمام حسابداران در DataGridView
            dgvAccountants.DataSource = _accountants.ToList();

            // فرمت‌بندی DataGridView
            FormatDataGridView();

            // *** تنظیم عرض ستون‌ها ***
            AdjustColumnWidths();

            // به‌روزرسانی نوار وضعیت
            UpdateStatusBar(_accountants.Count);
        }

        private void TestDatabaseConnection()
        {
            try
            {
                string connectionString = "Server=.;Database=PetrochemicalSalesDB;Integrated Security=true;";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    MessageBox.Show("اتصال به دیتابیس موفق بود!", "موفقیت");

                    // تست کوئری
                    string testQuery = "SELECT COUNT(*) FROM Accountants";
                    SqlCommand cmd = new SqlCommand(testQuery, conn);
                    int count = (int)cmd.ExecuteScalar();
                    MessageBox.Show($"تعداد حسابداران در دیتابیس: {count}");

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در اتصال به دیتابیس: {ex.Message}", "خطا");
            }
        }

        private void InitializeDesign()
        {
            // تنظیمات اصلی فرم
            this.Text = "سیستم مدیریت حسابداران - مجتمع پتروشیمی";
            this.Size = new Size(1300, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.BackColor = backgroundColor;
            this.Font = normalFont;

            // ایجاد کنترل‌ها
            CreateTopPanel();
            CreateSearchPanel();
            CreateButtonPanel();
            CreateDataGridView();
            CreateStatusStrip();

            // *** تغییر: فقط LoadAccountants را فراخوانی کن ***
            LoadAccountants();
        }

        private void CreateTopPanel()
        {
            topPanel = new Panel();
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 100;
            topPanel.BackColor = primaryColor;

            // عنوان اصلی
            Label titleLabel = new Label();
            titleLabel.Text = "سیستم مدیریت حسابداران پتروشیمی";
            titleLabel.Font = titleFont;
            titleLabel.ForeColor = Color.White;
            titleLabel.AutoSize = false;
            titleLabel.Size = new Size(800, 40);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Location = new Point(250, 20);

            // زیرعنوان
            Label subTitleLabel = new Label();
            subTitleLabel.Text = "مدیریت اطلاعات حسابداری و منابع انسانی";
            subTitleLabel.Font = new Font("B Nazanin", 12);
            subTitleLabel.ForeColor = Color.LightGray;
            subTitleLabel.AutoSize = false;
            subTitleLabel.Size = new Size(600, 30);
            subTitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            subTitleLabel.Location = new Point(350, 60);

            // لوگو (شبیه‌سازی با Panel)
            Panel logoPanel = new Panel();
            logoPanel.Size = new Size(80, 80);
            logoPanel.Location = new Point(20, 10);
            logoPanel.BackColor = Color.White;
            logoPanel.BorderStyle = BorderStyle.FixedSingle;

            Label logoText = new Label();
            logoText.Text = "P\nA\nS";
            logoText.Font = new Font("Arial", 14, FontStyle.Bold);
            logoText.ForeColor = primaryColor;
            logoText.TextAlign = ContentAlignment.MiddleCenter;
            logoText.Dock = DockStyle.Fill;
            logoPanel.Controls.Add(logoText);

            topPanel.Controls.Add(titleLabel);
            topPanel.Controls.Add(subTitleLabel);
            topPanel.Controls.Add(logoPanel);

            this.Controls.Add(topPanel);
        }

        private void CreateSearchPanel()
        {
            searchPanel = new Panel();
            searchPanel.Dock = DockStyle.Top;
            searchPanel.Height = 70;
            searchPanel.BackColor = Color.White;
            searchPanel.BorderStyle = BorderStyle.FixedSingle;

            // برچسب جستجو
            Label searchLabel = new Label();
            searchLabel.Text = "🔍 جستجو:";
            searchLabel.Location = new Point(1100, 25);
            searchLabel.Size = new Size(80, 30);
            searchLabel.TextAlign = ContentAlignment.MiddleLeft;

            // TextBox جستجو
            TextBox searchTextBox = new TextBox();
            searchTextBox.Location = new Point(800, 25);
            searchTextBox.Size = new Size(280, 30);
            searchTextBox.Font = normalFont;
            searchTextBox.Text = "جستجو بر اساس نام، کد پرسنلی یا کد ملی...";

            // دکمه جستجو
            Button searchButton = CreateStyledButton("جستجو", secondaryColor, Color.White);
            searchButton.Location = new Point(700, 20);
            searchButton.Size = new Size(90, 35);

            // فیلتر وضعیت
            Label filterLabel = new Label();
            filterLabel.Text = "وضعیت:";
            filterLabel.Location = new Point(600, 25);
            filterLabel.Size = new Size(50, 30);
            filterLabel.TextAlign = ContentAlignment.MiddleLeft;

            ComboBox filterCombo = new ComboBox();
            filterCombo.Location = new Point(480, 25);
            filterCombo.Size = new Size(110, 30);
            filterCombo.Items.AddRange(new string[] { "همه", "فعال", "غیرفعال" });
            filterCombo.SelectedIndex = 0;

            searchPanel.Controls.Add(searchLabel);
            searchPanel.Controls.Add(searchTextBox);
            searchPanel.Controls.Add(searchButton);
            searchPanel.Controls.Add(filterLabel);
            searchPanel.Controls.Add(filterCombo);

            this.Controls.Add(searchPanel);
        }

        private void CreateButtonPanel()
        {
            buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Top;
            buttonPanel.Height = 60;
            buttonPanel.BackColor = Color.FromArgb(248, 249, 250);
            buttonPanel.BorderStyle = BorderStyle.FixedSingle;

            // دکمه‌های عملیاتی
            Button addButton = CreateStyledButton("➕ افزودن حسابدار جدید", primaryColor, Color.White);
            addButton.Location = new Point(20, 10);
            addButton.Size = new Size(150, 40);
            addButton.Font = new Font("B Nazanin", 11, FontStyle.Bold);

            Button editButton = CreateStyledButton("✏️ ویرایش", Color.FromArgb(13, 110, 253), Color.White);
            editButton.Location = new Point(180, 10);
            editButton.Size = new Size(100, 40);

            Button deleteButton = CreateStyledButton("🗑️ حذف", Color.FromArgb(220, 53, 69), Color.White);
            deleteButton.Location = new Point(290, 10);
            deleteButton.Size = new Size(100, 40);

            Button refreshButton = CreateStyledButton("🔄 بروزرسانی", Color.FromArgb(108, 117, 125), Color.White);
            refreshButton.Location = new Point(400, 10);
            refreshButton.Size = new Size(120, 40);

            Button exportButton = CreateStyledButton("📊 خروجی Excel", Color.FromArgb(25, 135, 84), Color.White);
            exportButton.Location = new Point(530, 10);
            exportButton.Size = new Size(120, 40);

            Button reportButton = CreateStyledButton("📈 گزارش‌گیری", Color.FromArgb(111, 66, 193), Color.White);
            reportButton.Location = new Point(660, 10);
            reportButton.Size = new Size(120, 40);

            // دکمه جدید: نمایش همه
            Button showAllButton = CreateStyledButton("👁️ نمایش همه", Color.FromArgb(13, 202, 240), Color.White);
            showAllButton.Location = new Point(790, 10);
            showAllButton.Size = new Size(120, 40);
            showAllButton.Click += (s, e) => DisplayAllAccountants();

            buttonPanel.Controls.Add(addButton);
            buttonPanel.Controls.Add(editButton);
            buttonPanel.Controls.Add(deleteButton);
            buttonPanel.Controls.Add(refreshButton);
            buttonPanel.Controls.Add(exportButton);
            buttonPanel.Controls.Add(reportButton);
            buttonPanel.Controls.Add(showAllButton); // اضافه کردن دکمه جدید


            this.Controls.Add(buttonPanel);
        }

        /*
        private void LoadAccountantDataAlternative()
        {
            string connectionString = "Server=.;Database=YourDatabase;Integrated Security=true;";
            string query = "SELECT * FROM Accountant";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dt = new DataTable();

                try
                {
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;

                    // تنظیم عناوین فارسی برای ستون‌ها
                    SetPersianColumnHeaders();

                    // مخفی کردن ستون‌های غیرضروری
                    HideUnnecessaryColumns();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"خطا: {ex.Message}");
                }
            }
        }
        */

        private void SetPersianColumnHeaders()
        {
            // ایجاد دیکشنری برای نگاشت نام ستون‌های انگلیسی به فارسی
            Dictionary<string, string> columnHeaders = new Dictionary<string, string>()
        {
        {"Id", "کد"},
        {"FirstName", "نام"},
        {"LastName", "نام خانوادگی"},
        {"PhoneNumber", "شماره تلفن"},
        {"Email", "ایمیل"},
        {"NationalCode", "کد ملی"},
        {"HireDate", "تاریخ استخدام"}
        // سایر ستون‌ها...
        };

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                if (columnHeaders.ContainsKey(column.DataPropertyName))
                {
                    column.HeaderText = columnHeaders[column.DataPropertyName];
                }
            }
        }

        private void HideUnnecessaryColumns()
        {
            // لیست ستون‌هایی که می‌خواهید مخفی کنید
            string[] columnsToHide = { "PasswordHash", "Salt", "CreatedDate" };

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                if (columnsToHide.Contains(column.DataPropertyName))
                {
                    column.Visible = false;
                }
            }
        }
        /*
        private void LoadAccountantDataSimple()
        {
            try
            {
                string connectionString = "Server=.;Database=PetrochemicalSalesDB;Integrated Security=true;";
                string query = "SELECT AccountantID as 'کد', FirstName as 'نام', LastName as 'نام خانوادگی', " +
                              "Mobile as 'تلفن همراه', WorkEmail as 'ایمیل' FROM Accountants";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // ساده‌ترین روش
                    dgvAccountants.DataSource = dt;

                    // تنظیمات ظاهری
                    dgvAccountants.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgvAccountants.RowHeadersVisible = false;
                    dgvAccountants.AllowUserToAddRows = false;
                    dgvAccountants.ReadOnly = true;

                    // راست‌چین کردن برای فارسی
                    dgvAccountants.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvAccountants.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                    MessageBox.Show($"داده‌ها بارگذاری شدند. تعداد: {dt.Rows.Count}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا: {ex.Message}");
            }
        }
        */

        /*
        private void LoadAccountantDataWithPersianColumns()
        {
            try
            {
                string connectionString = "Server=.;Database=PetrochemicalSalesDB;Integrated Security=true;";
                string query = "SELECT AccountantID, FirstName, LastName, Mobile, WorkEmail FROM Accountants";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // **مهم: پاک کردن ستون‌های قبلی**
                    dgvAccountants.Columns.Clear();

                    // **غیرفعال کردن تولید خودکار ستون‌ها**
                    dgvAccountants.AutoGenerateColumns = false;

                    // **فقط 5 ستون ضروری را اضافه کنید**

                    // ستون 1: کد حسابدار
                    DataGridViewTextBoxColumn colId = new DataGridViewTextBoxColumn();
                    colId.Name = "AccountantID";
                    colId.DataPropertyName = "AccountantID";
                    colId.HeaderText = "کد حسابدار";
                    colId.Width = 100;
                    colId.ReadOnly = true;
                    colId.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvAccountants.Columns.Add(colId);

                    // ستون 2: نام
                    DataGridViewTextBoxColumn colFirstName = new DataGridViewTextBoxColumn();
                    colFirstName.Name = "FirstName";
                    colFirstName.DataPropertyName = "FirstName";
                    colFirstName.HeaderText = "نام";
                    colFirstName.Width = 120;
                    colFirstName.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvAccountants.Columns.Add(colFirstName);

                    // ستون 3: نام خانوادگی
                    DataGridViewTextBoxColumn colLastName = new DataGridViewTextBoxColumn();
                    colLastName.Name = "LastName";
                    colLastName.DataPropertyName = "LastName";
                    colLastName.HeaderText = "نام خانوادگی";
                    colLastName.Width = 150;
                    colLastName.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvAccountants.Columns.Add(colLastName);

                    // ستون 4: تلفن همراه
                    DataGridViewTextBoxColumn colMobile = new DataGridViewTextBoxColumn();
                    colMobile.Name = "Mobile";
                    colMobile.DataPropertyName = "Mobile";
                    colMobile.HeaderText = "تلفن همراه";
                    colMobile.Width = 120;
                    colMobile.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvAccountants.Columns.Add(colMobile);

                    // ستون 5: ایمیل کاری
                    DataGridViewTextBoxColumn colEmail = new DataGridViewTextBoxColumn();
                    colEmail.Name = "WorkEmail";
                    colEmail.DataPropertyName = "WorkEmail";
                    colEmail.HeaderText = "ایمیل کاری";
                    colEmail.Width = 200;
                    colEmail.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvAccountants.Columns.Add(colEmail);

                    // اتصال داده‌ها
                    dgvAccountants.DataSource = dt;

                    // تنظیمات ظاهری
                    dgvAccountants.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvAccountants.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvAccountants.DefaultCellStyle.Font = new Font("B Nazanin", 10);

                    // به‌روزرسانی نوار وضعیت
                    UpdateStatusBar(dt.Rows.Count);

                    MessageBox.Show($"تعداد حسابداران: {dt.Rows.Count}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا: {ex.Message}");
            }
        }
        */

        private void UpdateStatusBar(int count)
        {
            // پیدا کردن label مربوط به تعداد رکوردها در statusStrip
            if (statusStrip != null && statusStrip.Items.Count > 2)
            {
                ToolStripStatusLabel recordLabel = (ToolStripStatusLabel)statusStrip.Items[2];
                recordLabel.Text = $"📊 تعداد حسابداران: {count}";
            }
        }

        private void AddPersianColumns()
        {
            // افزودن هر ستون با عنوان فارسی
            dgvAccountants.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "AccountantID",  // نام ستون در دیتابیس
                HeaderText = "کد",
                Name = "colId",
                Width = 80,
                ReadOnly = true
            });

            dgvAccountants.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "FirstName",
                HeaderText = "نام",
                Name = "colFirstName",
                Width = 120
            });

            dgvAccountants.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "LastName",
                HeaderText = "نام خانوادگی",
                Name = "colLastName",
                Width = 150
            });

            dgvAccountants.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Mobile",
                HeaderText = "شماره تلفن",
                Name = "colPhone",
                Width = 120
            });

            dgvAccountants.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "WorkEmail",
                HeaderText = "ایمیل",
                Name = "colEmail",
                Width = 180
            });
        }


        /*
        private void LoadAccountantData()
        {
            try
            {
                string connectionString = "Server=.;Database=PetrochemicalSalesDB;Integrated Security=true;";
                string query = "SELECT AccountantID, FirstName, LastName, Mobile, WorkEmail FROM Accountants";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // پاک کردن ستون‌های قبلی
                    dgvAccountants.Columns.Clear();

                    // غیرفعال کردن تولید خودکار ستون‌ها
                    dgvAccountants.AutoGenerateColumns = false;

                    // اضافه کردن فقط 5 ستون مورد نظر

                    // ستون 1: کد حسابدار
                    DataGridViewTextBoxColumn colId = new DataGridViewTextBoxColumn();
                    colId.Name = "AccountantID";
                    colId.DataPropertyName = "AccountantID";
                    colId.HeaderText = "کد حسابدار";
                    colId.Width = 100;
                    colId.ReadOnly = true;
                    colId.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvAccountants.Columns.Add(colId);

                    // ستون 2: نام
                    DataGridViewTextBoxColumn colFirstName = new DataGridViewTextBoxColumn();
                    colFirstName.Name = "FirstName";
                    colFirstName.DataPropertyName = "FirstName";
                    colFirstName.HeaderText = "نام";
                    colFirstName.Width = 120;
                    colFirstName.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvAccountants.Columns.Add(colFirstName);

                    // ستون 3: نام خانوادگی
                    DataGridViewTextBoxColumn colLastName = new DataGridViewTextBoxColumn();
                    colLastName.Name = "LastName";
                    colLastName.DataPropertyName = "LastName";
                    colLastName.HeaderText = "نام خانوادگی";
                    colLastName.Width = 150;
                    colLastName.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvAccountants.Columns.Add(colLastName);

                    // ستون 4: تلفن همراه
                    DataGridViewTextBoxColumn colMobile = new DataGridViewTextBoxColumn();
                    colMobile.Name = "Mobile";
                    colMobile.DataPropertyName = "Mobile";
                    colMobile.HeaderText = "تلفن همراه";
                    colMobile.Width = 120;
                    colMobile.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvAccountants.Columns.Add(colMobile);

                    // ستون 5: ایمیل کاری
                    DataGridViewTextBoxColumn colEmail = new DataGridViewTextBoxColumn();
                    colEmail.Name = "WorkEmail";
                    colEmail.DataPropertyName = "WorkEmail";
                    colEmail.HeaderText = "ایمیل کاری";
                    colEmail.Width = 200;
                    colEmail.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvAccountants.Columns.Add(colEmail);

                    // اتصال داده‌ها
                    dgvAccountants.DataSource = dt;

                    // تنظیمات ظاهری
                    dgvAccountants.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle()
                    {
                        BackColor = primaryColor,
                        ForeColor = Color.White,
                        Font = new Font("B Nazanin", 11, FontStyle.Bold),
                        Alignment = DataGridViewContentAlignment.MiddleCenter
                    };

                    dgvAccountants.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvAccountants.DefaultCellStyle.Font = new Font("B Nazanin", 10);

                    // سطرهای متناوب
                    dgvAccountants.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);

                    // به‌روزرسانی نوار وضعیت
                    UpdateStatusBar(dt.Rows.Count);

                    MessageBox.Show($"داده‌ها با موفقیت بارگذاری شدند. تعداد حسابداران: {dt.Rows.Count}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بارگذاری داده‌ها: {ex.Message}", "خطا",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        */

        private void ConfigureGridViewColumns()
        {
            // غیرفعال کردن تولید خودکار ستون‌ها
            dgvAccountants.AutoGenerateColumns = false;

            // پاک کردن ستون‌های قبلی
            dgvAccountants.Columns.Clear();

            // اضافه کردن فقط ستون‌های مورد نظر
            AddCustomColumns();
        }
        private void AddCustomColumns()
        {
            // لیست فیلدهای مورد نظر شما با عنوان فارسی
            var columns = new[]
            {
        new { Name = "EmployeeCode", Header = "کد پرسنلی", Width = 100 },
        new { Name = "NationalID", Header = "کد ملی", Width = 100 },
        new { Name = "FirstName", Header = "نام", Width = 100 },
        new { Name = "LastName", Header = "نام خانوادگی", Width = 120 },
        new { Name = "Gender", Header = "جنسیت", Width = 80 },
        new { Name = "BirthDate", Header = "تاریخ تولد", Width = 100 },
        new { Name = "MaritalStatus", Header = "وضعیت تاهل", Width = 100 },
        new { Name = "EducationLevel", Header = "سطح تحصیلات", Width = 120 },
        new { Name = "DepartmentID", Header = "کد دپارتمان", Width = 100 },
        new { Name = "Position", Header = "سمت", Width = 150 },
        new { Name = "JobLevel", Header = "سطح شغلی", Width = 100 },
        new { Name = "EmploymentType", Header = "نوع استخدام", Width = 120 },
        new { Name = "HireDate", Header = "تاریخ استخدام", Width = 100 },
        new { Name = "BaseSalary", Header = "حقوق پایه", Width = 120 },
        new { Name = "BankAccountNo", Header = "شماره حساب", Width = 150 },
        new { Name = "BankName", Header = "نام بانک", Width = 120 },
        new { Name = "BankBranch", Header = "شعبه بانک", Width = 120 },
        new { Name = "Mobile", Header = "تلفن همراه", Width = 100 },
        new { Name = "WorkEmail", Header = "ایمیل کاری", Width = 180 },
        new { Name = "SystemUsername", Header = "نام کاربری", Width = 120 },
        new { Name = "CostCenterCode", Header = "کد مرکز هزینه", Width = 120 },
        new { Name = "CreatedDate", Header = "تاریخ ایجاد", Width = 120 },
        new { Name = "ModifiedDate", Header = "تاریخ ویرایش", Width = 120 },
        new { Name = "ERPUserID", Header = "کد کاربر ERP", Width = 120 }
    };

            foreach (var col in columns)
            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.Name = col.Name;
                column.DataPropertyName = col.Name;
                column.HeaderText = col.Header;
                column.Width = col.Width;
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // تنظیم فرمت برای ستون‌های تاریخ
                if (col.Name.Contains("Date"))
                {
                    column.DefaultCellStyle.Format = "yyyy/MM/dd";
                }

                // تنظیم فرمت برای ستون حقوق
                if (col.Name == "BaseSalary")
                {
                    column.DefaultCellStyle.Format = "N0"; // فرمت عدد با جداکننده هزارگان
                }

                dgvAccountants.Columns.Add(column);
            }
        }

        private void AdjustColumnWidths()
        {
            // تنظیم عرض ستون‌ها به صورت متناسب
            int totalWidth = dgvAccountants.Width - 20; // 20 پیکسل برای اسکرول بار

            // تنظیم عرض ثابت برای برخی ستون‌ها و انعطاف‌پذیر برای بقیه
            int fixedWidthColumns = 0;

            // ستون‌هایی که عرض ثابت دارند
            var fixedColumns = new Dictionary<string, int>
    {
        {"EmployeeCode", 100},
        {"NationalID", 100},
        {"FirstName", 100},
        {"LastName", 120},
        {"Gender", 80},
        {"BirthDate", 100},
        {"MaritalStatus", 100},
        {"EducationLevel", 120},
        {"DepartmentID", 100},
        {"Position", 150},
        {"JobLevel", 100},
        {"EmploymentType", 120},
        {"HireDate", 100},
        {"BaseSalary", 120},
        {"BankAccountNo", 150},
        {"BankName", 120},
        {"BankBranch", 120},
        {"Mobile", 100},
        {"WorkEmail", 180},
        {"SystemUsername", 120},
        {"CostCenterCode", 120},
        {"CreatedDate", 120},
        {"ModifiedDate", 120},
        {"ERPUserID", 120}
    };

            // اعمال عرض ثابت
            foreach (DataGridViewColumn column in dgvAccountants.Columns)
            {
                if (fixedColumns.ContainsKey(column.Name))
                {
                    column.Width = fixedColumns[column.Name];
                    fixedWidthColumns += column.Width;
                }
            }

            // اگر فضای اضافی وجود دارد، به ستون‌های مهم اضافه کنید
            int remainingWidth = totalWidth - fixedWidthColumns;
            if (remainingWidth > 0)
            {
                // ستون‌هایی که می‌خواهید عرض بیشتری داشته باشند
                string[] expandableColumns = { "Position", "WorkEmail", "BankAccountNo" };

                foreach (string colName in expandableColumns)
                {
                    if (dgvAccountants.Columns.Contains(colName))
                    {
                        int extraWidth = remainingWidth / expandableColumns.Length;
                        dgvAccountants.Columns[colName].Width += extraWidth;
                    }
                }
            }
        }
        private void CreateDataGridView()
        {
            dgvAccountants = new DataGridView();
            dgvAccountants.Dock = DockStyle.Fill;
            dgvAccountants.BackgroundColor = Color.White;
            dgvAccountants.BorderStyle = BorderStyle.Fixed3D;
            dgvAccountants.Font = new Font("B Nazanin", 10);
            dgvAccountants.RowHeadersVisible = false;
            dgvAccountants.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAccountants.MultiSelect = false;
            dgvAccountants.AllowUserToAddRows = false;
            dgvAccountants.AllowUserToDeleteRows = false;

            // *** تغییر مهم: از Fill به None تغییر دهید ***
            dgvAccountants.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            // ستون‌های DataGridView در متد DisplayAllAccountants تنظیم می‌شوند

            this.Controls.Add(dgvAccountants);
            // برای اطمینان از ترتیب صحیح نمایش
            dgvAccountants.BringToFront();
        }

        private void CreateStatusStrip()
        {
            statusStrip = new StatusStrip();
            statusStrip.BackColor = Color.White;

            ToolStripStatusLabel userLabel = new ToolStripStatusLabel();
            userLabel.Text = "👤 کاربر: مدیر سیستم";
            userLabel.BorderSides = ToolStripStatusLabelBorderSides.Right;
            userLabel.BorderStyle = Border3DStyle.Etched;

            ToolStripStatusLabel timeLabel = new ToolStripStatusLabel();
            timeLabel.Text = DateTime.Now.ToString("📅 yyyy/MM/dd ⏰ HH:mm");
            timeLabel.BorderSides = ToolStripStatusLabelBorderSides.Right;
            timeLabel.BorderStyle = Border3DStyle.Etched;

            ToolStripStatusLabel recordLabel = new ToolStripStatusLabel();
            recordLabel.Text = "📊 تعداد رکوردها: 0";
            recordLabel.BorderSides = ToolStripStatusLabelBorderSides.Right;
            recordLabel.BorderStyle = Border3DStyle.Etched;

            ToolStripStatusLabel dbLabel = new ToolStripStatusLabel();
            dbLabel.Text = "✅ پایگاه داده: آنلاین";
            dbLabel.ForeColor = Color.Green;

            statusStrip.Items.Add(userLabel);
            statusStrip.Items.Add(timeLabel);
            statusStrip.Items.Add(recordLabel);
            statusStrip.Items.Add(dbLabel);

            this.Controls.Add(statusStrip);
        }

        private Button CreateStyledButton(string text, Color backColor, Color foreColor)
        {
            Button button = new Button();
            button.Text = text;
            button.BackColor = backColor;
            button.ForeColor = foreColor;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = normalFont;
            button.Cursor = Cursors.Hand;
            button.Padding = new Padding(10, 5, 10, 5);

            // افکت hover
            button.MouseEnter += (s, e) =>
            {
                button.BackColor = ControlPaint.Light(backColor, 0.1f);
            };

            button.MouseLeave += (s, e) =>
            {
                button.BackColor = backColor;
            };

            return button;
        }




        //
        //
        //
        //




        private void LoadAccountantData()
        {
            try
            {
                string connectionString = "Server=.;Database=PetrochemicalSalesDB;Integrated Security=true;";
                string query = "SELECT AccountantID, FirstName, LastName, Mobile, WorkEmail FROM Accountants";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // *** دیباگ: نمایش DataTable ***
                    MessageBox.Show($"تعداد ردیف‌های DataTable: {dt.Rows.Count}\n" +
                                  $"تعداد ستون‌های DataTable: {dt.Columns.Count}",
                                  "اطلاعات DataTable");

                    // پاک کردن ستون‌های قبلی
                    dgvAccountants.Columns.Clear();

                    // غیرفعال کردن تولید خودکار ستون‌ها
                    dgvAccountants.AutoGenerateColumns = false;

                    // *** درست کردن DataPropertyName ها - توجه به حروف بزرگ و کوچک ***
                    // ستون 1: کد حسابدار
                    DataGridViewTextBoxColumn colId = new DataGridViewTextBoxColumn();
                    colId.Name = "AccountantID";
                    colId.DataPropertyName = "AccountantID"; // دقیقاً همان نام ستون در DataTable
                    colId.HeaderText = "کد حسابدار";
                    colId.Width = 100;
                    colId.ReadOnly = true;
                    colId.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvAccountants.Columns.Add(colId);

                    // ستون 2: نام - *** اصلاح شده ***
                    DataGridViewTextBoxColumn colFirstName = new DataGridViewTextBoxColumn();
                    colFirstName.Name = "FirstName";
                    colFirstName.DataPropertyName = "FirstName"; // با حرف بزرگ N
                    colFirstName.HeaderText = "نام"; // اینجا تصحیح شد
                    colFirstName.Width = 120;
                    colFirstName.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvAccountants.Columns.Add(colFirstName);

                    // ستون 3: نام خانوادگی - *** اصلاح شده ***
                    DataGridViewTextBoxColumn colLastName = new DataGridViewTextBoxColumn();
                    colLastName.Name = "LastName";
                    colLastName.DataPropertyName = "LastName"; // با حرف بزرگ N
                    colLastName.HeaderText = "نام خانوادگی"; // اینجا تصحیح شد
                    colLastName.Width = 150;
                    colLastName.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvAccountants.Columns.Add(colLastName);

                    // ستون 4: تلفن همراه
                    DataGridViewTextBoxColumn colMobile = new DataGridViewTextBoxColumn();
                    colMobile.Name = "Mobile";
                    colMobile.DataPropertyName = "Mobile";
                    colMobile.HeaderText = "تلفن همراه";
                    colMobile.Width = 120;
                    colMobile.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvAccountants.Columns.Add(colMobile);

                    // ستون 5: ایمیل کاری
                    DataGridViewTextBoxColumn colEmail = new DataGridViewTextBoxColumn();
                    colEmail.Name = "WorkEmail";
                    colEmail.DataPropertyName = "WorkEmail";
                    colEmail.HeaderText = "ایمیل کاری";
                    colEmail.Width = 200;
                    colEmail.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvAccountants.Columns.Add(colEmail);

                    // *** دیباگ: بررسی تطبیق ستون‌ها ***
                    CheckColumnMapping(dt);

                    // اتصال داده‌ها
                    dgvAccountants.DataSource = dt;

                    // *** تأیید نمایش داده‌ها ***
                    MessageBox.Show($"DataGridView پس از اتصال:\n" +
                                  $"تعداد ستون‌ها: {dgvAccountants.Columns.Count}\n" +
                                  $"تعداد ردیف‌ها: {dgvAccountants.Rows.Count}\n" +
                                  $"DataSource تنظیم شد: {(dgvAccountants.DataSource != null ? "بله" : "خیر")}",
                                  "تأیید اتصال");

                    UpdateStatusBar(dt.Rows.Count);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا: {ex.Message}", "خطا",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /*
        private void LoadAccountantDataFinal()
        {
            try
            {
                string connectionString = "Server=.;Database=PetrochemicalSalesDB;Integrated Security=true;";
                string query = "SELECT AccountantID, FirstName, LastName, Mobile, WorkEmail FROM Accountants";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // 1. پاک کردن کامل
                    dgvAccountants.DataSource = null;
                    dgvAccountants.Columns.Clear();
                    dgvAccountants.Rows.Clear();

                    // 2. ایجاد ستون‌ها با دقت
                    CreateAccurateColumns();

                    // 3. اتصال داده‌ها
                    dgvAccountants.DataSource = dt;

                    // 4. Reset binding
                    dgvAccountants.DataSource = null;
                    dgvAccountants.DataSource = dt;

                    // 5. Refresh
                    dgvAccountants.Refresh();
                    dgvAccountants.Invalidate();

                    // 6. بررسی نهایی
                    if (dgvAccountants.Rows.Count > 0)
                    {
                        MessageBox.Show($"موفق! داده‌ها نمایش داده می‌شوند.\n" +
                                      $"ردیف اول: {dgvAccountants.Rows[0].Cells["FirstName"].Value} " +
                                      $"{dgvAccountants.Rows[0].Cells["LastName"].Value}");
                    }

                    UpdateStatusBar(dt.Rows.Count);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا: {ex.Message}");
            }

        }
        */
        private void CreateAccurateColumns()
        {
            dgvAccountants.AutoGenerateColumns = false;

            // استفاده از لیست برای اطمینان از تطبیق دقیق
            var columns = new[]
            {
        new { Name = "AccountantID", Header = "کد حسابدار", Width = 100 },
        new { Name = "FirstName", Header = "نام", Width = 120 },
        new { Name = "LastName", Header = "نام خانوادگی", Width = 150 },
        new { Name = "Mobile", Header = "تلفن همراه", Width = 120 },
        new { Name = "WorkEmail", Header = "ایمیل کاری", Width = 200 }
    };

            foreach (var col in columns)
            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.Name = col.Name;
                column.DataPropertyName = col.Name; // تطبیق دقیق
                column.HeaderText = col.Header;
                column.Width = col.Width;
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dgvAccountants.Columns.Add(column);
            }
        }
        private void CheckColumnMapping(DataTable dt)
        {
            string mappingInfo = "بررسی تطبیق ستون‌ها:\n\n";

            // بررسی نام ستون‌های DataTable
            mappingInfo += "ستون‌های DataTable:\n";
            foreach (DataColumn col in dt.Columns)
            {
                mappingInfo += $"- {col.ColumnName}\n";
            }

            mappingInfo += "\nستون‌های DataGridView:\n";
            foreach (DataGridViewColumn col in dgvAccountants.Columns)
            {
                mappingInfo += $"- Name: {col.Name}, DataPropertyName: {col.DataPropertyName}, HeaderText: {col.HeaderText}\n";
            }

            MessageBox.Show(mappingInfo, "بررسی تطبیق");
        }

        private void DebugDataTable(DataTable dt)
        {
            MessageBox.Show(
                $"تعداد ردیف‌های DataTable: {dt.Rows.Count}\n" +
                $"تعداد ستون‌های DataTable: {dt.Columns.Count}",
                "اطلاعات DataTable");

            if (dt.Rows.Count > 0)
            {
                string firstRow = "محتویات اولین ردیف:\n";
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    firstRow += $"{dt.Columns[i].ColumnName}: {dt.Rows[0][i]}\n";
                }
                MessageBox.Show(firstRow, "نمونه داده");
            }
        }

        private void CreateColumnsWithExactMatch(DataTable dt)
        {
            // بررسی وجود ستون‌ها و ایجاد آنها
            string[] columnNames = { "AccountantID", "FirstName", "LastName", "Mobile", "WorkEmail" };
            string[] persianTitles = { "کد حسابدار", "نام", "نام خانوادگی", "تلفن همراه", "ایمیل کاری" };
            int[] columnWidths = { 100, 120, 150, 120, 200 };

            for (int i = 0; i < columnNames.Length; i++)
            {
                if (dt.Columns.Contains(columnNames[i]))
                {
                    DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                    column.Name = columnNames[i];
                    column.DataPropertyName = columnNames[i]; // این مهم است!
                    column.HeaderText = persianTitles[i];
                    column.Width = columnWidths[i];
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    dgvAccountants.Columns.Add(column);
                    MessageBox.Show($"ستون '{columnNames[i]}' با DataPropertyName '{column.DataPropertyName}' اضافه شد.",
                                   "ایجاد ستون");
                }
                else
                {
                    MessageBox.Show($"ستون '{columnNames[i]}' در DataTable یافت نشد!", "هشدار",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void DebugDataGridView()
        {
            MessageBox.Show(
                $"تعداد ستون‌های DataGridView: {dgvAccountants.Columns.Count}\n" +
                $"تعداد ردیف‌های DataGridView: {dgvAccountants.Rows.Count}\n" +
                $"AutoGenerateColumns: {dgvAccountants.AutoGenerateColumns}\n" +
                $"DataSource: {dgvAccountants.DataSource}",
                "اطلاعات DataGridView");

            // بررسی DataPropertyName ستون‌ها
            string dgvColumns = "ستون‌های DataGridView:\n";
            foreach (DataGridViewColumn col in dgvAccountants.Columns)
            {
                dgvColumns += $"نام: {col.Name}, DataPropertyName: {col.DataPropertyName}, HeaderText: {col.HeaderText}\n";
            }
            MessageBox.Show(dgvColumns, "مشخصات ستون‌ها");
        }





    }
}