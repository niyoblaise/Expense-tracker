
using ExpenseTracker.Client.Pages;
using ExpenseTracker.Shared;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;


namespace ExpenseTracker.Server.Data;


public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


    public DbSet<Expense> Expenses { get; set; } = default!;
    public DbSet<Income> Incomes { get; set; } = default!;
}