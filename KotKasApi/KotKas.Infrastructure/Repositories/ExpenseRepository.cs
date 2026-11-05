using KotKas.Core.Entities;
using KotKas.Core.Interfaces;
using KotKas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KotKas.Infrastructure.Repositories;

public class ExpenseRepository : GenericRepository<Expense>, IExpenseRepository
{
    public ExpenseRepository(KotKasContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Expense>> GetAllWithRoommateAsync()
    {
        return await _context.Expenses
            .Include(e => e.Roommate)
            .OrderByDescending(e => e.PurchaseDate)
            .ThenByDescending(e => e.Id)
            .ToListAsync();
    }

    public async Task<Expense?> GetByIdWithRoommateAsync(int id)
    {
        return await _context.Expenses
            .Include(e => e.Roommate)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Expense>> GetByRoommateIdAsync(int roommateId)
    {
        return await _context.Expenses
            .Include(e => e.Roommate)
            .Where(e => e.RoommateId == roommateId)
            .OrderByDescending(e => e.PurchaseDate)
            .ToListAsync();
    }
}
