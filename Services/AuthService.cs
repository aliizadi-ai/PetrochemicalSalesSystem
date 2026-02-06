using System;
using System.Data;
using System.Data.SqlClient;
using PetrochemicalSalesSystem.Models;

namespace PetrochemicalSalesSystem.Services
{
    public class AuthService
    {
        public (bool IsValid, Accountant Accountant, string ErrorMessage)
            ValidateLogin(string username, string password)
        {
            try
            {
                // کوئری برای بررسی اعتبار کاربر
                string query = @"
                    SELECT AccountantID, Username, Password, FirstName, LastName, 
                           IsActive, IsSystemAdmin, EmployeeCode, WorkEmail,
                           FinancialSystemAccessLevel, FullName
                    FROM Accountants 
                    WHERE Username = @Username 
                      AND Password = @Password 
                      AND IsActive = 1";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Username", SqlDbType.VarChar, 100) { Value = username },
                    new SqlParameter("@Password", SqlDbType.VarChar, 100) { Value = password }
                };

                // اجرای کوئری
                DataTable dt = Data.DatabaseHelper.ExecuteQuery(query, parameters);

                if (dt.Rows.Count == 0)
                {
                    // اگر کاربر پیدا نشد، بررسی می‌کنیم که آیا کاربر وجود دارد اما رمز اشتباه است
                    string checkUserQuery = "SELECT 1 FROM Accountants WHERE Username = @Username";
                    SqlParameter[] userParam = new SqlParameter[]
                    {
                        new SqlParameter("@Username", SqlDbType.VarChar, 100) { Value = username }
                    };

                    DataTable userCheck = Data.DatabaseHelper.ExecuteQuery(checkUserQuery, userParam);

                    if (userCheck.Rows.Count == 0)
                        return (false, null, "نام کاربری یافت نشد");
                    else
                        return (false, null, "رمز عبور اشتباه است");
                }

                // تبدیل ردیف دیتابیس به شیء Accountant
                DataRow row = dt.Rows[0];
                Accountant accountant = new Accountant
                {
                    AccountantID = Convert.ToInt64(row["AccountantID"]),
                    Username = row["Username"].ToString(),
                    FirstName = row["FirstName"].ToString(),
                    LastName = row["LastName"].ToString(),
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    EmployeeCode = row["EmployeeCode"]?.ToString(),
                    WorkEmail = row["WorkEmail"]?.ToString()
                };

                // فیلدهای اختیاری
                if (row.Table.Columns.Contains("IsSystemAdmin") && row["IsSystemAdmin"] != DBNull.Value)
                    accountant.IsSystemAdmin = Convert.ToBoolean(row["IsSystemAdmin"]);

                if (row.Table.Columns.Contains("FinancialSystemAccessLevel") && row["FinancialSystemAccessLevel"] != DBNull.Value)
                    accountant.FinancialSystemAccessLevel = Convert.ToByte(row["FinancialSystemAccessLevel"]);

                if (row.Table.Columns.Contains("FullName") && row["FullName"] != DBNull.Value)
                {
                    // اگر FullName از دیتابیس می‌آید
                }
                else
                {
                    // یا از FirstName و LastName استفاده می‌کنیم
                }

                return (true, accountant, "ورود موفقیت‌آمیز");
            }
            catch (Exception ex)
            {
                return (false, null, $"خطا در اتصال به پایگاه داده: {ex.Message}");
            }
        }

        /// <summary>
        /// بررسی می‌کند آیا کاربر مدیر سیستم است یا نه
        /// </summary>
        public bool IsSystemAdmin(string username)
        {
            try
            {
                string query = "SELECT IsSystemAdmin FROM Accountants WHERE Username = @Username";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Username", SqlDbType.VarChar, 100) { Value = username }
                };

                DataTable dt = Data.DatabaseHelper.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0 && dt.Rows[0]["IsSystemAdmin"] != DBNull.Value)
                    return Convert.ToBoolean(dt.Rows[0]["IsSystemAdmin"]);

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// تغییر رمز عبور کاربر
        /// </summary>
        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            try
            {
                // ابتدا اعتبارسنجی کاربر
                var validation = ValidateLogin(username, oldPassword);
                if (!validation.IsValid)
                    return false;

                // به‌روزرسانی رمز عبور
                string query = "UPDATE Accountants SET Password = @NewPassword WHERE Username = @Username";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@NewPassword", SqlDbType.VarChar, 100) { Value = newPassword },
                    new SqlParameter("@Username", SqlDbType.VarChar, 100) { Value = username }
                };

                int rowsAffected = Data.DatabaseHelper.ExecuteNonQuery(query, parameters);
                return rowsAffected > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}