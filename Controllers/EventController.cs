using Cucina_De_Corazon.Attributes;
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
        [Auth("Admin,Staff")]
        public IActionResult Calendar()
        {
            return View();
        }

        [Auth("Admin,Staff")]
        [HttpGet]
        public IActionResult GetEvents()
        {
            var events = _context.EventDetails
                .Select(o => new
                {
                    id = o.OrderId,
                    title = "Reserved",
                    start = o.EventDate.ToString("yyyy-MM-dd"),
                    color = "#EB6A00"
                })
                .ToList();

            return Json(events);
        }

        [Auth("Admin,Staff")]
        public IActionResult Details(int id)
        {
            var det = _context.Orders
                .Include(o => o.EventDetails)
                .Include(o => o.OrderProducts)
                .Include(o => o.Payments)
                .FirstOrDefault();
            return View(det);
        }
    }
}
