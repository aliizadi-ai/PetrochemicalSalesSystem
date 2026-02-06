using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetrochemicalSalesSystem.Models
{
    public class Accountant
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public long AccountantID { get; set; }
        public string EmployeeCode { get; set; }
        public string NationalID { get; set; }
        public string InsuranceID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FatherName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public char Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public char MaritalStatus { get; set; }
        public byte? NumberOfChildren { get; set; }
        public byte? DependentsCount { get; set; }
        public string EducationLevel { get; set; }
        public int DepartmentID { get; set; }
        public string Position { get; set; }
        public string JobTitle { get; set; }
        public byte JobLevel { get; set; }
        public string EmploymentType { get; set; }
        public DateTime HireDate { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal? NetSalary { get; set; }
        public decimal? GrossSalary { get; set; }
        public string SalaryCurrency { get; set; }
        public string BankAccountNo { get; set; }
        public string BankName { get; set; }
        public string BankBranch { get; set; }
        public string BankAccountType { get; set; }
        public string Mobile { get; set; }
        public string WorkEmail { get; set; }
        public string PersonalEmail { get; set; }
        public string WorkAddress { get; set; }
        public string HomeAddress { get; set; }
        public string SystemUsername { get; set; }
        public string CostCenterCode { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        // برای نمایش در لیست
        public string DepartmentName { get; set; }

        // Navigation property
        public Department Department { get; set; }
        // اطلاعات هویتی
        public Guid? GUID { get; set; }

        // اطلاعات شخصی
        public string EnglishFirstName { get; set; }
        public string EnglishLastName { get; set; }
        public string BirthCertificateNo { get; set; }
        public string BirthCertificateSerial { get; set; }
        public string BirthPlaceProvince { get; set; }
        public string BirthPlaceCity { get; set; }
        public DateTime? MarriageDate { get; set; }
        public string SpouseName { get; set; }
        public string SpouseNationalID { get; set; }

        // اطلاعات تحصیلی
        public string Major { get; set; }
        public string University { get; set; }
        public short? GraduationYear { get; set; }
        public decimal? GradePointAverage { get; set; }
        public string AccountingLicenseNo { get; set; }
        public DateTime? LicenseIssueDate { get; set; }
        public DateTime? LicenseExpiryDate { get; set; }
        public DateTime? LicenseRenewalDate { get; set; }
        public string TaxAdvisorLicenseNo { get; set; }
        public bool? IsCertifiedAccountant { get; set; }
        public string CertifiedAccountantNo { get; set; }
        public DateTime? CertifiedAccountantIssueDate { get; set; }
        public string MembershipNumber { get; set; }

        // اطلاعات شغلی
        public string UnitName { get; set; }
        public string EmploymentContractNo { get; set; }
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public DateTime? ContractRenewalDate { get; set; }
        public DateTime? ProbationEndDate { get; set; }
        public DateTime? PermanentDate { get; set; }
        public DateTime? RetirementDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public string TerminationType { get; set; }
        public string TerminationReason { get; set; }
        public DateTime? ExitInterviewDate { get; set; }
        public long? ManagerID { get; set; }
        public long? BackupPersonID { get; set; }

        // اطلاعات مالی
        public decimal? InsuranceBase { get; set; }
        public decimal? TaxBase { get; set; }
        public decimal? TaxExemptionAmount { get; set; }
        public decimal? HousingLoanAmount { get; set; }
        public decimal? OtherLoanAmount { get; set; }

        // اطلاعات بانکی
        public string BankBranchCode { get; set; }
        public string PaymentMethod { get; set; }
        public byte? PaymentDay { get; set; }
        public DateTime? LastSalaryIncreaseDate { get; set; }
        public decimal? LastSalaryIncreaseAmount { get; set; }
        public DateTime? NextSalaryReviewDate { get; set; }

        // اطلاعات تماس
        public string WorkPhone { get; set; }
        public string WorkExtension { get; set; }
        public string Mobile2 { get; set; }
        public string EmergencyPhone { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactRelation { get; set; }
        public string HomeProvince { get; set; }
        public string HomeCity { get; set; }
        public string HomePostalCode { get; set; }
        public decimal? DistanceFromWork { get; set; }

        // اطلاعات سیستم و دسترسی
        public string ADUsername { get; set; }
        public string DomainName { get; set; }
        public bool? IsSystemAdmin { get; set; }
        public byte? FinancialSystemAccessLevel { get; set; }
        public string ERPUserID { get; set; }
        public bool? CanApprovePayments { get; set; }
        public bool? CanPostJournalEntries { get; set; }
        public bool? CanCloseFiscalPeriod { get; set; }
        public bool? CanViewSensitiveData { get; set; }
        public bool? CanExportFinancialData { get; set; }
        public decimal? MaxApprovalAmount { get; set; }
        public string ApprovalLimitCurrency { get; set; }
        public DateTime? LastSystemAccessDate { get; set; }
        public DateTime? PasswordExpiryDate { get; set; }

        // امنیت و گواهی‌ها
        public bool? HasSecurityClearance { get; set; }
        public string SecurityClearanceLevel { get; set; }
        public DateTime? SecurityClearanceIssueDate { get; set; }
        public DateTime? SecurityClearanceExpiryDate { get; set; }
        public string MilitaryServiceStatus { get; set; }
        public string ExemptionReason { get; set; }

        // فایل‌ها و تصاویر
        public string PhotoURL { get; set; }
        public byte[] SignatureImage { get; set; }
        public byte[] DigitalSignatureCert { get; set; }
        public string ResumeURL { get; set; }
        public string ContractsURL { get; set; }
        public string DegreeScanURL { get; set; }

        // وضعیت و مرخصی
        public bool? IsOnLeave { get; set; }
        public DateTime? LeaveStartDate { get; set; }
        public DateTime? LeaveEndDate { get; set; }
        public string LeaveType { get; set; }
        public decimal? LeaveBalance { get; set; }
        public decimal? SickLeaveBalance { get; set; }

        // ارتقاء و ارزیابی
        public DateTime? LastPromotionDate { get; set; }
        public string LastPromotionType { get; set; }
        public DateTime? NextReviewDate { get; set; }
        public decimal? PerformanceRating { get; set; }
        public bool? IsUnderProbation { get; set; }

        // اطلاعات پتروشیمی
        public int? PetrochemicalComplexID { get; set; }
        public string PlantUnitCode { get; set; }
        public string ProfitCenterCode { get; set; }
        public string ProjectCode { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string LocationCode { get; set; }

        // گواهی‌های تخصصی
        public bool? IsHSE_Certified { get; set; }
        public DateTime? HSE_CertificationDate { get; set; }
        public DateTime? HSE_ExpiryDate { get; set; }
        public bool? IsISO9001Certified { get; set; }
        public DateTime? ISO9001CertDate { get; set; }
        public bool? EnvironmentalComplianceCert { get; set; }

        // قراردادها و تعهدات
        public string DataClassification { get; set; }
        public bool? NDASigned { get; set; }
        public DateTime? NDASignDate { get; set; }
        public DateTime? NDARenewalDate { get; set; }
        public DateTime? BackgroundCheckDate { get; set; }
        public DateTime? LastSecurityTrainingDate { get; set; }

        // دسترسی فیزیکی
        public string AccessCardNo { get; set; }
        public DateTime? AccessCardIssueDate { get; set; }
        public DateTime? AccessCardExpiryDate { get; set; }
        public string AccessZones { get; set; }
        public string BiometricID { get; set; }
        public bool? TwoFactorEnabled { get; set; }
        public DateTime? LastPrivacyConsentDate { get; set; }

        // مهارت‌ها و تخصص‌ها
        public string Specializations { get; set; }
        public string Languages { get; set; }
        public string Certifications { get; set; }
        public string TechnicalSkills { get; set; }
        public string SoftwareSkills { get; set; }
        public string AreasOfExpertise { get; set; }

        // اطلاعات پزشکی
        public string BloodType { get; set; }
        public string Allergies { get; set; }
        public string ChronicConditions { get; set; }
        public string DisabilityType { get; set; }
        public byte? DisabilityPercentage { get; set; }
        public DateTime? LastMedicalCheckup { get; set; }
        public DateTime? NextMedicalCheckup { get; set; }
        public string VaccinationStatus { get; set; }

        // اطلاعات سیستمی و لاگ
        public long? CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public long? ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public byte[] RowVersion { get; set; }
        public string AuditTrail { get; set; }

        // یادداشت‌ها
        public string Notes { get; set; }
        public string InternalNotes { get; set; }
        public string HRNotes { get; set; }
        public string FinanceNotes { get; set; }
        public string ExitNotes { get; set; }

        // داده‌های توسعه و سفارشی
        public string ExtensionData { get; set; }
        public string CustomField1 { get; set; }
        public string CustomField2 { get; set; }
        public string CustomField3 { get; set; }
        public decimal? CustomField4 { get; set; }
        public bool? CustomField5 { get; set; }

        // Navigation properties
        public Accountant Manager { get; set; }

        // سازنده
        public Accountant()
        {
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
            IsActive = true;
            Gender = 'M';
            MaritalStatus = 'S';
        }
    }
}