using PetrochemicalSalesSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace PetrochemicalSalesSystem.Data
{
    public class AccountantRepository
    {
        // گرفتن تمام حسابداران برای DataGridView
        public List<Accountant> GetAllAccountants()
        {
            List<Accountant> accountants = new List<Accountant>();

            string query = @"
                SELECT 
                    a.AccountantID,
                    a.EmployeeCode,
                    a.NationalID,
                    a.FirstName,
                    a.LastName,
                    a.FatherName,
                    a.Gender,
                    a.BirthDate,
                    a.MaritalStatus,
                    a.EducationLevel,
                    a.DepartmentID,
                    a.Position,
                    a.JobTitle,
                    a.JobLevel,
                    a.EmploymentType,
                    a.HireDate,
                    a.BaseSalary,
                    a.Mobile,
                    a.WorkEmail,
                    a.BankAccountNo,
                    a.BankName,
                    a.BankBranch,
                    a.CostCenterCode,
                    a.IsActive,
                    a.CreatedDate,
                    a.ModifiedDate,
                    d.DepartmentName
                FROM Accountants a
                LEFT JOIN Departments d ON a.DepartmentID = d.DepartmentID
                ORDER BY a.AccountantID DESC";

            DataTable dt = DatabaseHelper.ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                accountants.Add(new Accountant
                {
                    AccountantID = Convert.ToInt64(row["AccountantID"]),
                    EmployeeCode = row["EmployeeCode"].ToString(),
                    NationalID = row["NationalID"].ToString(),
                    FirstName = row["FirstName"].ToString(),
                    LastName = row["LastName"].ToString(),
                    FatherName = row["FatherName"] != DBNull.Value ? row["FatherName"].ToString() : null,
                    Gender = Convert.ToChar(row["Gender"]),
                    BirthDate = Convert.ToDateTime(row["BirthDate"]),
                    MaritalStatus = Convert.ToChar(row["MaritalStatus"]),
                    EducationLevel = row["EducationLevel"].ToString(),
                    DepartmentID = Convert.ToInt32(row["DepartmentID"]),
                    Position = row["Position"].ToString(),
                    JobTitle = row["JobTitle"] != DBNull.Value ? row["JobTitle"].ToString() : null,
                    JobLevel = Convert.ToByte(row["JobLevel"]),
                    EmploymentType = row["EmploymentType"].ToString(),
                    HireDate = Convert.ToDateTime(row["HireDate"]),
                    BaseSalary = Convert.ToDecimal(row["BaseSalary"]),
                    Mobile = row["Mobile"].ToString(),
                    WorkEmail = row["WorkEmail"].ToString(),
                    BankAccountNo = row["BankAccountNo"].ToString(),
                    BankName = row["BankName"].ToString(),
                    BankBranch = row["BankBranch"].ToString(),
                    CostCenterCode = row["CostCenterCode"] != DBNull.Value ? row["CostCenterCode"].ToString() : null,
                    IsActive = row["IsActive"] != DBNull.Value ? Convert.ToBoolean(row["IsActive"]) : false,
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    ModifiedDate = Convert.ToDateTime(row["ModifiedDate"]),
                    DepartmentName = row["DepartmentName"] != DBNull.Value ? row["DepartmentName"].ToString() : ""
                });
            }

            return accountants;
        }

        public Accountant GetAccountantById(long id)
        {
            try
            {
                string query = @"
                    SELECT 
                        a.AccountantID,
                        a.EmployeeCode,
                        a.NationalID,
                        a.InsuranceID,
                        a.FirstName,
                        a.LastName,
                        a.FatherName,
                        a.Gender,
                        a.BirthDate,
                        a.MaritalStatus,
                        a.NumberOfChildren,
                        a.DependentsCount,
                        a.EducationLevel,
                        a.DepartmentID,
                        a.Position,
                        a.JobTitle,
                        a.JobLevel,
                        a.EmploymentType,
                        a.HireDate,
                        a.BaseSalary,
                        a.NetSalary,
                        a.GrossSalary,
                        a.SalaryCurrency,
                        a.BankAccountNo,
                        a.BankName,
                        a.BankBranch,
                        a.BankAccountType,
                        a.Mobile,
                        a.WorkEmail,
                        a.PersonalEmail,
                        a.WorkAddress,
                        a.HomeAddress,
                        a.SystemUsername,
                        a.CostCenterCode,
                        a.IsActive,
                        a.CreatedDate,
                        a.ModifiedDate,
                        d.DepartmentName
                    FROM Accountants a
                    LEFT JOIN Departments d ON a.DepartmentID = d.DepartmentID
                    WHERE a.AccountantID = @AccountantID";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@AccountantID", id)
                };

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new Accountant
                    {
                        AccountantID = Convert.ToInt64(row["AccountantID"]),
                        EmployeeCode = row["EmployeeCode"].ToString(),
                        NationalID = row["NationalID"].ToString(),
                        InsuranceID = row["InsuranceID"] != DBNull.Value ? row["InsuranceID"].ToString() : null,
                        FirstName = row["FirstName"].ToString(),
                        LastName = row["LastName"].ToString(),
                        FatherName = row["FatherName"] != DBNull.Value ? row["FatherName"].ToString() : null,
                        Gender = Convert.ToChar(row["Gender"]),
                        BirthDate = Convert.ToDateTime(row["BirthDate"]),
                        MaritalStatus = Convert.ToChar(row["MaritalStatus"]),
                        NumberOfChildren = row["NumberOfChildren"] != DBNull.Value ? Convert.ToByte(row["NumberOfChildren"]) : (byte?)null,
                        DependentsCount = row["DependentsCount"] != DBNull.Value ? Convert.ToByte(row["DependentsCount"]) : (byte?)null,
                        EducationLevel = row["EducationLevel"].ToString(),
                        DepartmentID = Convert.ToInt32(row["DepartmentID"]),
                        Position = row["Position"].ToString(),
                        JobTitle = row["JobTitle"] != DBNull.Value ? row["JobTitle"].ToString() : null,
                        JobLevel = Convert.ToByte(row["JobLevel"]),
                        EmploymentType = row["EmploymentType"].ToString(),
                        HireDate = Convert.ToDateTime(row["HireDate"]),
                        BaseSalary = Convert.ToDecimal(row["BaseSalary"]),
                        NetSalary = row["NetSalary"] != DBNull.Value ? Convert.ToDecimal(row["NetSalary"]) : (decimal?)null,
                        GrossSalary = row["GrossSalary"] != DBNull.Value ? Convert.ToDecimal(row["GrossSalary"]) : (decimal?)null,
                        SalaryCurrency = row["SalaryCurrency"] != DBNull.Value ? row["SalaryCurrency"].ToString() : null,
                        BankAccountNo = row["BankAccountNo"].ToString(),
                        BankName = row["BankName"].ToString(),
                        BankBranch = row["BankBranch"].ToString(),
                        BankAccountType = row["BankAccountType"] != DBNull.Value ? row["BankAccountType"].ToString() : null,
                        Mobile = row["Mobile"].ToString(),
                        WorkEmail = row["WorkEmail"].ToString(),
                        PersonalEmail = row["PersonalEmail"] != DBNull.Value ? row["PersonalEmail"].ToString() : null,
                        WorkAddress = row["WorkAddress"] != DBNull.Value ? row["WorkAddress"].ToString() : null,
                        HomeAddress = row["HomeAddress"] != DBNull.Value ? row["HomeAddress"].ToString() : null,
                        SystemUsername = row["SystemUsername"] != DBNull.Value ? row["SystemUsername"].ToString() : null,
                        CostCenterCode = row["CostCenterCode"] != DBNull.Value ? row["CostCenterCode"].ToString() : null,
                        IsActive = row["IsActive"] != DBNull.Value ? Convert.ToBoolean(row["IsActive"]) : (bool?)null,
                        CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                        ModifiedDate = Convert.ToDateTime(row["ModifiedDate"]),
                        DepartmentName = row["DepartmentName"] != DBNull.Value ? row["DepartmentName"].ToString() : null
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting accountant by ID: {ex.Message}");
            }
        }

        public bool InsertAccountant(Accountant accountant)
        {
            try
            {
                string query = @"
                    INSERT INTO Accountants (
                        EmployeeCode, NationalID, InsuranceID, FirstName, LastName, FatherName,
                        Gender, BirthDate, MaritalStatus, NumberOfChildren, DependentsCount,
                        EducationLevel, DepartmentID, Position, JobTitle, JobLevel, EmploymentType,
                        HireDate, BaseSalary, BankAccountNo, BankName, BankBranch, BankAccountType,
                        Mobile, WorkEmail, PersonalEmail, WorkAddress, HomeAddress, SystemUsername,
                        CostCenterCode, IsActive, CreatedDate, ModifiedDate
                    ) VALUES (
                        @EmployeeCode, @NationalID, @InsuranceID, @FirstName, @LastName, @FatherName,
                        @Gender, @BirthDate, @MaritalStatus, @NumberOfChildren, @DependentsCount,
                        @EducationLevel, @DepartmentID, @Position, @JobTitle, @JobLevel, @EmploymentType,
                        @HireDate, @BaseSalary, @BankAccountNo, @BankName, @BankBranch, @BankAccountType,
                        @Mobile, @WorkEmail, @PersonalEmail, @WorkAddress, @HomeAddress, @SystemUsername,
                        @CostCenterCode, @IsActive, @CreatedDate, @ModifiedDate
                    )";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@EmployeeCode", accountant.EmployeeCode),
                    new SqlParameter("@NationalID", accountant.NationalID),
                    new SqlParameter("@InsuranceID", (object)accountant.InsuranceID ?? DBNull.Value),
                    new SqlParameter("@FirstName", accountant.FirstName),
                    new SqlParameter("@LastName", accountant.LastName),
                    new SqlParameter("@FatherName", (object)accountant.FatherName ?? DBNull.Value),
                    new SqlParameter("@Gender", accountant.Gender.ToString()),
                    new SqlParameter("@BirthDate", accountant.BirthDate),
                    new SqlParameter("@MaritalStatus", accountant.MaritalStatus.ToString()),
                    new SqlParameter("@NumberOfChildren", (object)accountant.NumberOfChildren ?? DBNull.Value),
                    new SqlParameter("@DependentsCount", (object)accountant.DependentsCount ?? DBNull.Value),
                    new SqlParameter("@EducationLevel", accountant.EducationLevel),
                    new SqlParameter("@DepartmentID", accountant.DepartmentID),
                    new SqlParameter("@Position", accountant.Position),
                    new SqlParameter("@JobTitle", (object)accountant.JobTitle ?? DBNull.Value),
                    new SqlParameter("@JobLevel", accountant.JobLevel),
                    new SqlParameter("@EmploymentType", accountant.EmploymentType),
                    new SqlParameter("@HireDate", accountant.HireDate),
                    new SqlParameter("@BaseSalary", accountant.BaseSalary),
                    new SqlParameter("@BankAccountNo", accountant.BankAccountNo),
                    new SqlParameter("@BankName", accountant.BankName),
                    new SqlParameter("@BankBranch", accountant.BankBranch),
                    new SqlParameter("@BankAccountType", (object)accountant.BankAccountType ?? DBNull.Value),
                    new SqlParameter("@Mobile", accountant.Mobile),
                    new SqlParameter("@WorkEmail", accountant.WorkEmail),
                    new SqlParameter("@PersonalEmail", (object)accountant.PersonalEmail ?? DBNull.Value),
                    new SqlParameter("@WorkAddress", (object)accountant.WorkAddress ?? DBNull.Value),
                    new SqlParameter("@HomeAddress", (object)accountant.HomeAddress ?? DBNull.Value),
                    new SqlParameter("@SystemUsername", (object)accountant.SystemUsername ?? DBNull.Value),
                    new SqlParameter("@CostCenterCode", (object)accountant.CostCenterCode ?? DBNull.Value),
                    new SqlParameter("@IsActive", accountant.IsActive ?? true),
                    new SqlParameter("@CreatedDate", DateTime.Now),
                    new SqlParameter("@ModifiedDate", DateTime.Now)
                };

                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
                return rowsAffected > 0;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // Unique constraint error
                {
                    if (ex.Message.Contains("EmployeeCode"))
                        throw new Exception("کد پرسنلی تکراری است");
                    if (ex.Message.Contains("NationalID"))
                        throw new Exception("کد ملی تکراری است");
                }
                throw new Exception($"خطا در ذخیره اطلاعات: {ex.Message}");
            }
        }

        public bool UpdateAccountant(Accountant accountant)
        {
            try
            {
                string query = @"
                    UPDATE Accountants SET
                        EmployeeCode = @EmployeeCode,
                        NationalID = @NationalID,
                        InsuranceID = @InsuranceID,
                        FirstName = @FirstName,
                        LastName = @LastName,
                        FatherName = @FatherName,
                        Gender = @Gender,
                        BirthDate = @BirthDate,
                        MaritalStatus = @MaritalStatus,
                        NumberOfChildren = @NumberOfChildren,
                        DependentsCount = @DependentsCount,
                        EducationLevel = @EducationLevel,
                        DepartmentID = @DepartmentID,
                        Position = @Position,
                        JobTitle = @JobTitle,
                        JobLevel = @JobLevel,
                        EmploymentType = @EmploymentType,
                        HireDate = @HireDate,
                        BaseSalary = @BaseSalary,
                        BankAccountNo = @BankAccountNo,
                        BankName = @BankName,
                        BankBranch = @BankBranch,
                        BankAccountType = @BankAccountType,
                        Mobile = @Mobile,
                        WorkEmail = @WorkEmail,
                        PersonalEmail = @PersonalEmail,
                        WorkAddress = @WorkAddress,
                        HomeAddress = @HomeAddress,
                        SystemUsername = @SystemUsername,
                        CostCenterCode = @CostCenterCode,
                        IsActive = @IsActive,
                        ModifiedDate = @ModifiedDate
                    WHERE AccountantID = @AccountantID";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@AccountantID", accountant.AccountantID),
                    new SqlParameter("@EmployeeCode", accountant.EmployeeCode),
                    new SqlParameter("@NationalID", accountant.NationalID),
                    new SqlParameter("@InsuranceID", (object)accountant.InsuranceID ?? DBNull.Value),
                    new SqlParameter("@FirstName", accountant.FirstName),
                    new SqlParameter("@LastName", accountant.LastName),
                    new SqlParameter("@FatherName", (object)accountant.FatherName ?? DBNull.Value),
                    new SqlParameter("@Gender", accountant.Gender.ToString()),
                    new SqlParameter("@BirthDate", accountant.BirthDate),
                    new SqlParameter("@MaritalStatus", accountant.MaritalStatus.ToString()),
                    new SqlParameter("@NumberOfChildren", (object)accountant.NumberOfChildren ?? DBNull.Value),
                    new SqlParameter("@DependentsCount", (object)accountant.DependentsCount ?? DBNull.Value),
                    new SqlParameter("@EducationLevel", accountant.EducationLevel),
                    new SqlParameter("@DepartmentID", accountant.DepartmentID),
                    new SqlParameter("@Position", accountant.Position),
                    new SqlParameter("@JobTitle", (object)accountant.JobTitle ?? DBNull.Value),
                    new SqlParameter("@JobLevel", accountant.JobLevel),
                    new SqlParameter("@EmploymentType", accountant.EmploymentType),
                    new SqlParameter("@HireDate", accountant.HireDate),
                    new SqlParameter("@BaseSalary", accountant.BaseSalary),
                    new SqlParameter("@BankAccountNo", accountant.BankAccountNo),
                    new SqlParameter("@BankName", accountant.BankName),
                    new SqlParameter("@BankBranch", accountant.BankBranch),
                    new SqlParameter("@BankAccountType", (object)accountant.BankAccountType ?? DBNull.Value),
                    new SqlParameter("@Mobile", accountant.Mobile),
                    new SqlParameter("@WorkEmail", accountant.WorkEmail),
                    new SqlParameter("@PersonalEmail", (object)accountant.PersonalEmail ?? DBNull.Value),
                    new SqlParameter("@WorkAddress", (object)accountant.WorkAddress ?? DBNull.Value),
                    new SqlParameter("@HomeAddress", (object)accountant.HomeAddress ?? DBNull.Value),
                    new SqlParameter("@SystemUsername", (object)accountant.SystemUsername ?? DBNull.Value),
                    new SqlParameter("@CostCenterCode", (object)accountant.CostCenterCode ?? DBNull.Value),
                    new SqlParameter("@IsActive", accountant.IsActive ?? true),
                    new SqlParameter("@ModifiedDate", DateTime.Now)
                };

                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"خطا در بروزرسانی حسابدار: {ex.Message}");
            }
        }

        public bool DeleteAccountant(long id)
        {
            try
            {
                string query = "DELETE FROM Accountants WHERE AccountantID = @AccountantID";
                SqlParameter[] parameters = new SqlParameter[]
                {
                new SqlParameter("@AccountantID", id)
                };

                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
                return rowsAffected > 0;
            }
            catch (SqlException ex)
            {
                // بررسی اگر حسابدار در جای دیگری استفاده شده
                if (ex.Number == 547) // Foreign key constraint
                {
                    throw new Exception("این حسابدار در سیستم استفاده شده و نمی‌توان حذف کرد.");
                }
                throw new Exception($"خطا در حذف حسابدار: {ex.Message}");
            }
        }
    }
}
