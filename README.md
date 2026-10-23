# C# Files & JSON — Hands-on Examples

Part **10** of a hands-on C# practice series. Five runnable, heavily commented examples: paths done right (including a **path-traversal guard**), `File.*` one-liners vs lazy `ReadLines` on a 100k-line file, streams demystified (FileStream → StreamWriter → **GZip composition**), and `System.Text.Json` from basics (camelCase gotcha, `JsonDocument`) to practice (settings files with `SerializeAsync`, enums as strings, tolerant reading).

All file operations write **only** to a scratch folder inside the system temp directory.

## Running

```bash
dotnet run        # all examples
dotnet run 4      # just one (e.g. JSON basics)
```

## What's inside

| # | Topic | Highlights |
|---|-------|------------|
| 01 | Paths | `Path.Combine`, dissecting, OS folders, **traversal-attack check** |
| 02 | Read & write | one-liners + async twins, `ReadLines` streams 100k lines lazily |
| 03 | Streams | what `File.*` hides, `MemoryStream`, **gzip by wrapping**, `CopyToAsync` |
| 04 | System.Text.Json | records as DTOs, camelCase **silent-null gotcha**, `JsonDocument` |
| 05 | JSON in practice | settings round-trip, enums as strings, tolerant reading, `Web` defaults |

## The rules to remember

1. Paths are built by `Path.Combine`, never by `+ "/"`.
2. Files bigger than RAM: `File.ReadLines` / streams — the lazy twins.
3. Streams compose by wrapping, and every one lives in a `using`.
4. One shared `JsonSerializerOptions` (camelCase!) — per-call options kill caching.
5. Enums as strings; dates as ISO 8601; numbers invariant — machine data is culture-free.

## Further reading

- [File and stream I/O](https://learn.microsoft.com/en-us/dotnet/standard/io/)
- [System.Text.Json overview](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/overview)

## The series

← Previous: [09 Strings & text](https://github.com/zlatunicm/csharp-strings-examples) · Next: [11 MiniShop microservices (finale)](https://github.com/zlatunicm/minishop-microservices) →

| Part | Topic |
|---|---|
| 01 | [Collections](https://github.com/zlatunicm/csharp-collections-examples) |
| 02 | [LINQ](https://github.com/zlatunicm/csharp-linq-examples) |
| 03 | [Delegates & events](https://github.com/zlatunicm/csharp-delegates-events-examples) |
| 04 | [Async/await](https://github.com/zlatunicm/csharp-async-examples) |
| 05 | [Generics](https://github.com/zlatunicm/csharp-generics-examples) |
| 06 | [Types & memory](https://github.com/zlatunicm/csharp-types-memory-examples) |
| 07 | [Exceptions](https://github.com/zlatunicm/csharp-exceptions-examples) |
| 08 | [Modern C# 8–12](https://github.com/zlatunicm/csharp-modern-examples) |
| 09 | [Strings & text](https://github.com/zlatunicm/csharp-strings-examples) |
| 10 | **Files & JSON** (this repo) |
| 11 | [MiniShop microservices (finale)](https://github.com/zlatunicm/minishop-microservices) |

## Author

**Matej Zlatunić**

## License

Released under the [MIT License](LICENSE). Feel free to use it for learning.
