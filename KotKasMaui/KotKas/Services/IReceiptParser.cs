using KotKas.Models;

namespace KotKas.Services;

public interface IReceiptParser
{
    // Zoekt in de OCR-regels de winkel, de datum en het totaal
    ScannedReceipt Parse(List<string> lines, Store store);
}
