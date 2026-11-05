using KotKas.Core.Entities;
using KotKas.Core.Interfaces;

namespace KotKas.Core.Services;

public class BalanceService : IBalanceService
{
    private readonly IRoommateRepository _roommateRepository;

    public BalanceService(IRoommateRepository roommateRepository)
    {
        _roommateRepository = roommateRepository;
    }

    public async Task<Dictionary<int, decimal>> CalculateBalancesAsync()
    {
        var roommates = (await _roommateRepository.GetAllWithExpensesAsync()).ToList();
        var balances = new Dictionary<int, decimal>();

        if (roommates.Count == 0)
            return balances;

        decimal total = roommates.Sum(CalculateTotalPaid);
        decimal share = total / roommates.Count;

        foreach (var roommate in roommates)
        {
            balances[roommate.Id] = Math.Round(CalculateTotalPaid(roommate) - share, 2);
        }

        return balances;
    }

    public decimal CalculateTotalPaid(Roommate roommate)
    {
        return roommate.Expenses.Sum(e => e.TotalAmount);
    }
}
