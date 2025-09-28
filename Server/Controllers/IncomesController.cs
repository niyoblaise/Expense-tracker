using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Server.Data;
using ExpenseTracker.Shared;
using System.Text;
using System.Globalization;

namespace ExpenseTracker.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IncomesController : ControllerBase
{
	private readonly AppDbContext _db;

	public IncomesController(AppDbContext db) => _db = db;

	[HttpGet]
	public async Task<ActionResult<IEnumerable<Income>>> Get()
	{
		var items = await _db.Incomes.OrderByDescending(i => i.Date).ToListAsync();
		return Ok(items);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<Income>> Get(Guid id)
	{
		var income = await _db.Incomes.FindAsync(id);
		if (income == null) return NotFound();
		return Ok(income);
	}

	[HttpPost]
	public async Task<ActionResult<Income>> Post(Income income)
	{
		_db.Incomes.Add(income);
		await _db.SaveChangesAsync();
		return CreatedAtAction(nameof(Get), new { id = income.Id }, income);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Put(Guid id, Income income)
	{
		if (id != income.Id) return BadRequest();
		_db.Entry(income).State = EntityState.Modified;
		await _db.SaveChangesAsync();
		return NoContent();
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(Guid id)
	{
		var income = await _db.Incomes.FindAsync(id);
		if (income == null) return NotFound();
		_db.Incomes.Remove(income);
		await _db.SaveChangesAsync();
		return NoContent();
	}

	[HttpGet("export/csv")]
	public async Task<IActionResult> ExportCsv()
	{
        var rows = await _db.Incomes.AsNoTracking().OrderBy(i => i.Date).ToListAsync();
        var total = rows.Sum(r => r.Amount ?? 0);
        var sb = new StringBuilder();
        sb.AppendLine("Report: Incomes");
        sb.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine($"Total Incomes: {total.ToString("F2", CultureInfo.InvariantCulture)}");
        sb.AppendLine();
        sb.AppendLine("Id,Title,Amount,Date,Source,Notes");
        foreach (var r in rows)
        {
            var csvLine = string.Join(',',
                Escape(r.Id.ToString()),
                Escape(r.Title),
                Escape((r.Amount ?? 0).ToString(CultureInfo.InvariantCulture)),
                Escape(r.Date?.ToString("yyyy-MM-dd")),
                Escape(r.Source),
                Escape(r.Notes));
            sb.AppendLine(csvLine);
        }
        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
		return File(bytes, "text/csv", $"incomes_{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
	}

	private static string Escape(string? value)
	{
		if (string.IsNullOrEmpty(value)) return string.Empty;
		var needsQuotes = value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r');
		var escaped = value.Replace("\"", "\"\"");
		return needsQuotes ? $"\"{escaped}\"" : escaped;
	}
}
