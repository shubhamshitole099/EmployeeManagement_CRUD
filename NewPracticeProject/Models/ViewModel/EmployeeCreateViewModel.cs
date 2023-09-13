using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace NewPracticeProject.Models.ViewModel
{
    public class EmployeeCreateViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Department")]
        public int SelectedDepartmentId { get; set; }

        public IEnumerable<SelectListItem> Departments { get; set; }
    }
}
