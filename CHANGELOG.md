# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/). The history before the AI setup is
grouped by month (from `git log`).

## Attributors

- Bert Berrevoets (<bert@berrevoets.net>) — Berrevoets Systems
- Claude (Claude Code on the web) — author of the first `CLAUDE.md` (2026-01-31)
- Claude Opus 5.5 — co-author of the 2026-10 changes

## [Unreleased]

### Added

Author: *Bert Berrevoets*

- AI agent setup: [AGENTS.md](AGENTS.md) as the single instruction file, a [CLAUDE.md](CLAUDE.md) stub that imports it,
  markdownlint (180 characters), Git LFS rules and this changelog.

### Changed

Author: *Bert Berrevoets*

- The old `CLAUDE.md` moved to AGENTS.md and was rewritten for the current code (it still described the single-file
  ID3v1 version).
- GitHub Flow (2026-10-29): `main` is the only branch and the default; `development` was merged in (#3) and removed;
  `main` is protected by the ruleset "Protect main".
- `.claude/settings.local.json` is no longer tracked (personal Claude Code settings); the README describes the tool.

### Fixed

Author: *Bert Berrevoets*

- Removing an existing ID3v2.4 tag now also removes its optional 10-byte footer, and a file that consists only of a
  tag is emptied instead of left as it was (#3, found by Codex).

## 2026-02

### Changed

Author: *Bert Berrevoets*

- .NET 10 and Visual Studio 2022; Claude Code build permissions.
- Rewritten into modules: silent MPEG-1 Layer III audio (`SilentMp3Generator`), ID3v1 and ID3v2.4 tag writers with
  text, comment and timestamp frames, `Mp3Metadata`, and file helpers (#2).

## 2026-01

### Added

Author: *Claude*

- `CLAUDE.md` with guidance for AI assistants (#1).

## 2022-02

### Added

Author: *Bert Berrevoets*

- First version: a console app that writes an MP3 file with an ID3v1 tag; cleaned up and moved to .NET 6. Release
  V1.0.0.
