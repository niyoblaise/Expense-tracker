using ExpenseTracker.Shared;

namespace ExpenseTracker.Client.Services
{
    public interface IExpenseService
    {
        Task<IEnumerable<Expense>> GetExpensesAsync();
        Task<Expense?> GetExpenseByIdAsync(Guid id);
        Task AddExpenseAsync(Expense expense);
        Task UpdateExpenseAsync(Expense expense);
        Task DeleteExpenseAsync(Guid id);
    }
}
