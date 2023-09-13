namespace NewPracticeProject.Models.ViewModel
{
    public class AuditHistoryViewModel
    {
        public string UserId { get; set; }
        public string Action { get; set; }
        public DateTime Timestamp { get; set; }
        public string EmployeeName { get; set; }
        public string Details { get; set; }
    }
}
