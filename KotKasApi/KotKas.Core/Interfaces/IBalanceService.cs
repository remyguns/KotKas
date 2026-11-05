using KotKas.Core.Entities;

namespace KotKas.Core.Interfaces;

public interface IBalanceService
{
    // Saldo = wat de kotgenoot betaald heeft - zijn deel van het totaal
    Task<Dictionary<int, decimal>> CalculateBalancesAsync();
    decimal CalculateTotalPaid(Roommate roommate);
}
