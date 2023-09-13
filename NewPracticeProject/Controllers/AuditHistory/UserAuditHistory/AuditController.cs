using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using NewPracticeProject.Data;

public class AuditController : Controller
{
    private readonly AppDbContext _dbContext;

    public AuditController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [Authorize]
    public IActionResult DownloadUserAuditHistory()
    {
        // Get the currently logged-in user's ID
        var userId = User.Identity.Name;

        // Query the audit history records for the current user
        var userAuditHistory = _dbContext.Audits
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Timestamp)
            .ToList();

        // Create a new Excel package
        using (var package = new ExcelPackage())
        {
            // Create a worksheet
            var worksheet = package.Workbook.Worksheets.Add("UserAuditHistory");

            // Define column headers
            worksheet.Cells[1, 1].Value = "Action";
            worksheet.Cells[1, 2].Value = "Timestamp";
            worksheet.Cells[1, 3].Value = "Employee Name";
            worksheet.Cells[1, 4].Value = "Details";

            // Populate the worksheet with audit history data
            for (var i = 0; i < userAuditHistory.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = userAuditHistory[i].Action;
                worksheet.Cells[i + 2, 2].Value = userAuditHistory[i].Timestamp.ToString("dd-MM-yyyy HH:mm:ss");
                worksheet.Cells[i + 2, 3].Value = userAuditHistory[i].EmployeeName;
                worksheet.Cells[i + 2, 4].Value = userAuditHistory[i].Details;
            }

            // Set content type and disposition for the response
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Headers.Add("Content-Disposition", "attachment; filename=UserAuditHistory.xlsx");

            // Write the Excel package to the response stream
            using (var stream = new MemoryStream(package.GetAsByteArray()))
            {
                stream.CopyTo(Response.Body);
            }

            return new EmptyResult();
        }
    }
}
