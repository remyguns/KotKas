using System.Net.Http.Json;
using System.Text.Json;
using KotKas.Helpers;

namespace KotKas.Services;

// Generieke abstracte klasse: elke DataService erft hiervan en geeft enkel zijn endpoint mee
public abstract class ApiService<T> : IApiService<T> where T : class
{
    protected readonly HttpClient _httpClient;
    protected readonly string _endpoint;

    protected static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    protected ApiService(string endpoint)
    {
        _endpoint = endpoint;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(Constants.ApiBaseUrl),
            Timeout = TimeSpan.FromSeconds(20)
        };
        // Anders toont ngrok eerst een waarschuwingspagina i.p.v. JSON
        _httpClient.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "true");
    }

    public virtual async Task<List<T>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<T>>(_endpoint, _jsonOptions) ?? new List<T>();
    }

    public virtual async Task<T?> GetAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<T>($"{_endpoint}/{id}", _jsonOptions);
    }

    public virtual async Task<T?> PostAsync(T item)
    {
        var response = await _httpClient.PostAsJsonAsync(_endpoint, item);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
    }

    public virtual async Task<bool> PutAsync(int id, T item)
    {
        var response = await _httpClient.PutAsJsonAsync($"{_endpoint}/{id}", item);
        return response.IsSuccessStatusCode;
    }

    public virtual async Task<bool> DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{_endpoint}/{id}");
        return response.IsSuccessStatusCode;
    }
}
