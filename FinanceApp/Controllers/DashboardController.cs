using FinanceApp.Data;
using FinanceApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class DashboardController : Controller
{
    private readonly FinanceAppContext _context;

    public DashboardController(FinanceAppContext context)
    {
        _context = context;
    }

    // GET: Dashboard/Index
    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToAction("Login", "Users");
        }

        var transactions = await _context.Transactions
            .Where(t => t.UserId == userId.Value)
            .Include(t => t.Category)
            .OrderByDescending(t => t.Date)
            .ToListAsync();

        return View(transactions);
    }

    // GET: Dashboard/Create
    public async Task<IActionResult> Create(string type)
    {
        ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
        ViewBag.Type = type; // Pass Income/Expense type to the view

        return View();
    }

    // POST: Dashboard/Create
    [HttpPost]
    [ValidateAntiForgeryToken]

    public async Task<IActionResult> Create(Transaction transaction)
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToAction("Login", "Users");
        }

        if (ModelState.IsValid)
        {
            transaction.UserId = userId.Value; // 🔥 Assign the logged-in user ID

            transaction.Date = transaction.Date == default ? DateTime.Now : transaction.Date;

            _context.Add(transaction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", transaction.CategoryId);
        return View(transaction);
    }



    // GET: Dashboard/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var transaction = await _context.Transactions.FindAsync(id);
        if (transaction == null) return NotFound();

        ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", transaction.CategoryId);
        return View(transaction);
    }

    // POST: Dashboard/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Transaction transaction)
    {
        if (id != transaction.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(transaction);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Transactions.Any(e => e.Id == transaction.Id))
                    return NotFound();
                else
                    throw;
            }
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", transaction.CategoryId);
        return View(transaction);
    }

    // GET: Dashboard/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var transaction = await _context.Transactions
            .Include(t => t.Category)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (transaction == null) return NotFound();

        return View(transaction);
    }

    // POST: Dashboard/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var transaction = await _context.Transactions.FindAsync(id);
        if (transaction != null)
        {
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
