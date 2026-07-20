using System.Text.Json;
using System.Text.Json.Serialization;

namespace FilesLab.Examples;

/// <summary>
/// System.Text.Json — .NET's built-in JSON engine (fast, UTF-8 native).
///
///   JsonSerializer.Serialize(obj)          object -> JSON string
///   JsonSerializer.Deserialize&lt;T&gt;(json)    JSON  -> strongly-typed object
///
/// The knobs live on JsonSerializerOptions — and the one everyone needs:
/// PropertyNamingPolicy = CamelCase (C# PascalCase &lt;-&gt; JSON camelCase).
/// Records are the perfect JSON partners (immutable DTOs).
///
/// (Newtonsoft.Json still exists in older codebases; new code: System.Text.Json.)
/// </summary>
public static class JsonDemo
{
    private record Order(
        string Product,
        int Quantity,
        decimal Price,
        [property: JsonPropertyName("orderedAt")] DateTime OrderedAt,
        string? Note = null);

    public static Task Run()
    {
        var order = new Order("kava", 2, 1.70m, new DateTime(2026, 7, 20, 9, 30, 0), Note: null);

        // ── Serialize ────────────────────────────────────────────────────────
        Helper.Heading("Serialize — default vs configured");
        Helper.Info($"default:   {JsonSerializer.Serialize(order)}");

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,   // product, quantity...
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false,
        };
        string json = JsonSerializer.Serialize(order, options);
        Helper.Info($"camelCase: {json}");
        Helper.Info("(note: null Note dropped, DateTime is ISO 8601 — culture-proof!)");

        // ── Deserialize ──────────────────────────────────────────────────────
        Helper.Heading("Deserialize<T> — JSON into a record");
        var back = JsonSerializer.Deserialize<Order>(json, options)!;
        Helper.Info($"back.Product={back.Product}, Quantity={back.Quantity}, equal to original: {back == order}");
        Helper.Info("Records give free value equality — perfect round-trip check.");

        // ── Case sensitivity — the classic gotcha ────────────────────────────
        Helper.Heading("Gotcha: property-name matching");
        var lower = """{"product":"sok","quantity":1,"price":2.0,"orderedAt":"2026-07-20T10:00:00"}""";
        var strict = JsonSerializer.Deserialize<Order>(lower);                 // no options!
        var relaxed = JsonSerializer.Deserialize<Order>(lower, options);
        Helper.Info($"without options: Product='{strict?.Product}' (null-ish — names didn't match!)");
        Helper.Info($"with CamelCase:  Product='{relaxed?.Product}'");
        Helper.Info("Silent nulls, no exception — configure ONE shared options object.");

        // ── Collections & nesting just work ──────────────────────────────────
        Helper.Heading("Collections and nesting");
        var cart = new Dictionary<string, List<Order>>
        {
            ["Ana"] = [order, order with { Product = "kolac", Price = 3.50m }],
        };
        var cartJson = JsonSerializer.Serialize(cart, options);
        Helper.Info(cartJson.Length > 100 ? cartJson[..100] + "..." : cartJson);

        var restored = JsonSerializer.Deserialize<Dictionary<string, List<Order>>>(cartJson, options)!;
        Helper.Info($"restored[\"Ana\"].Count = {restored["Ana"].Count}");

        // ── JsonDocument — peeking without classes ───────────────────────────
        Helper.Heading("JsonDocument — read WITHOUT defining a type");
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        Helper.Info($"root.GetProperty(\"product\") = {root.GetProperty("product").GetString()}");
        Helper.Info("For 'just give me one field from this blob' — no DTO ceremony.");
        Helper.Info("(Writing dynamic shapes: JsonNode / JsonObject are the mutable cousins.)");

        return Task.CompletedTask;
    }
}
