using Cucina_De_Corazon.Context;
using Cucina_De_Corazon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace Cucina_De_Corazon.Controllers
{
    public class UserController : Controller
    {
        private readonly MyDBContext _context;

        public UserController(MyDBContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var users = _context.Accounts
                .Include(a => a.Person)
                .ToList();
            return View(users);
        }

        [HttpGet]
        public IActionResult CreateStaff()
        {
            // Optional: Check if current user is admin
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
                return RedirectToAction("AccessDenied", "Home");

            return View();
        }

        [HttpPost]
        public IActionResult CreateStaff(string fullName, string email, string username, string password)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
                return Json(new { success = false, message = "Unauthorized access." });

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return Json(new { success = false, message = "Username and password are required." });

            if (_context.Accounts.Any(a => a.Username == username))
                return Json(new { success = false, message = "Username already exists." });

            // Create new Person record
            var person = new Person
            {
                FullName = fullName,
                Email = email,
                CreatedAt = DateTime.Now
            };
            _context.People.Add(person);
            _context.SaveChanges();

            // Create new Staff Account
            var account = new Account
            {
                PersonId = person.PersonId,
                Username = username,
                Password = password, // 🔒 TODO: Hash password in production
                Role = "Staff",
                CreatedAt = DateTime.Now
            };
            _context.Accounts.Add(account);
            _context.SaveChanges();

            return Json(new { success = true, message = "Staff account created successfully!" });
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string fullName, string email, string number, string username, string password)
        {
            if (_context.Accounts.Any(a => a.Username == username))
            {
                ViewBag.Error = "Username already exists!";
                return View();
            }

            if (_context.People.Any(p => p.Email == email))
            {
                ViewBag.Error = "Email already registered!";
                return View();
            }

            var person = new Person
            {
                FullName = fullName,
                Email = email,
                ContactNumber = number,
                CreatedAt = DateTime.Now
            };
            _context.People.Add(person);
            _context.SaveChanges();

            var account = new Account
            {
                PersonId = person.PersonId,
                Username = username,
                Password = HashPassword(password),
                Role = "Customer"
            };
            _context.Accounts.Add(account);
            _context.SaveChanges();

            TempData["Success"] = "Registration successful! You can now login.";
            return RedirectToAction("Login");
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var hashed = HashPassword(password);
            var account = _context.Accounts
                .Where(a => a.Username == username && a.Password == hashed)
                .FirstOrDefault();

            if (account != null)
            {
                HttpContext.Session.SetInt32("User", account.AccountId);
                HttpContext.Session.SetString("Role", account.Role);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid username or password!";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}


