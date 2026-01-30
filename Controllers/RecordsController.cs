using Cucina_De_Corazon.Context;
using Cucina_De_Corazon.Models;
using Cucina_De_Corazon.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cucina_De_Corazon.Controllers
{
    public class RecordsController : Controller
    {
        private readonly MyDBContext _context;
        private readonly IEmailService _emailService;
        public RecordsController(MyDBContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public IActionResult Index()
        {

            var records = new Records()
            {
                Orders = _context.Orders
                        .Include(o => o.EventDetails)
                        .Include(o => o.Payments)
                        .Include(o => o.OrderProducts)  
                        .ToList(),
                Products = _context.Products.Include(x => x.Category).ToList()
            };

            return View(records);
        }

        public IActionResult Details(int id)
        {
            var record = new OrderViewModel()
            {
                Order = _context.Orders
                       .Include(o => o.EventDetails)
                       .Include(o => o.Payments)
                       .Include(o => o.OrderProducts)
                    .FirstOrDefault(x => x.OrderId == id),
                Products = _context.Products.ToList()
            };
            if (record.Order == null) return NotFound();
            return View(record);
        }

        [HttpPost]
        public IActionResult AddPayment(Payment payment)
        {
            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Invalid please try again.";
                return RedirectToAction("Details", new { id = payment.OrderId });
            }

            payment.PaymentDate = DateTime.Now;

            // Save the new payment first
            _context.Payments.Add(payment);
            _context.SaveChanges();

            var order = _context.Orders
                .Include(o => o.Payments)
                .FirstOrDefault(o => o.OrderId == payment.OrderId);

            if (order == null)
            {
                TempData["Message"] = "Order not found.";
                return RedirectToAction("Details", new { id = payment.OrderId });
            }

            // Calculate totals
            decimal totalPaid = order.Payments.Sum(p => p.PaymentAmount ?? 0);
            decimal orderTotal = order.TotalAmount ?? 0;

            // Determine status
            string status = totalPaid >= orderTotal ? "Paid" : "Pending";

            // Update ALL payments for this order
            foreach (var p in order.Payments)
            {
                p.PaymentStatus = status;
            }

            _context.SaveChanges();

            TempData["Message"] = status == "Paid"
                ? "Order fully paid."
                : "Partial payment recorded.";

            return RedirectToAction("Details", new { id = payment.OrderId });

        }


    }
}
