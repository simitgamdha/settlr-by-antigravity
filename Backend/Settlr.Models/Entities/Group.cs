using System.ComponentModel.DataAnnotations;

namespace Settlr.Models.Entities;

public class Group
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public int CreatedById { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User CreatedBy { get; set; } = null!;
    public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
