using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace NewPracticeProject.Models.ViewModel
{
    public class EmployeeEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the first name.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter the last name.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please select a department.")]
        public int SelectedDepartmentId { get; set; }

        public IEnumerable<SelectListItem> Departments { get; set; }
    }
}
