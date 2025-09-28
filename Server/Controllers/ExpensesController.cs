using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Server.Data;
using ExpenseTracker.Shared;
using System.Text;
using System.Globalization;


namespace ExpenseTracker.Server.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ExpensesController : ControllerBase
{
    private readonly AppDbContext _db;


    public ExpensesController(AppDbContext db) => _db = db;


    [HttpGet]
    public async Task<ActionResult<IEnumerable<Expense>>> Get()
    {
        var items = await _db.Expenses.OrderByDescending(e => e.Date).ToListAsync();
        return Ok(items);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<Expense>> Get(Guid id)
    {
        var e = await _db.Expenses.FindAsync(id);
        if (e == null) return NotFound();
        return Ok(e);
    }


    [HttpPost]
    public async Task<ActionResult<Expense>> Post(Expense expense)
    {
        // Business rule: Cannot create an expense if there are no incomes yet
        var hasAnyIncome = await _db.Incomes.AnyAsync();
        if (!hasAnyIncome)
        {
            return BadRequest("Cannot add an expense before any income is recorded.");
        }

        // Validation: Total expenses cannot exceed total incomes
        // SQLite provider can't Sum decimals server-side reliably; sum in memory
        var incomeAmounts = await _db.Incomes.AsNoTracking().Select(i => i.Amount ?? 0).ToListAsync();
        var expenseAmounts = await _db.Expenses.AsNoTracking().Select(e => e.Amount ?? 0).ToListAsync();
        var totalIncome = incomeAmounts.Sum();
        var currentExpenses = expenseAmounts.Sum();
        var proposedTotalExpenses = currentExpenses + (expense.Amount ?? 0);
        if (proposedTotalExpenses > totalIncome)
        {
            return BadRequest("Expense exceeds total income. Please add more income first or reduce the expense amount.");
        }

        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = expense.Id }, expense);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, Expense expense)
    {
        if (id != expense.Id) return BadRequest();
        // Validation: Total expenses cannot exceed total incomes after update
        var incomeAmounts = await _db.Incomes.AsNoTracking().Select(i => i.Amount ?? 0).ToListAsync();
        var otherExpenseAmounts = await _db.Expenses.AsNoTracking().Where(e => e.Id != id).Select(e => e.Amount ?? 0).ToListAsync();
        var totalIncome = incomeAmounts.Sum();
        var proposedTotalExpenses = otherExpenseAmounts.Sum() + (expense.Amount ?? 0);
        if (proposedTotalExpenses > totalIncome)
        {
            return BadRequest("Updated expense exceeds total income. Please adjust the amount.");
        }

        _db.Entry(expense).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var e = await _db.Expenses.FindAsync(id);
        if (e == null) return NotFound();
        _db.Expenses.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("export/csv")]
    public async Task<IActionResult> ExportCsv()
    {
        var rows = await _db.Expenses.AsNoTracking().OrderBy(e => e.Date).ToListAsync();
        var total = rows.Sum(r => r.Amount ?? 0);
        var sb = new StringBuilder();
        sb.AppendLine("Report: Expenses");
        sb.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine($"Total Expenses: {total.ToString("F2", CultureInfo.InvariantCulture)}");
        sb.AppendLine();
        sb.AppendLine("Id,Title,Amount,Date,Category,Notes");
        foreach (var r in rows)
        {
            var csvLine = string.Join(',',
                Escape(r.Id.ToString()),
                Escape(r.Title),
                Escape((r.Amount ?? 0).ToString(CultureInfo.InvariantCulture)),
                Escape(r.Date?.ToString("yyyy-MM-dd")),
                Escape(r.Category),
                Escape(r.Notes));
            sb.AppendLine(csvLine);
        }
        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        return File(bytes, "text/csv", $"expenses_{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
    }

    private static string Escape(string? value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        var needsQuotes = value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r');
        var escaped = value.Replace("\"", "\"\"");
        return needsQuotes ? $"\"{escaped}\"" : escaped;
    }
}
