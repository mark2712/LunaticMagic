using System;
using System.Text.RegularExpressions;

public static class GeneratorId
{
    /// <summary>
    /// Генерирует ID формата: префикс_название_токен
    /// </summary>
    /// <param name="name">Отображаемое имя для нормализации</param>
    /// <param name="tokenLength">Длина случайного хвоста (по умолчанию 12)</param>
    public static string GenerateId(string name, int tokenLength = 12)
    {
        // 1. Очищаем имя от спецсимволов
        string normalized = Regex.Replace(
            name.ToLowerInvariant(),
            @"[^a-z0-9]+",
            "_"
        ).Trim('_');

        // 2. Генерируем случайный токен нужной длины
        // Используем Guid для уникальности, обрезая до нужной длины
        string token = Guid.NewGuid().ToString("n");
        
        if (tokenLength > token.Length) tokenLength = token.Length;
        token = token.Substring(0, tokenLength);

        // 3. Собираем результат
        return $"{normalized}_{token}";
    }

    // Пример использования:
    // GenerateId("user", 8) -> "user_a1b2c3d4"
    // GenerateId("profile") -> "profile_e5f6g7h8i9j0"
}