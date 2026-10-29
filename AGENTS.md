# AGENTS.md

Instructions for AI coding agents (Claude Code, Codex and others) working in this repository. `CLAUDE.md` only imports
this file, so edit this one. [README.md](README.md) describes the project for people; this file lists what the code
alone does not tell you.

<!-- repo-init:start -->
## Repository Info

- **Company:** Berrevoets Systems
- **Author:** Bert Berrevoets
- **Contact:** <bert@berrevoets.net>
- **Remote:** <https://github.com/bberrevoets/CreateMp3TestFile> (public)

## Workflow Rules

- **GitHub Flow:** `main` is the default and only long-lived branch. It is protected by the ruleset "Protect main":
  no direct pushes, no deletion, changes only through a pull request, and all PR conversations must be resolved
  before merging. Merged head branches are deleted automatically.
- After cloning, run `git config pull.rebase true` once, so `git pull` rebases instead of creating merge commits.
- Work on a branch per topic, push it, and open a PR to `main` only when Bert asks for it, assigned to him
  (`gh pr create --assignee bberrevoets`). PRs are squash-merged. After a merge: switch to `main` and pull, then delete the
  local branch with `git branch -D <branch>` (`-d` refuses after a squash merge) once
  `gh pr view <branch> --json state,headRefOid` shows `MERGED` and the same commit as `git rev-parse <branch>`.
- **"Update the documentation"** means `AGENTS.md`, the project's own `README.md` files (not vendored third-party ones)
  and `CHANGELOG.md`; then commit and push.
- [CHANGELOG.md](CHANGELOG.md) follows Keep a Changelog: the Attributors list at the top, and one `Author: *Name*`
  line under each section heading (`### Added`, `### Changed`, ...).
- Markdown must pass `markdownlint-cli2 "**/*.md"` with [.markdownlint.json](.markdownlint.json) (180-character
  lines) before every commit.
- Binaries go through Git LFS: every `filter=lfs` pattern in `.gitattributes` — the standard set (PDF, ZIP, PNG,
  JPEG, STEP/STP, STL, 3MF, F3D/F3Z) plus any pattern this repository adds. Add a pattern there before committing
  another binary type (GIF, BMP, ...). The patterns are lowercase and match `X.PNG` only where `core.ignorecase` is
  true (Windows, macOS); on a case-sensitive system check a new binary with `git check-attr filter -- <file>` before
  committing it.
- Commit messages use the conventional style (`feat:`, `fix:`, `docs:`, `chore:`).
<!-- repo-init:end -->

## Project Overview

`CreateMp3TestFile` is a small .NET 10 console tool that writes a valid, silent MP3 file (`Test.mp3` in the working
directory) with both an **ID3v2.4** tag at the start and an **ID3v1** tag at the end, so music players and tag readers
can be tested against known metadata. Started in 2022 (ID3v1 only, one `Program.cs`), rewritten on 2026-02-01 into
separate audio, tag and file modules. No external NuGet packages: only the .NET base class library. Licence GPL-3.0;
release V1.0.0 (2022) on GitHub.

## Commands

Needs the .NET 10 SDK.

```sh
dotnet build CreateMp3TestFile.sln -c Release
dotnet run --project CreateMp3TestFile -c Release     # writes Test.mp3 in the current directory
npx markdownlint-cli2 "**/*.md"
```

There are no automated tests and no CI workflows. To check the output: `ffprobe Test.mp3` shows the stream and tags;
`xxd Test.mp3 | head` shows the `ID3` header (version byte `04`), and the last 128 bytes start with `TAG`.

## Repository Layout

- `CreateMp3TestFile/Program.cs` — entry point: deletes an old `Test.mp3`, writes the silent audio, fills an
  `Mp3Metadata` and calls `Id3TagWriter.WriteTags`, then prints what it wrote.
- `CreateMp3TestFile/Audio/SilentMp3Generator.cs` — silent MPEG-1 Layer III frames, 128 kbps / 44.1 kHz,
  417-byte frames; `Generate(seconds)`, `GenerateFrames(count)`, `GetDuration(frames)`.
- `CreateMp3TestFile/Id3/Id3TagWriter.cs` — writes both tags (options in `Id3TagWriterOptions`).
- `CreateMp3TestFile/Id3/Id3v1/Id3v1TagWriter.cs` — the fixed 128-byte `TAG` block appended at the end (ASCII).
- `CreateMp3TestFile/Id3/Id3v2/` — `Id3v2TagWriter` (builds, writes and removes the tag at the start of the file),
  `Id3v2Header` (10-byte header and flags), `SyncsafeInteger` (the 7-bit size encoding), and `Frames/`
  (`TextFrame`, `CommentFrame`, `TimestampFrame`; UTF-8 text).
- `CreateMp3TestFile/Models/Mp3Metadata.cs` — the metadata (title, artist, album, year, comment, track "n/total",
  genre id and text).
- `CreateMp3TestFile/Utilities/FileUtilities.cs` — prepend, append and remove-from-start helpers, through a `.tmp`
  file next to the original.

## Conventions

- Every `.cs` file starts with Bert's header comment (Author, Created, Updated, Solution, Project, Filename); update
  the `Updated` date when changing a file.
- Classes are mostly `static` with small, single-purpose methods; keep it free of external dependencies unless Bert
  asks for one.
- **Tag replacement:** writing removes any existing ID3v2 tag first (`Id3v2TagWriter.RemoveExistingTag`) and any
  ID3v1 tag, so repeated writes do not stack tags. The ID3v2 size field excludes the header and the optional v2.4
  footer; `GetExistingTagSize` adds both (fixed in #3).
- Generated `*.mp3` files are git-ignored; never commit test output.
- `.claude/settings.local.json` is personal Claude Code configuration and stays untracked.
