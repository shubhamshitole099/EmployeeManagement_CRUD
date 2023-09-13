namespace NewPracticeProject.Models
{
    public class StudentRegister
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public bool IsActive { get; set; }

        public bool IsConfirmed { get; set; }

    }
}
