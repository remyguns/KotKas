using System.Net.Http.Json;
using KotKas.Models;

namespace KotKas.Services;

public class ExpenseDataService : ApiService<Expense>, IExpenseDataService
{
    public ExpenseDataService() : base("expenses")
    {
    }

    // GET api/roommates/{id}/expenses: alle uitgaven die een kotgenoot betaald heeft
    public async Task<List<Expense>> GetByRoommateAsync(int roommateId)
    {
        return await _httpClient.GetFromJsonAsync<List<Expense>>($"roommates/{roommateId}/expenses", _jsonOptions)
               ?? new List<Expense>();
    }
}
