using ExpenseTracker.Shared;

namespace ExpenseTracker.Client.Services
{
	public interface IIncomeService
	{
		Task<IEnumerable<Income>> GetIncomesAsync();
		Task<Income?> GetIncomeByIdAsync(Guid id);
		Task AddIncomeAsync(Income income);
		Task UpdateIncomeAsync(Income income);
		Task DeleteIncomeAsync(Guid id);
	}
}
