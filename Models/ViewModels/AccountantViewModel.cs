using System;

namespace PetrochemicalSalesSystem.Models.ViewModels
{
    public class AccountantViewModel
    {
        // فقط فیلدهای اصلی که در فرم استفاده می‌کنید
        public long AccountantID { get; set; }
        public string EmployeeCode { get; set; }
        public string NationalID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FatherName { get; set; }
        public char Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public char MaritalStatus { get; set; }
        public string EducationLevel { get; set; }
        public int DepartmentID { get; set; }
        public string Position { get; set; }
        public string JobTitle { get; set; }
        public byte JobLevel { get; set; }
        public string EmploymentType { get; set; }
        public DateTime HireDate { get; set; }
        public decimal BaseSalary { get; set; }
        public string Mobile { get; set; }
        public string WorkEmail { get; set; }
        public bool IsActive { get; set; }
        public string BankAccountNo { get; set; }
        public string BankName { get; set; }
        public string BankBranch { get; set; }
        public string CostCenterCode { get; set; }

        // برای نمایش در ComboBoxها
        public string FullName { get; set; }
        public string DepartmentName { get; set; }

        public AccountantViewModel() { }

        public AccountantViewModel(Accountant accountant)
        {
            AccountantID = accountant.AccountantID;
            EmployeeCode = accountant.EmployeeCode;
            NationalID = accountant.NationalID;
            FirstName = accountant.FirstName;
            LastName = accountant.LastName;
            FatherName = accountant.FatherName;
            Gender = accountant.Gender;
            BirthDate = accountant.BirthDate;
            MaritalStatus = accountant.MaritalStatus;
            EducationLevel = accountant.EducationLevel;
            DepartmentID = accountant.DepartmentID;
            Position = accountant.Position;
            JobTitle = accountant.JobTitle;
            JobLevel = accountant.JobLevel;
            EmploymentType = accountant.EmploymentType;
            HireDate = accountant.HireDate;
            BaseSalary = accountant.BaseSalary;
            Mobile = accountant.Mobile;
            WorkEmail = accountant.WorkEmail;
            IsActive = accountant.IsActive ?? true;
            BankAccountNo = accountant.BankAccountNo;
            BankName = accountant.BankName;
            BankBranch = accountant.BankBranch;
            CostCenterCode = accountant.CostCenterCode;
            FullName = accountant.FullName;
            DepartmentName = accountant.Department?.DepartmentName;
        }

        public Accountant ToAccountant()
        {
            return new Accountant
            {
                AccountantID = AccountantID,
                EmployeeCode = EmployeeCode,
                NationalID = NationalID,
                FirstName = FirstName,
                LastName = LastName,
                FatherName = FatherName,
                Gender = Gender,
                BirthDate = BirthDate,
                MaritalStatus = MaritalStatus,
                EducationLevel = EducationLevel,
                DepartmentID = DepartmentID,
                Position = Position,
                JobTitle = JobTitle,
                JobLevel = JobLevel,
                EmploymentType = EmploymentType,
                HireDate = HireDate,
                BaseSalary = BaseSalary,
                Mobile = Mobile,
                WorkEmail = WorkEmail,
                IsActive = IsActive,
                BankAccountNo = BankAccountNo,
                BankName = BankName,
                BankBranch = BankBranch,
                CostCenterCode = CostCenterCode,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
        }
    }
}