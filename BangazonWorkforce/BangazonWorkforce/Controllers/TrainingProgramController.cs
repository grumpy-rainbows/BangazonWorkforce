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
                                        tr.MaxAtendees
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
                            MaxAtendees = reader.GetInt32(reader.GetOrdinal("MaxAtendees"))
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
                                    SELECT tr.Id,
                                        tr.Name,
                                        tr.StartDate,
                                        tr.EndDate,
                                        tr.MaxAttendees
                                        FROM TrainingProgram tr";

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    TrainingProgram trainingProgram = null;
                    if (reader.Read())
                    {
                        trainingProgram = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAtendees = reader.GetInt32(reader.GetOrdinal("MaxAtendees"))
                        };


                        reader.Close();

                        return View(trainingProgram);
                    }
                    else
                    {
                        return new StatusCodeResult(StatusCodes.Status404NotFound);
                    }

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
                ( Id, Name, StartDate, EndDate, MaxAtendees )
                VALUES
                ( @Id, @Name, @StartDate, @EndDate, @MaxAtendees )";
                        cmd.Parameters.Add(new SqlParameter("@Id", model.trainingProgram.Id));
                        cmd.Parameters.Add(new SqlParameter("@Name", model.trainingProgram.Name));
                        cmd.Parameters.Add(new SqlParameter("@StartDate", model.trainingProgram.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@EndDate", model.trainingProgram.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@MaxAttendees", model.trainingProgram.MaxAtendees));
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
    }
}