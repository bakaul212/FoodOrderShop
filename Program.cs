using FoodOrderShop.Data;
using FoodOrderShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// SQLite Database Configuration (Reading from appsettings.json)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=FoodOrderShopV3.db";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 4;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Seed Data & Database Initialization
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();

    if (!context.Categories.Any())
    {
        var fastFood = new Category { CategoryName = "Fast Food" };
        var beverages = new Category { CategoryName = "Beverages" };
        var desserts = new Category { CategoryName = "Desserts" };
        var indian = new Category { CategoryName = "Traditional & Biryani" };

        context.Categories.AddRange(fastFood, beverages, desserts, indian);
        context.SaveChanges();

        context.FoodItems.AddRange(
            new FoodItem
            {
                Name = "Cheesy Beef Burger",
                Description = "Juicy beef patty with melted cheddar cheese and fresh lettuce.",
                Price = 250,
                CategoryId = fastFood.Id,
                ImageUrl = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?q=80&w=500"
            },
            new FoodItem
            {
                Name = "Pepperoni Pizza",
                Description = "Crispy crust topped with rich tomato sauce and spicy pepperoni.",
                Price = 650,
                CategoryId = fastFood.Id,
                ImageUrl = "https://images.unsplash.com/photo-1513104890138-7c749659a591?q=80&w=500"
            },
            new FoodItem
            {
                Name = "Crispy Fried Chicken",
                Description = "Deep-fried golden crispy chicken served with garlic sauce.",
                Price = 320,
                CategoryId = fastFood.Id,
                ImageUrl = "https://images.unsplash.com/photo-1626645738196-c2a7c87a8f58?q=80&w=500"
            },
            new FoodItem
            {
                Name = "Crispy Chicken Wings",
                Description = "Hot and spicy fried chicken wings served with dip sauce.",
                Price = 220,
                CategoryId = fastFood.Id,
                ImageUrl = "https://images.unsplash.com/photo-1567620832903-9fc6debc209f?q=80&w=500"
            },
            new FoodItem
            {
                Name = "Kacchi Biryani",
                Description = "Traditional aromatic Basmati rice cooked with tender mutton.",
                Price = 380,
                CategoryId = indian.Id,
                ImageUrl = "https://images.unsplash.com/photo-1563379091339-03b21ab4a4f8?q=80&w=500"
            },
            new FoodItem
            {
                Name = "Chicken Butter Masala",
                Description = "Rich and creamy chicken curry cooked with butter and Indian spices.",
                Price = 340,
                CategoryId = indian.Id,
                ImageUrl = "https://images.unsplash.com/photo-1588166524941-3bf61a9c41db?q=80&w=500"
            },
            new FoodItem
            {
                Name = "Cold Coffee",
                Description = "Chilled creamy coffee served with ice and chocolate syrup.",
                Price = 120,
                CategoryId = beverages.Id,
                ImageUrl = "https://images.unsplash.com/photo-1517701604599-bb29b565090c?q=80&w=500"
            },
            new FoodItem
            {
                Name = "Mango Smoothie",
                Description = "Refreshing blend of ripe mangoes, yogurt, and ice.",
                Price = 150,
                CategoryId = beverages.Id,
                ImageUrl = "https://images.unsplash.com/photo-1546173159-315724a31696?q=80&w=500"
            },
            new FoodItem
            {
                Name = "Chocolate Doughnut",
                Description = "Soft baked doughnut dipped in rich dark chocolate glaze.",
                Price = 90,
                CategoryId = desserts.Id,
                ImageUrl = "https://images.unsplash.com/photo-1551024709-8f23befc6f87?q=80&w=500"
            },
            new FoodItem
            {
                Name = "Red Velvet Pastry",
                Description = "Moist red velvet cake layer with sweet cream cheese frosting.",
                Price = 160,
                CategoryId = desserts.Id,
                ImageUrl = "https://images.unsplash.com/photo-1586985289688-ca3cf47d3e6e?q=80&w=500"
            }
        );
        context.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();
app.UseSession();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();