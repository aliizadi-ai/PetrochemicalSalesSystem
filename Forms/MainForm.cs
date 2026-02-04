using PetrochemicalSalesSystem.Data;
using PetrochemicalSalesSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
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
        }

        // ========== رویدادهای دکمه‌ها ==========

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            OpenAccountantEditForm(0); // 0 به معنای ایجاد جدید
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
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
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
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
                if (dataGridView1.Columns[e.ColumnIndex].HeaderText == "عملیات" ||
                    dataGridView1.Columns[e.ColumnIndex].HeaderText == "مشاهده جزئیات")
                {
                    long accountantId = Convert.ToInt64(dataGridView1.Rows[e.RowIndex].Cells["AccountantID"].Value);
                    OpenAccountantDetailForm(accountantId);
                }
            }
        }

        private void DgvAccountants_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                long accountantId = Convert.ToInt64(dataGridView1.Rows[e.RowIndex].Cells["AccountantID"].Value);
                OpenAccountantEditForm(accountantId);
            }
        }

        private void DgvAccountants_SelectionChanged(object sender, EventArgs e)
        {
            // به‌روزرسانی وضعیت دکمه‌ها بر اساس انتخاب
            bool hasSelection = dataGridView1.SelectedRows.Count > 0;

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
            if (dataGridView1.Rows.Count == 0)
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
                    foreach (DataGridViewColumn column in dataGridView1.Columns)
                    {
                        if (column.Visible && !column.HeaderText.Contains("عملیات"))
                        {
                            headers.Add(column.HeaderText);
                        }
                    }
                    writer.WriteLine(string.Join(",", headers));

                    // نوشتن داده‌ها
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.IsNewRow) continue;

                        List<string> cells = new List<string>();
                        foreach (DataGridViewColumn column in dataGridView1.Columns)
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

        private void UpdateStatusBar()
        {
            ToolStripStatusLabel recordLabel = statusStrip.Items.OfType<ToolStripStatusLabel>()
                .FirstOrDefault(l => l.Text.Contains("تعداد رکوردها"));

            if (recordLabel != null)
            {
                int totalCount = _accountants?.Count ?? 0;
                int filteredCount = dataGridView1.Rows.Count;

                if (totalCount == filteredCount)
                {
                    recordLabel.Text = $"📊 تعداد حسابداران: {totalCount}";
                }
                else
                {
                    recordLabel.Text = $"📊 نمایش {filteredCount} از {totalCount} حسابدار";
                }
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

            // دکمه بروزرسانی
            Button refreshButton = buttonPanel.Controls.OfType<Button>()
                .FirstOrDefault(b => b.Text.Contains("بروزرسانی"));
            if (refreshButton != null)
            {
                refreshButton.Click += BtnRefresh_Click;
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

            // رویداد DataGridView
            dataGridView1.CellContentClick += DgvAccountants_CellContentClick;
            dataGridView1.CellDoubleClick += DgvAccountants_CellDoubleClick;
            dataGridView1.SelectionChanged += DgvAccountants_SelectionChanged;
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
            dataGridView1.DataSource = filteredList.ToList();
            FormatDataGridView();
        }
        private void FormatDataGridView()
        {
            if (dataGridView1.Columns.Count == 0) return;

            // تنظیم فرمت ستون‌ها
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                if (column.Name.Contains("Date") || column.HeaderText.Contains("تاریخ"))
                {
                    column.DefaultCellStyle.Format = "yyyy/MM/dd";
                }
                else if (column.Name.Contains("Salary") || column.HeaderText.Contains("حقوق"))
                {
                    column.DefaultCellStyle.Format = "N0";
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                }
            }

            // ستون وضعیت
            if (dataGridView1.Columns.Contains("IsActive"))
            {
                dataGridView1.Columns["IsActive"].Visible = false; // مخفی کنیم چون CheckBox داریم
            }
        }


        private void LoadAccountants()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // دریافت همه حسابداران
                _accountants = _accountantRepository.GetAllAccountants();

                // اعمال فیلتر جستجو
                ApplyFilters();

                UpdateStatusBar();
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

            // ایجاد پنل بالا
            CreateTopPanel();

            // ایجاد پنل جستجو
            CreateSearchPanel();

            // ایجاد پنل دکمه‌ها
            CreateButtonPanel();

            // ایجاد DataGridView
            CreateDataGridView();

            // ایجاد StatusStrip
            CreateStatusStrip();
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

            buttonPanel.Controls.Add(addButton);
            buttonPanel.Controls.Add(editButton);
            buttonPanel.Controls.Add(deleteButton);
            buttonPanel.Controls.Add(refreshButton);
            buttonPanel.Controls.Add(exportButton);
            buttonPanel.Controls.Add(reportButton);

            this.Controls.Add(buttonPanel);
        }

        private void CreateDataGridView()
        {
            dgvAccountants = new DataGridView();
            dgvAccountants.Dock = DockStyle.Fill;
            dgvAccountants.Location = new Point(0, 230);
            dgvAccountants.Size = new Size(1280, 450);

            // تنظیمات ظاهری
            dgvAccountants.BackgroundColor = Color.White;
            dgvAccountants.BorderStyle = BorderStyle.Fixed3D;
            dgvAccountants.Font = smallFont;
            dgvAccountants.RowHeadersVisible = false;
            dgvAccountants.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAccountants.MultiSelect = false;
            dgvAccountants.AllowUserToAddRows = false;
            dgvAccountants.AllowUserToDeleteRows = false;
            dgvAccountants.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAccountants.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle()
            {
                BackColor = primaryColor,
                ForeColor = Color.White,
                Font = new Font("B Nazanin", 11, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            // تنظیم ستون‌ها
            dgvAccountants.Columns.Clear();

            DataGridViewTextBoxColumn col1 = new DataGridViewTextBoxColumn();
            col1.HeaderText = "کد پرسنلی";
            col1.DataPropertyName = "EmployeeCode";
            col1.Width = 100;

            DataGridViewTextBoxColumn col2 = new DataGridViewTextBoxColumn();
            col2.HeaderText = "نام و نام خانوادگی";
            col2.DataPropertyName = "FullName";
            col2.Width = 200;

            DataGridViewTextBoxColumn col3 = new DataGridViewTextBoxColumn();
            col3.HeaderText = "کد ملی";
            col3.DataPropertyName = "NationalID";
            col3.Width = 100;

            DataGridViewTextBoxColumn col4 = new DataGridViewTextBoxColumn();
            col4.HeaderText = "دپارتمان";
            col4.DataPropertyName = "DepartmentName";
            col4.Width = 150;

            DataGridViewTextBoxColumn col5 = new DataGridViewTextBoxColumn();
            col5.HeaderText = "سمت";
            col5.DataPropertyName = "Position";
            col5.Width = 150;

            DataGridViewTextBoxColumn col6 = new DataGridViewTextBoxColumn();
            col6.HeaderText = "تاریخ استخدام";
            col6.DataPropertyName = "HireDate";
            col6.Width = 120;

            DataGridViewCheckBoxColumn col7 = new DataGridViewCheckBoxColumn();
            col7.HeaderText = "فعال";
            col7.DataPropertyName = "IsActive";
            col7.Width = 60;

            DataGridViewButtonColumn actionCol = new DataGridViewButtonColumn();
            actionCol.HeaderText = "عملیات";
            actionCol.Text = "مشاهده جزئیات";
            actionCol.UseColumnTextForButtonValue = true;
            actionCol.Width = 120;

            dgvAccountants.Columns.AddRange(col1, col2, col3, col4, col5, col6, col7, actionCol);

            // ستون وضعیت فعال/غیرفعال
            dgvAccountants.CellFormatting += (sender, e) =>
            {
                if (e.ColumnIndex == 6 && e.RowIndex >= 0) // ستون IsActive
                {
                    if (e.Value != null && bool.TryParse(e.Value.ToString(), out bool isActive))
                    {
                        e.CellStyle.BackColor = isActive ? Color.FromArgb(212, 237, 218) : Color.FromArgb(248, 215, 218);
                        e.CellStyle.ForeColor = isActive ? Color.FromArgb(21, 87, 36) : Color.FromArgb(114, 28, 36);
                    }
                }
            };

            // سطرهای متناوب
            dgvAccountants.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);

            this.Controls.Add(dgvAccountants);
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
    }
}