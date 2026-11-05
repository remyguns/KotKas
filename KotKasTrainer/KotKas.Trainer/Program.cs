using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;

// KotKas.Trainer: traint het Custom Vision-model dat herkent van welke winkel een kasticket komt.
// Afbeeldingen staan in Images/<Tag>/ (Negative = ticket van een andere winkel).
// Sleutels staan in kotkas-keys.json (niet mee in de zip).

const string ProjectName = "KotKas Winkels";
const string PublishedName = "KotKasStoreModel";

var projectFolder = FindProjectFolder();
var logPath = Path.Combine(projectFolder, "trainer_log.txt");
using var log = new StreamWriter(logPath, append: false);

void Log(string message)
{
    Console.WriteLine(message);
    log.WriteLine(message);
    log.Flush();
}

try
{
    var keys = JsonNode.Parse(await File.ReadAllTextAsync(Path.Combine(projectFolder, "kotkas-keys.json")))!;
    var trainingEndpoint = keys["TrainingEndpoint"]!.GetValue<string>().TrimEnd('/');
    var predictionResourceId = keys["PredictionResourceId"]!.GetValue<string>();

    using var client = new HttpClient { BaseAddress = new Uri($"{trainingEndpoint}/customvision/v3.3/training/") };
    client.DefaultRequestHeaders.Add("Training-Key", keys["TrainingKey"]!.GetValue<string>());

    // 1. Project zoeken of aanmaken
    var projects = await GetJsonAsync(client, "projects");
    var project = projects.AsArray().FirstOrDefault(p => p!["name"]!.GetValue<string>() == ProjectName);
    if (project == null)
    {
        project = await PostJsonAsync(client, $"projects?name={Uri.EscapeDataString(ProjectName)}&classificationType=Multiclass");
        Log($"Project aangemaakt: {ProjectName}");
    }
    var projectId = project["id"]!.GetValue<string>();
    Log($"ProjectId: {projectId}");

    // 2. Tags + afbeeldingen
    var existingTags = (await GetJsonAsync(client, $"projects/{projectId}/tags")).AsArray();
    foreach (var tagFolder in Directory.GetDirectories(Path.Combine(projectFolder, "Images")))
    {
        var tagName = Path.GetFileName(tagFolder);
        var tag = existingTags.FirstOrDefault(t => t!["name"]!.GetValue<string>() == tagName);
        if (tag == null)
        {
            var type = tagName == "Negative" ? "Negative" : "Regular";
            tag = await PostJsonAsync(client, $"projects/{projectId}/tags?name={tagName}&type={type}");
            Log($"Tag aangemaakt: {tagName}");
        }

        var tagId = tag["id"]!.GetValue<string>();
        var count = (await GetJsonAsync(client, $"projects/{projectId}/images/tagged/count?tagIds={tagId}")).GetValue<int>();
        if (count > 0)
        {
            Log($"{tagName}: al {count} afbeeldingen, uploaden overgeslagen");
            continue;
        }

        foreach (var file in Directory.GetFiles(tagFolder))
        {
            using var content = new ByteArrayContent(await File.ReadAllBytesAsync(file));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            var response = await client.PostAsync($"projects/{projectId}/images?tagIds={tagId}", content);
            response.EnsureSuccessStatusCode();
        }
        Log($"{tagName}: {Directory.GetFiles(tagFolder).Length} afbeeldingen geupload");
    }

    // 3. Trainen
    var iteration = await PostJsonAsync(client, $"projects/{projectId}/train");
    var iterationId = iteration["id"]!.GetValue<string>();
    Log($"Training gestart (iteratie {iterationId})...");

    string status;
    do
    {
        await Task.Delay(5000);
        iteration = await GetJsonAsync(client, $"projects/{projectId}/iterations/{iterationId}");
        status = iteration["status"]!.GetValue<string>();
        Log($"  status: {status}");
    } while (status == "Training" || status == "Queued");

    if (status != "Completed")
        throw new Exception($"Training mislukt: {status}");

    // 4. Prestaties tonen
    var performance = await GetJsonAsync(client, $"projects/{projectId}/iterations/{iterationId}/performance?threshold=0.5");
    Log($"Precision: {performance["precision"]}  Recall: {performance["recall"]}");
    foreach (var tagPerformance in performance["perTagPerformance"]!.AsArray())
        Log($"  {tagPerformance!["name"]}: precision {tagPerformance["precision"]}, recall {tagPerformance["recall"]}");

    // 5. Publiceren naar de Prediction-resource
    var publish = await client.PostAsync(
        $"projects/{projectId}/iterations/{iterationId}/publish?publishName={PublishedName}&predictionId={Uri.EscapeDataString(predictionResourceId)}",
        null);
    publish.EnsureSuccessStatusCode();
    Log($"Gepubliceerd als {PublishedName}");
    Log("KLAAR");
}
catch (Exception ex)
{
    Log($"FOUT: {ex.Message}");
}

static async Task<JsonNode> GetJsonAsync(HttpClient client, string url)
{
    var response = await client.GetAsync(url);
    var json = await response.Content.ReadAsStringAsync();
    if (!response.IsSuccessStatusCode)
        throw new Exception($"GET {url}: {(int)response.StatusCode} {json}");
    return JsonNode.Parse(json)!;
}

static async Task<JsonNode> PostJsonAsync(HttpClient client, string url)
{
    var response = await client.PostAsync(url, null);
    var json = await response.Content.ReadAsStringAsync();
    if (!response.IsSuccessStatusCode)
        throw new Exception($"POST {url}: {(int)response.StatusCode} {json}");
    return JsonNode.Parse(json)!;
}

static string FindProjectFolder()
{
    // bin/Debug/net10.0 -> projectmap
    var folder = new DirectoryInfo(AppContext.BaseDirectory);
    while (folder != null && !File.Exists(Path.Combine(folder.FullName, "KotKas.Trainer.csproj")))
        folder = folder.Parent;
    return folder?.FullName ?? AppContext.BaseDirectory;
}
