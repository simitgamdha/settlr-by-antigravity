using Microsoft.EntityFrameworkCore;
using Settlr.Models.Entities;

namespace Settlr.Data.DbContext;

public class ApplicationDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupMember> GroupMembers { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<ExpenseSplit> ExpenseSplits { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired();
        });

        // Group configuration
        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            
            entity.HasOne(e => e.CreatedBy)
                .WithMany(u => u.CreatedGroups)
                .HasForeignKey(e => e.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // GroupMember configuration
        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Group)
                .WithMany(g => g.Members)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.GroupMemberships)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => new { e.GroupId, e.UserId }).IsUnique();
        });

        // Expense configuration
        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            
            entity.HasOne(e => e.Group)
                .WithMany(g => g.Expenses)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.PaidBy)
                .WithMany(u => u.PaidExpenses)
                .HasForeignKey(e => e.PaidById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ExpenseSplit configuration
        modelBuilder.Entity<ExpenseSplit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ShareAmount).HasColumnType("decimal(18,2)");
            
            entity.HasOne(e => e.Expense)
                .WithMany(ex => ex.Splits)
                .HasForeignKey(e => e.ExpenseId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.ExpenseSplits)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
