using BudgetMVC.Data;
using BudgetMVC.Data.Models;
using BudgetMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BudgetMVC.Controllers;

public class TransactionController : Controller
{
    private readonly BudgetDbContext _context;
    public TransactionController(BudgetDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        var vm = new TransactionsViewModel
        {
            NewTransaction = new Transaction(),
            Transactions = _context.Transactions.Include(t => t.Category).ToList(),
            Categories = new SelectList(_context.Categories, "Id", "Name")
        };
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Create([Bind(Prefix = "NewTransaction")] Transaction transaction)
    {
        if (!ModelState.IsValid)
        {
            var vm = new TransactionsViewModel
            {
                NewTransaction = new Transaction(),
                Transactions = _context.Transactions.Include(t => t.Category).ToList(),
                Categories = new SelectList(_context.Categories, "Id", "Name")
            };
            return View("Index", vm);
        }

        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(Transaction model)
    {
        if (ModelState.IsValid)
        {
            _context.Transactions.Update(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var transaction = _context.Transactions.Find(id);
        if (transaction != null)
        {
            _context.Transactions.Remove(transaction); // Using model
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }
}
