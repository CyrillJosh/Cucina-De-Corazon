using Cucina_De_Corazon.Context;
using Microsoft.AspNetCore.Mvc;

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
                    title = "Reserved",
                    start = o.ReservedDate.Value.ToString("yyyy-MM-dd"),
                    color = "#f6c23e" // golden-yellow
                })
                .ToList();

            return Json(events);
        }
    }
}
