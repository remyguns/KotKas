namespace KotKas.Api.DTOs;

public class ExpenseDto
{
    public int Id { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public decimal TotalAmount { get; set; }
    public int RoommateId { get; set; }
    public string RoommateName { get; set; } = string.Empty;
}

public class ExpenseCreateUpdateDto
{
    public string StoreName { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public decimal TotalAmount { get; set; }
    public int RoommateId { get; set; }
}
