using Microsoft.AspNetCore.Mvc;
using FinanceApp.Data; // for your DbContext
using FinanceApp.Models; // for your User model

namespace FinanceApp.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly FinanceAppContext _context;

        public RegistrationController(FinanceAppContext context)
        {
            _context = context;
        }

        // GET: /Registration/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Registration/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(user);
        }
    }
}
