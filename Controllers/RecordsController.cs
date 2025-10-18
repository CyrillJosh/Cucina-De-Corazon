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
            var records = _context.Bills
             .Include(x => x.Orders)
             .Include(x => x.Person)
             .Where(x => x.Orders.Any())
             .OrderBy(x => x.Orders.First().ReservedDate)
             .ToList();
            return View(records);
        }

        public IActionResult Details(int id)
        {
            var record = _context.Bills
                .Include(x => x.Person)
                .Include(b => b.Orders)
                .ThenInclude(x => x.OrderProducts)
                .ThenInclude(op => op.Product)
                .FirstOrDefault(b => b.BillId == id);
            if (record == null) return NotFound();
            return View(record);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmBill(int id)
        {
            var bill = _context.Bills
                .Include(b => b.Person)
                .Include(b => b.Orders)
                .FirstOrDefault(b => b.BillId == id);

            if (bill == null || bill.Person == null)
                return NotFound();

            // Update the status
            bill.Status = "Confirmed";
            _context.Update(bill);
            await _context.SaveChangesAsync();

            var reservedDate = bill.Orders.FirstOrDefault()?.ReservedDate ?? DateTime.Now;
            var emailService = HttpContext.RequestServices.GetService<IEmailService>();
            Task.Run(() => emailService.SendOrderConfirmationAsync(bill.Person.Email,
                bill.Person.FullName,
                reservedDate,
                "Confirmed"));

            TempData["Message"] = "Bill has been confirmed and email sent successfully.";
            return RedirectToAction("Details", new { id = bill.BillId });
        }

    }
}
