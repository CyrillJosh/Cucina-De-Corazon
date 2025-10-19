using Cucina_De_Corazon.Context;
using Cucina_De_Corazon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Cucina_De_Corazon.Controllers
{
    public class CartController : Controller
    {
        private readonly MyDBContext _context;

        public CartController(MyDBContext context)
        {
            _context = context;
        }

        //public IActionResult Test()
        //{
        //    var emailService = HttpContext.RequestServices.GetService<IEmailService>();
        //    Task.Run(() => emailService.SendOrderConfirmationAsync("odatocyrilljosh@gmail.com", "Cyrill Josh", DateTime.Now));
        //    return RedirectToAction("Index");
        //}

        public IActionResult GetUnavailableDates()
        {
            var dates = _context.Orders
                .Select(o => o.ReservedDate)
                .Distinct()
                .ToList();

            return Json(dates);
        }
        [HttpPost]
        [HttpPost]
        public IActionResult ConfirmOrder(string type, string address, DateTime? reservedDate = null, string instructions = "")
        {
            int? sessionid = HttpContext.Session.GetInt32("User");
            if (sessionid == null|| sessionid <= 0)
                return Json(new { location = "/User/Login" });

            if (!reservedDate.HasValue)
                return Json(new { success = false, message = "Please select a valid reservation date." });

            if (string.IsNullOrEmpty(address))
                return Json(new { success = false, message = "Please provide a delivery or event address." });

            var cartJson = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(cartJson))
                return Json(new { success = false, message = "Cart is empty." });

            var cart = JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
            if (cart == null || !cart.Any())
                return Json(new { success = false, message = "Cart is empty." });

            // Create new order
            var order = new Order
            {
                Instructions = instructions,
                ReservedDate = reservedDate,
                Type = type,
                Address = address,
                IsActive = false
            };

            foreach (var c in cart)
            {
                order.OrderProducts.Add(new OrderProduct
                {
                    ProductId = c.ProductId,
                    Quantity = c.Qty,
                    Price = c.Price
                });
            }

            _context.Orders.Add(order);
            _context.SaveChanges();

            // Associate a bill
            var bill = new Bill
            {
                Total = cart.Sum(x => x.Total),
                Status = "Pending",
                PersonId = sessionid
            };
            bill.Orders.Add(order);
            _context.Bills.Add(bill);
            _context.SaveChanges();

            HttpContext.Session.Remove("Cart"); // clear cart

            // --- SEND EMAIL AS BACKGROUND TASK ---
            var user = _context.Accounts.Include(x => x.Person).FirstOrDefault(u => u.AccountId == sessionid);
            if (user != null && !string.IsNullOrEmpty(user.Person.Email))
            {
                var emailService = HttpContext.RequestServices.GetService<IEmailService>();
                Task.Run(() => emailService.SendOrderConfirmationAsync(user.Person.Email, user.Username, reservedDate.Value, "Pending"));
            }

            return Json(new { success = true, message = "Order confirmed successfully!" });
        }


        [HttpPost]
        public IActionResult AddToCart(int productId, string productName, string productPic, decimal price, int qty, int pax)
        {
            // Get cart from session (or create a new one)
            var cartJson = HttpContext.Session.GetString("Cart");
            List<CartItem> cart = string.IsNullOrEmpty(cartJson)
                ? new List<CartItem>()
                : JsonConvert.DeserializeObject<List<CartItem>>(cartJson);

            // Check if item already exists in cart
            var existingItem = cart.FirstOrDefault(x => x.ProductId == productId && x.Pax == pax);
            if (existingItem != null)
            {
                existingItem.Qty += qty;
                existingItem.Total = existingItem.Qty * existingItem.Price;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = productId,
                    ProductName = productName,
                    ProductPic = productPic,
                    Price = price/qty,
                    Qty = qty,
                    Total = price,
                    Pax = pax
                });
            }

            // Save back to session
            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));

            return Json(new { success = true, message = "Added to cart successfully!" });
        }
        [HttpPost]
        public IActionResult UpdateQuantity([FromBody] UpdateQuantityRequest request)
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(cartJson))
                return Json(new { success = false, message = "Your cart is empty." });

            var cart = JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
            var item = cart.FirstOrDefault(x => x.ProductId == request.ProductId);

            if (item == null)
                return Json(new { success = false, message = "Item not found in cart." });

            if (request.Quantity <= 0)
                return Json(new { success = false, message = "Quantity must be at least 1." });

            // ✅ Update quantity and total
            item.Qty = request.Quantity;
            item.Total = item.Price * item.Qty;

            // ✅ Save back to session
            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));

            var cartTotal = cart.Sum(x => x.Total);

            return Json(new
            {
                success = true,
                itemTotal = item.Total,
                cartTotal = cartTotal
            });
        }

        public class UpdateQuantityRequest
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }


        public IActionResult Index()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            var cart = string.IsNullOrEmpty(cartJson)
                ? new List<CartItem>()
                : JsonConvert.DeserializeObject<List<CartItem>>(cartJson);

            return View(cart);
        }

        [HttpPost]
        public IActionResult Remove(int productId)
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(cartJson))
                return Json(new { success = false, message = "Cart is empty." });

            var cart = JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
            var item = cart.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
                cart.Remove(item);

            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
            return Json(new { success = true, message = "Item removed successfully." });
        }

        [HttpPost]
        public IActionResult Clear()
        {
            HttpContext.Session.Remove("Cart");
            return Json(new { success = true, message = "Cart cleared." });
        }

        public class CartItem
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string ProductPic { get; set; }
            public int Pax { get; set; }
            public decimal Price { get; set; }
            public int Qty { get; set; }
            public decimal Total { get; set; }
        }
    }
}
