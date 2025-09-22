using FinanceApp.Data;
using FinanceApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApp.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly FinanceAppContext _context;

        public TransactionsController(FinanceAppContext context)
        {
            _context = context;
        }

        // GET: Transactions
        public IActionResult Index()
        {
            var transactions = _context.Transactions.ToList();
            return View(transactions);
        }

        // GET: Transactions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Transactions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                _context.Transactions.Add(transaction);   // добавя в контекста
                _context.SaveChanges();                  // записва в базата
                return RedirectToAction(nameof(Index));  // връща към списъка
            }
            return View(transaction);
        }
    }
}
