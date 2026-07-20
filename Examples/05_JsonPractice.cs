using System.Text.Json;
using System.Text.Json.Serialization;

namespace FilesLab.Examples;

/// <summary>
/// JSON IN PRACTICE — the patterns real apps need:
///
///   1. Save/load app state to a JSON FILE (settings, small local DB).
///   2. Enums as STRINGS (readable, refactor-friendly with care).
///   3. Tolerant reading: unknown fields, missing fields, defaults.
///   4. File streams + JSON = SerializeAsync (no giant intermediate string).
/// </summary>
public static class JsonPracticeDemo
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    private enum OrderStatus { Pending, Paid, Shipped }

    private record OrderRecord(string Product, OrderStatus Status, decimal Total);

    private record AppSettings
    {
        public string Theme { get; init; } = "light";
        public int FontSize { get; init; } = 14;
        public List<string> RecentFiles { get; init; } = [];
    }

    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    public static async Task Run()
    {
        var dir = Path.Combine(Helper.ScratchDir, "json");
        Directory.CreateDirectory(dir);

        // ── 1. Settings file round-trip ──────────────────────────────────────
        Helper.Heading("1) Settings.json — save and load app state");
        var path = Path.Combine(dir, "settings.json");
        var settings = new AppSettings { Theme = "dark", RecentFiles = ["a.csv", "b.csv"] };

        await using (var stream = File.Create(path))
            await JsonSerializer.SerializeAsync(stream, settings, Options);      // straight to disk
        Helper.Info($"saved {new FileInfo(path).Length} B:");
        foreach (var line in (await File.ReadAllLinesAsync(path)).Take(4))
            Helper.Info($"  {line}");

        await using (var stream = File.OpenRead(path))
        {
            var loaded = await JsonSerializer.DeserializeAsync<AppSettings>(stream, Options);
            Helper.Info($"loaded: Theme={loaded!.Theme}, {loaded.RecentFiles.Count} recent files");
        }

        // ── 2. Enums as strings ──────────────────────────────────────────────
        Helper.Heading("2) Enums as strings (JsonStringEnumConverter)");
        var order = new OrderRecord("kava", OrderStatus.Shipped, 3.40m);
        var json = JsonSerializer.Serialize(order, Options);
        Helper.Info(json.Replace("\n", " ").Replace("  ", ""));
        Helper.Info("Without the converter: \"status\": 2 — meaningless in logs and");
        Helper.Info("BREAKS silently if enum members are ever reordered. Strings, always.");

        // ── 3. Tolerant reading ──────────────────────────────────────────────
        Helper.Heading("3) Tolerant reading — real-world JSON is messy");
        var messy = """
            {
              "product": "sok",
              "status": "Paid",
              "total": 2.0,
              "somethingNewFromV2": true
            }
            """;
        var parsed = JsonSerializer.Deserialize<OrderRecord>(messy, Options)!;
        Helper.Info($"unknown field ignored by default -> {parsed}");

        var missingField = """{ "product": "voda", "status": "Pending" }""";
        var sparse = JsonSerializer.Deserialize<OrderRecord>(missingField, Options)!;
        Helper.Info($"missing 'total' -> default(decimal): {sparse.Total}  (no exception!)");
        Helper.Info("Need strictness? required properties / JsonRequired attribute exist.");

        // ── 4. The web defaults shortcut ─────────────────────────────────────
        Helper.Heading("4) JsonSerializerDefaults.Web");
        var web = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        Helper.Info("new JsonSerializerOptions(JsonSerializerDefaults.Web) = camelCase +");
        Helper.Info("case-insensitive + number-from-string — the ASP.NET Core defaults.");
        Helper.Info("One shared static options instance per app; creating options per");
        Helper.Info("call kills its internal caching. (Ours is a static field — copy that.)");
    }
}
