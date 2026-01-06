using Cucina_De_Corazon.Context;
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
            var records = _context.Orders
                .Include(o => o.EventDetails)
                .Include(o => o.Payments)
                .Include(o => o.OrderProducts)
             .ToList();
            return View(records);
        }

        public IActionResult Details(int id)
        {
            var record = _context.Orders
                .Include(o => o.EventDetails)
                .Include(o => o.Payments)
                .Include(o => o.OrderProducts)
                .FirstOrDefault(b => b.OrderId == id);
            if (record == null) return NotFound();
            return View(record);
        }
    }
}
