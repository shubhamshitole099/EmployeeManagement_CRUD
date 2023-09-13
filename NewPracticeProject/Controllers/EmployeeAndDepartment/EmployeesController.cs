using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewPracticeProject.Data;
using NewPracticeProject.Models;
using NewPracticeProject.Models.ViewModel;
using OfficeOpenXml;
using System.IO;


namespace NewPracticeProject.Controllers.EmployeeAndDepartment
{
    public class EmployeesController : Controller
    {
        private readonly AppDbContext _dbContext;

        public EmployeesController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }



        [Authorize]
        public IActionResult Index()
        {
            var employees = _dbContext.Employees.Include(e => e.Department).ToList();
            return View(employees);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            var departments = _dbContext.Departments.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name
            });

            var viewModel = new EmployeeCreateViewModel
            {
                Departments = departments
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeCreateViewModel viewModel)
        {
          
                var employee = new Employee
                {
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    DepartmentId = viewModel.SelectedDepartmentId
                };

                _dbContext.Employees.Add(employee);
                await _dbContext.SaveChangesAsync();

                // Log the action in the Audit table
                var audit = new Audit
                {
                    UserId = User.Identity.Name,
                    Action = "AddEmployee",
                    Timestamp = DateTime.Now,
                    EmployeeName = $"{employee.FirstName} {employee.LastName}",
                    Details = "Employee added successfully"
                };

                _dbContext.Audits.Add(audit);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            

        }



        [HttpGet]
        [Authorize]
        public IActionResult Edit(int id)
        {
            var employee = _dbContext.Employees.Include(e => e.Department).FirstOrDefault(e => e.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            var departments = _dbContext.Departments.ToList();
            var viewModel = new EmployeeEditViewModel
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                SelectedDepartmentId = employee.DepartmentId,
                Departments = departments.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name })
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EmployeeEditViewModel viewModel)
        {

            var employee = _dbContext.Employees.FirstOrDefault(e => e.Id == viewModel.Id);
            if (employee == null)
            {
                return NotFound();
            }

            employee.FirstName = viewModel.FirstName;
            employee.LastName = viewModel.LastName;
            employee.DepartmentId = viewModel.SelectedDepartmentId;


            var audit = new Audit
            {
                UserId = User.Identity.Name,
                Action = "EditEmployee",
                Timestamp = DateTime.Now,
                EmployeeName = $"{employee.FirstName} {employee.LastName}",
                Details = "Employee details updated"
            };

            _dbContext.Audits.Add(audit);

            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));


           
        }



        [HttpGet]
        [Authorize]
        public IActionResult Delete(int id)
        {
            var employee = _dbContext.Employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            // Log the action in the Audit table
         
            _dbContext.SaveChanges();

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmDelete(int id)
        {
            var employee = _dbContext.Employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            _dbContext.Employees.Remove(employee);

            // Log the action in the Audit table
            var audit = new Audit
            {
                UserId = User.Identity.Name,
                Action = "DeleteEmployee",
                Timestamp = DateTime.Now,
                EmployeeName = $"{employee.FirstName} {employee.LastName}",
                Details = "Employee deleted successfully"
            };

            _dbContext.Audits.Add(audit);
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public IActionResult Details(int id)
        {
            var employee = _dbContext.Employees.Include(e => e.Department).FirstOrDefault(e => e.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            // Log the action in the Audit table
            var audit = new Audit
            {
                UserId = User.Identity.Name,
                Action = "ViewEmployeeDetails",
                Timestamp = DateTime.Now,
                EmployeeName = $"{employee.FirstName} {employee.LastName}",
                Details = "Viewed employee details"
            };

            _dbContext.Audits.Add(audit);
            _dbContext.SaveChanges();

            return View(employee);
        }


        [Authorize]
        public IActionResult AuditHistory()
        {
            var userId = User.Identity.Name;

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

            return View(auditHistory);
        }


        [Authorize]
        public IActionResult UserAuditHistory()
        {
            // Get the currently logged-in user's ID
            var userId = User.Identity.Name;

            // Query the audit history records for the current user
            var userAuditHistory = _dbContext.Audits
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Timestamp)
                .ToList();

            return View(userAuditHistory);
        }


    }
}
