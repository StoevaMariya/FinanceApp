using System.Diagnostics;
using FinanceApp.Data;
using FinanceApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly FinanceAppContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(FinanceAppContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }
       

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Username == username && u.PasswordHash == password);

            if (user != null)
            {
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetInt32("UserId", user.Id);
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.ErrorMessage = "Невалидно потребителско име или парола.";
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
