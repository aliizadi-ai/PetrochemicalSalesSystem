using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PetrochemicalSalesSystem.Forms
{
    public partial class InvoiceDetailForm : Form
    {
        private int invoiceId;
        private int customerID;
        private DataGridView dgvItems;
        private Panel mainPanel;

        // کنترل‌های اطلاعات فاکتور
        private Label lblInvoiceNo, lblInvoiceDate, lblCustomerName, lblCustomerCode,
                      lblNationalCode, lblPhone, lblAddress, lblSubTotal, lblDiscount,
                      lblTax, lblGrandTotal, lblPaymentMethod, lblPaymentDate,
                      lblStatus, lblNotes;

        public InvoiceDetailForm(int invoiceId)
        {
            InitializeComponent();
            this.invoiceId = invoiceId;
            MessageBox.Show(this.invoiceId.ToString());
            InitializeInvoiceDetailForm();
            LoadInvoiceDetails();
            LoadInvoiceItems();
        }

        private void InitializeInvoiceDetailForm()
        {
            this.Size = new Size(1100, 750);
            this.Text = "جزئیات فاکتور";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.BackColor = Color.FromArgb(240, 242, 245);
            this.Font = new Font("Tahoma", 9.75f, FontStyle.Regular);

            CreateHeader();
            CreateInvoiceInfoSection();
            CreateCustomerInfoSection();
            CreateItemsGridSection();
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
            title.Text = "🧾 جزئیات فاکتور";
            title.Font = new Font("Tahoma", 16, FontStyle.Bold);
            title.ForeColor = Color.White;
            title.Dock = DockStyle.Fill;
            title.TextAlign = ContentAlignment.MiddleCenter;

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
            this.Controls.Add(headerPanel);
        }

        private void CreateInvoiceInfoSection()
        {
            Panel infoPanel = new Panel();
            infoPanel.Size = new Size(1050, 150);
            infoPanel.Location = new Point(25, 85);
            infoPanel.BackColor = Color.White;
            infoPanel.BorderStyle = BorderStyle.FixedSingle;
            infoPanel.Padding = new Padding(10);

            Label sectionTitle = new Label();
            sectionTitle.Text = "📋 اطلاعات فاکتور";
            sectionTitle.Font = new Font("Tahoma", 12, FontStyle.Bold);
            sectionTitle.ForeColor = Color.FromArgb(0, 102, 51);
            sectionTitle.Size = new Size(200, 30);
            sectionTitle.Location = new Point(20, 15);
            infoPanel.Controls.Add(sectionTitle);

            int y = 50;
            int x1 = 30, x2 = 280, x3 = 530, x4 = 780;

            // ردیف اول
            CreateDetailField("شماره فاکتور:", lblInvoiceNo = new Label(), x1, y, infoPanel);
            CreateDetailField("تاریخ فاکتور:", lblInvoiceDate = new Label(), x2, y, infoPanel);
            CreateDetailField("روش پرداخت:", lblPaymentMethod = new Label(), x3, y, infoPanel);

            // ردیف دوم
            y += 40;
            CreateDetailField("وضعیت:", lblStatus = new Label(), x1, y, infoPanel);
            CreateDetailField("تاریخ پرداخت:", lblPaymentDate = new Label(), x2, y, infoPanel);

            // ردیف سوم - مبالغ
            y += 40;
            CreateDetailField("جمع کل:", lblSubTotal = new Label(), x1, y, infoPanel);
            CreateDetailField("تخفیف:", lblDiscount = new Label(), x2, y, infoPanel);
            CreateDetailField("مالیات:", lblTax = new Label(), x3, y, infoPanel);
            CreateDetailField("مبلغ قابل پرداخت:", lblGrandTotal = new Label(), x4, y, infoPanel);
            lblGrandTotal.ForeColor = Color.Green;
            lblGrandTotal.Font = new Font("Tahoma", 11, FontStyle.Bold);

            this.Controls.Add(infoPanel);
        }

        private void CreateCustomerInfoSection()
        {
            Panel customerPanel = new Panel();
            customerPanel.Size = new Size(1050, 130);
            customerPanel.Location = new Point(25, 250);
            customerPanel.BackColor = Color.White;
            customerPanel.BorderStyle = BorderStyle.FixedSingle;
            customerPanel.Padding = new Padding(10);

            Label sectionTitle = new Label();
            sectionTitle.Text = "👤 اطلاعات مشتری";
            sectionTitle.Font = new Font("Tahoma", 12, FontStyle.Bold);
            sectionTitle.ForeColor = Color.FromArgb(0, 102, 51);
            sectionTitle.Size = new Size(200, 30);
            sectionTitle.Location = new Point(20, 15);
            customerPanel.Controls.Add(sectionTitle);

            int y = 50;
            int x1 = 30, x2 = 280, x3 = 530;

            // ردیف اول
            CreateDetailField("نام مشتری:", lblCustomerName = new Label(), x1, y, customerPanel);
            CreateDetailField("کد مشتری:", lblCustomerCode = new Label(), x2, y, customerPanel);
            CreateDetailField("کد ملی:", lblNationalCode = new Label(), x3, y, customerPanel);

            // ردیف دوم
            y += 40;
            CreateDetailField("تلفن:", lblPhone = new Label(), x1, y, customerPanel);
            CreateDetailField("آدرس:", lblAddress = new Label(), x2, y, customerPanel, 500);

            this.Controls.Add(customerPanel);
        }

        private void CreateItemsGridSection()
        {
            Panel itemsPanel = new Panel();
            itemsPanel.Size = new Size(1050, 300);
            itemsPanel.Location = new Point(25, 395);
            itemsPanel.BackColor = Color.White;
            itemsPanel.BorderStyle = BorderStyle.FixedSingle;
            itemsPanel.Padding = new Padding(10);

            Label sectionTitle = new Label();
            sectionTitle.Text = "📦 آیتم‌های فاکتور";
            sectionTitle.Font = new Font("Tahoma", 12, FontStyle.Bold);
            sectionTitle.ForeColor = Color.FromArgb(0, 102, 51);
            sectionTitle.Size = new Size(200, 30);
            sectionTitle.Location = new Point(20, 15);
            itemsPanel.Controls.Add(sectionTitle);

            // DataGridView برای آیتم‌ها
            dgvItems = new DataGridView();
            dgvItems.Size = new Size(1010, 220);
            dgvItems.Location = new Point(20, 55);
            dgvItems.BackgroundColor = Color.White;
            dgvItems.BorderStyle = BorderStyle.FixedSingle;
            dgvItems.RowHeadersVisible = false;
            dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvItems.AllowUserToAddRows = false;
            dgvItems.AllowUserToDeleteRows = false;
            dgvItems.ReadOnly = true;
            dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvItems.ScrollBars = ScrollBars.Both;

            // استایل‌دهی
            dgvItems.EnableHeadersVisualStyles = false;
            dgvItems.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 102, 51);
            dgvItems.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvItems.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 10, FontStyle.Bold);
            dgvItems.ColumnHeadersHeight = 40;
            dgvItems.RowTemplate.Height = 35;
            dgvItems.DefaultCellStyle.Font = new Font("Tahoma", 9);
            dgvItems.DefaultCellStyle.Padding = new Padding(5);

            itemsPanel.Controls.Add(dgvItems);
            this.Controls.Add(itemsPanel);
        }

        private void CreateButtonsSection()
        {
            Panel buttonPanel = new Panel();
            buttonPanel.Size = new Size(1050, 60);
            buttonPanel.Location = new Point(25, 710);
            buttonPanel.BackColor = Color.Transparent;

            Button btnPrint = new Button();
            btnPrint.Text = "🖨️ چاپ فاکتور";
            btnPrint.Size = new Size(150, 45);
            btnPrint.Location = new Point(700, 0);
            btnPrint.Font = new Font("Tahoma", 11);
            btnPrint.BackColor = Color.FromArgb(52, 152, 219);
            btnPrint.ForeColor = Color.White;
            btnPrint.FlatStyle = FlatStyle.Flat;
            btnPrint.FlatAppearance.BorderSize = 0;
            btnPrint.Cursor = Cursors.Hand;
            btnPrint.Click += BtnPrint_Click;

            Button btnExportPDF = new Button();
            btnExportPDF.Text = "📄 خروجی PDF";
            btnExportPDF.Size = new Size(150, 45);
            btnExportPDF.Location = new Point(530, 0);
            btnExportPDF.Font = new Font("Tahoma", 11);
            btnExportPDF.BackColor = Color.FromArgb(231, 76, 60);
            btnExportPDF.ForeColor = Color.White;
            btnExportPDF.FlatStyle = FlatStyle.Flat;
            btnExportPDF.FlatAppearance.BorderSize = 0;
            btnExportPDF.Cursor = Cursors.Hand;
            btnExportPDF.Click += BtnExportPDF_Click;

            Button btnClose = new Button();
            btnClose.Text = "❌ بستن";
            btnClose.Size = new Size(150, 45);
            btnClose.Location = new Point(360, 0);
            btnClose.Font = new Font("Tahoma", 11);
            btnClose.BackColor = Color.FromArgb(155, 89, 182);
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += (s, e) => this.Close();

            buttonPanel.Controls.Add(btnPrint);
            buttonPanel.Controls.Add(btnExportPDF);
            buttonPanel.Controls.Add(btnClose);
            this.Controls.Add(buttonPanel);
        }

        private void CreateDetailField(string labelText, Label valueLabel, int x, int y, Panel parent, int width = 200)
        {
            Label lbl = new Label();
            lbl.Text = labelText;
            lbl.Font = new Font("Tahoma", 10);
            lbl.Size = new Size(100, 25);
            lbl.Location = new Point(x, y);
            lbl.TextAlign = ContentAlignment.MiddleRight;
            parent.Controls.Add(lbl);

            valueLabel.Text = "";
            valueLabel.Font = new Font("Tahoma", 10);
            valueLabel.Size = new Size(width, 25);
            valueLabel.Location = new Point(x + 105, y);
            valueLabel.TextAlign = ContentAlignment.MiddleLeft;
            valueLabel.BorderStyle = BorderStyle.FixedSingle;
            valueLabel.BackColor = Color.FromArgb(250, 250, 250);
            valueLabel.Padding = new Padding(5, 0, 5, 0);
            parent.Controls.Add(valueLabel);
        }

        private string GetConnectionString()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["PetrochemicalSalesDB"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                return "Server=localhost;Database=PetrochemicalSalesDB;Integrated Security=true;TrustServerCertificate=true;";
            }
            return connectionString;
        }

        private void LoadInvoiceDetails()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    string q1 = @"SELECT * FROM Invoices";

                    string query = @"SELECT *
                                    FROM 
                                    Invoices  i
                                    INNER JOIN 
                                    Customers c ON i.CustomerID = c.CustomerID
                                    INNER JOIN 
                                    InvoiceItems t ON i.InvoiceID = t.InvoiceID";

                    string q2 = @"SELECT 
                            i.*,
                            c.CompanyName,
                            c.CustomerID,
                            c.NationalID,
                            c.Phone,
                            c.Address,
                        FROM Invoices i
                        LEFT JOIN Customers c ON i.CustomerID = c.CustomerID
                        LEFT JOIN InvoiceItems t ON t.InvoiceID = i.InvoiceID
                        WHERE i.InvoiceID = @InvoiceID";

                    string q3 = @"SELECT * FROM Invoices
                                    UNION ALL
                                    SELECT * FROM InvoiceItems
                                    UNION ALL
                                    SELECT * FROM Customers";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@InvoiceID", invoiceId);
                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // پر کردن اطلاعات فاکتور
                                lblInvoiceNo.Text = reader["InvoiceNo"].ToString();
                                lblInvoiceDate.Text = Convert.ToDateTime(reader["InvoiceDate"]).ToString("yyyy/MM/dd");
                                lblPaymentMethod.Text = reader["PaymentMethod"].ToString();
                                lblPaymentDate.Text = reader["PaymentDate"] != DBNull.Value ?
                                    Convert.ToDateTime(reader["PaymentDate"]).ToString("yyyy/MM/dd") : "---";

                                // وضعیت
                                string status = reader["Status"].ToString();
                                lblStatus.Text = GetStatusText(status);
                                lblStatus.ForeColor = GetStatusColor(status);

                                // مبالغ
                                lblSubTotal.Text = Convert.ToDecimal(reader["SubTotal"]).ToString("N0") + " تومان";
                                lblDiscount.Text = Convert.ToDecimal(reader["DiscountAmount"]).ToString("N0") + " تومان";
                                lblTax.Text = Convert.ToDecimal(reader["TaxAmount"]).ToString("N0") + " تومان";
                                lblGrandTotal.Text = Convert.ToDecimal(reader["TotalAmount"]).ToString("N0") + " تومان";

                                // اطلاعات مشتری
                                lblCustomerName.Text = reader["CompanyName"].ToString();
                                lblCustomerCode.Text = reader["CustomerID"].ToString();
                                lblNationalCode.Text = reader["NationalID"].ToString();
                                lblPhone.Text = reader["Phone"].ToString();
                                lblAddress.Text = reader["Address"].ToString();

                                // یادداشت
                                if (!string.IsNullOrEmpty(reader["Notes"].ToString()))
                                {
                                    lblNotes = new Label();
                                    lblNotes.Text = "یادداشت: " + reader["Notes"].ToString();
                                    lblNotes.Font = new Font("Tahoma", 10);
                                    lblNotes.Size = new Size(400, 50);
                                    lblNotes.Location = new Point(25, 640);
                                    lblNotes.TextAlign = ContentAlignment.MiddleRight;
                                    this.Controls.Add(lblNotes);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بارگذاری اطلاعات فاکتور: {ex.Message}", "خطا",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetStatusText(string status)
        {
            switch (status)
            {
                case "1": return "✅ پرداخت شده";
                case "0": return "⏳ پرداخت نشده";
                case "2": return "💰 بخشی پرداخت شده";
                case "3": return "❌ لغو شده";
                default: return "نامشخص";
            }
        }

        private Color GetStatusColor(string status)
        {
            switch (status)
            {
                case "1": return Color.Green;
                case "0": return Color.Red;
                case "2": return Color.Orange;
                case "3": return Color.Gray;
                default: return Color.Black;
            }
        }

        private void LoadInvoiceItems()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    string query = @"
                        SELECT 
                            ProductCode,
                            ProductName,
                            Unit,
                            Quantity,
                            UnitPrice,
                            DiscountPercent,
                            DiscountAmount,
                            TaxPercent,
                            TaxAmount,
                            LineTotal
                        FROM InvoiceItems
                        WHERE InvoiceID = @InvoiceID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@InvoiceID", invoiceId);
                        conn.Open();

                        DataTable dt = new DataTable();
                        dt.Load(cmd.ExecuteReader());

                        // نمایش در DataGridView
                        DisplayInvoiceItems(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بارگذاری آیتم‌های فاکتور: {ex.Message}", "خطا",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayInvoiceItems(DataTable data)
        {
            dgvItems.DataSource = data;

            // تنظیم نام ستون‌های فارسی
            if (dgvItems.Columns.Count > 0)
            {
                string[] persianColumns = {
                    "کد کالا", "نام کالا", "واحد", "تعداد", "قیمت واحد",
                    "تخفیف %", "مبلغ تخفیف", "مالیات %", "مبلغ مالیات", "جمع ردیف"
                };

                for (int i = 0; i < dgvItems.Columns.Count && i < persianColumns.Length; i++)
                {
                    dgvItems.Columns[i].HeaderText = persianColumns[i];
                }

                // فرمت‌دهی ستون‌های عددی
                string[] numericColumns = { "قیمت واحد", "مبلغ تخفیف", "مبلغ مالیات", "جمع ردیف" };
                string[] percentColumns = { "تخفیف %", "مالیات %" };

                foreach (DataGridViewColumn column in dgvItems.Columns)
                {
                    if (numericColumns.Contains(column.HeaderText))
                    {
                        column.DefaultCellStyle.Format = "N0";
                        column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                    else if (percentColumns.Contains(column.HeaderText))
                    {
                        column.DefaultCellStyle.Format = "N2";
                        column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                    else if (column.HeaderText == "تعداد")
                    {
                        column.DefaultCellStyle.Format = "N2";
                        column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                }

                // محاسبه جمع کل ردیف‌ها
                decimal itemsTotal = 0;
                foreach (DataRow row in data.Rows)
                {
                    itemsTotal += Convert.ToDecimal(row["LineTotal"]);
                }

                // نمایش جمع کل آیتم‌ها
                Label lblItemsTotal = new Label();
                lblItemsTotal.Text = $"جمع کل آیتم‌ها: {itemsTotal.ToString("N0")} تومان";
                lblItemsTotal.Font = new Font("Tahoma", 11, FontStyle.Bold);
                lblItemsTotal.ForeColor = Color.FromArgb(0, 102, 51);
                lblItemsTotal.Size = new Size(250, 30);
                lblItemsTotal.Location = new Point(25, 655);
                lblItemsTotal.TextAlign = ContentAlignment.MiddleRight;
                this.Controls.Add(lblItemsTotal);
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            MessageBox.Show("امکان چاپ در نسخه بعدی اضافه خواهد شد", "اطلاع",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnExportPDF_Click(object sender, EventArgs e)
        {
            MessageBox.Show("امکان خروجی PDF در نسخه بعدی اضافه خواهد شد", "اطلاع",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}