﻿using System.ComponentModel.DataAnnotations;

namespace NewPracticeProject.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}
