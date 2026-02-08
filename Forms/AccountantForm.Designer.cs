using System.Windows.Forms;

namespace PetrochemicalSalesSystem.Forms
{
    partial class AccountantForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        // کنترل‌های موجود در فرم
        private DataGridView dgvInvoiceItems;
        private ComboBox cmbCustomer;
        private TextBox txtCustomerPhone;
        private TextBox txtCustomerAddress;
        private TextBox txtNotes;
        private TextBox txtInvoiceNo;
        private Label lblSubTotal;
        private Label lblDiscountTotal;
        private Label lblTaxTotal;
        private Label lblGrandTotal;
        private CheckBox chkIsPaid;
        private ComboBox cmbPaymentMethod;
        private DateTimePicker dtpInvoiceDate;
        private DateTimePicker dtpPaymentDate;
        private Button btnSaveInvoice;
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "AccountantForm";


                        this.SuspendLayout();

            // ایجاد کنترل‌های ضروری
            dgvInvoiceItems = new DataGridView();
            cmbCustomer = new ComboBox();
            txtCustomerPhone = new TextBox();
            txtCustomerAddress = new TextBox();
            txtNotes = new TextBox();
            txtInvoiceNo = new TextBox();
            lblSubTotal = new Label();
            lblDiscountTotal = new Label();
            lblTaxTotal = new Label();
            lblGrandTotal = new Label();
            chkIsPaid = new CheckBox();
            cmbPaymentMethod = new ComboBox();
            dtpInvoiceDate = new DateTimePicker();
            dtpPaymentDate = new DateTimePicker();
            btnSaveInvoice = new Button();

            // تنظیمات اولیه کنترل‌ها
            //SetupControls();

            this.ResumeLayout(false);
        }

        #endregion
    }
}