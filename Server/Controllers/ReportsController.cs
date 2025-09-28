using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Server.Data;
using System.Globalization;
using System.Text;

namespace ExpenseTracker.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
	private readonly AppDbContext _db;
	public ReportsController(AppDbContext db) => _db = db;

	[HttpGet("transactions/csv")]
	public async Task<IActionResult> ExportTransactions()
	{
		var incomes = await _db.Incomes.AsNoTracking().OrderBy(i => i.Date).ToListAsync();
		var expenses = await _db.Expenses.AsNoTracking().OrderBy(e => e.Date).ToListAsync();
		var totalIncome = incomes.Sum(i => i.Amount ?? 0);
		var totalExpense = expenses.Sum(e => e.Amount ?? 0);
		var net = totalIncome - totalExpense;

		var sb = new StringBuilder();
		sb.AppendLine("Report: Transactions Summary");
		sb.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
		sb.AppendLine($"Total Incomes: {totalIncome.ToString("F2", CultureInfo.InvariantCulture)}");
		sb.AppendLine($"Total Expenses: {totalExpense.ToString("F2", CultureInfo.InvariantCulture)}");
		sb.AppendLine($"Net Balance: {net.ToString("F2", CultureInfo.InvariantCulture)}");
		sb.AppendLine();
		sb.AppendLine("Type,Title,Amount,Date,Category/Source,Notes");
		foreach (var i in incomes)
		{
			sb.AppendLine(string.Join(',',
				Escape("Income"),
				Escape(i.Title),
				Escape((i.Amount ?? 0).ToString(CultureInfo.InvariantCulture)),
				Escape(i.Date?.ToString("yyyy-MM-dd")),
				Escape(i.Source),
				Escape(i.Notes)));
		}
		foreach (var e in expenses)
		{
			sb.AppendLine(string.Join(',',
				Escape("Expense"),
				Escape(e.Title),
				Escape((e.Amount ?? 0).ToString(CultureInfo.InvariantCulture)),
				Escape(e.Date?.ToString("yyyy-MM-dd")),
				Escape(e.Category),
				Escape(e.Notes)));
		}
		var bytes = Encoding.UTF8.GetBytes(sb.ToString());
		return File(bytes, "text/csv", $"transactions_{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
	}

	private static string Escape(string? value)
	{
		if (string.IsNullOrEmpty(value)) return string.Empty;
		var needsQuotes = value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r');
		var escaped = value.Replace("\"", "\"\"");
		return needsQuotes ? $"\"{escaped}\"" : escaped;
	}
}
