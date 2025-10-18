using Cucina_De_Corazon.Context;
using Cucina_De_Corazon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Cucina_De_Corazon.Controllers
{
    public class MenuController : Controller
    {
        private readonly MyDBContext _context;

        public MenuController(MyDBContext context)
        {
            _context = context;
        }

        public IActionResult MyOrders()
        {
            var userid = HttpContext.Session.GetInt32("User");
            var orders = _context.Bills.Include(x => x.Orders).ThenInclude(o => o.OrderProducts).ThenInclude(op => op.Product).Where(x => x.PersonId == userid).ToList();
            return View(orders);
        }
        public IActionResult Products(int category)
        {
            var prods = _context.Products.Include(x => x.Category).Where(x => x.CategoryId == category && x.IsAvailable).ToList();
            return View(prods);
        }
        public IActionResult Index()
        {
            var products = _context.Products.Include(p => p.Category).Where(x => x.IsAvailable).ToList();
            return View(products);
        }

        public IActionResult Details(int id)
        {
            var product = _context.Products.Include(p => p.Category)
                                           .FirstOrDefault(p => p.ProductId == id);
            if (product == null) return NotFound();
            return View(product);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("ProductName,ProductDescription,ProductPicture,CategoryId")] Product product, decimal MinPrice, decimal MaxPrice)
        {
            if (MinPrice < 0 || MaxPrice < 0)
            {
                ModelState.AddModelError("ProductPrice", "Price cannot be negative.");
            }
            if (MaxPrice < MinPrice)
            {
                ModelState.AddModelError("", "10 pax price cannot be less than 5 pax price.");
            }
            else
            {
                // Combine MinPrice and MaxPrice into a single string
                product.ProductPrice = $"{MinPrice}-{MaxPrice}";
            }

            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }
        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            // Parse existing price
            decimal minPrice = 0, maxPrice = 0;
            if (!string.IsNullOrEmpty(product.ProductPrice))
            {
                var prices = product.ProductPrice.Split('-');
                if (prices.Length == 2)
                {
                    decimal.TryParse(prices[0].Trim(), out minPrice);
                    decimal.TryParse(prices[1].Trim(), out maxPrice);
                }
            }

            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("ProductId,ProductName,ProductDescription,ProductPicture,CategoryId")] Product product, decimal MinPrice, decimal MaxPrice)
        {
            if (id != product.ProductId) return NotFound();

            // Validate 10 pax price
            if (MaxPrice < MinPrice)
            {
                ModelState.AddModelError("", "10 pax price cannot be less than 5 pax price.");
            }

            if (ModelState.IsValid)
            {
                // Format price
                product.ProductPrice = $"{MinPrice:0.00} - {MaxPrice:0.00}";

                _context.Update(product);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
                return Json(new { success = false, message = "Product not found." });

            product.IsAvailable = false;
            _context.SaveChanges();

            return Json(new { success = true });
        }
    }
}
