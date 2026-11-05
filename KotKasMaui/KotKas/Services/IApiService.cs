namespace KotKas.Services;

// Generieke interface met alle CRUD-methodes naar de KotKas API
public interface IApiService<T> where T : class
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetAsync(int id);
    Task<T?> PostAsync(T item);
    Task<bool> PutAsync(int id, T item);
    Task<bool> DeleteAsync(int id);
}
