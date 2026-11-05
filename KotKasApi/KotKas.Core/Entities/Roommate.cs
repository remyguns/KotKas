namespace KotKas.Core.Entities;

public class Roommate
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // One-to-many: één kotgenoot kan meerdere uitgaven betalen
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
