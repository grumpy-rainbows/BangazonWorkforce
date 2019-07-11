using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkforce.Models;
using BangazonWorkforce.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonWorkforce.Controllers
{
    public class EmployeesController : Controller
    {

        private readonly IConfiguration _config;

        public EmployeesController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET: Employees
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT 
                                               e.Id AS EmployeeId,
                                               e.FirstName AS EmployeeFirstName ,
	                                           e.LastName AS EmployeeLastName ,
                                               d.Id as DepartmentId ,
                                               d.Budget AS DepartmentBudget ,
	                                           d.Name AS DepartmentName
                                        FROM Employee e
                                        JOIN Department AS d ON d.Id = e.DepartmentId";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Employee> employees = new List<Employee>();

                    while (reader.Read())
                    {
                        Employee employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                            FirstName = reader.GetString(reader.GetOrdinal("EmployeeFirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("EmployeeLastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            Department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Name = reader.GetString(reader.GetOrdinal("DepartmentName")),
                                Budget = reader.GetInt32(reader.GetOrdinal("DepartmentBudget"))
                            }
                        };

                        employees.Add(employee);
                    }
                    reader.Close();
                    return View(employees);
                }
            }
        }

        // GET: Employees/Details/5
        public ActionResult Details(int id)
        {

            using (SqlConnection conn = Connection)

            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT 
                                        e.Id AS EmployeeId,
                                        e.FirstName ,
                                        e.LastName ,
                                        e.IsSupervisor,
                                        d.Id as DepartmentId,
                                        d.Name AS DepartmentName,
                                        d.Budget AS DepartmentBudget,
                                        c.Id as ComputerId,
                                        c.Make as ComputerMake,
                                        c.Manufacturer as ComputerManufacturer,
                                        c.PurchaseDate as ComputerPurchaseDate,
                                        c.DecomissionDate as ComputerDecomissionDate,
                                        ce.AssignDate as ComputerAssignDate ,
                                        ce.UnassignDate as ComputerUnassignDate ,
                                        tp.Id as TrainingProgramId,
                                        tp.Name as TrainingProgramName,
                                        tp.StartDate as TrainingProgramStartDate,
                                        tp.EndDate as TrainingProgramEndDate,
                                        tp.MaxAttendees as TrainingProgramMaxAtendees
                                        FROM Employee e
                                        JOIN Department AS d on d.Id = e.DepartmentId
                                        LEFT JOIN ComputerEmployee AS ce on ce.EmployeeId = e.Id
                                        LEFT JOIN Computer AS c on c.Id = ce.ComputerId 
                                        LEFT JOIN EmployeeTraining AS et on et.EmployeeId = e.Id
                                        LEFT JOIN TrainingProgram AS tp on tp.Id = et.TrainingProgramId
                                        WHERE e.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@Id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    EmployeeDetailViewModel employee = null;

                    while (reader.Read())
                    {

                        if (employee == null)
                        {
                            employee = new EmployeeDetailViewModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Department = new Department
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                    Name = reader.GetString(reader.GetOrdinal("DepartmentName")),
                                    Budget = reader.GetInt32(reader.GetOrdinal("DepartmentBudget"))
                                },
                                Computer = new Computer()
                            };
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("ComputerId")))

                        {
                            employee.Computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("ComputerPurchaseDate")),
                                DecomissionDate = reader.IsDBNull(reader.GetOrdinal("ComputerDecomissionDate")) ? (DateTime?)null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("ComputerDecomissionDate")),
                                Make = reader.GetString(reader.GetOrdinal("ComputerMake")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("ComputerManufacturer"))

                            };
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("TrainingProgramId")))
                        {

                            if (!employee.TrainingProgramList.Exists(x => x.Id == reader.GetInt32(reader.GetOrdinal("TrainingProgramId"))))
                            {
                                employee.TrainingProgramList.Add(
                            new TrainingProgram
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("TrainingProgramId")),
                                Name = reader.GetString(reader.GetOrdinal("TrainingProgramName")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("TrainingProgramStartDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("TrainingProgramEndDate")),
                                MaxAttendees = reader.GetInt32(reader.GetOrdinal("TrainingProgramMaxAtendees"))
                            });
                            }
                        }

                    }
                        reader.Close();
                        return View(employee);
                }
            }
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            List<Department> departments = GetAllDepartments();

            EmployeeCreateViewModel viewModel =
                     new EmployeeCreateViewModel(_config.GetConnectionString("DefaultConnection"));

            viewModel.Departments = departments;

            return View(viewModel);
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee employee)
        {
            try
            {
                // TODO: Add insert logic here
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSuperVisor)
                                             VALUES (@firstname, @lastname, @departmentid, @issupervisor)";
                        cmd.Parameters.Add(new SqlParameter("@firstname", employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastname", employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@departmentid", employee.DepartmentId));
                        cmd.Parameters.Add(new SqlParameter("@issupervisor", employee.IsSuperVisor));

                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Employees/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        private List<Department> GetAllDepartments()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @" SELECT Id, Name, Budget FROM Department";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Department> departments = new List<Department>();
                    while (reader.Read())
                    {
                        Department department = new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                        };

                        departments.Add(department);
                    }

                    reader.Close();

                    return departments;
                }
            }
        }


        private Employee GetEmployeeById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT 
                                               e.Id AS EmployeeId,                                             
                                               e.FirstName ,
	                                           e.LastName ,
                                               e.IsSuperVisor ,
                                               d.Id as DepartmentId,
	                                           d.Name AS DepartmentName,
                                               c.Id as ComputerId,
											   c.Make as ComputerMake,
											   c.PurchaseDate as PurchaseDate,
											   c.DecomissionDate as DecomissionDate,
											   c.Manufacturer as Manufacturer
                                        FROM Employee e
                                        JOIN Department d on d.Id = e.DepartmentId
										LEFT JOIN ComputerEmployee AS ce on ce.EmployeeId = e.Id
                                        AND ce.UnAssignDate IS NULL
										LEFT JOIN Computer AS c on c.Id = ce.ComputerId								
										WHERE e.Id= @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Employee employee = null;

                    if (reader.Read())
                    {

                        employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                            FirstName = reader.GetString(reader.GetOrdinal("EmployeeFirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("EmployeeLastName")),
                            IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId"))
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("ComputerId")) && !reader.IsDBNull(reader.GetOrdinal("ComputerMake")) && !reader.IsDBNull(reader.GetOrdinal("PurchaseDate")) && !reader.IsDBNull(reader.GetOrdinal("Manufacturer")) && !reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                        {

                            employee.Computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate")),
                                Make = reader.GetString(reader.GetOrdinal("ComputerMake")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                            };

                        }
                    }
                    reader.Close();
                    return employee;
                }
            }
        }


    }
}