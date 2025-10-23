using BudgetMVC.Data;
using BudgetMVC.Data.Models;
using BudgetMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BudgetMVC.Controllers;

public class TransactionsController : Controller
{
    private readonly BudgetDbContext _context;
    private readonly string[] _currencies = { "USD", "EUR", "GBP" };
    public TransactionsController(BudgetDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        var vm = new TransactionsViewModel
        {
            NewTransaction = new Transaction(),
            Transactions = _context.Transactions.Include(t => t.Category).ToList(),
            Categories = new SelectList(_context.Categories, "Id", "Name"),
            Currencies = new SelectList(_currencies, "USD")
        };
        return View(vm);
    }

    public IActionResult Search(string q)
    {
        var query = _context.Transactions.Include(t => t.Category).AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(t => EF.Functions.Like(t.Description, $"%{q}%"));
        }
        else
        {
            return RedirectToAction("Index");
        }

        var matches = query.ToList();
        var vm = new TransactionsViewModel
        {
            NewTransaction = new Transaction(),
            Transactions = matches,
            Categories = new SelectList(_context.Categories, "Id", "Name"),
            Currencies = new SelectList(_currencies, "USD")
        };

        return View("Index", vm);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(Prefix = "NewTransaction")] Transaction transaction, string selectedCurrency)
    {

        if (!ModelState.IsValid)
        {
            var vm = new TransactionsViewModel
            {
                NewTransaction = new Transaction(),
                Transactions = _context.Transactions.Include(t => t.Category).ToList(),
                Categories = new SelectList(_context.Categories, "Id", "Name"),
                Currencies = new SelectList(_currencies, "USD")
            };
            return View("Index", vm);
        }
        if (selectedCurrency is not null)
        {
            transaction.Currency = selectedCurrency;
        }

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Transaction added successfully!";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult EditModalPartial(int id)
    {
        var record = _context.Transactions.FirstOrDefault(r => r.Id == id);
        if (record is null)
        {
            return NotFound();
        }
        var vm = new EditTransactionViewModel
        {
            NewTransaction = new Transaction
            {
                Date = record.Date,
                CategoryId = record.CategoryId,
                Amount = record.Amount,
                Description = record.Description,
                Currency = record.Currency
            },
            CurrentTransaction = record,
            Categories = new SelectList(_context.Categories, "Id", "Name", record.CategoryId),
            Currencies = new SelectList(_currencies, record.Currency)
        };
        return PartialView("_EditTransaction", vm);

    }
    [HttpPost]
    public async Task<IActionResult> Edit(int id, [Bind(Prefix = "NewTransaction")] Transaction newTransaction, string selectedCurrency)
    {
        Transaction? recordToUpdate = _context.Transactions.FirstOrDefault(r => r.Id == id);
        if (recordToUpdate is null)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            recordToUpdate.Date = newTransaction.Date;
            recordToUpdate.Currency = selectedCurrency;
            recordToUpdate.Amount = newTransaction.Amount;
            recordToUpdate.Description = newTransaction.Description;
            await _context.SaveChangesAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }
            TempData["SuccessMessage"] = $"Successfully edited transaction #{recordToUpdate.Id} !";
            return RedirectToAction("Index");
        }
        var vm = new EditTransactionViewModel
        {
            NewTransaction = newTransaction,
            CurrentTransaction = recordToUpdate,
            Categories = new SelectList(_context.Categories, "Id", "Name", newTransaction?.Category),
            Currencies = new SelectList(_currencies, newTransaction?.Currency)
        };
        return PartialView("_EditTransaction", vm);
    }

    public IActionResult DeleteModalPartial(int id)
    {
        var record = _context.Transactions.Include(t => t.Category).FirstOrDefault(r => r.Id == id);
        if (record is null)
        {
            return NotFound();
        }
        return PartialView("_DeleteTransaction", record);
    }
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var transaction = _context.Transactions.Find(id);
        if (transaction is not null)
        {
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Succesfully deleted transaction #{transaction.Id} !";
        }
        return RedirectToAction("Index");
    }
}
