using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPracticeProject.Data;
using NewPracticeProject.Models;

namespace NewPracticeProject.Controllers.EmployeeAndDepartment
{
    public class DepartmentsController : Controller
    {
        private readonly AppDbContext _dbContext;

        public DepartmentsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            var departments = _dbContext.Departments.ToList();
            return View(departments);
        }

        [Authorize]

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Department department)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Departments.Add(department);
                _dbContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Department department)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Entry(department).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return RedirectToAction("Index"); // Redirect to department list
            }
            return View(department);
        }




        [Authorize]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var department = _dbContext.Departments.Find(id);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }




        [HttpGet]
        [Authorize]
        public IActionResult Edit(int id)
        {
            var department = _dbContext.Departments.Find(id);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }









        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmDelete(int id)
        {
            var department = _dbContext.Departments.Find(id);
            if (department == null)
            {
                return NotFound();
            }

            _dbContext.Departments.Remove(department);
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }



        [Authorize]
        public IActionResult Details(int id)
        {
            var department = _dbContext.Departments.FirstOrDefault(d => d.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }




    }
}
