using PetrochemicalSalesSystem.Data;
using System;

namespace PetrochemicalSalesSystem.Utilities
{
    public static class BackupHelper
    {
        public static bool BackupDatabase(string backupPath)
        {
            try
            {
                string backupQuery = $@"
                    BACKUP DATABASE [PetrochemicalAccountantDB] 
                    TO DISK = '{backupPath}' 
                    WITH FORMAT, 
                    MEDIANAME = 'PetrochemicalBackup', 
                    NAME = 'Full Backup of PetrochemicalAccountantDB'";

                DatabaseHelper.ExecuteNonQuery(backupQuery);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"خطا در تهیه پشتیبان: {ex.Message}");
            }
        }

        public static bool RestoreDatabase(string backupPath)
        {
            try
            {
                // ابتدا باید تمام اتصالات به دیتابیس قطع شود
                string singleUserQuery = @"
                    ALTER DATABASE [PetrochemicalAccountantDB] 
                    SET SINGLE_USER WITH ROLLBACK IMMEDIATE";

                string restoreQuery = $@"
                    RESTORE DATABASE [PetrochemicalAccountantDB] 
                    FROM DISK = '{backupPath}' 
                    WITH REPLACE";

                string multiUserQuery = @"
                    ALTER DATABASE [PetrochemicalAccountantDB] 
                    SET MULTI_USER";

                DatabaseHelper.ExecuteNonQuery(singleUserQuery);
                DatabaseHelper.ExecuteNonQuery(restoreQuery);
                DatabaseHelper.ExecuteNonQuery(multiUserQuery);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"خطا در بازیابی پشتیبان: {ex.Message}");
            }
        }
    }
}