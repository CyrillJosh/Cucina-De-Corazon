using Cucina_De_Corazon.Context;
using Cucina_De_Corazon.Models;
using Cucina_De_Corazon.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cucina_De_Corazon.Controllers
{
    public class OrderController : Controller
    {
        private readonly MyDBContext _context;
        public OrderController(MyDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            OrderViewModel ovm = new OrderViewModel
            {
                Order = new Order(),
                Products = _context.Products.ToList()
            };
            return View(ovm);
        }
    }
}
