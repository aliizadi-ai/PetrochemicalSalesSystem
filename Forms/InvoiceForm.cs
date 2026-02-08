using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PetrochemicalSalesSystem.Forms
{
    public partial class InvoiceForm : Form
    {
        // کنترل‌های اصلی
        private DataGridView dgvProducts;
        private ComboBox cmbCustomer;
        private TextBox txtNationalID;
        private TextBox txtCustomerName;
        private TextBox txtCustomerPhone;
        private TextBox txtCustomerAddress;
        private TextBox txtNotes;
        private Label lblSubTotal;
        private Label lblDiscountTotal;
        private Label lblTaxTotal;
        private Label lblGrandTotal;
        private CheckBox chkIsPaid;
        private ComboBox cmbPaymentMethod;
        private DateTimePicker dtpInvoiceDate;
        private DateTimePicker dtpPaymentDate;
        private Button btnSave;
        private Button btnPrint;
        private Button btnClear;

        // پنل اصلی برای اسکرول
        private Panel mainPanel;

        // کنترل جدید برای کد ملی
        private TextBox txtNationalCode;

        public InvoiceForm()
        {
            InitializeComponent();
            InitializeComponent2();
            InitializeInvoiceForm();
            LoadCustomers();
            LoadPaymentMethods();
        }

        private void InitializeComponent2()
        {
            this.SuspendLayout();
            this.Size = new Size(1200, 850); // افزایش اندازه فرم
            this.Text = "ثبت فاکتور جدید";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.BackColor = Color.FromArgb(240, 242, 245);
            this.Font = new Font("IRANSans", 9.75f, FontStyle.Regular);
            this.ResumeLayout(false);
        }

        private void InitializeInvoiceForm()
        {
            // پاک کردن کنترل‌های قبلی
            this.Controls.Clear();

            // ایجاد پنل اصلی برای اسکرول
            mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.AutoScroll = true;
            mainPanel.AutoScrollMinSize = new Size(1150, 850); // حداقل اندازه برای اسکرول
            this.Controls.Add(mainPanel);

            CreateHeader();
            CreateCustomerSection();
            CreateProductsGrid();
            CreateTotalsSection();
            CreateButtonsSection();
        }

        private void CreateHeader()
        {
            Panel headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 70;
            headerPanel.BackColor = Color.FromArgb(0, 102, 51);
            headerPanel.Padding = new Padding(20, 0, 20, 0);

            Label title = new Label();
            title.Text = "🧾 ثبت فاکتور جدید";
            title.Font = new Font("IRANSans", 16, FontStyle.Bold);
            title.ForeColor = Color.White;
            title.Dock = DockStyle.Fill;
            title.TextAlign = ContentAlignment.MiddleCenter;

            // دکمه بستن
            Button btnClose = new Button();
            btnClose.Text = "✕";
            btnClose.Font = new Font("Arial", 12, FontStyle.Bold);
            btnClose.Size = new Size(40, 40);
            btnClose.Location = new Point(20, 15);
            btnClose.BackColor = Color.Transparent;
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += (s, e) => this.Close();

            headerPanel.Controls.Add(title);
            headerPanel.Controls.Add(btnClose);
            mainPanel.Controls.Add(headerPanel);
        }

        private void CreateCustomerSection()
        {
            Panel customerPanel = new Panel();
            customerPanel.Size = new Size(1250, 200); // افزایش عرض
            customerPanel.Location = new Point(25, 85); // فاصله بیشتر از هدر
            customerPanel.BackColor = Color.White;
            customerPanel.BorderStyle = BorderStyle.FixedSingle;
            customerPanel.Padding = new Padding(10);

            // عنوان بخش
            Label sectionTitle = new Label();
            sectionTitle.Text = "📋 اطلاعات مشتری";
            sectionTitle.Font = new Font("IRANSans", 12, FontStyle.Bold);
            sectionTitle.ForeColor = Color.FromArgb(0, 102, 51);
            sectionTitle.Size = new Size(500, 25);
            sectionTitle.Location = new Point(250, 25);
            customerPanel.Controls.Add(sectionTitle);

            int y = 75;
            int x1 = 30, x2 = 430, x3 = 570, x4 = 840; // تنظیم موقعیت‌ها با فاصله بیشتر

            // ردیف اول - مطابق تصویر
            CreateField("کد ملی", txtNationalCode = new TextBox(), x1, y, customerPanel, 220); // اضافه کردن فیلد کد ملی
            txtNationalID = new TextBox(); // این TextBox برای کد مشتری است

            CreateField("مشتری", cmbCustomer = new ComboBox(), x2, y, customerPanel, 220);
            cmbCustomer.SelectedIndexChanged += CmbCustomer_SelectedIndexChanged;

            // ردیف دوم
            //y += 50;
            CreateField("نام مشتری", txtCustomerName = new TextBox(), x4, y, customerPanel, 220);
            txtCustomerName.ReadOnly = true;

            // ردیف سوم
            y += 50;
            CreateField("تاریخ فاکتور", dtpInvoiceDate = new DateTimePicker(), x1, y, customerPanel, 220);
            dtpInvoiceDate.Value = DateTime.Now;
            dtpInvoiceDate.Format = DateTimePickerFormat.Short;
            dtpInvoiceDate.RightToLeftLayout = true;

            CreateField("روش پرداخت", cmbPaymentMethod = new ComboBox(), x2, y, customerPanel, 220);

            chkIsPaid = new CheckBox();
            chkIsPaid.Text = "پرداخت شده";
            chkIsPaid.Font = new Font("IRANSans", 10);
            chkIsPaid.Size = new Size(120, 30);
            chkIsPaid.Location = new Point(x3 + 20, y - 5);
            chkIsPaid.CheckedChanged += (s, e) =>
            {
                dtpPaymentDate.Enabled = chkIsPaid.Checked;
            };
            customerPanel.Controls.Add(chkIsPaid);

            CreateField("تاریخ پرداخت", dtpPaymentDate = new DateTimePicker(), x4, y, customerPanel, 220);
            dtpPaymentDate.Value = DateTime.Now.AddDays(30);
            dtpPaymentDate.Format = DateTimePickerFormat.Short;
            dtpPaymentDate.RightToLeftLayout = true;
            dtpPaymentDate.Enabled = false;

            mainPanel.Controls.Add(customerPanel);
        }

        private void CreateProductsGrid()
        {
            Panel productsPanel = new Panel();
            productsPanel.Size = new Size(1250, 400); // افزایش ارتفاع
            productsPanel.Location = new Point(25, 300); // فاصله از بخش مشتری
            productsPanel.BackColor = Color.White;
            productsPanel.BorderStyle = BorderStyle.FixedSingle;
            productsPanel.Padding = new Padding(10);

            // عنوان بخش
            Label sectionTitle = new Label();
            sectionTitle.Text = "📦 لیست کالاها";
            sectionTitle.Font = new Font("IRANSans", 12, FontStyle.Bold);
            sectionTitle.ForeColor = Color.FromArgb(0, 102, 51);
            sectionTitle.Size = new Size(500, 25);
            sectionTitle.Location = new Point(250, 25);
            productsPanel.Controls.Add(sectionTitle);

            // DataGridView با اندازه بزرگتر
            dgvProducts = new DataGridView();
            dgvProducts.Size = new Size(1200, 200); // اندازه بزرگتر
            dgvProducts.Location = new Point(20, 55);
            dgvProducts.BackgroundColor = Color.White;
            dgvProducts.BorderStyle = BorderStyle.FixedSingle;
            dgvProducts.RowHeadersVisible = false;
            dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProducts.AllowUserToAddRows = true;
            dgvProducts.AllowUserToDeleteRows = true;
            dgvProducts.EditMode = DataGridViewEditMode.EditOnEnter;
            dgvProducts.CellEndEdit += DgvProducts_CellEndEdit;
            dgvProducts.ScrollBars = ScrollBars.Both; // اسکرول بار عمودی و افقی

            // استایل‌دهی به جدول
            dgvProducts.EnableHeadersVisualStyles = false;
            dgvProducts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 102, 51);
            dgvProducts.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvProducts.ColumnHeadersDefaultCellStyle.Font = new Font("IRANSans", 10, FontStyle.Bold);
            dgvProducts.ColumnHeadersHeight = 40;
            dgvProducts.RowTemplate.Height = 35;
            dgvProducts.DefaultCellStyle.Font = new Font("IRANSans", 9);
            dgvProducts.DefaultCellStyle.Padding = new Padding(5);

            // اضافه کردن ستون‌ها مطابق تصویر
            string[] columns = {
                "جمع ردیف", "مبلغ مالیات", "% مالیات", "مبلغ تخفیف", "% تخفیف",
                "قیمت واحد", "تعداد", "واحد", "نام کالا", "کد کالا" // ترتیب معکوس مطابق تصویر
            };

            string[] columnNames = {
                "LineTotal", "TaxAmount", "TaxPercent", "DiscountAmount", "DiscountPercent",
                "UnitPrice", "Quantity", "Unit", "ProductName", "ProductCode"
            };

            for (int i = 0; i < columns.Length; i++)
            {
                dgvProducts.Columns.Add(columnNames[i], columns[i]);

                // تنظیمات ستون‌های عددی
                if (i <= 8) // همه ستون‌ها به جز نام کالا و کد کالا
                {
                    dgvProducts.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    if (i == 6 || i == 2 || i == 4) // تعداد، %مالیات، %تخفیف
                        dgvProducts.Columns[i].DefaultCellStyle.Format = "N2";
                    else if (i == 0 || i == 1 || i == 3 || i == 5) // مبالغ
                        dgvProducts.Columns[i].DefaultCellStyle.Format = "N0";
                }

                // ستون‌های محاسباتی فقط خواندنی
                if (i == 0 || i == 1 || i == 3) // جمع ردیف، مبلغ مالیات، مبلغ تخفیف
                    dgvProducts.Columns[i].ReadOnly = true;
            }

            productsPanel.Controls.Add(dgvProducts);

            // دکمه‌های مدیریت در پایین جدول
            Panel buttonPanel = new Panel();
            buttonPanel.Size = new Size(1110, 40);
            buttonPanel.Location = new Point(20, 315);

            Button btnAdd = new Button();
            btnAdd.Text = "➕ افزودن کالا";
            btnAdd.Size = new Size(120, 35);
            btnAdd.Location = new Point(0, 0);
            btnAdd.Font = new Font("IRANSans", 10);
            btnAdd.BackColor = Color.FromArgb(52, 152, 219);
            btnAdd.ForeColor = Color.White;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Cursor = Cursors.Hand;
            btnAdd.Click += (s, e) => dgvProducts.Rows.Add();

            /*
            Button btnRemove = new Button();
            btnRemove.Text = "➖ حذف کالا";
            btnRemove.Size = new Size(120, 35);
            btnRemove.Location = new Point(130, 0);
            btnRemove.Font = new Font("IRANSans", 10);
            btnRemove.BackColor = Color.FromArgb(231, 76, 60);
            btnRemove.ForeColor = Color.White;
            btnRemove.FlatStyle = FlatStyle.Flat;
            btnRemove.FlatAppearance.BorderSize = 0;
            btnRemove.Cursor = Cursors.Hand;
            btnRemove.Click += (s, e) =>
            {
                if (dgvProducts.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow row in dgvProducts.SelectedRows)
                    {
                        if (!row.IsNewRow)
                            dgvProducts.Rows.Remove(row);
                    }
                    CalculateTotals();
                }
            };
            */


            Button btnRemove = new Button();
            btnRemove.Text = "➖ حذف کالا";
            btnRemove.Size = new Size(120, 35);
            btnRemove.Location = new Point(130, 0);
            btnRemove.Font = new Font("IRANSans", 10);
            btnRemove.BackColor = Color.FromArgb(231, 76, 60);
            btnRemove.ForeColor = Color.White;
            btnRemove.FlatStyle = FlatStyle.Flat;
            btnRemove.FlatAppearance.BorderSize = 0;
            btnRemove.Cursor = Cursors.Hand;
            btnRemove.Click += (s, e) =>
            {
                // پایان دادن به حالت ویرایش
                dgvProducts.EndEdit();

                // اگر هیچ ردیفی انتخاب نشده باشد، از ردیف جاری استفاده کن
                if (dgvProducts.SelectedRows.Count == 0 && dgvProducts.CurrentRow != null)
                {
                    if (!dgvProducts.CurrentRow.IsNewRow)
                    {
                        dgvProducts.Rows.Remove(dgvProducts.CurrentRow);
                        CalculateTotals();
                    }
                    return;
                }

                // جمع‌آوری ردیف‌ها برای حذف
                List<DataGridViewRow> rowsToDelete = new List<DataGridViewRow>();

                foreach (DataGridViewRow row in dgvProducts.SelectedRows)
                {
                    if (!row.IsNewRow && !rowsToDelete.Contains(row))
                    {
                        rowsToDelete.Add(row);
                    }
                }

                // حذف ردیف‌ها
                foreach (DataGridViewRow row in rowsToDelete)
                {
                    dgvProducts.Rows.Remove(row);
                }

                // پاک کردن انتخاب
                dgvProducts.ClearSelection();

                // محاسبه مجدد
                CalculateTotals();
            };




            Button btnLoad = new Button();
            btnLoad.Text = "📋 بارگذاری محصولات";
            btnLoad.Size = new Size(150, 35);
            btnLoad.Location = new Point(260, 0);
            btnLoad.Font = new Font("IRANSans", 10);
            btnLoad.BackColor = Color.FromArgb(155, 89, 182);
            btnLoad.ForeColor = Color.White;
            btnLoad.FlatStyle = FlatStyle.Flat;
            btnLoad.FlatAppearance.BorderSize = 0;
            btnLoad.Cursor = Cursors.Hand;
            btnLoad.Click += BtnLoadProducts_Click;

            buttonPanel.Controls.Add(btnAdd);
            buttonPanel.Controls.Add(btnRemove);
            buttonPanel.Controls.Add(btnLoad);
            productsPanel.Controls.Add(buttonPanel);

            mainPanel.Controls.Add(productsPanel);
        }

        private void CreateTotalsSection()
        {
            Panel totalsPanel = new Panel();
            totalsPanel.Size = new Size(1250, 250);
            totalsPanel.Location = new Point(25, 720);
            totalsPanel.BackColor = Color.White;
            totalsPanel.BorderStyle = BorderStyle.FixedSingle;
            totalsPanel.Padding = new Padding(10);

            // عنوان بخش
            Label sectionTitle = new Label();
            sectionTitle.Text = "💰 جمع‌های فاکتور";
            sectionTitle.Font = new Font("IRANSans", 12, FontStyle.Bold);
            sectionTitle.ForeColor = Color.FromArgb(0, 102, 51);
            sectionTitle.Size = new Size(500, 25);
            sectionTitle.Location = new Point(250, 25);
            totalsPanel.Controls.Add(sectionTitle);

            // مبالغ در سمت چپ
            int y = 80;
            int x1 = 50, x2 = 450, x3 = 900;

            // ردیف اول
            CreateAmountField("جمع کل", lblSubTotal = new Label(), "0", x1, y, totalsPanel);
            CreateAmountField("تخفیف", lblDiscountTotal = new Label(), "0", x2, y, totalsPanel);
            CreateAmountField("مالیات", lblTaxTotal = new Label(), "0", x3, y, totalsPanel);

            // ردیف دوم
            y += 60;
            CreateAmountField("مبلغ قابل پرداخت", lblGrandTotal = new Label(), "0", x1, y, totalsPanel);

            /*
            // فیلد یادداشت در سمت راست
            Label lblNotes = new Label();
            lblNotes.Text = "یادداشت:";
            lblNotes.Font = new Font("IRANSans", 10);
            lblNotes.Size = new Size(60, 25);
            lblNotes.Location = new Point(700, 15);
            totalsPanel.Controls.Add(lblNotes);

            txtNotes = new TextBox();
            txtNotes.Size = new Size(400, 100);
            txtNotes.Location = new Point(770, 15);
            txtNotes.Font = new Font("IRANSans", 10);
            txtNotes.Multiline = true;
            txtNotes.ScrollBars = ScrollBars.Vertical;
            totalsPanel.Controls.Add(txtNotes);
            */

            mainPanel.Controls.Add(totalsPanel);
        }

        private void CreateButtonsSection()
        {
            Panel buttonPanel = new Panel();
            buttonPanel.Size = new Size(1250, 250);
            buttonPanel.Location = new Point(25, 1000);
            buttonPanel.BackColor = Color.Transparent;

            btnSave = new Button();
            btnSave.Text = "💾 ذخیره فاکتور";
            btnSave.Size = new Size(150, 45);
            btnSave.Location = new Point(850, 10);
            btnSave.Font = new Font("IRANSans", 11, FontStyle.Bold);
            btnSave.BackColor = Color.FromArgb(46, 204, 113);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.FlatAppearance.MouseOverBackColor = Color.FromArgb(39, 174, 96);
            btnSave.Cursor = Cursors.Hand;
            btnSave.Click += BtnSave_Click;

            btnPrint = new Button();
            btnPrint.Text = "🖨️ چاپ فاکتور";
            btnPrint.Size = new Size(150, 45);
            btnPrint.Location = new Point(680, 10);
            btnPrint.Font = new Font("IRANSans", 11);
            btnPrint.BackColor = Color.FromArgb(52, 152, 219);
            btnPrint.ForeColor = Color.White;
            btnPrint.FlatStyle = FlatStyle.Flat;
            btnPrint.FlatAppearance.BorderSize = 0;
            btnPrint.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 128, 185);
            btnPrint.Cursor = Cursors.Hand;
            btnPrint.Click += BtnPrint_Click;

            btnClear = new Button();
            btnClear.Text = "🗑️ پاک کردن فرم";
            btnClear.Size = new Size(150, 45);
            btnClear.Location = new Point(510, 10);
            btnClear.Font = new Font("IRANSans", 11);
            btnClear.BackColor = Color.FromArgb(241, 196, 15);
            btnClear.ForeColor = Color.White;
            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.FlatAppearance.BorderSize = 0;
            btnClear.FlatAppearance.MouseOverBackColor = Color.FromArgb(243, 156, 18);
            btnClear.Cursor = Cursors.Hand;
            btnClear.Click += BtnClear_Click;

            Button btnCancel = new Button();
            btnCancel.Text = "❌ انصراف";
            btnCancel.Size = new Size(150, 45);
            btnCancel.Location = new Point(340, 10);
            btnCancel.Font = new Font("IRANSans", 11);
            btnCancel.BackColor = Color.FromArgb(231, 76, 60);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 57, 43);
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.Click += (s, e) => this.Close();

            buttonPanel.Controls.Add(btnSave);
            buttonPanel.Controls.Add(btnPrint);
            buttonPanel.Controls.Add(btnClear);
            buttonPanel.Controls.Add(btnCancel);
            mainPanel.Controls.Add(buttonPanel);
        }

        private void CreateField(string labelText, Control control, int x, int y, Panel parent, int width = 200)
        {
            Label lbl = new Label();
            lbl.Text = labelText;
            lbl.Font = new Font("IRANSans", 10);
            lbl.Size = new Size(80, 25);
            lbl.Location = new Point(x, y);
            lbl.TextAlign = ContentAlignment.MiddleRight;
            parent.Controls.Add(lbl);

            control.Size = new Size(width, 35);
            control.Location = new Point(x + 85, y - 5);
            control.Font = new Font("IRANSans", 10);

            if (control is ComboBox)
                ((ComboBox)control).DropDownStyle = ComboBoxStyle.DropDownList;

            parent.Controls.Add(control);
        }

        private void CreateAmountField(string labelText, Label valueLabel, string value, int x, int y, Panel parent, bool isBold = false)
        {
            Label lbl = new Label();
            lbl.Text = labelText;
            lbl.Font = isBold ? new Font("IRANSans", 12, FontStyle.Bold) : new Font("IRANSans", 11);
            lbl.Size = new Size(130, 30);
            lbl.Location = new Point(x, y);
            lbl.TextAlign = ContentAlignment.MiddleRight;
            parent.Controls.Add(lbl);

            valueLabel.Text = value;
            valueLabel.Font = isBold ? new Font("IRANSans", 14, FontStyle.Bold) : new Font("IRANSans", 12);
            valueLabel.ForeColor = isBold ? Color.Green : Color.Black;
            valueLabel.Size = new Size(150, 30);
            valueLabel.Location = new Point(x + 135, y);
            valueLabel.TextAlign = ContentAlignment.MiddleLeft;
            valueLabel.BorderStyle = BorderStyle.FixedSingle;
            valueLabel.BackColor = Color.FromArgb(250, 250, 250);
            valueLabel.Padding = new Padding(5, 0, 5, 0);
            parent.Controls.Add(valueLabel);

            // اضافه کردن واحد تومان
            Label lblUnit = new Label();
            lblUnit.Text = "تومان";
            lblUnit.Font = new Font("IRANSans", 10);
            lblUnit.Size = new Size(50, 30);
            lblUnit.Location = new Point(x + 290, y);
            lblUnit.TextAlign = ContentAlignment.MiddleRight;
            parent.Controls.Add(lblUnit);
        }

        // ==================== متدهای منطقی ====================

        private string GetConnectionString()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["PetrochemicalSalesDB"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                return "Server=localhost;Database=PetrochemicalSalesDB;Integrated Security=true;TrustServerCertificate=true;";
            }
            return connectionString;
        }

        private void LoadCustomers()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    string query = "SELECT CustomerID, CompanyName, NationalID, Phone, Address FROM Customers";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        DataTable dt = new DataTable();
                        dt.Load(cmd.ExecuteReader());

                        // اضافه کردن آیتم خالی
                        DataRow emptyRow = dt.NewRow();
                        emptyRow["CustomerID"] = 0;
                        emptyRow["CompanyName"] = "-- انتخاب مشتری --";
                        emptyRow["NationalID"] = "";
                        emptyRow["Phone"] = "";
                        emptyRow["Address"] = "";
                        dt.Rows.InsertAt(emptyRow, 0);

                        cmbCustomer.DataSource = dt;
                        cmbCustomer.DisplayMember = "CompanyName";
                        cmbCustomer.ValueMember = "CustomerID";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بارگذاری مشتریان: {ex.Message}", "خطا",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPaymentMethods()
        {
            cmbPaymentMethod.Items.AddRange(new string[]
            {
                "نقدی",
                "کارت بانکی",
                "چک",
                "حواله بانکی",
                "اعتباری"
            });
            cmbPaymentMethod.SelectedIndex = 0;
        }

        private void CmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCustomer.SelectedIndex > 0 && cmbCustomer.SelectedValue != null)
            {
                try
                {
                    DataRowView selectedRow = (DataRowView)cmbCustomer.SelectedItem;
                    txtNationalID.Text = selectedRow["NationalID"].ToString();
                    txtCustomerName.Text = selectedRow["CompanyName"].ToString();
                    //txtCustomerPhone.Text = selectedRow["Phone"].ToString();
                    //txtCustomerAddress.Text = selectedRow["Address"].ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"خطا در بارگذاری اطلاعات مشتری: {ex.Message}", "خطا",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                ClearCustomerFields();
            }
        }

        private void DgvProducts_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvProducts.Rows.Count - 1)
            {
                CalculateLineTotal(e.RowIndex);
                CalculateTotals();
            }
        }

        private void CalculateLineTotal(int rowIndex)
        {
            try
            {
                DataGridViewRow row = dgvProducts.Rows[rowIndex];

                // توجه: ترتیب ستون‌ها تغییر کرده است
                decimal quantity = Convert.ToDecimal(row.Cells["Quantity"].Value ?? 0);
                decimal unitPrice = Convert.ToDecimal(row.Cells["UnitPrice"].Value ?? 0);
                decimal discountPercent = Convert.ToDecimal(row.Cells["DiscountPercent"].Value ?? 0);
                decimal taxPercent = Convert.ToDecimal(row.Cells["TaxPercent"].Value ?? 0);

                decimal lineSubTotal = quantity * unitPrice;
                decimal discountAmount = lineSubTotal * (discountPercent / 100);
                decimal taxAmount = (lineSubTotal - discountAmount) * (taxPercent / 100);
                decimal lineTotal = lineSubTotal - discountAmount + taxAmount;

                row.Cells["DiscountAmount"].Value = Math.Round(discountAmount, 0);
                row.Cells["TaxAmount"].Value = Math.Round(taxAmount, 0);
                row.Cells["LineTotal"].Value = Math.Round(lineTotal, 0);
            }
            catch (Exception)
            {
                // در صورت خطا در تبدیل اعداد
            }
        }

        private void CalculateTotals()
        {
            decimal subTotal = 0;
            decimal discountTotal = 0;
            decimal taxTotal = 0;

            foreach (DataGridViewRow row in dgvProducts.Rows)
            {
                if (row.IsNewRow || row.Cells["LineTotal"].Value == null)
                    continue;

                decimal lineTotal = Convert.ToDecimal(row.Cells["LineTotal"].Value ?? 0);
                decimal lineDiscount = Convert.ToDecimal(row.Cells["DiscountAmount"].Value ?? 0);
                decimal lineTax = Convert.ToDecimal(row.Cells["TaxAmount"].Value ?? 0);
                decimal lineSubTotal = lineTotal + lineDiscount - lineTax;

                subTotal += lineSubTotal;
                discountTotal += lineDiscount;
                taxTotal += lineTax;
            }

            decimal grandTotal = subTotal - discountTotal + taxTotal;

            lblSubTotal.Text = subTotal.ToString("N0");
            lblDiscountTotal.Text = discountTotal.ToString("N0");
            lblTaxTotal.Text = taxTotal.ToString("N0");
            lblGrandTotal.Text = grandTotal.ToString("N0");
        }

        private void BtnLoadProducts_Click(object sender, EventArgs e)
        {
            // داده‌های نمونه برای تست - توجه به ترتیب ستون‌های جدید
            dgvProducts.Rows.Clear();

            object[] sampleData1 = {
                0, 0, 9, 0, 5, // جمع ردیف، مبلغ مالیات، %مالیات، مبلغ تخفیف، %تخفیف
                15000, 1000, "لیتر", "گازوئیل درجه یک", "P001" // قیمت واحد، تعداد، واحد، نام کالا، کد کالا
            };

            object[] sampleData2 = {
                0, 0, 9, 0, 3,
                12000, 500, "لیتر", "نفت سفید", "P002"
            };

            object[] sampleData3 = {
                0, 0, 9, 0, 0,
                20000, 800, "لیتر", "بنزین سوپر", "P003"
            };

            dgvProducts.Rows.Add(sampleData1);
            dgvProducts.Rows.Add(sampleData2);
            dgvProducts.Rows.Add(sampleData3);

            // محاسبه مقادیر
            for (int i = 0; i < dgvProducts.Rows.Count - 1; i++)
            {
                CalculateLineTotal(i);
            }
            CalculateTotals();

            MessageBox.Show("۳ محصول نمونه بارگذاری شد", "اطلاع",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateInvoice())
            {
                try
                {
                    CalculateTotals();
                    int invoiceId = SaveInvoiceToDatabase();

                    if (invoiceId > 0)
                    {
                        MessageBox.Show($"فاکتور با شماره {invoiceId} با موفقیت ثبت شد", "موفقیت",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        ClearForm();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"خطا در ذخیره فاکتور: {ex.Message}", "خطا",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            if (ValidateInvoice())
            {
                MessageBox.Show("امکان چاپ در نسخه بعدی اضافه خواهد شد", "اطلاع",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("آیا می‌خواهید فرم را پاک کنید؟", "تأیید",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ClearForm();
            }
        }

        private bool ValidateInvoice()
        {
            if (cmbCustomer.SelectedIndex <= 0)
            {
                MessageBox.Show("لطفاً مشتری را انتخاب کنید", "خطا",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbCustomer.Focus();
                return false;
            }

            if (dgvProducts.Rows.Count <= 1)
            {
                MessageBox.Show("حداقل یک کالا به فاکتور اضافه کنید", "خطا",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // بررسی اینکه تمام ردیف‌ها دارای کد کالا هستند
            foreach (DataGridViewRow row in dgvProducts.Rows)
            {
                if (row.IsNewRow) continue;

                if (string.IsNullOrEmpty(row.Cells["ProductCode"].Value?.ToString()))
                {
                    MessageBox.Show("کد کالا نمی‌تواند خالی باشد", "خطا",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            return true;
        }

        private int SaveInvoiceToDatabase()
        {
            // کد ذخیره در دیتابیس - باید با توجه به ساختار دیتابیس شما تنظیم شود
            // این یک پیاده‌سازی نمونه است

            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    conn.Open();

                    // 1. ذخیره فاکتور اصلی
                    string insertInvoice = @"
                        INSERT INTO Invoices 
                        (CustomerID, InvoiceDate, TotalAmount, InvoiceNo, CustomerName, 
                         CustomerPhone, CustomerAddress, SubTotal, DiscountAmount, TaxAmount,
                         Status, PaymentMethod, PaymentDate, Notes, CreatedBy, CreatedDate)
                        VALUES 
                        (@CustomerID, @InvoiceDate, @TotalAmount, @InvoiceNo, @CustomerName,
                         @CustomerPhone, @CustomerAddress, @SubTotal, @DiscountAmount, @TaxAmount,
                         @Status, @PaymentMethod, @PaymentDate, @Notes, @CreatedBy, GETDATE());
                        SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(insertInvoice, conn))
                    {
                        cmd.Parameters.AddWithValue("@CustomerID", cmbCustomer.SelectedValue);
                        cmd.Parameters.AddWithValue("@InvoiceDate", dtpInvoiceDate.Value);
                        cmd.Parameters.AddWithValue("@TotalAmount", decimal.Parse(lblGrandTotal.Text.Replace(",", "")));
                        cmd.Parameters.AddWithValue("@InvoiceNo", GenerateInvoiceNumber());
                        cmd.Parameters.AddWithValue("@CustomerName", txtCustomerName.Text);
                        cmd.Parameters.AddWithValue("@CustomerPhone", "");
                        cmd.Parameters.AddWithValue("@CustomerAddress", "");
                        cmd.Parameters.AddWithValue("@SubTotal", decimal.Parse(lblSubTotal.Text.Replace(",", "")));
                        cmd.Parameters.AddWithValue("@DiscountAmount", decimal.Parse(lblDiscountTotal.Text.Replace(",", "")));
                        cmd.Parameters.AddWithValue("@TaxAmount", decimal.Parse(lblTaxTotal.Text.Replace(",", "")));
                        cmd.Parameters.AddWithValue("@Status", chkIsPaid.Checked ? 1 : 0);
                        cmd.Parameters.AddWithValue("@PaymentMethod", cmbPaymentMethod.SelectedItem);
                        cmd.Parameters.AddWithValue("@PaymentDate", chkIsPaid.Checked ? (object)dtpPaymentDate.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@Notes", "");
                        cmd.Parameters.AddWithValue("@CreatedBy", 1); // شناسه کاربر جاری

                        int invoiceId = Convert.ToInt32(cmd.ExecuteScalar());

                        // 2. ذخیره آیتم‌های فاکتور
                        SaveInvoiceItems(conn, invoiceId);

                        return invoiceId;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"خطا در ذخیره دیتابیس: {ex.Message}");
            }
        }

        private string GenerateInvoiceNumber()
        {
            return $"INV-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";
        }

        private void SaveInvoiceItems(SqlConnection conn, int invoiceId)
        {
            foreach (DataGridViewRow row in dgvProducts.Rows)
            {
                if (row.IsNewRow || row.Cells["ProductCode"].Value == null)
                    continue;

                string insertItem = @"
                    INSERT INTO InvoiceItems 
                    (InvoiceID, ProductCode, ProductName, Unit, Quantity, UnitPrice,
                     DiscountPercent, DiscountAmount, TaxPercent, TaxAmount)
                    VALUES 
                    (@InvoiceID, @ProductCode, @ProductName, @Unit, @Quantity, @UnitPrice,
                     @DiscountPercent, @DiscountAmount, @TaxPercent, @TaxAmount)";

                using (SqlCommand cmd = new SqlCommand(insertItem, conn))
                {
                    cmd.Parameters.AddWithValue("@InvoiceID", invoiceId);
                    cmd.Parameters.AddWithValue("@ProductCode", row.Cells["ProductCode"].Value);
                    cmd.Parameters.AddWithValue("@ProductName", row.Cells["ProductName"].Value);
                    cmd.Parameters.AddWithValue("@Unit", row.Cells["Unit"].Value);
                    cmd.Parameters.AddWithValue("@Quantity", row.Cells["Quantity"].Value);
                    cmd.Parameters.AddWithValue("@UnitPrice", row.Cells["UnitPrice"].Value);
                    cmd.Parameters.AddWithValue("@DiscountPercent", row.Cells["DiscountPercent"].Value);
                    cmd.Parameters.AddWithValue("@DiscountAmount", row.Cells["DiscountAmount"].Value);
                    cmd.Parameters.AddWithValue("@TaxPercent", row.Cells["TaxPercent"].Value);
                    cmd.Parameters.AddWithValue("@TaxAmount", row.Cells["TaxAmount"].Value);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void ClearCustomerFields()
        {
            txtNationalID.Clear();
            txtCustomerName.Clear();
            //txtCustomerPhone.Clear();
            //txtCustomerAddress.Clear();
        }

        private void ClearForm()
        {
            cmbCustomer.SelectedIndex = 0;
            ClearCustomerFields();
            dgvProducts.Rows.Clear();
            //txtNotes.Clear();
            lblSubTotal.Text = "0";
            lblDiscountTotal.Text = "0";
            lblTaxTotal.Text = "0";
            lblGrandTotal.Text = "0";
            chkIsPaid.Checked = false;
            cmbPaymentMethod.SelectedIndex = 0;
            dtpInvoiceDate.Value = DateTime.Now;
            dtpPaymentDate.Value = DateTime.Now.AddDays(30);
            dtpPaymentDate.Enabled = false;
        }
    }
}