# Moggify

A fake music catalog that never runs out of songs. Pick a language, type in a seed, and Moggify makes up an endless list of tracks — titles, artists, albums, reviews, lyrics, cover art, and even a short audio snippet for each one. Nothing is stored anywhere; every song is generated on the fly from the seed, so the same seed always gives you back the exact same catalog.

It started as a "generate realistic-looking fake data" exercise and turned into something more fun once the covers and audio got involved.

## What it does

- **Deterministic generation.** A song is a pure function of `(seed, locale, likes, index)`. Same inputs → same song, every time. Change the seed and the whole catalog reshuffles.
- **Four languages.** English, Russian, Kazakh, and Belarusian, each with its own word lists so the names actually read like they belong to that language.
- **Two ways to browse.** A paged table for scanning quickly, and a gallery with infinite scroll and album covers.
- **A "likes" dial.** Set an average number of likes per song and the generator spreads them out probabilistically — fractional averages work too (e.g. 0.5 means roughly every other song gets a like).
- **Generated cover art.** Each cover is drawn from scratch with SkiaSharp based on the song's data — no image assets shipped.
- **Generated audio.** Every track gets a short procedurally-composed clip (chords, bass, a melody line, and drums), synthesized to PCM and encoded to Ogg/Opus. It's deterministic too, so a song always sounds the same.
- **A real player.** Play/pause, next/previous, shuffle, repeat, autoplay, volume with fades, a seekable progress bar, and download.

## Tech stack

**Backend** — .NET 10, [FastEndpoints](https://fast-endpoints.com/), Serilog, [Bogus](https://github.com/bchavez/Bogus) for the fake data, SkiaSharp for covers, and [Concentus](https://github.com/lostromb/concentus) for Opus encoding. The audio synthesis itself is hand-rolled — see `Infrastructure/AudioGeneration`.

**Frontend** — Angular 21 (standalone, zoneless, signals throughout) with Angular Material.

The solution is split the usual clean-architecture way:

```
Core            domain model + service interfaces
UseCases        query handlers (CQRS-ish, via a mediator)
Infrastructure  the actual generators: data, covers, audio
Web             FastEndpoints API + the Angular client (ClientApp)
```

## Running it

### With Docker (easiest)

```bash
docker compose up --build
```

Then open http://localhost:8080. The image builds the Angular client and the .NET backend in one go and serves the static frontend from the API.

### Locally, for development

You'll need the .NET 10 SDK and Node 22+.

Backend:

```bash
dotnet run --project Web/Web.csproj
```

Frontend (in a second terminal):

```bash
cd Web/ClientApp
npm install
npm start
```

The Angular dev server runs on http://localhost:4200 and proxies `/api` to the backend (see `proxy.conf.json`).

## API

Everything lives under `/api`. The seed and likes parameters are what make the output deterministic.

| Method | Route | What you get |
|--------|-------|--------------|
| `GET` | `/api/songs?locale=&seed=&page=&pageSize=&likes=` | A page of songs |
| `GET` | `/api/songs/{index}/details?locale=&seed=&likes=` | Full details for one song (label, year, review, lyrics) |
| `GET` | `/api/songs/{seed}/{index}/cover` | Generated cover image |
| `GET` | `/api/songs/{seed}/{index}/audio` | Generated Ogg/Opus clip |

Swagger UI is available in development.

## A couple of notes

- **Audio is cached in memory.** Rendering and encoding a 40–60s clip costs a few CPU-seconds, which is too slow to redo on every request — especially the range requests a browser's `<audio>` element fires while seeking. So each clip is generated once per `(seed, index)` and served from cache afterwards. On a small/throttled host this is the difference between a clip that plays fully and one that gets cut off.
- **Sample rate is 16 kHz.** Plenty for these synth voices, and it keeps the per-request render cost down.
- **Covers and audio are stateless.** They're recomputed from the seed, not stored — so the app has no database and no file storage to manage.

## License

Personal project, no formal license. Feel free to read it and borrow ideas.
