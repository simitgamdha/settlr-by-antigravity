using System.ComponentModel.DataAnnotations;

namespace Settlr.Models.Entities;

public class Expense
{
    public int Id { get; set; }
    
    [Required]
    public int GroupId { get; set; }
    
    [Required]
    [Range(0.01, 999999.99)]
    public decimal Amount { get; set; }
    
    [Required]
    [StringLength(500, MinimumLength = 2)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public int PaidById { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Group Group { get; set; } = null!;
    public User PaidBy { get; set; } = null!;
    public ICollection<ExpenseSplit> Splits { get; set; } = new List<ExpenseSplit>();
}
