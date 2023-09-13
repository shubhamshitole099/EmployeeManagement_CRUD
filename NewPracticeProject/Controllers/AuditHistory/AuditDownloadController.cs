using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using NewPracticeProject.Models;
using NewPracticeProject.Models.ViewModel;
using NewPracticeProject.Data;
using Microsoft.AspNetCore.Authorization;

namespace NewPracticeProject.Controllers.AuditHistory
{
    public class AuditDownloadController : Controller
    {
        private readonly AppDbContext _dbContext;

        public AuditDownloadController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [Authorize]
        [HttpGet]
        public IActionResult DownloadExcel()
        {
            var auditHistory = _dbContext.Audits.OrderByDescending(a => a.Timestamp)
                .Select(a => new AuditHistoryViewModel
                {
                    UserId = a.UserId,
                    Action = a.Action,
                    Timestamp = a.Timestamp,
                    EmployeeName = a.EmployeeName,
                    Details = a.Details
                })
                .ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Audit History");

                // Specify the column headers
                worksheet.Cells[1, 1].Value = "User ID";
                worksheet.Cells[1, 2].Value = "Action";
                worksheet.Cells[1, 3].Value = "Timestamp";
                worksheet.Cells[1, 4].Value = "Employee Name";
                worksheet.Cells[1, 5].Value = "Details";

                // Fill in the data
                for (int i = 0; i < auditHistory.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = auditHistory[i].UserId;
                    worksheet.Cells[i + 2, 2].Value = auditHistory[i].Action;

                    worksheet.Cells[i + 2, 3].Value = auditHistory[i].Timestamp.ToString("dd-MM-yyyy HH:mm:ss");
               

                    worksheet.Cells[i + 2, 4].Value = auditHistory[i].EmployeeName;
                    worksheet.Cells[i + 2, 5].Value = auditHistory[i].Details;
                }

                // Save the Excel package
                byte[] excelData = package.GetAsByteArray();
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AuditHistory.xlsx");
            }
        }
    }
}
