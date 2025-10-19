using Cucina_De_Corazon.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cucina_De_Corazon.Controllers
{
    public class EventController : Controller
    {
        private readonly MyDBContext _context;

        public EventController(MyDBContext context)
        {
            _context = context;
        }

        // Returns calendar page
        public IActionResult Calendar()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetEvents()
        {
            var events = _context.Orders
                .Where(o => o.ReservedDate != null)
                .Select(o => new
                {
                    id = o.OrderId,
                    title = "Reserved",
                    start = o.ReservedDate.Value.ToString("yyyy-MM-dd"),
                    color = "#EB6A00"
                })
                .ToList();

            return Json(events);
        }

        public IActionResult Details(int id)
        {
            var det = _context.Bills
                .Include(x => x.Person)
                .Include(x => x.Orders)
                .ThenInclude(x => x.OrderProducts)
                .ThenInclude(x => x.Product)
                .Where(b => b.BillId == id)
                .FirstOrDefault();
            return View(det);
        }
    }
}
