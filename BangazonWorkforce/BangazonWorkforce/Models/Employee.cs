/*
    Author: Jameka Echols
    Purpose: This is the data model which mirrors the columns in the EmployeeTable in the db.
    Methods: None
 
*/


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public int DepartmentId { get; set; }
        [Required]
        public bool IsSupervisor { get; set; }
        public Department Department { get; set; }
        public List<TrainingProgram> EmployeeTrainingPrograms { get; set; }

    }
}
