using System;

namespace PetrochemicalSalesSystem.Forms
{
    partial class AccountantEditForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
        private void AccountantEditForm_Load(object sender, EventArgs e)
        {

        }
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // AccountantEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "AccountantEditForm";
            this.Text = "AccountantEditForm";
            this.Load += new System.EventHandler(this.AccountantEditForm_Load);
            this.ResumeLayout(false);

        }

        #endregion
    }
}