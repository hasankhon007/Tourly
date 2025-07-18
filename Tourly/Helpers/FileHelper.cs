namespace Tourly.Helpers;

public class FileHelper
{
    public static string ReadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close(); // Ensure the file exists
        }

        return File.ReadAllText(filePath);
    }

    public static void WriteToFile(string filePath, List<string> content)
    {
        File.WriteAllLines(filePath, content);
    }

    public static async Task<string> ReadFromFileAsync(string path)
    {
        if (!File.Exists(path))
            return string.Empty;

        using StreamReader reader = new StreamReader(path);
        return await reader.ReadToEndAsync();
    }

    public static async Task WriteToFileAsync(string path, string content)
    {
        using StreamWriter writer = new StreamWriter(path, append: true);
        await writer.WriteLineAsync(content);
    }

    public static async Task OverwriteFileAsync(string path, string content)
    {
        using StreamWriter writer = new StreamWriter(path, append: false);
        await writer.WriteAsync(content);
    }
}
