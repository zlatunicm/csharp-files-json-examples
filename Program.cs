using FilesLab;
using FilesLab.Examples;

// ─────────────────────────────────────────────────────────────────────────────
//  FILES & JSON  —  part 10 of the C# practice series
//  (All file examples write ONLY inside a scratch folder in the temp dir.)
// ─────────────────────────────────────────────────────────────────────────────

var demos = new (string Title, Func<Task> Method)[]
{
    ("01 — Paths done right",                     PathsDemo.Run),
    ("02 — Reading & writing files",              ReadWriteDemo.Run),
    ("03 — Streams (what File.* hides)",          StreamsDemo.Run),
    ("04 — System.Text.Json essentials",          JsonDemo.Run),
    ("05 — JSON in practice (files, options)",    JsonPracticeDemo.Run),
};

if (args.Length > 0 && int.TryParse(args[0], out var number) && number >= 1 && number <= demos.Length)
{
    await RunDemo(demos[number - 1]);
}
else
{
    Console.WriteLine("Running ALL examples. (Tip: `dotnet run 3` runs only example #3.)");
    foreach (var demo in demos)
        await RunDemo(demo);
}

static async Task RunDemo((string Title, Func<Task> Method) demo)
{
    Console.WriteLine();
    Console.WriteLine(new string('═', 62));
    Console.WriteLine($"  {demo.Title}");
    Console.WriteLine(new string('═', 62));
    await demo.Method();
}
