using FinanceApp.Data;
using FinanceApp.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<FinanceAppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// �������� Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // ����� ����� ����� �������
    options.Cookie.HttpOnly = true;                 // cookie �������� ���� �� �������
    options.Cookie.IsEssential = true;              // ������ �� �� ���������
});

// �������� HttpContextAccessor �� ������ �� Session �� _Layout.cshtml
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// �����: ��������� Session ����� ���������������
app.UseSession();

// ������� �������� �� User � ������
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FinanceAppContext>();

    if (!db.Users.Any())
    {
        db.Users.Add(new User
        {
            Username = "testuser",
            PasswordHash = "123456", // �� ����� � ����� �� ������ ��������
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
