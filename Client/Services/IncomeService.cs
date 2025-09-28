using ExpenseTracker.Shared;
using System.Net.Http.Json;

namespace ExpenseTracker.Client.Services
{
	public class IncomeService : IIncomeService
	{
		private readonly HttpClient _http;

		public IncomeService(HttpClient http)
		{
			_http = http;
		}

		public async Task<IEnumerable<Income>> GetIncomesAsync()
		{
			return await _http.GetFromJsonAsync<IEnumerable<Income>>("api/incomes")
					?? new List<Income>();
		}

		public async Task<Income?> GetIncomeByIdAsync(Guid id)
		{
			return await _http.GetFromJsonAsync<Income>($"api/incomes/{id}");
		}

		public async Task AddIncomeAsync(Income income)
		{
			await _http.PostAsJsonAsync("api/incomes", income);
		}

		public async Task UpdateIncomeAsync(Income income)
		{
			await _http.PutAsJsonAsync($"api/incomes/{income.Id}", income);
		}

		public async Task DeleteIncomeAsync(Guid id)
		{
			await _http.DeleteAsync($"api/incomes/{id}");
		}
	}
}
