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

        public IActionResult Products(int category)
        {
            var prods = _context.Products.Include(x => x.Category).Where(x => x.CategoryId == category && x.IsAvailable).ToList();
            return View(prods);
        }
        public IActionResult Index()
        {
            var products = _context.Products.Include(p => p.Category).Where(x => x.IsAvailable).OrderBy(x => x.CategoryId).ToList();
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
        public IActionResult Create([Bind("ProductName,ProductDescription,ProductPicture,CategoryId")] Product product)
        {
            if (Convert.ToInt32(product.ProductPrice) < 0)
            {
                ModelState.AddModelError("ProductPrice", "Price cannot be negative.");
            }

            if (ModelState.IsValid)
            {
                product.IsAvailable = true;
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
