using Azure;
using Azure.AI.Vision.ImageAnalysis;
using KotKas.Helpers;

namespace KotKas.Services;

public class ComputerVisionService : IComputerVisionService
{
    private readonly ImageAnalysisClient _client;

    public ComputerVisionService()
    {
        _client = new ImageAnalysisClient(
            new Uri(Constants.ComputerVisionEndpoint),
            new AzureKeyCredential(Constants.ComputerVisionKey));
    }

    public async Task<List<string>> ReadLinesAsync(Stream imageStream)
    {
        var imageData = await BinaryData.FromStreamAsync(imageStream);
        ImageAnalysisResult result = await _client.AnalyzeAsync(imageData, VisualFeatures.Read);

        var lines = new List<string>();
        if (result.Read == null)
            return lines;

        foreach (var block in result.Read.Blocks)
        {
            foreach (var line in block.Lines)
            {
                lines.Add(line.Text);
            }
        }

        return lines;
    }
}
