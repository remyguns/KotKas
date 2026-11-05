using KotKas.Core.Entities;

namespace KotKas.Core.Interfaces;

public interface IExpenseRepository : IGenericRepository<Expense>
{
    Task<IEnumerable<Expense>> GetAllWithRoommateAsync();
    Task<Expense?> GetByIdWithRoommateAsync(int id);
    Task<IEnumerable<Expense>> GetByRoommateIdAsync(int roommateId);
}
