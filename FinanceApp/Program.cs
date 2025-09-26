using FinanceApp.Data;
using FinanceApp.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<FinanceAppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Добавяме Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true;                 // cookie достъпно само от сървъра
    options.Cookie.IsEssential = true;              // винаги да се съхранява
});


builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseSession();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FinanceAppContext>();

    if (!db.Users.Any())
    {
        db.Users.Add(new User
        {
            Username = "testuser",
            PasswordHash = "123456", // за теста – после ще сложим хеширане
            DefaultCurrency = "BGN"
        });

        db.SaveChanges();
    }
    var orphaned = db.Transactions.Where(t => t.UserId == null).ToList();
    if (orphaned.Any())
    {
        foreach (var t in orphaned)
        {
            t.UserId = 1; // Assuming testuser has ID = 1
        }
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
