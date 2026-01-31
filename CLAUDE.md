# CLAUDE.md - AI Assistant Guide

This file provides guidance for AI assistants working with the CreateMp3TestFile codebase.

## Project Overview

**CreateMp3TestFile** is a .NET 10 console application that demonstrates how to programmatically create MP3 files with ID3 v1.0 metadata tags. It serves as an educational example for low-level MP3 file manipulation.

- **Author:** Bert Berrevoets
- **License:** GNU GPL v3
- **Target Framework:** .NET 10

## Quick Commands

```bash
# Build the project
dotnet build

# Run the application
dotnet run --project CreateMp3TestFile

# Build for release
dotnet build -c Release

# Clean build artifacts
dotnet clean
```

## Project Structure

```
CreateMp3TestFile/
├── CreateMp3TestFile.sln           # Visual Studio solution file
├── CreateMp3TestFile/
│   ├── CreateMp3TestFile.csproj    # .NET 10 project file
│   └── Program.cs                   # Main application source (~120 lines)
├── README.md
├── LICENSE
└── .gitignore
```

## Codebase Architecture

### Main Components (Program.cs)

| Component | Purpose |
|-----------|---------|
| `Program.Main()` | Entry point - creates dummy MP3 file and applies ID3 tags |
| `Program.UpdateMp3Tag()` | Validates metadata, constructs 128-byte ID3 tag, writes to file |
| `Mp3` struct | Data container for file path and ID3 metadata fields |

### ID3 v1.0 Tag Structure (128 bytes)

| Bytes | Field | Max Length |
|-------|-------|------------|
| 0-2 | "TAG" marker | 3 |
| 3-32 | Title | 30 |
| 33-62 | Artist | 30 |
| 63-92 | Album | 30 |
| 93-96 | Year | 4 |
| 97-124 | Comment | 28 |
| 126 | Track Number | 1 |
| 127 | Genre | 1 |

## Code Conventions

### Naming Patterns

- **Methods/Classes:** PascalCase (`UpdateMp3Tag`, `Program`)
- **Parameters:** `param` prefix (`paramMp3`)
- **Instance variables:** `inst` prefix (`instEncoding`)
- **Local variables:** camelCase (`tagByteArray`, `workingByteArray`)

### Patterns Used

- Struct-based data modeling for `Mp3` metadata
- Direct FileStream manipulation (no abstraction layers)
- ASCII encoding for ID3 tag bytes
- Manual byte array construction

### Known TODOs

- Line 65: Consider caching `ASCIIEncoding` instance as a static field instead of creating new instances

## Dependencies

**None** - Uses only .NET Base Class Library:
- `System`
- `System.IO`
- `System.Text`

## Testing

No formal testing infrastructure exists. The application is a standalone utility that creates `Test.mp3` in the working directory when run.

## Important Notes for AI Assistants

1. **Minimal Codebase:** This is a single-file educational project (~120 lines). Avoid over-engineering.

2. **No External Dependencies:** Keep it that way unless explicitly requested.

3. **FileStream Pattern:** The code uses manual `Dispose()`/`Close()` calls. If modernizing, prefer `using` statements.

4. **ID3 v1.0 Only:** This implements the legacy ID3 v1.0 spec, not ID3v2. The 128-byte tag is appended to the end of the file.

5. **Generated Files:** Running the app creates `Test.mp3` in the working directory. This file should not be committed.

6. **GPL License:** Any modifications must remain GPL-compatible.

## Common Tasks

### Adding New ID3 Fields
Modify the `Mp3` struct and update `UpdateMp3Tag()` to write the new field at the appropriate byte offset.

### Changing Output Filename
Modify the filename strings in `Main()` (currently hardcoded as "Test.mp3" and "TestFile.mp3").

### Supporting ID3v2
Would require significant restructuring - ID3v2 tags are variable-length and placed at the beginning of the file, unlike v1.0.
