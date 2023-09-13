using System.ComponentModel.DataAnnotations;

namespace NewPracticeProject.Models.ViewModel
{
    public class LoginSignUpViewModel
    {

        public int Id { get; set; }

        public string Email { get; set; }
 
        public string Password { get; set; }

        [Compare ("Password" ,ErrorMessage= "Password and ConfirmPassword do not match" )]
        public string ConfirmPassword { get; set; }
        public bool IsActive { get; set; }

    }
}
