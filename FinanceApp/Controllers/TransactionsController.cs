using FinanceApp.Data;
using FinanceApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class TransactionsController : Controller
{
    private readonly FinanceAppContext _context;

    public TransactionsController(FinanceAppContext context)
    {
        _context = context;
    }

    // GET: Transactions/Create
    public async Task<IActionResult> Create(string type)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Home");
        }

        ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
        ViewBag.Type = type; // Income или Expense
        return View();
    }

    // POST: Transactions/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Transaction transaction)
    {
        if (ModelState.IsValid)
        {
            transaction.Date = transaction.Date == default ? DateTime.Now : transaction.Date;

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Home");
            }

            transaction.UserId = userId.Value;

            _context.Add(transaction);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Dashboard");
        }

        ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", transaction.CategoryId);
        return View(transaction);
    }

    // GET: Transactions/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Home");
        }

        var transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (transaction == null) return NotFound();

        ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", transaction.CategoryId);
        return View(transaction);
    }

    // POST: Transactions/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Transaction transaction)
    {
        if (id != transaction.Id) return NotFound();

        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Home");
        }

        transaction.UserId = userId.Value;

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(transaction);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Transactions.Any(e => e.Id == transaction.Id && e.UserId == userId))
                    return NotFound();
                else
                    throw;
            }
            return RedirectToAction("Index", "Dashboard");
        }

        ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", transaction.CategoryId);
        return View(transaction);
    }

    // POST: Transactions/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Home");
        }

        var transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (transaction != null)
        {
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index", "Dashboard");
    }
}
