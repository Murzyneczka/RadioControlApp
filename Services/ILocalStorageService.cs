namespace RadioControlApp.Services;

/// <summary>
/// Interfejs serwisu do lokalnego przechowywania danych.
/// </summary>
public interface ILocalStorageService
{
    /// <summary>
    /// Zapisuje dane do lokalnego magazynu.
    /// </summary>
    Task SaveAsync<T>(string key, T data);
    
    /// <summary>
    /// Odczytuje dane z lokalnego magazynu.
    /// </summary>
    Task<T?> LoadAsync<T>(string key);
    
    /// <summary>
    /// Usuwa dane z lokalnego magazynu.
    /// </summary>
    Task DeleteAsync(string key);
    
    /// <summary>
    /// Sprawdza czy klucz istnieje w magazynie.
    /// </summary>
    Task<bool> ExistsAsync(string key);
    
    /// <summary>
    /// Pobiera wszystkie klucze z magazynu.
    /// </summary>
    Task<List<string>> GetAllKeysAsync();
}