using KotKas.Models;

namespace KotKas.Services;

public interface IExpenseDataService : IApiService<Expense>
{
    Task<List<Expense>> GetByRoommateAsync(int roommateId);
}
