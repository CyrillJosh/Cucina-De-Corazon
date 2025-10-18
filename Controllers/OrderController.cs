using Cucina_De_Corazon.Context;
using Cucina_De_Corazon.Models;
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
        public IActionResult Edit(int id)
        {
            var order = _context.Orders
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .Include(o => o.Bills)
                .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
                return NotFound();

            ViewBag.Products = _context.Products.ToList();

            return View(order);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, string specialInstructions, List<OrderProduct> updatedProducts)
        {
            var order = _context.Orders
                .Include(x => x.Bills)
                .Include(o => o.OrderProducts)
                .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
                return NotFound();

            // Update instructions
            order.Instructions = specialInstructions;

            // Update or add products
            foreach (var p in updatedProducts)
            {
                var existing = order.OrderProducts.FirstOrDefault(op => op.ProductId == p.ProductId);
                if (existing != null)
                {
                    existing.Quantity += p.Quantity;
                    existing.Price = p.Price;
                }
                else
                {
                    order.OrderProducts.Add(new OrderProduct
                    {
                        OrderId = id,
                        ProductId = p.ProductId,
                        Quantity = p.Quantity,
                        Price = p.Price
                    });
                }
            }

            // Recalculate total
            order.Bills.First().Total = (decimal)order.OrderProducts.Sum(op => op.Price * op.Quantity);

            // Reset status
            order.Bills.First().Status = "Pending";

            _context.SaveChangesAsync();

            return RedirectToAction("MyOrders", "Menu");
        }

        // LIVE Quantity Update (AJAX)
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int orderProductId, int quantity)
        {
            var orderProduct = await _context.OrderProducts
                .Include(op => op.Order)
                .ThenInclude(x => x.Bills)
                .FirstOrDefaultAsync(op => op.ProductId == orderProductId);

            if (orderProduct == null)
                return NotFound();

            orderProduct.Quantity = quantity;
            orderProduct.Order.Bills.First().Total = (decimal)orderProduct.Order.OrderProducts.Sum(p => p.Price * p.Quantity);
            orderProduct.Order.Bills.First().Status = "Pending";

            await _context.SaveChangesAsync();

            return Json(new { success = true, newTotal = orderProduct.Order.Bills.First().Total.ToString("N2") });
        }
        [HttpPost]
        public JsonResult RemoveProduct([FromBody] RemoveProductRequest request)
        {
            if (request == null || request.OrderId == 0 || request.ProductId == 0)
                return Json(new { success = false, message = "Invalid request." });

            var orderProduct = _context.OrderProducts
                .FirstOrDefault(op => op.OrderId == request.OrderId && op.ProductId == request.ProductId);

            if (orderProduct == null)
                return Json(new { success = false, message = "Product not found in order." });

            try
            {
                // Remove the product from the order
                _context.OrderProducts.Remove(orderProduct);
                _context.SaveChanges();

                // Recalculate total
                var newTotal = _context.OrderProducts
                    .Where(op => op.OrderId == request.OrderId)
                    .Sum(op => op.Price * op.Quantity);

                // Update or create a bill record
                var bill = _context.Bills.Include(x => x.Orders).FirstOrDefault(b => b.Orders.First().OrderId == request.OrderId);
                if (bill != null)
                {
                    bill.Status = "Pending";
                    bill.Total = (decimal)newTotal;
                    _context.SaveChanges();
                }

                return Json(new { success = true, newTotal = newTotal});
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public class RemoveProductRequest
        {
            public int OrderId { get; set; }
            public int ProductId { get; set; }
        }

    }
}
