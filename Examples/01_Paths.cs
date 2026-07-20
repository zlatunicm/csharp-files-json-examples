namespace FilesLab.Examples;

/// <summary>
/// PATHS DONE RIGHT — System.IO.Path, the API that saves you from '\' vs '/'.
///
/// Rules:
///   - NEVER glue paths with + "/" — Path.Combine/Join handle separators.
///   - The temp dir, home dir, app data — ask the OS (Path/Environment),
///     don't hardcode.
///   - Path.GetFullPath resolves '..' — essential for validating user input
///     (path traversal attacks!).
/// </summary>
public static class PathsDemo
{
    public static Task Run()
    {
        // ── Building paths ───────────────────────────────────────────────────
        Helper.Heading("Path.Combine — separators handled for you");
        var dir = Path.Combine(Helper.ScratchDir, "izvjestaji", "2026");
        Helper.Info($"Combine:          {dir}");
        Helper.Info($"Separator here:   '{Path.DirectorySeparatorChar}' (macOS/Linux '/', Windows '\\')");

        // ── Dissecting paths ─────────────────────────────────────────────────
        Helper.Heading("Taking a path apart");
        var file = Path.Combine(dir, "promet-07.csv");
        Helper.Info($"GetFileName:             {Path.GetFileName(file)}");
        Helper.Info($"GetFileNameWithoutExt:   {Path.GetFileNameWithoutExtension(file)}");
        Helper.Info($"GetExtension:            {Path.GetExtension(file)}");
        Helper.Info($"GetDirectoryName:        ...{Path.GetDirectoryName(file)![^16..]}");
        Helper.Info($"ChangeExtension:         {Path.GetFileName(Path.ChangeExtension(file, ".xlsx"))}");

        // ── Special folders ──────────────────────────────────────────────────
        Helper.Heading("Ask the OS for its folders");
        Helper.Info($"Temp:      {Path.GetTempPath()}");
        Helper.Info($"Home:      {Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}");
        Helper.Info($"AppData:   {Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}");
        Helper.Info($"Random tmp file name: {Path.GetRandomFileName()}");

        // ── GetFullPath & the traversal attack ───────────────────────────────
        Helper.Heading("GetFullPath — normalize before you trust");
        var baseDir = Path.Combine(Helper.ScratchDir, "public");
        Directory.CreateDirectory(baseDir);
        foreach (var userInput in new[] { "slika.png", "../tajna.txt" })
        {
            var requested = Path.GetFullPath(Path.Combine(baseDir, userInput));
            bool safe = requested.StartsWith(baseDir + Path.DirectorySeparatorChar, StringComparison.Ordinal);
            Helper.Info($"'{userInput}'".PadRight(18) + $"-> {(safe ? "OK unutar baze" : "BLOKIRAJ — bijeg iz foldera!")} ");
        }
        Helper.Info("Every 'download by name' endpoint needs exactly this check.");

        return Task.CompletedTask;
    }
}
