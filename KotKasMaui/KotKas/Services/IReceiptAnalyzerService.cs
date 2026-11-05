using KotKas.Models;

namespace KotKas.Services;

public interface IReceiptAnalyzerService
{
    // Custom Vision (winkel) + Computer Vision (tekst) + ReceiptParser
    Task<ScannedReceipt> AnalyzeAsync(string imagePath);
}
