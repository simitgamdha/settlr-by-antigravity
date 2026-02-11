using System.ComponentModel.DataAnnotations;

namespace Settlr.Models.Entities;

public class User
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Group> CreatedGroups { get; set; } = new List<Group>();
    public ICollection<GroupMember> GroupMemberships { get; set; } = new List<GroupMember>();
    public ICollection<Expense> PaidExpenses { get; set; } = new List<Expense>();
    public ICollection<ExpenseSplit> ExpenseSplits { get; set; } = new List<ExpenseSplit>();
}
