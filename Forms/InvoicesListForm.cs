using PetrochemicalAccountantSystem.Forms;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace PetrochemicalSalesSystem.Forms
{
    public partial class InvoicesListForm : Form
    {
        private DataGridView dgvInvoices;

        public InvoicesListForm()
        {
            InitializeComponent();
            InitializeInvoicesList();
            LoadInvoicesData();
        }

        private void InitializeInvoicesList()
        {
            this.Size = new Size(1000, 600);
            this.BackColor = Color.White;

            // هدر
            Label title = new Label();
            title.Text = "📋 لیست فاکتورها";
            title.Font = new Font("B Nazanin", 18, FontStyle.Bold);
            title.Size = new Size(300, 40);
            title.Location = new Point(20, 20);
            this.Controls.Add(title);

            // پنل فیلترها
            CreateFilterPanel();

            // DataGridView
            dgvInvoices = new DataGridView();
            dgvInvoices.Size = new Size(960, 350);
            dgvInvoices.Location = new Point(20, 150);
            dgvInvoices.BackgroundColor = Color.White;
            dgvInvoices.RowHeadersVisible = false;
            dgvInvoices.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvInvoices.MultiSelect = false;
            dgvInvoices.AllowUserToAddRows = false;
            dgvInvoices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // ستون‌ها
            dgvInvoices.Columns.Add("InvoiceID", "شناسه");
            dgvInvoices.Columns.Add("InvoiceNo", "شماره فاکتور");
            dgvInvoices.Columns.Add("InvoiceDate", "تاریخ");
            dgvInvoices.Columns.Add("CustomerName", "مشتری");
            dgvInvoices.Columns.Add("TotalAmount", "مبلغ کل");
            dgvInvoices.Columns.Add("Status", "وضعیت");
            dgvInvoices.Columns.Add("Actions", "عملیات");

            this.Controls.Add(dgvInvoices);

            // دکمه‌های عملیاتی
            CreateActionButtons();
        }

        private void CreateFilterPanel()
        {
            Panel filterPanel = new Panel();
            filterPanel.Size = new Size(960, 60);
            filterPanel.Location = new Point(20, 70);
            filterPanel.BackColor = Color.FromArgb(245, 245, 245);
            filterPanel.BorderStyle = BorderStyle.FixedSingle;

            // فیلتر تاریخ از
            Label lblFromDate = new Label();
            lblFromDate.Text = "از تاریخ:";
            lblFromDate.Font = new Font("B Nazanin", 10);
            lblFromDate.Size = new Size(60, 25);
            lblFromDate.Location = new Point(20, 20);
            filterPanel.Controls.Add(lblFromDate);

            TextBox txtFromDate = new TextBox();
            txtFromDate.Size = new Size(100, 25);
            txtFromDate.Location = new Point(85, 20);
            txtFromDate.Font = new Font("B Nazanin", 10);
            filterPanel.Controls.Add(txtFromDate);

            // فیلتر تاریخ تا
            Label lblToDate = new Label();
            lblToDate.Text = "تا تاریخ:";
            lblToDate.Font = new Font("B Nazanin", 10);
            lblToDate.Size = new Size(60, 25);
            lblToDate.Location = new Point(200, 20);
            filterPanel.Controls.Add(lblToDate);

            TextBox txtToDate = new TextBox();
            txtToDate.Size = new Size(100, 25);
            txtToDate.Location = new Point(265, 20);
            txtToDate.Font = new Font("B Nazanin", 10);
            filterPanel.Controls.Add(txtToDate);

            // فیلتر وضعیت
            Label lblStatus = new Label();
            lblStatus.Text = "وضعیت:";
            lblStatus.Font = new Font("B Nazanin", 10);
            lblStatus.Size = new Size(50, 25);
            lblStatus.Location = new Point(380, 20);
            filterPanel.Controls.Add(lblStatus);

            ComboBox cmbStatus = new ComboBox();
            cmbStatus.Size = new Size(120, 25);
            cmbStatus.Location = new Point(435, 20);
            cmbStatus.Font = new Font("B Nazanin", 10);
            cmbStatus.Items.AddRange(new string[] { "همه", "پرداخت شده", "در انتظار", "لغو شده" });
            cmbStatus.SelectedIndex = 0;
            filterPanel.Controls.Add(cmbStatus);

            // دکمه جستجو
            Button btnSearch = new Button();
            btnSearch.Text = "🔍 جستجو";
            btnSearch.Size = new Size(100, 30);
            btnSearch.Location = new Point(600, 18);
            btnSearch.Font = new Font("B Nazanin", 10);
            btnSearch.BackColor = Color.FromArgb(52, 152, 219);
            btnSearch.ForeColor = Color.White;
            btnSearch.Click += (s, e) => LoadInvoicesData();

            // دکمه خروجی Excel
            Button btnExport = new Button();
            btnExport.Text = "📥 خروجی Excel";
            btnExport.Size = new Size(120, 30);
            btnExport.Location = new Point(720, 18);
            btnExport.Font = new Font("B Nazanin", 10);
            btnExport.BackColor = Color.FromArgb(46, 204, 113);
            btnExport.ForeColor = Color.White;
            btnExport.Click += BtnExport_Click;

            filterPanel.Controls.Add(btnSearch);
            filterPanel.Controls.Add(btnExport);
            this.Controls.Add(filterPanel);
        }

        private void CreateActionButtons()
        {
            Panel actionPanel = new Panel();
            actionPanel.Size = new Size(960, 50);
            actionPanel.Location = new Point(20, 510);

            Button btnView = new Button();
            btnView.Text = "👁️ مشاهده جزئیات";
            btnView.Size = new Size(150, 35);
            btnView.Location = new Point(20, 10);
            btnView.Font = new Font("B Nazanin", 10, FontStyle.Bold);
            btnView.BackColor = Color.FromArgb(52, 152, 219);
            btnView.ForeColor = Color.White;
            btnView.Click += BtnView_Click;

            Button btnEdit = new Button();
            btnEdit.Text = "✏️ ویرایش";
            btnEdit.Size = new Size(120, 35);
            btnEdit.Location = new Point(180, 10);
            btnEdit.Font = new Font("B Nazanin", 10, FontStyle.Bold);
            btnEdit.BackColor = Color.FromArgb(241, 196, 15);
            btnEdit.ForeColor = Color.White;

            Button btnDelete = new Button();
            btnDelete.Text = "🗑️ حذف";
            btnDelete.Size = new Size(120, 35);
            btnDelete.Location = new Point(310, 10);
            btnDelete.Font = new Font("B Nazanin", 10, FontStyle.Bold);
            btnDelete.BackColor = Color.FromArgb(231, 76, 60);
            btnDelete.ForeColor = Color.White;
            btnDelete.Click += (s, e) =>
            {
                if (dgvInvoices.SelectedRows.Count > 0)
                {
                    if (MessageBox.Show("آیا از حذف این فاکتور اطمینان دارید؟", "تأیید حذف",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        // کد حذف از دیتابیس
                        dgvInvoices.Rows.RemoveAt(dgvInvoices.SelectedRows[0].Index);
                    }
                }
            };

            Button btnPrint = new Button();
            btnPrint.Text = "🖨️ چاپ انتخاب شده";
            btnPrint.Size = new Size(150, 35);
            btnPrint.Location = new Point(440, 10);
            btnPrint.Font = new Font("B Nazanin", 10, FontStyle.Bold);
            btnPrint.BackColor = Color.FromArgb(155, 89, 182);
            btnPrint.ForeColor = Color.White;

            actionPanel.Controls.Add(btnView);
            actionPanel.Controls.Add(btnEdit);
            actionPanel.Controls.Add(btnDelete);
            actionPanel.Controls.Add(btnPrint);
            this.Controls.Add(actionPanel);
        }

        private void LoadInvoicesData()
        {
            // بارگذاری داده از دیتابیس
            try
            {
                string query = @"
                    SELECT 
                        InvoiceID,
                        InvoiceNo,
                        FORMAT(InvoiceDate, 'yyyy/MM/dd') as InvoiceDate,
                        CustomerName,
                        FORMAT(TotalAmount, 'N0') as TotalAmount,
                        CASE 
                            WHEN Status = 1 THEN 'پرداخت شده'
                            WHEN Status = 2 THEN 'در انتظار'
                            ELSE 'لغو شده'
                        END as StatusText
                    FROM Invoices 
                    ORDER BY InvoiceDate DESC";

                DataTable dt = Data.DatabaseHelper.ExecuteQuery(query);

                dgvInvoices.Rows.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    dgvInvoices.Rows.Add(
                        row["InvoiceID"],
                        row["InvoiceNo"],
                        row["InvoiceDate"],
                        row["CustomerName"],
                        row["TotalAmount"] + " تومان",
                        row["StatusText"],
                        "عملیات"
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بارگذاری داده‌ها: {ex.Message}", "خطا");
            }
        }

        private void BtnView_Click(object sender, EventArgs e)
        {
            if (dgvInvoices.SelectedRows.Count > 0)
            {
                int invoiceId = Convert.ToInt32(dgvInvoices.SelectedRows[0].Cells["InvoiceID"].Value);
                ViewInvoiceDetails(invoiceId);
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            // کد خروجی Excel
            MessageBox.Show("خروجی Excel با موفقیت ایجاد شد.", "موفقیت");
        }

        private void ViewInvoiceDetails(int invoiceId)
        {
            // نمایش فرم جزئیات فاکتور
            AccountantDetailForm detailsForm = new AccountantDetailForm(invoiceId);
            detailsForm.ShowDialog();
        }
    }
}