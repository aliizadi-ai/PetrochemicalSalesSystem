using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using PetrochemicalSalesSystem.Models;

namespace PetrochemicalSalesSystem.Services
{
    public class InvoiceService
    {
        /// <summary>
        /// دریافت لیست فاکتورها
        /// </summary>
        public List<Invoice> GetInvoices(DateTime? fromDate = null, DateTime? toDate = null, int? status = null)
        {
            List<Invoice> invoices = new List<Invoice>();

            string query = @"SELECT * FROM Invoices WHERE 1=1";
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (fromDate.HasValue)
            {
                query += " AND InvoiceDate >= @FromDate";
                parameters.Add(new SqlParameter("@FromDate", fromDate.Value));
            }

            if (toDate.HasValue)
            {
                query += " AND InvoiceDate <= @ToDate";
                parameters.Add(new SqlParameter("@ToDate", toDate.Value));
            }

            if (status.HasValue)
            {
                query += " AND Status = @Status";
                parameters.Add(new SqlParameter("@Status", status.Value));
            }

            query += " ORDER BY InvoiceDate DESC";

            try
            {
                DataTable dt = Data.DatabaseHelper.ExecuteQuery(query, parameters.Count > 0 ? parameters.ToArray() : null);

                foreach (DataRow row in dt.Rows)
                {
                    invoices.Add(MapDataRowToInvoice(row));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"خطا در دریافت فاکتورها: {ex.Message}");
            }

            return invoices;
        }

        /// <summary>
        /// دریافت یک فاکتور با ID
        /// </summary>
        public Invoice GetInvoiceById(long invoiceId)
        {
            string query = "SELECT * FROM Invoices WHERE InvoiceID = @InvoiceID";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@InvoiceID", invoiceId)
            };

            try
            {
                DataTable dt = Data.DatabaseHelper.ExecuteQuery(query, parameters);

                if (dt.Rows.Count == 0)
                    return null;

                return MapDataRowToInvoice(dt.Rows[0]);
            }
            catch (Exception ex)
            {
                throw new Exception($"خطا در دریافت فاکتور: {ex.Message}");
            }
        }

        /// <summary>
        /// جستجوی فاکتورها
        /// </summary>
        public List<Invoice> SearchInvoices(string searchTerm)
        {
            List<Invoice> invoices = new List<Invoice>();

            string query = @"
                SELECT * FROM Invoices 
                WHERE InvoiceNo LIKE @Search 
                   OR CustomerName LIKE @Search 
                   OR CustomerPhone LIKE @Search
                ORDER BY InvoiceDate DESC";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Search", $"%{searchTerm}%")
            };

            try
            {
                DataTable dt = Data.DatabaseHelper.ExecuteQuery(query, parameters);

                foreach (DataRow row in dt.Rows)
                {
                    invoices.Add(MapDataRowToInvoice(row));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"خطا در جستجوی فاکتورها: {ex.Message}");
            }

            return invoices;
        }

        /// <summary>
        /// حذف فاکتور
        /// </summary>
        public bool DeleteInvoice(long invoiceId)
        {
            string query = "DELETE FROM Invoices WHERE InvoiceID = @InvoiceID";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@InvoiceID", invoiceId)
            };

            try
            {
                int rowsAffected = Data.DatabaseHelper.ExecuteNonQuery(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"خطا در حذف فاکتور: {ex.Message}");
            }
        }

        /// <summary>
        /// آمار فاکتورها
        /// </summary>
        public DataTable GetInvoiceStatistics()
        {
            string query = @"
                SELECT 
                    COUNT(*) as TotalInvoices,
                    SUM(CASE WHEN Status = 1 THEN 1 ELSE 0 END) as PaidCount,
                    SUM(CASE WHEN Status = 2 THEN 1 ELSE 0 END) as PendingCount,
                    SUM(TotalAmount) as TotalRevenue,
                    AVG(TotalAmount) as AverageAmount
                FROM Invoices";

            return Data.DatabaseHelper.ExecuteQuery(query);
        }

        /// <summary>
        /// تبدیل DataRow به Invoice
        /// </summary>
        private Invoice MapDataRowToInvoice(DataRow row)
        {
            return new Invoice
            {
                InvoiceID = Convert.ToInt64(row["InvoiceID"]),
                InvoiceNo = row["InvoiceNo"].ToString(),
                InvoiceDate = Convert.ToDateTime(row["InvoiceDate"]),
                CustomerID = row["CustomerID"] != DBNull.Value ? Convert.ToInt64(row["CustomerID"]) : (long?)null,
                CustomerName = row["CustomerName"].ToString(),
                CustomerPhone = row["CustomerPhone"]?.ToString(),
                CustomerAddress = row["CustomerAddress"]?.ToString(),
                SubTotal = Convert.ToDecimal(row["SubTotal"]),
                DiscountAmount = Convert.ToDecimal(row["DiscountAmount"]),
                TaxAmount = Convert.ToDecimal(row["TaxAmount"]),
                TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                Status = Convert.ToInt32(row["Status"]),
                PaymentMethod = row["PaymentMethod"]?.ToString(),
                PaymentDate = row["PaymentDate"] != DBNull.Value ? Convert.ToDateTime(row["PaymentDate"]) : (DateTime?)null,
                Notes = row["Notes"]?.ToString(),
                CreatedBy = Convert.ToInt64(row["CreatedBy"]),
                CreatedDate = Convert.ToDateTime(row["CreatedDate"])
            };
        }
    }
}