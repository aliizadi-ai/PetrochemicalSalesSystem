using System;
using System.Drawing;
using System.Windows.Forms;

namespace PetrochemicalSalesSystem.Forms
{
    public partial class InvoiceForm : Form
    {
        public InvoiceForm()
        {
            InitializeComponent();
            InitializeInvoiceForm();
        }

        private void InitializeInvoiceForm()
        {
            this.Size = new Size(900, 600);
            this.BackColor = Color.White;

            CreateInvoiceHeader();
            CreateCustomerSection();
            CreateProductsGrid();
            CreateTotalsSection();
            CreateButtons();
        }

        private void CreateInvoiceHeader()
        {
            Panel headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 80;
            headerPanel.BackColor = Color.FromArgb(0, 102, 51);

            Label title = new Label();
            title.Text = "🧾 ثبت فاکتور جدید";
            title.Font = new Font("B Nazanin", 18, FontStyle.Bold);
            title.ForeColor = Color.White;
            title.Dock = DockStyle.Fill;
            title.TextAlign = ContentAlignment.MiddleCenter;

            headerPanel.Controls.Add(title);
            this.Controls.Add(headerPanel);
        }

        private void CreateCustomerSection()
        {
            GroupBox customerGroup = new GroupBox();
            customerGroup.Text = "اطلاعات مشتری";
            customerGroup.Font = new Font("B Nazanin", 11, FontStyle.Bold);
            customerGroup.Size = new Size(850, 120);
            customerGroup.Location = new Point(20, 100);

            // فیلدهای مشتری
            CreateLabelAndTextBox("کد مشتری:", "C001", 20, 30, customerGroup);
            CreateLabelAndTextBox("نام مشتری:", "", 250, 30, customerGroup);
            CreateLabelAndTextBox("شماره تماس:", "", 480, 30, customerGroup);
            CreateLabelAndTextBox("آدرس:", "", 20, 70, customerGroup, 600);

            this.Controls.Add(customerGroup);
        }

        private void CreateProductsGrid()
        {
            GroupBox productsGroup = new GroupBox();
            productsGroup.Text = "لیست کالاها";
            productsGroup.Font = new Font("B Nazanin", 11, FontStyle.Bold);
            productsGroup.Size = new Size(850, 250);
            productsGroup.Location = new Point(20, 240);

            // DataGridView برای کالاها
            DataGridView dgvProducts = new DataGridView();
            dgvProducts.Size = new Size(810, 180);
            dgvProducts.Location = new Point(20, 40);
            dgvProducts.BackgroundColor = Color.White;
            dgvProducts.RowHeadersVisible = false;

            // ستون‌ها
            dgvProducts.Columns.Add("ProductCode", "کد کالا");
            dgvProducts.Columns.Add("ProductName", "نام کالا");
            dgvProducts.Columns.Add("Unit", "واحد");
            dgvProducts.Columns.Add("Quantity", "تعداد");
            dgvProducts.Columns.Add("UnitPrice", "قیمت واحد");
            dgvProducts.Columns.Add("Discount", "تخفیف (%)");
            dgvProducts.Columns.Add("Total", "جمع");

            // دکمه‌های مدیریت
            Button btnAddProduct = new Button();
            btnAddProduct.Text = "➕ افزودن کالا";
            btnAddProduct.Size = new Size(120, 30);
            btnAddProduct.Location = new Point(20, 230);
            btnAddProduct.Font = new Font("B Nazanin", 10);
            btnAddProduct.BackColor = Color.FromArgb(52, 152, 219);
            btnAddProduct.ForeColor = Color.White;
            btnAddProduct.Click += (s, e) =>
            {
                dgvProducts.Rows.Add("", "", "عدد", "1", "0", "0", "0");
            };

            productsGroup.Controls.Add(dgvProducts);
            productsGroup.Controls.Add(btnAddProduct);
            this.Controls.Add(productsGroup);
        }

