using BudgetMVC.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BudgetMVC.Models;

public class TransactionsViewModel
{
    public Transaction NewTransaction { get; set; }
    public List<Transaction> Transactions { get; set; }
    public SelectList Categories { get; set; }
}
