using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class EmployeeCreateViewModel
    {
        public Employee employee { get; set; }

        /*public List<Department> Departments { get; set; }*/

        public List<Department> AvailableDepartments { get; set; }

        public List<SelectListItem> AvailableDepartmentsSelectList
        {
            get
            {
                if (AvailableDepartments == null)
                {
                    return null;
                }
                return AvailableDepartments
                       .Select(d => new SelectListItem( d.Name, d.Id.ToString()))
                       .ToList();
            }
        }

        public EmployeeCreateViewModel(string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name, Budget FROM Department";
                    SqlDataReader reader = cmd.ExecuteReader();

                    AvailableDepartments = new List<Department>();

                    while (reader.Read())
                    {
                        AvailableDepartments.Add(new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                        });
                    }
                    reader.Close();
                }
            }
        }
    }
}
