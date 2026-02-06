using PetrochemicalSalesSystem.Models;

namespace PetrochemicalSalesSystem.Utilities
{
    public static class SessionManager
    {
        private static Accountant _currentUser;

        public static Accountant CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                // می‌توانید رویدادهایی برای تغییر وضعیت کاربر اضافه کنید
            }
        }

        public static bool IsLoggedIn => CurrentUser != null;

        public static bool IsAdmin => CurrentUser?.IsSystemAdmin == true;

        public static void Logout()
        {
            _currentUser = null;
            // پاک کردن کوکی‌ها یا اطلاعات نشست
        }

        public static bool HasPermission(byte requiredAccessLevel)
        {
            return CurrentUser?.FinancialSystemAccessLevel >= requiredAccessLevel;
        }
    }
}