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
    public class ComputersController : Controller

    {
        private readonly IConfiguration _config;

        public ComputersController(IConfiguration config)
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
        // GET: Computers
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id,
                        Make,
                        Manufacturer
                        FROM Computer
";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Computer> computers = new List<Computer>();
                    while (reader.Read())
                    {

                        Computer computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                        };
                        computers.Add(computer);

                    }
                    reader.Close();
                    return View(computers);
                }

            }

        }

        // GET: Computers/Details/5
        public ActionResult Details(int id)
        {
            //Computer computer = GetComputerById(id);
            //return View(computer);
            using (SqlConnection conn = Connection)
            {
                // open the connection 
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // run the query 
                    cmd.CommandText = $@"SELECT Id,
                                                PurchaseDate,
                                                DecomissionDate,
                                                Make,
                                                Manufacturer
                                        FROM Computer
                                        WHERE Id = @id;";

                    // parameters
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    Computer computer = null;
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                        {
                            computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                            };
                        }
                        else
                        {
                            //DateTime? nullDate = null;
                            
                            computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                DecomissionDate = null ,
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                            };
                        }
                    }
                    // close the connection and return the computer
                    reader.Close();
                    return View(computer);
                }
            }

        }

        // GET: Computers/Create
        public ActionResult Create()
        {
            // create a new instance of the computer
            ComputerCreateViewModel viewModel = new ComputerCreateViewModel();
            return View(viewModel);
        }

        // POST: Computers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ComputerCreateViewModel viewModel)
        {
            try
            {
                // TODO: Add insert logic here
                using(SqlConnection conn = Connection)
                {
                    conn.Open();
                    using(SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $@"INSERT INTO Computer(PurchaseDate, Make, Manufacturer)
                                            VALUES(@purchaseDate, @make, @manufacturer);";
                        // parameters
                        cmd.Parameters.Add(new SqlParameter("@purchaseDate", viewModel.Computer.PurchaseDate));
                        cmd.Parameters.Add(new SqlParameter("@make", viewModel.Computer.Make));
                        cmd.Parameters.Add(new SqlParameter("@manufacturer", viewModel.Computer.Manufacturer));

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

        // GET: Computers/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Computers/Edit/5
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

        // GET: Computers/Delete/5
        public ActionResult Delete(int id)
        {
            Computer computer = GetComputerById(id);
            return View();
        }

        // POST: Computers/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using(SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                                            DELETE FROM Computer WHERE Id = @Id, ";
                        cmd.Parameters.Add(new SqlParameter("@Id", id));
                        cmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // this is the function to be able to get one computer by id
        private Computer GetComputerById(int id)
        {
            using(SqlConnection conn = Connection)
            {
                // open the connection 
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    // run the query 
                    cmd.CommandText = $@"SELECT Id,
                                                PurchaseDate,
                                                DecomissionDate,
                                                Make,
                                                Manufacturer
                                        FROM Computer
                                        WHERE Id = @id;";

                    // parameters
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    Computer computer = null;
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                        };
                    }
                    // close the connection and return the computer
                    reader.Close();
                    return computer;
                }
            }
        }
    }
}