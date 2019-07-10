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
    public class TrainingProgramController : Controller
    {

        private readonly IConfiguration _config;

        public TrainingProgramController(IConfiguration config)
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

        // GET: TrainingProgram
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                    SELECT tr.Id,
                                        tr.Name,
                                        tr.StartDate,
                                        tr.EndDate,
                                        tr.MaxAttendees
                                        FROM TrainingProgram tr";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();
                    while (reader.Read())
                    {
                        TrainingProgram trainingProgram = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        };

                        trainingPrograms.Add(trainingProgram);
                    }

                    reader.Close();

                    return View(trainingPrograms);
                }
            }
        }



        // GET: TrainingProgram/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                    SELECT tr.Id As TrainingProgramId,
                                        tr.Name,
                                        tr.StartDate,
                                        tr.EndDate,
                                        tr.MaxAttendees,
                                        e.Id As EmployeeId,
                                        e.FirstName,
                                        e.LastName,
                                        e.DepartmentId,
                                        e.Issupervisor
                                        FROM TrainingProgram tr
                                        LEFT JOIN EmployeeTraining et ON et.TrainingProgramId = tr.Id
                                        LEFT JOIN Employee e ON e.Id = et.EmployeeId
                                        WHERE tr.Id = @Id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    TrainingProgram trainingProgram = null;
                    while (reader.Read())
                    {
                        if (trainingProgram == null)
                        {
                            trainingProgram = new TrainingProgram
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("TrainingProgramId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                            };
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("TrainingProgramId")))
                        {
                            int employeeId = reader.GetInt32(reader.GetOrdinal("employeeId"));
                            if (!trainingProgram.Attendees.Any(e => e.Id == employeeId))
                            {
                                Employee employee = new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("employeeId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    DepartmentId = reader.GetInt32(reader.GetOrdinal("departmentId")),
                                    IsSupervisor = reader.GetBoolean(reader.GetOrdinal("isSupervisor"))
                                };
                                trainingProgram.Attendees.Add(employee);
                            }
                        }
                    }
                    reader.Close();

                    return View(trainingProgram);
                }


            }
        }



        // GET: TrainingProgram/Create
        public ActionResult Create()
        {
            TrainingProgramCreateViewModel viewModel = new TrainingProgramCreateViewModel();

            return View();
        }

        // POST: TrainingProgram/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TrainingProgramCreateViewModel model)
        {

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO TrainingProgram
                (  Name, StartDate, EndDate, MaxAttendees )
                VALUES
                (  @Name, @StartDate, @EndDate, @MaxAttendees )";

                    cmd.Parameters.Add(new SqlParameter("@Name", model.trainingProgram.Name));
                    cmd.Parameters.Add(new SqlParameter("@StartDate", model.trainingProgram.StartDate));
                    cmd.Parameters.Add(new SqlParameter("@EndDate", model.trainingProgram.EndDate));
                    cmd.Parameters.Add(new SqlParameter("@MaxAttendees", model.trainingProgram.MaxAttendees));
                    await cmd.ExecuteNonQueryAsync();

                    return RedirectToAction(nameof(Index));



                }
            }

        }


        // GET: TrainingProgram/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TrainingProgram/Edit/5
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

        // GET: TrainingProgram/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TrainingProgram/Delete/5
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

        private TrainingProgram GetTrainingProgramById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                    SELECT tr.Id As TrainingProgramId,
                                        tr.Name,
                                        tr.StartDate,
                                        tr.EndDate,
                                        tr.MaxAttendees,
                                        e.Id As EmployeeId,
                                        e.FirstName,
                                        e.LastName,
                                        e.DepartmentId,
                                        e.Issupervisor
                                        FROM TrainingProgram tr
                                        LEFT JOIN EmployeeTraining et ON et.TrainingProgramId = tr.Id
                                        LEFT JOIN Employee e ON e.Id = et.EmployeeId
                                        WHERE tr.Id = @Id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    TrainingProgram trainingProgram = null;
                    while (reader.Read())
                    {
                        if (trainingProgram == null)
                        {
                            trainingProgram = new TrainingProgram
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("TrainingProgramId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                            };
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("TrainingProgramId")))
                        {
                            int employeeId = reader.GetInt32(reader.GetOrdinal("employeeId"));
                            if (!trainingProgram.Attendees.Any(e => e.Id == employeeId))
                            {
                                Employee employee = new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("employeeId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    DepartmentId = reader.GetInt32(reader.GetOrdinal("departmentId")),
                                    IsSupervisor = reader.GetBoolean(reader.GetOrdinal("isSupervisor"))
                                };
                                trainingProgram.Attendees.Add(employee);
                            }
                        }
                    }
                    reader.Close();
                    return trainingProgram;
                }
            }
        }
    }
}
