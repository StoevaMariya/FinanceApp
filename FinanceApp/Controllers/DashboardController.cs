using FinanceApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly FinanceAppContext _context;

        public DashboardController(FinanceAppContext context)
        {
            _context = context;
        }

        // История на транзакциите
        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var transactions = _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Date)
                .ToList();

            return View(transactions);
        }

        // Баланс (общо + филтри по месец/година)
        public IActionResult Balance(int? month, int? year)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var transactions = _context.Transactions
                .Where(t => t.UserId == userId);

            // Ако е избран месец и година
            if (month.HasValue && year.HasValue)
            {
                transactions = transactions.Where(t => t.Date.Month == month.Value && t.Date.Year == year.Value);
            }
            // Ако е избрана само година
            else if (year.HasValue)
            {
                transactions = transactions.Where(t => t.Date.Year == year.Value);
            }

            var incomes = transactions
                .Where(t => t.Type == Models.TransactionType.Income)
                .Sum(t => (decimal?)t.Amount) ?? 0;

            var expenses = transactions
                .Where(t => t.Type == Models.TransactionType.Expense)
                .Sum(t => (decimal?)t.Amount) ?? 0;

            var balance = incomes - expenses;

            ViewBag.Incomes = incomes;
            ViewBag.Expenses = expenses;
            ViewBag.Balance = balance;

            ViewBag.SelectedMonth = month;
            ViewBag.SelectedYear = year;

            return View();
        }
    }
}
