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
}
