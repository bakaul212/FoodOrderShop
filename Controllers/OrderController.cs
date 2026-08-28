using System.Security.Claims;
using System.Text.Json;
using FoodOrderShop.Data;
using FoodOrderShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrderShop.Controllers
{
    [Authorize] // শুধুমাত্র লগইন করা ইউজার অর্ডার করতে পারবে
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string CART_KEY = "UserCart";

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Order/Checkout
        public IActionResult Checkout()
        {
            var cart = GetCartItems();
            if (!cart.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            ViewBag.Cart = cart;
            ViewBag.Subtotal = cart.Sum(i => i.Total);
            ViewBag.DeliveryFee = 60;
            ViewBag.GrandTotal = ViewBag.Subtotal + ViewBag.DeliveryFee;

            return View();
        }

        // POST: Order/PlaceOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(Order orderModel)
        {
            var cart = GetCartItems();
            if (!cart.Any()) return RedirectToAction("Index", "Cart");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = new Order
            {
                UserId = userId,
                FullName = orderModel.FullName,
                PhoneNumber = orderModel.PhoneNumber,
                Address = orderModel.Address,
                TotalAmount = cart.Sum(i => i.Total) + 60,
                OrderDate = DateTime.Now,
                OrderStatus = "Pending"
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in cart)
            {
                var detail = new OrderDetails
                {
                    OrderId = order.Id,
                    FoodItemId = item.FoodItemId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price
                };
                _context.OrderDetails.Add(detail);
            }

            await _context.SaveChangesAsync();

            // অর্ডার সম্পূর্ণ হলে কার্ট খালি করা
            HttpContext.Session.Remove(CART_KEY);

            return RedirectToAction("Confirmation", new { orderId = order.Id });
        }

        // GET: Order/Confirmation
        public IActionResult Confirmation(int orderId)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null) return NotFound();

            return View(order);
        }

        private List<CartItem> GetCartItems()
        {
            var cartJson = HttpContext.Session.GetString(CART_KEY);
            return string.IsNullOrEmpty(cartJson)
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }
    }
}