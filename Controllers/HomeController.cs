using System.Diagnostics;
using FoodOrderShop.Data;
using FoodOrderShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            // ক্যাটাগরি লিস্ট ভিউতে পাঠানো
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.SelectedCategory = categoryId;

            // ক্যাটাগরি ফিল্টার অনুযায়ী ফুড আইটেম ফিল্টার করা
            var foodItemsQuery = _context.FoodItems.Include(f => f.Category).AsQueryable();

            if (categoryId.HasValue)
            {
                foodItemsQuery = foodItemsQuery.Where(f => f.CategoryId == categoryId.Value);
            }

            var foodItems = await foodItemsQuery.ToListAsync();
            return View(foodItems);
        }

        // নতুন কন্টাক্ট পেজের জন্য অ্যাকশন
        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}