using System;
using System.ComponentModel.DataAnnotations;

namespace NewPracticeProject.Models
{
    public class Audit
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } // User who performed the action

        [Required]
        public string Action { get; set; } // Action performed (e.g., "Login", "AddEmployee", "DeleteEmployee")

        [Required]
        public DateTime Timestamp { get; set; } // Timestamp of the action

        // Additional properties you may want to include
        public string EmployeeName { get; set; } // Name of the affected employee (for employee-related actions)
        public string Details { get; set; } // Additional details about the action
    }
}
