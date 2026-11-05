namespace KotKas.Models;

// Resultaat van het analyseren van een kasticket (Custom Vision + OCR + ReceiptParser)
public class ScannedReceipt
{
    public string StoreName { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; } = DateTime.Today;
    public decimal TotalAmount { get; set; }

    public Store DetectedStore { get; set; } = Store.Other;
    public double StoreProbability { get; set; }
    public bool TotalFound { get; set; }
    public bool DateFound { get; set; }

    public string ImagePath { get; set; } = string.Empty;
    public List<string> Lines { get; set; } = new();
}
