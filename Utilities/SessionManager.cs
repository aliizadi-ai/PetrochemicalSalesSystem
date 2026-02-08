using System;
using PetrochemicalSalesSystem.Models;
using PetrochemicalSalesSystem.Services;

namespace PetrochemicalSalesSystem.Utilities
{
    public static class SessionManager
    {
        private static object _currentUser;
        private static AuthService.UserType _userType;

        public static object CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                // تعیین نوع کاربر
                if (value is Admins)
                    _userType = AuthService.UserType.Admin;
                else if (value is Accountant)
                    _userType = AuthService.UserType.Accountant;
                else
                    _userType = AuthService.UserType.None;
            }
        }

        public static AuthService.UserType UserType => _userType;

        public static bool IsLoggedIn => CurrentUser != null;

        public static bool IsAdmin => UserType == AuthService.UserType.Admin;

        public static bool IsAccountant => UserType == AuthService.UserType.Accountant;

        public static bool IsSuperAdmin
        {
            get
            {
                if (IsAdmin && CurrentUser is Admins admin)
                    return admin.IsSuperAdmin;
                return false;
            }
        }

        // Property های کمکی برای دسترسی آسان
        public static string FullName
        {
            get
            {
                if (CurrentUser is Admins admin)
                    return admin.FullName;
                else if (CurrentUser is Accountant accountant)
                    return accountant.FullName;
                return "کاربر";
            }
        }

        public static string EmployeeCode
        {
            get
            {
                if (CurrentUser is Admins admin)
                    return "";
                else if (CurrentUser is Accountant accountant)
                    return accountant.EmployeeCode;
                return "کاربر";
            }
        }

        public static string Username
        {
            get
            {
                if (CurrentUser is Admins admin)
                    return admin.Username;
                else if (CurrentUser is Accountant accountant)
                    return accountant.Username;
                return "";
            }
        }

        public static string UserTypeText
        {
            get
            {
                // برای C# 7.3 و قدیمی‌تر
                switch (UserType)
                {
                    case AuthService.UserType.Admin:
                        return "مدیر سیستم";
                    case AuthService.UserType.Accountant:
                        return "حسابدار";
                    default:
                        return "کاربر";
                }
            }
        }

        public static void Logout()
        {
            _currentUser = null;
            _userType = AuthService.UserType.None;
        }

        /// <summary>
        /// بررسی دسترسی کاربر
        /// </summary>
        public static bool HasPermission(string requiredPermission)
        {
            // منطق بررسی دسترسی بر اساس نوع کاربر
            if (IsAdmin)
            {
                // مدیران تمام دسترسی‌ها را دارند
                return true;
            }
            else if (IsAccountant)
            {
                // حسابداران دسترسی محدود دارند
                var accountant = CurrentUser as Accountant;
                bool isAdmin = accountant?.IsSystemAdmin == true;

                switch (requiredPermission)
                {
                    case "ViewInvoices":
                    case "CreateInvoice":
                    case "ViewReports":
                        return true;

                    case "EditInvoice":
                    case "DeleteInvoice":
                        return isAdmin;

                    case "ManageUsers":
                        return false;

                    default:
                        return false;
                }
            }

            return false;
        }
    }
}