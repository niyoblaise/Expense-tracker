using ExpenseTracker.Shared;
using System.Net.Http.Json;

namespace ExpenseTracker.Client.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly HttpClient _http;

        public ExpenseService(HttpClient http)
        {
            _http = http;
        }

        public async Task<IEnumerable<Expense>> GetExpensesAsync()
        {
            return await _http.GetFromJsonAsync<IEnumerable<Expense>>("api/expenses")
                   ?? new List<Expense>();
        }

        public async Task<Expense?> GetExpenseByIdAsync(Guid id)
        {
            return await _http.GetFromJsonAsync<Expense>($"api/expenses/{id}");
        }

        public async Task AddExpenseAsync(Expense expense)
        {
            await _http.PostAsJsonAsync("api/expenses", expense);
        }

        public async Task UpdateExpenseAsync(Expense expense)
        {
            await _http.PutAsJsonAsync($"api/expenses/{expense.Id}", expense);
        }

        public async Task DeleteExpenseAsync(Guid id)
        {
            await _http.DeleteAsync($"api/expenses/{id}");
        }
    }
}
