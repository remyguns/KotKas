using KotKas.Core.Entities;
using KotKas.Core.Interfaces;
using KotKas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KotKas.Infrastructure.Repositories;

public class RoommateRepository : GenericRepository<Roommate>, IRoommateRepository
{
    public RoommateRepository(KotKasContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Roommate>> GetAllWithExpensesAsync()
    {
        return await _context.Roommates
            .Include(r => r.Expenses)
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    public async Task<Roommate?> GetByIdWithExpensesAsync(int id)
    {
        return await _context.Roommates
            .Include(r => r.Expenses)
            .FirstOrDefaultAsync(r => r.Id == id);
    }
}
