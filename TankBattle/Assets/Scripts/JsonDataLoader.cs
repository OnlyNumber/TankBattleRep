using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class JsonDataLoader
{
    /// <summary>
    /// Асинхронно серіалізує об'єкт і зберігає його в JSON-файл
    /// </summary>
    public static async UniTask<bool> SaveAsync<T>(string filePath, T data, bool prettyPrint = true, CancellationToken ct = default)
    {
        try
        {
            // 1. Автоматично створюємо директорію, якщо її ще не існує
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 2. Преобразуємо C# об'єкт у JSON-рядок
            // prettyPrint = true робить файл читабельним з форматуванням та відступами
            string jsonText = JsonUtility.ToJson(data, prettyPrint);

            // 3. Асинхронно записуємо рядок у файл
            await File.WriteAllTextAsync(filePath, jsonText, ct);

            Debug.Log($"[JsonDataLoader] Дані успішно збережено: {filePath}");
            return true;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[JsonDataLoader] Помилка при збереженні JSON: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Асинхронно завантажує дані з JSON-файлу
    /// </summary>
    public static async UniTask<T> LoadAsync<T>(string filePath, CancellationToken ct = default)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"[JsonDataLoader] Файл не знайдено: {filePath}");
            return default;
        }

        try
        {
            string jsonText = await File.ReadAllTextAsync(filePath, ct);
            return JsonUtility.FromJson<T>(jsonText);
        }
        catch (OperationCanceledException)
        {
            return default;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[JsonDataLoader] Помилка при читанні JSON: {ex.Message}");
            return default;
        }
    }
}