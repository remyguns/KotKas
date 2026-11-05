namespace KotKas.Helpers;

public static class Constants
{
    // API lokaal, publiek gemaakt met ngrok:  ngrok http 5080
    public const string NgrokApiUrl = "https://JOUW-NGROK-URL.ngrok-free.app/api/";
    public const string LocalApiUrl = "http://localhost:5080/api/";

    // Op Windows kan de app de lokale API rechtstreeks bereiken, op Android via ngrok
    public static string ApiBaseUrl =>
        DeviceInfo.Current.Platform == DevicePlatform.WinUI ? LocalApiUrl : NgrokApiUrl;

    // Azure Computer Vision (OCR)
    public const string ComputerVisionEndpoint = "https://kotkas.cognitiveservices.azure.com/";
    public const string ComputerVisionKey = "3hwhSzEHSpLds27FLqZtfaalUCl6e4b0O9dzj7O4n3HM88I1AT7KJQQJ99CJACPV0roXJ3w3AAAFACOGnmhi";

    // Azure Custom Vision (Prediction) - eigen model dat de winkel van het kasticket herkent
    public const string CustomVisionEndpoint = "https://rgbigfive-prediction.cognitiveservices.azure.com/";
    public const string CustomVisionPredictionKey = "DDkbRj219jLcCqUn0qS3zPxapsDDU0dwD6zmr7NAHkT4UOsAgvrYJQQJ99CIACPV0roXJ3w3AAAIACOGZq3J";
    public const string CustomVisionProjectId = "edd87757-a0f2-4da9-b0b8-44f1d86d6630";
    public const string CustomVisionPublishedName = "KotKasStoreModel";

    // Onder deze zekerheid vertrouwen we het Custom Vision-resultaat niet
    public const double MinimumProbability = 0.6;

    // Zolang de sleutels niet ingevuld zijn, kan de app geen tickets analyseren
    public static bool IsAzureConfigured => !string.IsNullOrWhiteSpace(ComputerVisionKey);
}
