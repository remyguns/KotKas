namespace KotKas.Api.DTOs;

public class RoommateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal TotalPaid { get; set; }
    public decimal Balance { get; set; }
}

public class RoommateCreateUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
