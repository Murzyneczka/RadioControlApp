using SQLite;
using System.Text.Json;

namespace RadioControlApp.Services;

/// <summary>
/// Implementacja serwisu lokalnego przechowywania danych używająca SQLite.
/// </summary>
public class LocalStorageService : ILocalStorageService
{
    private readonly SQLiteAsyncConnection _database;

    public LocalStorageService()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "storage.db");
        _database = new SQLiteAsyncConnection(dbPath);
        InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await _database.CreateTableAsync<StorageItem>();
    }

    public async Task SaveAsync<T>(string key, T data)
    {
        var json = JsonSerializer.Serialize(data);
        var item = new StorageItem
        {
            Key = key,
            Value = json,
            Type = typeof(T).FullName ?? typeof(T).Name,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        await _database.InsertOrReplaceAsync(item);
    }

    public async Task<T?> LoadAsync<T>(string key)
    {
        var item = await _database.Table<StorageItem>()
            .Where(x => x.Key == key)
            .FirstOrDefaultAsync();

        if (item == null)
            return default;

        return JsonSerializer.Deserialize<T>(item.Value);
    }

    public async Task DeleteAsync(string key)
    {
        await _database.Table<StorageItem>()
            .Where(x => x.Key == key)
            .DeleteAsync();
    }

    public async Task<bool> ExistsAsync(string key)
    {
        var count = await _database.Table<StorageItem>()
            .Where(x => x.Key == key)
            .CountAsync();
        return count > 0;
    }

    public async Task<List<string>> GetAllKeysAsync()
    {
        var items = await _database.Table<StorageItem>().ToListAsync();
        return items.Select(x => x.Key).ToList();
    }
}

/// <summary>
/// Model elementu przechowywanego w bazie danych.
/// </summary>
[Table("StorageItems")]
public class StorageItem
{
    [PrimaryKey]
    public string Key { get; set; } = string.Empty;
    
    public string Value { get; set; } = string.Empty;
    
    public string Type { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}