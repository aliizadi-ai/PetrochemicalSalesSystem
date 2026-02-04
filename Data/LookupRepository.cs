using PetrochemicalSalesSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace PetrochemicalSalesSystem.Data
{
    public class LookupRepository
    {
        public List<Department> GetDepartments()
        {
            List<Department> departments = new List<Department>();

            string query = "SELECT DepartmentID, DepartmentName FROM Departments WHERE IsActive = 1 ORDER BY DepartmentName";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                departments.Add(new Department
                {
                    DepartmentID = Convert.ToInt32(row["DepartmentID"]),
                    DepartmentName = row["DepartmentName"].ToString()
                });
            }

            return departments;
        }

        public List<Accountant> GetManagers()
        {
            List<Accountant> managers = new List<Accountant>();

            string query = @"
                SELECT AccountantID, FirstName, LastName, EmployeeCode 
                FROM Accountants 
                WHERE IsActive = 1 
                ORDER BY LastName, FirstName";

            DataTable dt = DatabaseHelper.ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                managers.Add(new Accountant
                {
                    AccountantID = Convert.ToInt64(row["AccountantID"]),
                    FirstName = row["FirstName"].ToString(),
                    LastName = row["LastName"].ToString(),
                    EmployeeCode = row["EmployeeCode"].ToString()
                });
            }

            return managers;
        }

        // گرفتن لیست استان‌ها
        public List<string> GetProvinces()
        {
            return new List<string>
            {
                "آذربایجان شرقی", "آذربایجان غربی", "اردبیل", "اصفهان",
                "البرز", "ایلام", "بوشهر", "تهران", "چهارمحال و بختیاری",
                "خراسان جنوبی", "خراسان رضوی", "خراسان شمالی", "خوزستان",
                "زنجان", "سمنان", "سیستان و بلوچستان", "فارس", "قزوین",
                "قم", "کردستان", "کرمان", "کرمانشاه", "کهگیلویه و بویراحمد",
                "گلستان", "گیلان", "لرستان", "مازندران", "مرکزی",
                "هرمزگان", "همدان", "یزد"
            };
        }

        // گرفتن لیست سطوح تحصیلی
        public List<string> GetEducationLevels()
        {
            return new List<string>
            {
                "زیر دیپلم", "دیپلم", "کاردانی", "کارشناسی",
                "کارشناسی ارشد", "دکتری", "فوق دکتری"
            };
        }

        public List<Accountant> GetAllAccountants()
        {
            List<Accountant> accountants = new List<Accountant>();

            string query = @"
                SELECT AccountantID, FirstName, LastName, EmployeeCode 
                FROM Accountants 
                WHERE IsActive = 1 
                ORDER BY LastName, FirstName";

            DataTable dt = DatabaseHelper.ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                accountants.Add(new Accountant
                {
                    AccountantID = Convert.ToInt64(row["AccountantID"]),
                    FirstName = row["FirstName"].ToString(),
                    LastName = row["LastName"].ToString(),
                    EmployeeCode = row["EmployeeCode"].ToString()
                });
            }

            return accountants;
        }
    }
}