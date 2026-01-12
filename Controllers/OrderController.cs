using Cucina_De_Corazon.Context;
using Cucina_De_Corazon.Models;
using Cucina_De_Corazon.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Cucina_De_Corazon.Controllers
{
    public class OrderController : Controller
    {
        private readonly MyDBContext _context;
        public OrderController(MyDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(OrderViewModel ovm = null)
        {
            ovm.Order ??= new Order();
            ovm.OrderItems ??= new List<OrderItem>();
            ovm.Products = _context.Products.Include(x => x.Category).OrderBy(x => x.CategoryId).ToList();
            return View(ovm);
        }

        [HttpPost]
        public IActionResult AddOrder(OrderViewModel ovm)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Please fill all required fields.");
                ovm.Products = _context.Products.ToList();
                return View("Index", ovm);
            }

            ovm.Order.OrderProducts = ovm.OrderItems.Select(i => new OrderProduct
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList();

            ovm.Order.TotalAmount = ovm.OrderItems.Sum(i => i.Price * i.Quantity);

            var payment = new Payment
            {
                PaymentAmount = ovm.Payment.PaymentAmount ?? 0,
                PaymentDate = DateTime.Now,
                PaymentMethod = ovm.Payment?.PaymentMethod ?? "Cash",
                PaymentStatus = ovm.Payment?.PaymentStatus ?? "Pending",
                ReferenceNumber = ovm.Payment?.ReferenceNumber
            };

            ovm.Order.Payments = new List<Payment> { payment };

            var user = HttpContext.Session.GetInt32("User");
            ovm.Order.CreatedBy = _context.Accounts.Find(user).Username;
            ovm.Order.OrderStatus = "Pending";


            ovm.Payment.PaymentStatus = ovm.Payment.PaymentAmount == ovm.Order.TotalAmount ? "Paid" : ovm.Payment.PaymentAmount.ToString()??"Partially Paid";

            _context.Orders.Add(ovm.Order);
            _context.SaveChanges();

            TempData["Message"] = "Successfully Added";
            return RedirectToAction("Index");
        }

    }
}
