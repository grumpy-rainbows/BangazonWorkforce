using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models
{
    public class ComputerEmployee
    {
        [Required]
        public int EmployeeId { get; set; }
        [Required]
        public int ComputerId { get; set; }
        
        public DateTime AssignDate { get; set; }
        [Required]
        public DateTime UnassignDate { get; set; }
    }
}