        private void CreateTotalsSection()
        {
            Panel totalsPanel = new Panel();
            totalsPanel.Size = new Size(300, 150);
            totalsPanel.Location = new Point(570, 500);
            totalsPanel.BackColor = Color.FromArgb(250, 250, 250);
            totalsPanel.BorderStyle = BorderStyle.FixedSingle;

            // مبالغ
            CreateTotalRow("جمع کل:", "0 تومان", 20, totalsPanel);
            CreateTotalRow("تخفیف:", "0 تومان", 50, totalsPanel);
            CreateTotalRow("مالیات (9%):", "0 تومان", 80, totalsPanel);
            CreateTotalRow("مبلغ قابل پرداخت:", "0 تومان", 110, totalsPanel, true);

            this.Controls.Add(totalsPanel);
        }

        private void CreateButtons()
        {
            Panel buttonPanel = new Panel();
            buttonPanel.Size = new Size(850, 60);
            buttonPanel.Location = new Point(20, 660);

            Button btnSave = new Button();
            btnSave.Text = "💾 ذخیره فاکتور";
            btnSave.Size = new Size(150, 40);
            btnSave.Location = new Point(600, 10);
            btnSave.Font = new Font("B Nazanin", 12, FontStyle.Bold);
            btnSave.BackColor = Color.FromArgb(46, 204, 113);
            btnSave.ForeColor = Color.White;
            btnSave.Click += BtnSave_Click;

            Button btnPrint = new Button();
            btnPrint.Text = "🖨️ چاپ فاکتور";
            btnPrint.Size = new Size(150, 40);
            btnPrint.Location = new Point(430, 10);
            btnPrint.Font = new Font("B Nazanin", 12, FontStyle.Bold);
            btnPrint.BackColor = Color.FromArgb(52, 152, 219);
            btnPrint.ForeColor = Color.White;

            Button btnCancel = new Button();
            btnCancel.Text = "❌ انصراف";
            btnCancel.Size = new Size(150, 40);
            btnCancel.Location = new Point(260, 10);
            btnCancel.Font = new Font("B Nazanin", 12);
            btnCancel.BackColor = Color.FromArgb(231, 76, 60);
            btnCancel.ForeColor = Color.White;
            btnCancel.Click += (s, e) => this.Close();

            buttonPanel.Controls.Add(btnSave);
            buttonPanel.Controls.Add(btnPrint);
            buttonPanel.Controls.Add(btnCancel);
            this.Controls.Add(buttonPanel);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            MessageBox.Show("فاکتور با موفقیت ذخیره شد!", "ذخیره موفق",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // متدهای کمکی
        private void CreateLabelAndTextBox(string labelText, string placeholder, int x, int y, Control parent, int width = 200)
        {
            Label lbl = new Label();
            lbl.Text = labelText;
            lbl.Font = new Font("B Nazanin", 10);
            lbl.Size = new Size(80, 25);
            lbl.Location = new Point(x, y);
            parent.Controls.Add(lbl);

            TextBox txt = new TextBox();
            txt.Size = new Size(width, 30);
            txt.Location = new Point(x + 90, y);
            txt.Font = new Font("B Nazanin", 10);
            txt.Text = placeholder;
            parent.Controls.Add(txt);
        }

        private void CreateTotalRow(string label, string value, int y, Panel parent, bool isBold = false)
        {
            Label lbl = new Label();
            lbl.Text = label;
            lbl.Font = isBold ? new Font("B Nazanin", 12, FontStyle.Bold) : new Font("B Nazanin", 11);
            lbl.Size = new Size(120, 25);
            lbl.Location = new Point(20, y);
            parent.Controls.Add(lbl);

            Label lblValue = new Label();
            lblValue.Text = value;
            lblValue.Font = isBold ? new Font("B Nazanin", 14, FontStyle.Bold) : new Font("B Nazanin", 11);
            lblValue.ForeColor = isBold ? Color.Green : Color.Black;
            lblValue.Size = new Size(150, 25);
            lblValue.Location = new Point(140, y);
            lblValue.TextAlign = ContentAlignment.MiddleLeft;
            parent.Controls.Add(lblValue);
        }
    }
}