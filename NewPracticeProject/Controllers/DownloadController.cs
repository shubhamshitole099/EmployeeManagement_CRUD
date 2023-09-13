using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewPracticeProject.Data;
using NewPracticeProject.Models;
using NewPracticeProject.Services; // Include your service namespace
using OfficeOpenXml;

namespace NewPracticeProject.Controllers
{
    [Authorize]
    public class DownloadController : Controller
    {
        private readonly IExcelExportService _excelExportService;
        private readonly AppDbContext _dbContext; // Replace with your DbContext

        public DownloadController(IExcelExportService excelExportService, AppDbContext dbContext)
        {
            _excelExportService = excelExportService;
            _dbContext = dbContext;
        }

        // Action to download Excel files
       
        public IActionResult Download(string dataType)
        {
            IEnumerable<object> data;
            string fileName = "";

            // Fetch data based on the dataType parameter
            switch (dataType.ToLower())
            {
                case "employees":
                    data = GetEmployeeData();
                    fileName = "Employees.xlsx";
                    break;

                case "departments":
                    data = GetDepartmentData();
                    fileName = "Departments.xlsx";
                    break;

                case "audithistory":
                    data = GetAuditData();
                    fileName = "AuditHistory.xlsx";
                    break;

                default:
                    // Handle unknown dataType or provide a default option
                    return RedirectToAction("Index", "Account");
            }

            // Generate and return the Excel file
            var excelFile = _excelExportService.GenerateExcel(data, fileName);
            return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        // Implement methods to fetch data for employees, departments, and audit history
        private IEnumerable<object> GetEmployeeData()
        {
            // Replace this with your data retrieval logic from the database
            var employees = _dbContext.Employees.Select(e => new
            {
                e.Id,
                e.FirstName,
                e.LastName,
                e.DepartmentId // Include only the relevant columns
            });
            return employees;
        }

        private IEnumerable<object> GetDepartmentData()
        {
            var departments = _dbContext.Departments.Select(d => new
            {
                d.Id,
                d.Name // Include only the relevant columns
            });
            return departments;
        }
        private IEnumerable<object> GetAuditData()
        {
            var userEmail = User.Identity.Name;

            var auditHistory = _dbContext.Audits
                .Where(a => a.UserId == userEmail)
                .OrderByDescending(a => a.Timestamp)
                .ToList();

            var formattedAuditHistory = auditHistory.Select(audit => new
            {
                audit.Action,
                Timestamp = audit.Timestamp.ToString("dd-MM-yyyy HH:mm:ss"),
                audit.EmployeeName,
                audit.Details
            });

            return formattedAuditHistory;
        }




    }
}

