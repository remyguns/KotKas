using KotKas.Models;

namespace KotKas.Services;

// Laat de twee Azure AI-diensten samenwerken
public class ReceiptAnalyzerService : IReceiptAnalyzerService
{
    private readonly ICustomVisionService _customVisionService;
    private readonly IComputerVisionService _computerVisionService;
    private readonly IReceiptParser _receiptParser;

    public ReceiptAnalyzerService(ICustomVisionService customVisionService,
                                  IComputerVisionService computerVisionService,
                                  IReceiptParser receiptParser)
    {
        _customVisionService = customVisionService;
        _computerVisionService = computerVisionService;
        _receiptParser = receiptParser;
    }

    public async Task<ScannedReceipt> AnalyzeAsync(string imagePath)
    {
        var bytes = await File.ReadAllBytesAsync(imagePath);

        // 1. Custom Vision: welke winkel? (geen ramp als dit mislukt, de parser zoekt dan zelf)
        var store = Store.Other;
        double probability = 0;
        try
        {
            using var classifyStream = new MemoryStream(bytes);
            (store, probability) = await _customVisionService.ClassifyStoreAsync(classifyStream);
        }
        catch (Exception)
        {
            store = Store.Other;
        }

        // 2. Computer Vision OCR: alle tekstregels
        using var ocrStream = new MemoryStream(bytes);
        var lines = await _computerVisionService.ReadLinesAsync(ocrStream);

        // 3. Eigen ReceiptParser met de regels van de herkende winkel
        var receipt = _receiptParser.Parse(lines, store);
        receipt.StoreProbability = probability;
        receipt.ImagePath = imagePath;
        return receipt;
    }
}
