﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models
{
    public class Department
    {

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public int Budget { get; set; }
        List<Employee> Employees { get; set; } = new List<Employee>();

        public int? NumberofEmployees { get; set; }

    }
}
