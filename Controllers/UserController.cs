using Cucina_De_Corazon.Context;
using Cucina_De_Corazon.Models;
using Microsoft.AspNetCore.Mvc;
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


