using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using KotKas.Helpers;
using KotKas.Models;

namespace KotKas.Services;

// DTO's om de JSON response van de Custom Vision REST API op te vangen
public class CustomVisionResponse
{
    [JsonPropertyName("predictions")]
    public PredictionModel[]? Predictions { get; set; }
}

public class PredictionModel
{
    [JsonPropertyName("tagName")]
    public string? TagName { get; set; }

    [JsonPropertyName("probability")]
    public double Probability { get; set; }
}

public class CustomVisionService : ICustomVisionService
{
    private static readonly HttpClient _httpClient = new();

    public async Task<(Store Store, double Probability)> ClassifyStoreAsync(Stream imageStream)
    {
        string baseUrl = Constants.CustomVisionEndpoint.TrimEnd('/');
        string url = $"{baseUrl}/customvision/v3.1/Prediction/{Constants.CustomVisionProjectId}/classify/iterations/{Constants.CustomVisionPublishedName}/image";

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("Prediction-Key", Constants.CustomVisionPredictionKey);

        if (imageStream.CanSeek)
            imageStream.Position = 0;

        using var content = new StreamContent(imageStream);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        request.Content = content;

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<CustomVisionResponse>(json);

        var best = result?.Predictions?
            .OrderByDescending(p => p.Probability)
            .FirstOrDefault();

        if (best == null || best.Probability < Constants.MinimumProbability)
            return (Store.Other, best?.Probability ?? 0);

        // Tags in Custom Vision: Colruyt, Lidl (Negative = ticket van een andere winkel)
        var store = Enum.TryParse<Store>(best.TagName, true, out var parsed) ? parsed : Store.Other;
        return (store, best.Probability);
    }
}
