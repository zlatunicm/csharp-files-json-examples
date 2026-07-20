using System.Text;

namespace FilesLab.Examples;

/// <summary>
/// STREAMS — the abstraction underneath every File.*, network call and zip.
///
/// A Stream is "a sequence of bytes you can read/write, maybe seek". The same
/// API serves files (FileStream), memory (MemoryStream), network sockets,
/// compression (GZipStream)... and they COMPOSE by wrapping.
///
/// On top of raw bytes sit the text adapters: StreamReader / StreamWriter
/// (bytes &lt;-&gt; strings via an Encoding, UTF-8 by default).
///
/// Streams are IDisposable — 'using' everything, always (project 06!).
/// </summary>
public static class StreamsDemo
{
    public static async Task Run()
    {
        var dir = Path.Combine(Helper.ScratchDir, "streams");
        Directory.CreateDirectory(dir);

        // ── FileStream + StreamWriter, explicitly ────────────────────────────
        Helper.Heading("What File.WriteAllText actually does");
        var path = Path.Combine(dir, "rucno.txt");
        await using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
        await using (var writer = new StreamWriter(fs, Encoding.UTF8))
        {
            await writer.WriteLineAsync("prva linija");
            await writer.WriteLineAsync("druga linija");
        }   // Dispose flushes the writer, then closes the file — ORDER MATTERS
        Helper.Info($"wrote {new FileInfo(path).Length} B via FileStream+StreamWriter");
        Helper.Info("(File.WriteAllText is exactly this, packaged.)");

        // ── Reading line by line from a stream ───────────────────────────────
        Helper.Heading("StreamReader — the reading twin");
        using (var reader = new StreamReader(path))
        {
            string? line;
            while ((line = await reader.ReadLineAsync()) is not null)
                Helper.Info($"read: {line}");
        }

        // ── MemoryStream — a stream over RAM ─────────────────────────────────
        Helper.Heading("MemoryStream — testing & buffering without disk");
        using var memory = new MemoryStream();
        await using (var w = new StreamWriter(memory, Encoding.UTF8, leaveOpen: true))
        {
            await w.WriteAsync("podaci u RAM-u");
        }
        Helper.Info($"MemoryStream holds {memory.Length} bytes; ToArray() gives byte[]");
        Helper.Info("Perfect for unit tests of code that 'needs a Stream'.");

        // ── Composition: compression by wrapping ─────────────────────────────
        Helper.Heading("Streams compose: GZip by wrapping");
        var gzPath = Path.Combine(dir, "izvjestaj.txt.gz");
        var payload = string.Join('\n', Enumerable.Repeat("kava;2;3.40", 1000));

        await using (var file = File.Create(gzPath))
        await using (var gzip = new System.IO.Compression.GZipStream(file, System.IO.Compression.CompressionLevel.Optimal))
        await using (var writer = new StreamWriter(gzip))
        {
            await writer.WriteAsync(payload);
        }
        Helper.Info($"plain: {payload.Length:N0} B  ->  gzipped file: {new FileInfo(gzPath).Length:N0} B");

        await using (var file = File.OpenRead(gzPath))
        await using (var gunzip = new System.IO.Compression.GZipStream(file, System.IO.Compression.CompressionMode.Decompress))
        using (var reader = new StreamReader(gunzip))
        {
            var restored = await reader.ReadToEndAsync();
            Helper.Info($"decompressed back to {restored.Length:N0} chars — writer->gzip->file, reader<-gunzip<-file");
        }

        // ── Copying streams ──────────────────────────────────────────────────
        Helper.Heading("CopyToAsync — the universal pump");
        Helper.Info("await source.CopyToAsync(destination) moves any stream into any");
        Helper.Info("other: file->response, upload->disk, memory->socket. One line.");

        // ── The mental model ─────────────────────────────────────────────────
        Helper.Heading("Mental model");
        Helper.Info("File.* one-liners  -> daily driver (small/medium whole files)");
        Helper.Info("Streams            -> big data, network, composition (gzip/crypto)");
        Helper.Info("Every stream in a 'using' — an unflushed writer = lost data.");
    }
}
