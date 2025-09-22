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
    options.IdleTimeout = TimeSpan.FromMinutes(30); // колко време пазим сесията
    options.Cookie.HttpOnly = true;                 // cookie достъпно само от сървъра
    options.Cookie.IsEssential = true;              // винаги да се съхранява
});

// Добавяме HttpContextAccessor за достъп до Session от _Layout.cshtml
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// ВАЖНО: включваме Session преди маршрутизацията
app.UseSession();

// Тестово добавяне на User в базата
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
