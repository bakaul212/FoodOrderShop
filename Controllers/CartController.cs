using FoodOrderShop.Data;
using FoodOrderShop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FoodOrderShop.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string CART_KEY = "UserCart";

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Cart Page View
        public IActionResult Index()
        {
            var cart = GetCartItems();
            return View(cart);
        }

        // Add to Cart Action
        public async Task<IActionResult> AddToCart(int foodId)
        {
            var foodItem = await _context.FoodItems.FindAsync(foodId);
            if (foodItem == null) return NotFound();

            var cart = GetCartItems();
            var existingItem = cart.FirstOrDefault(c => c.FoodItemId == foodId);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    FoodItemId = foodItem.Id,
                    FoodName = foodItem.Name,
                    Price = foodItem.Price,
                    Quantity = 1,
                    ImageUrl = foodItem.ImageUrl
                });
            }

            SaveCartItems(cart);
            return RedirectToAction("Index");
        }

        // Remove item from Cart
        public IActionResult RemoveFromCart(int foodId)
        {
            var cart = GetCartItems();
            var itemToRemove = cart.FirstOrDefault(c => c.FoodItemId == foodId);
            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                SaveCartItems(cart);
            }
            return RedirectToAction("Index");
        }

        // Helper Methods to handle Session
        private List<CartItem> GetCartItems()
        {
            var cartJson = HttpContext.Session.GetString(CART_KEY);
            return string.IsNullOrEmpty(cartJson)
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }

        private void SaveCartItems(List<CartItem> cart)
        {
            HttpContext.Session.SetString(CART_KEY, JsonSerializer.Serialize(cart));
        }
    }
}