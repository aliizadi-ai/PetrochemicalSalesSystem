using PetrochemicalSalesSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace PetrochemicalSalesSystem.Data
{
    public class DepartmentRepository
    {
        public List<Department> GetAllDepartments()
        {
            List<Department> departments = new List<Department>();

            string query = @"
                SELECT d.*, p.DepartmentName as ParentDepartmentName
                FROM Departments d
                LEFT JOIN Departments p ON d.ParentDepartmentID = p.DepartmentID
                ORDER BY d.DepartmentName";

            DataTable dt = DatabaseHelper.ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                departments.Add(new Department
                {
                    DepartmentID = Convert.ToInt32(row["DepartmentID"]),
                    DepartmentCode = row["DepartmentCode"].ToString(),
                    DepartmentName = row["DepartmentName"].ToString(),
                    ParentDepartmentID = row["ParentDepartmentID"] != DBNull.Value ?
                        Convert.ToInt32(row["ParentDepartmentID"]) : (int?)null,
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    Description = row["Description"] != DBNull.Value ? row["Description"].ToString() : null,
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"])
                });
            }

            return departments;
        }

        public bool AddDepartment(Department department)
        {
            string query = @"
                INSERT INTO Departments (DepartmentCode, DepartmentName, ParentDepartmentID, IsActive, Description)
                VALUES (@Code, @Name, @ParentID, @IsActive, @Description)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Code", department.DepartmentCode),
                new SqlParameter("@Name", department.DepartmentName),
                new SqlParameter("@ParentID", (object)department.ParentDepartmentID ?? DBNull.Value),
                new SqlParameter("@IsActive", department.IsActive),
                new SqlParameter("@Description", (object)department.Description ?? DBNull.Value)
            };

            int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
            return rowsAffected > 0;
        }

        public bool UpdateDepartment(Department department)
        {
            string query = @"
                UPDATE Departments SET
                    DepartmentCode = @Code,
                    DepartmentName = @Name,
                    ParentDepartmentID = @ParentID,
                    IsActive = @IsActive,
                    Description = @Description
                WHERE DepartmentID = @ID";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ID", department.DepartmentID),
                new SqlParameter("@Code", department.DepartmentCode),
                new SqlParameter("@Name", department.DepartmentName),
                new SqlParameter("@ParentID", (object)department.ParentDepartmentID ?? DBNull.Value),
                new SqlParameter("@IsActive", department.IsActive),
                new SqlParameter("@Description", (object)department.Description ?? DBNull.Value)
            };

            int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
            return rowsAffected > 0;
        }

        public bool DeleteDepartment(int id)
        {
            // ابتدا بررسی کنیم آیا این دپارتمان استفاده شده یا نه
            string checkQuery = "SELECT COUNT(*) FROM Accountants WHERE DepartmentID = @ID";
            SqlParameter[] checkParams = new SqlParameter[]
            {
                new SqlParameter("@ID", id)
            };

            int count = Convert.ToInt32(DatabaseHelper.ExecuteScalar(checkQuery, checkParams));

            if (count > 0)
            {
                throw new Exception("این دپارتمان در حال استفاده است و نمی‌توان آن را حذف کرد.");
            }

            string query = "DELETE FROM Departments WHERE DepartmentID = @ID";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ID", id)
            };

            int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
            return rowsAffected > 0;
        }

        public Department GetDepartmentById(int id)
        {
            string query = "SELECT * FROM Departments WHERE DepartmentID = @ID";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ID", id)
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return new Department
                {
                    DepartmentID = Convert.ToInt32(row["DepartmentID"]),
                    DepartmentCode = row["DepartmentCode"].ToString(),
                    DepartmentName = row["DepartmentName"].ToString(),
                    ParentDepartmentID = row["ParentDepartmentID"] != DBNull.Value ?
                        Convert.ToInt32(row["ParentDepartmentID"]) : (int?)null,
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    Description = row["Description"] != DBNull.Value ? row["Description"].ToString() : null,
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"])
                };
            }

            return null;
        }
    }
}