using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewPracticeProject.Data;
using NewPracticeProject.Models;
using NewPracticeProject.Models.ViewModel;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

//using Microsoft.AspNetCore.Http;


namespace NewPracticeProject.Controllers.Account
{

    public class AccountController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly EmailService _emailService;
      /*  private readonly IHttpContextAccessor _httpContextAccessor;*/
        public AccountController(AppDbContext dbContext, IEmailService emailService /*, IHttpContextAccessor httpContextAccessor*/)
        {
            _dbContext = dbContext;
            _emailService = (EmailService?)emailService;
           /* _httpContextAccessor = httpContextAccessor;*/
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }


       
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginSignUpViewModel model)
        {
            var user = await _dbContext.studentRegisters.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError("Password", "Password is required.");
                return View(model);
            }

            if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                if (user.IsConfirmed)
                {
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                        
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                  /*  // Set session variable to indicate user is authenticated
                    HttpContext.Session.SetString("IsAuthenticated", "true");
*/

                    return RedirectToAction("Index", "Account");
                }
                else
                {
                    TempData["NotConfirmedErrorMessage"] = "Your registration is not yet confirmed. Please check your email for a confirmation link.";
                }
            }
            else
            {
                TempData["LoginErrorMessage"] = "Incorrect username or password.";
                return RedirectToAction("Login");
            }

            return View(model);
        }




        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

       
        public IActionResult SignUp() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(LoginSignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Hash the user's password using BCrypt
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                var newUser = new StudentRegister
                {
                    Email = model.Email,
                    Password = hashedPassword, // Store the hashed password
                    IsActive = true,
                    IsConfirmed = false// Mark as inactive until confirmed
                };

                _dbContext.studentRegisters.Add(newUser);
                _dbContext.SaveChanges();

                // Send HTML email with the confirmation link
                string confirmationLink = Url.Action("ConfirmRegistration", "Account", new { email = model.Email }, Request.Scheme);
                string confirmationMessage = $@"
            <html>
            <body>
                <p>Thank you for registering!</p>
                <p>Please click the following link to confirm your registration:</p>
                <a href='{confirmationLink}'>Confirm Registration</a>
            </body>
            </html>";

                await _emailService.SendEmailAsync(model.Email, "Confirm Your Registration", confirmationMessage);

                TempData["ThankYouMessage"] = "Thank you for registering! An email has been sent to you.";

                return RedirectToAction("Login");
            }

            return View(model);
        }


        public IActionResult ConfirmRegistration(string email)
        {
            var user = _dbContext.studentRegisters.FirstOrDefault(u => u.Email == email && !u.IsConfirmed);

            if (user != null)
            {
                user.IsConfirmed = true;
                _dbContext.SaveChanges();
                TempData["ConfirmationMessage"] = "Your registration is confirmed. Please log in.";
            }
            else
            {
                TempData["ConfirmationMessage"] = "Confirmation failed. User not found.";
            }

            return RedirectToAction("Login");
        }


    }
}

