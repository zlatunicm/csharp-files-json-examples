namespace FilesLab.Examples;

/// <summary>
/// READING &amp; WRITING FILES — the File.* one-liners cover 90% of needs.
///
///   File.WriteAllText / ReadAllText     — whole file as ONE string
///   File.AppendAllText / AppendAllLines — add to the end
///   File.WriteAllLines / ReadAllLines   — string[] in, string[] out
///   File.ReadLines                      — LAZY line stream (huge files!)
///   + every one has an Async twin (await File.ReadAllTextAsync...)
///
/// ReadAllLines returns the WHOLE array (all in memory);
/// ReadLines yields line by line — 10 GB logs welcome.
/// </summary>
public static class ReadWriteDemo
{
    public static async Task Run()
    {
        var dir = Path.Combine(Helper.ScratchDir, "demo");
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, "narudzbe.txt");

        // ── Write / read whole files ─────────────────────────────────────────
        Helper.Heading("WriteAllText / ReadAllText");
        await File.WriteAllTextAsync(path, "kava;2\nkolac;1\n");
        Helper.Info($"written: {new FileInfo(path).Length} bytes to {Path.GetFileName(path)}");

        string content = await File.ReadAllTextAsync(path);
        Helper.Info($"read back: {content.Replace("\n", " | ")}");

        // ── Append ───────────────────────────────────────────────────────────
        Helper.Heading("AppendAllText — logs love it");
        await File.AppendAllTextAsync(path, "sendvic;3\n");
        var lines = await File.ReadAllLinesAsync(path);
        Helper.Info($"after append, {lines.Length} lines: {Helper.AsText(lines)}");

        // ── Lazy line reading ────────────────────────────────────────────────
        Helper.Heading("ReadLines — lazy, for files bigger than RAM");
        var bigPath = Path.Combine(dir, "veliki.log");
        await File.WriteAllLinesAsync(bigPath, Enumerable.Range(1, 100_000).Select(i => $"linija {i}"));

        // Find the first interesting line WITHOUT loading 100k lines:
        var hit = File.ReadLines(bigPath).First(l => l.EndsWith("42"));
        Helper.Info($"first line ending in 42: '{hit}' — enumeration stopped right there");
        Helper.Info("ReadAllLines would materialize all 100,000 first. ReadLines streams.");

        // ── Existence, info, copy, move, delete ──────────────────────────────
        Helper.Heading("The file-management verbs");
        var copy = Path.Combine(dir, "narudzbe-backup.txt");
        File.Copy(path, copy, overwrite: true);
        Helper.Info($"File.Exists(copy):  {File.Exists(copy)}");
        Helper.Info($"FileInfo: {new FileInfo(copy).Length} B, written {File.GetLastWriteTime(copy):HH:mm:ss}");

        var renamed = Path.Combine(dir, "narudzbe-arhiva.txt");
        File.Move(copy, renamed, overwrite: true);
        File.Delete(renamed);
        Helper.Info($"after Move+Delete, exists: {File.Exists(renamed)}");

        // ── Directories ──────────────────────────────────────────────────────
        Helper.Heading("Directory — listing with patterns");
        var found = Directory.EnumerateFiles(dir, "*.txt", SearchOption.AllDirectories)
                             .Select(Path.GetFileName);
        Helper.Info($"*.txt under demo/: {Helper.AsText(found)}");
        Helper.Info("(EnumerateFiles is the lazy twin of GetFiles — same idea as ReadLines.)");

        // ── Culture note ─────────────────────────────────────────────────────
        Helper.Heading("Reminder from project 09");
        Helper.Info("CSV/config numbers & dates: write and parse with InvariantCulture.");
        Helper.Info("A file written on hr-HR and parsed on en-US is how 3,50 became 350.");
    }
}
