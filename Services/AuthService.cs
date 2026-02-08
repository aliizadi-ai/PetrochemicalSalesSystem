using PetrochemicalSalesSystem.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace PetrochemicalSalesSystem.Services
{
    public class AuthService
    {
        public enum UserType { None, Accountant, Admin }

        public class AuthResult
        {
            public bool IsValid { get; set; }
            public string ErrorMessage { get; set; }
            public UserType Type { get; set; }
            public object User { get; set; }
            public string DebugInfo { get; set; }
        }

        /// <summary>
        /// بررسی اعتبار کاربر در هر دو جدول
        /// </summary>
        public AuthResult ValidateLogin(string username, string password)
        {
            string debugInfo = "";

            try
            {
                debugInfo += $"شروع لاگین برای کاربر: {username}\n";

                // ابتدا در Admins جستجو می‌کنیم
                debugInfo += "در حال جستجو در جدول Admins...\n";
                var adminResult = ValidateAdminLogin(username, password);

                if (adminResult.IsValid)
                {
                    debugInfo += "✅ کاربر در جدول Admins یافت شد.\n";
                    return new AuthResult
                    {
                        IsValid = true,
                        Type = UserType.Admin,
                        User = adminResult.User,
                        ErrorMessage = "ورود مدیر موفقیت‌آمیز بود",
                        DebugInfo = debugInfo
                    };
                }
                else
                {
                    debugInfo += "❌ کاربر در جدول Admins یافت نشد.\n";
                }

                // سپس در Accountants جستجو می‌کنیم
                debugInfo += "در حال جستجو در جدول Accountants...\n";
                var accountantResult = ValidateAccountantLogin(username, password);

                if (accountantResult.IsValid)
                {
                    debugInfo += "✅ کاربر در جدول Accountants یافت شد.\n";
                    return new AuthResult
                    {
                        IsValid = true,
                        Type = UserType.Accountant,
                        User = accountantResult.User,
                        ErrorMessage = "ورود حسابدار موفقیت‌آمیز بود",
                        DebugInfo = debugInfo
                    };
                }
                else
                {
                    debugInfo += "❌ کاربر در جدول Accountants یافت نشد.\n";
                }

                // اگر هیچکدام نبود، خطای عمومی می‌دهیم
                debugInfo += "⚠️ کاربر در هیچ جدولی یافت نشد.\n";
                return new AuthResult
                {
                    IsValid = false,
                    Type = UserType.None,
                    User = null,
                    ErrorMessage = "نام کاربری یا رمز عبور اشتباه است",
                    DebugInfo = debugInfo
                };
            }
            catch (Exception ex)
            {
                debugInfo += $"💥 خطا: {ex.Message}\n";
                return new AuthResult
                {
                    IsValid = false,
                    Type = UserType.None,
                    User = null,
                    ErrorMessage = $"خطا در سیستم: {ex.Message}",
                    DebugInfo = debugInfo
                };
            }
        }

        /// <summary>
        /// بررسی اعتبار مدیر با لاگینگ پیشرفته
        /// </summary>
        private (bool IsValid, Admins User) ValidateAdminLogin(string username, string password)
        {
            try
            {
                Debug.WriteLine($"--- شروع ValidateAdminLogin برای {username} ---");

                // کوئری ساده‌تر برای تست
                string query = "SELECT * FROM Admins WHERE Username = @Username";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Username", SqlDbType.VarChar) { Value = username }
                };

                Debug.WriteLine($"اجرای کوئری: {query}");
                Debug.WriteLine($"پارامتر: @Username = {username}");

                DataTable dt = Data.DatabaseHelper.ExecuteQuery(query, parameters);

                Debug.WriteLine($"تعداد ردیف‌های بازگشتی: {dt.Rows.Count}");

                if (dt.Rows.Count == 0)
                {
                    Debug.WriteLine("کاربر یافت نشد.");
                    return (false, null);
                }

                DataRow row = dt.Rows[0];

                // بررسی فیلدهای موجود
                Debug.WriteLine("فیلدهای موجود در ردیف:");
                foreach (DataColumn col in dt.Columns)
                {
                    Debug.WriteLine($"  {col.ColumnName}: {row[col]}");
                }

                // بررسی فعال بودن
                bool isActive = Convert.ToBoolean(row["IsActive"]);
                if (!isActive)
                {
                    Debug.WriteLine("کاربر غیرفعال است.");
                    return (false, null);
                }

                // بررسی رمز عبور
                string storedPassword = row["Password"].ToString();
                Debug.WriteLine($"رمز ذخیره شده: '{storedPassword}', رمز ورودی: '{password}'");

                // نسخه ساده مقایسه (بعداً با هش جایگزین کنید)
                if (storedPassword.Trim() != password.Trim())
                {
                    Debug.WriteLine("رمز عبور مطابقت ندارد.");
                    return (false, null);
                }

                Debug.WriteLine("رمز عبور صحیح است.");

                Admins admin = new Admins
                {
                    AdminID = Convert.ToInt64(row["AdminID"]),
                    Username = row["Username"].ToString().Trim(),
                    FirstName = row["FirstName"].ToString(),
                    LastName = row["LastName"].ToString(),
                    Email = row["Email"].ToString(),
                    IsSuperAdmin = Convert.ToBoolean(row["IsSuperAdmin"]),
                    IsActive = true
                };

                // آپدیت لاگین
                UpdateAdminLastLogin(admin.AdminID);

                Debug.WriteLine($"--- پایان ValidateAdminLogin (موفق) برای {username} ---");
                return (true, admin);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"خطا در ValidateAdminLogin: {ex.Message}");
                Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                return (false, null);
            }
        }

        private void UpdateAdminLastLogin(long adminId)
        {
            try
            {
                string query = "UPDATE Admins SET LastLogin = GETDATE() WHERE AdminID = @AdminID";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@AdminID", SqlDbType.BigInt) { Value = adminId }
                };

                Data.DatabaseHelper.ExecuteNonQuery(query, parameters);
                Debug.WriteLine($"آپدیت LastLogin برای AdminID: {adminId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"خطا در UpdateAdminLastLogin: {ex.Message}");
            }
        }


        /// <summary>
        /// بررسی اعتبار کاربر در هر دو جدول
        /// </summary>

        /// <summary>
        /// بررسی اعتبار مدیر
        /// </summary>

        /// <summary>
        /// بررسی اعتبار مدیر
        /// </summary>

        /// <summary>
        /// متد تست برای بررسی دسترسی به جدول Admins
        /// </summary>
        public string TestAdminLogin()
        {
            try
            {
                // تست اتصال به جدول Admins
                string testQuery = "SELECT COUNT(*) as AdminCount FROM Admins";
                DataTable dt = Data.DatabaseHelper.ExecuteQuery(testQuery);

                int adminCount = Convert.ToInt32(dt.Rows[0]["AdminCount"]);

                // نمایش تمام ادمین‌ها
                string listQuery = "SELECT Username, IsActive FROM Admins";
                DataTable listDt = Data.DatabaseHelper.ExecuteQuery(listQuery);

                string result = $"تعداد مدیران در سیستم: {adminCount}\n";

                foreach (DataRow row in listDt.Rows)
                {
                    result += $"- {row["Username"]} (فعال: {row["IsActive"]})\n";
                }

                return result;
            }
            catch (Exception ex)
            {
                return $"خطا در دسترسی به جدول Admins: {ex.Message}";
            }
        }

        /// <summary>
        /// بررسی اعتبار حسابدار
        /// </summary>
        private (bool IsValid, Accountant User) ValidateAccountantLogin(string username, string password)
        {
            try
            {
                string query = @"
                    SELECT AccountantID, Username, FirstName, LastName, 
                           IsActive, IsSystemAdmin, EmployeeCode, WorkEmail
                    FROM Accountants 
                    WHERE Username = @Username 
                      AND Password = @Password 
                      AND IsActive = 1";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Username", SqlDbType.VarChar, 100) { Value = username },
                    new SqlParameter("@Password", SqlDbType.VarChar, 100) { Value = password }
                };

                DataTable dt = Data.DatabaseHelper.ExecuteQuery(query, parameters);

                if (dt.Rows.Count == 0)
                    return (false, null);

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

                if (row.Table.Columns.Contains("IsSystemAdmin") && row["IsSystemAdmin"] != DBNull.Value)
                    accountant.IsSystemAdmin = Convert.ToBoolean(row["IsSystemAdmin"]);

                return (true, accountant);
            }
            catch (Exception)
            {
                return (false, null);
            }
        }

        /// <summary>
        /// به‌روزرسانی زمان آخرین لاگین مدیر
        /// </summary>

        /// <summary>
        /// بررسی می‌کند آیا کاربر مدیر است یا نه
        /// </summary>
        public bool IsUserAdmin(string username)
        {
            try
            {
                string query = "SELECT 1 FROM Admins WHERE Username = @Username AND IsActive = 1";
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Username", SqlDbType.VarChar, 100) { Value = username }
                };

                DataTable dt = Data.DatabaseHelper.ExecuteQuery(query, parameters);
                return dt.Rows.Count > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}