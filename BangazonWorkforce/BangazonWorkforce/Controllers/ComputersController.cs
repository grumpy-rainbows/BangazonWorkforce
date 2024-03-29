﻿using System;
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
            return View();
        }

        // POST: Computers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
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
        public ActionResult Delete(int id, ComputerDeleteViewModel viewModel)
        {
            ComputerDeleteViewModel comp = viewModel;
            List<Computer> AssignedComputers = isAssigned();
            foreach (Computer c in AssignedComputers)
            {
                if (c.Id == id)
                {
                    comp.isAssigned = true;
                    break;
                }
                else
                {
                    comp.isAssigned = false;
                }
            }
            using (SqlConnection conn = Connection) { 
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
                            DecomissionDate = null,
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                        };
                    }
                }
                    comp.Computer = computer;
                // close the connection and return the computer
                reader.Close();
                return View(comp);
            }
        }

    }

    // POST: Computers/Delete/5
    [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, ComputerDeleteViewModel viewModel )
        {

     
            
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                                       DELETE FROM Computer WHERE Id = @Id 
                                       AND Id NOT IN (SELECT ComputerId FROM ComputerEmployee)

                                                ";

                        cmd.Parameters.Add(new SqlParameter("@Id", id));
                        SqlDataReader reader = cmd.ExecuteReader();
                 
                        return RedirectToAction(nameof(Index));
                    }
                }
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
        //Tthis is a private method to filter out which computers are assigned.
        private List<Computer> isAssigned()
        {
            using(SqlConnection conn = Connection )
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //sql statement to select the computers which have ever been assigned. These computers only exist in the join table, so we're checking to see if the id from the Computer table matches the computerId in employeeComputer.
                    cmd.CommandText = $@"SELECT Id,
                                                Make, 
                                                Manufacturer,
                                                PurchaseDate
                                              
                                                FROM Computer 
                                                WHERE Id IN (SELECT ComputerId FROM ComputerEmployee)
";
                    
                   
                    
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Computer> AssignedComputers = new List<Computer>();
                    while (reader.Read())
                    {
                       Computer computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                        };
                        AssignedComputers.Add(computer);
                    }
                    // close the connection and return the assigned computer
                    reader.Close();
                    return AssignedComputers;

                }

            }
        }
    }
}