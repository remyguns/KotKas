using KotKas.Core.Entities;

namespace KotKas.Core.Interfaces;

public interface IRoommateRepository : IGenericRepository<Roommate>
{
    Task<IEnumerable<Roommate>> GetAllWithExpensesAsync();
    Task<Roommate?> GetByIdWithExpensesAsync(int id);
}
