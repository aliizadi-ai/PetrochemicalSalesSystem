using System;

namespace PetrochemicalSalesSystem.Models
{
    public class Invoice
    {
        public long InvoiceID { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public long? CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public int Status { get; set; } // 1:پرداخت شده, 2:در انتظار, 3:لغو شده
        public string PaymentMethod { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string Notes { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        // Property های کمکی
        public string StatusText
        {
            get
            {
                switch (Status)
                {
                    case 1: return "پرداخت شده";
                    case 2: return "در انتظار";
                    case 3: return "لغو شده";
                    default: return "نامشخص";
                }
            }
        }

        public string FormattedTotalAmount => TotalAmount.ToString("N0") + " تومان";
        public string FormattedDate => InvoiceDate.ToString("yyyy/MM/dd");
    }
}