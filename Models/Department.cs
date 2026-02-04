using System;

namespace PetrochemicalSalesSystem.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public int? ParentDepartmentID { get; set; }
        public bool IsActive { get; set; } = true;
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation property
        public Department ParentDepartment { get; set; }
    }
}