namespace FilesLab;

/// <summary>Helper methods for tidier console output (same as the whole series).</summary>
public static class Helper
{
    public static void Heading(string text)
    {
        Console.WriteLine();
        Console.WriteLine($"── {text} " + new string('─', Math.Max(3, 58 - text.Length)));
    }

    public static void Info(string text) => Console.WriteLine("   " + text);

    public static string AsText<T>(IEnumerable<T> collection) =>
        "[" + string.Join(", ", collection) + "]";

    /// <summary>A per-run scratch folder inside the system temp directory.</summary>
    public static string ScratchDir
    {
        get
        {
            var dir = Path.Combine(Path.GetTempPath(), "FilesLab");
            Directory.CreateDirectory(dir);
            return dir;
        }
    }
}
