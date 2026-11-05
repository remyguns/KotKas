namespace KotKas.Core.Entities;

public class Expense
{
    public int Id { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public decimal TotalAmount { get; set; }

    // Foreign key naar de kotgenoot die betaald heeft
    public int RoommateId { get; set; }
    public Roommate? Roommate { get; set; }
}
