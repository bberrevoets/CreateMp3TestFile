# CreateMp3TestFile

A small .NET 10 console tool that writes a valid, silent MP3 file with an ID3v2.4 tag at the start and an ID3v1 tag at
the end: a known test file for music players, tag readers and media libraries. No external packages.

```sh
dotnet run --project CreateMp3TestFile -c Release     # writes Test.mp3 in the current directory
```

Change the duration and the metadata (title, artist, album, year, comment, track, genre) in
`CreateMp3TestFile/Program.cs`. See [AGENTS.md](AGENTS.md) for the code layout.

## Conventions

- **Branching:** GitHub Flow. `main` is protected and accepts changes only through pull requests.
- **Markdown:** linted with `markdownlint-cli2`, 180 character limit.

  ```sh
  npx markdownlint-cli2 "**/*.md"
  ```

- **Binaries:** tracked with Git LFS; the covered types and any deliberate exceptions are listed in
  [AGENTS.md](AGENTS.md). Run `git lfs install` once per machine before cloning; in a clone made without it, run
  `git lfs install` and then `git lfs pull` to download the files.
- **Agent instructions:** in [AGENTS.md](AGENTS.md), read by Claude Code, Codex and any other coding agent.
  `CLAUDE.md` only imports it, so edit `AGENTS.md`.

## Author

Bert Berrevoets — Berrevoets Systems — <bert@berrevoets.net>
