using System.ComponentModel.DataAnnotations;

namespace BudgetMVC.Data.Models;

public class Transaction
{
    public int Id { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; set; }

    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime Date { get; set; } = DateTime.Now;

    [Required]
    public string Currency { get; set; } = "USD";

    [Required]
    [MinLength(5, ErrorMessage = "Description must be at least 5 characters long.")]
    public string? Description { get; set; }

    [Required]
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
}


