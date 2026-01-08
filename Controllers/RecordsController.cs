using Cucina_De_Corazon.Context;
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
                Products = _context.Products.ToList()
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
    }
}
