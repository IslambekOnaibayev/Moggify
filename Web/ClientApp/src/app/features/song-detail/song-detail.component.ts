import { Component, Input, ViewChild, ElementRef, effect, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { SongDetails } from '../../core/models/song.model';
import { SongApiService } from '../../infrastructure/api/song-api.service';
import { SongStore } from '../../core/store/song.store';
import { PlayerStore } from '../../core/store/player.store';
import { CoverModalStore } from '../../core/store/cover-modal.store';

interface LyricLine {
  gap: boolean;
  idx: number;
  text: string;
  words: string[];
  start: number;
  end: number;
}

@Component({
  selector: 'app-song-detail',
  standalone: true,
  imports: [MatButtonModule, MatIconModule],
  templateUrl: './song-detail.component.html',
  styleUrl: './song-detail.component.scss',
  host: { '[class.compact]': 'compact' },
})
export class SongDetailComponent {
  @Input({ required: true }) details!: SongDetails;
  @Input() compact = false;
  @ViewChild('lyricsContainer') lyricsContainerRef?: ElementRef<HTMLElement>;

  private api = inject(SongApiService);
  private songStore = inject(SongStore);
  private coverModal = inject(CoverModalStore);
  player = inject(PlayerStore);

  private lastActiveIdx = -1;
  private modelSource = '';
  lines: LyricLine[] = [];

  constructor() {
    effect(() => {
      this.player.progress();
      this.player.duration();
      if (!this.isCurrent) return;
      this.scrollToActiveLine();
    });
  }

  get coverUrl(): string {
    return this.api.getCoverUrl(
      this.songStore.seed(), this.details.index, this.songStore.likesAvg(),
      this.songStore.locale(), this.details.title, this.details.artist
    );
  }

  get audioUrl(): string {
    return this.api.getAudioUrl(this.songStore.seed(), this.details.index);
  }

  get downloadName(): string {
    return `${this.details.title} - ${this.details.artist}.wav`.replace(/[\\/:*?"<>|]/g, '_');
  }

  get isCurrent(): boolean {
    return this.player.isCurrent(this.details.index, this.songStore.seed());
  }

  get isPlayingThis(): boolean {
    return this.isCurrent && this.player.isPlaying();
  }

  playThis(): void {
    this.player.play(this.details, this.songStore.seed());
  }

  openCover(): void {
    this.coverModal.open(this.coverUrl);
  }

  get likes(): number {
    return this.songStore.displayLikes(this.details.index, this.details.likes);
  }
  like(): void {
    this.songStore.likeSong(this.details.index, this.details.likes);
  }

  get displayLines(): LyricLine[] {
    const lyrics = this.details?.lyrics ?? '';
    if (lyrics === this.modelSource) return this.lines;
    this.modelSource = lyrics;

    const raw = lyrics.split('\n');
    const weights = raw.map(t => (t.trim() ? Math.max(1, t.trim().split(/\s+/).length) : 0));
    const total = weights.reduce((a, b) => a + b, 0) || 1;

    let acc = 0;
    this.lines = raw.map((t, i) => {
      const trimmed = t.trim();
      if (!trimmed) return { gap: true, idx: i, text: '', words: [], start: 0, end: 0 };
      const start = acc / total;
      acc += weights[i];
      const end = acc / total;
      return { gap: false, idx: i, text: trimmed, words: trimmed.split(/\s+/), start, end };
    });
    return this.lines;
  }

  lineState(line: LyricLine): 'past' | 'active' | 'future' {
    if (!this.isCurrent) return 'future';
    const duration = this.player.duration();
    if (!duration) return 'future';
    const t = this.player.progress();
    if (t >= line.end * duration) return 'past';
    if (t < line.start * duration) return 'future';
    return 'active';
  }

  sungWords(line: LyricLine): number {
    const duration = this.player.duration();
    if (!duration) return 0;
    const startT = line.start * duration;
    const endT = line.end * duration;
    if (endT <= startT) return line.words.length;
    const frac = (this.player.progress() - startT) / (endT - startT);
    return Math.max(0, Math.min(line.words.length, Math.floor(frac * line.words.length)));
  }

  private get currentLineIndex(): number {
    const active = this.displayLines.find(l => !l.gap && this.lineState(l) === 'active');
    return active ? active.idx : -1;
  }

  private scrollToActiveLine(): void {
    const idx = this.currentLineIndex;
    if (idx === this.lastActiveIdx) return;
    this.lastActiveIdx = idx;
    const outer = this.lyricsContainerRef?.nativeElement;
    if (!outer) return;
    const inner = outer.querySelector('.lyrics-scroll') as HTMLElement | null;
    const el = outer.querySelector(`[data-idx="${idx}"]`) as HTMLElement | null;
    if (!inner || !el) return;
    const center = el.offsetTop - inner.clientHeight / 2 + el.clientHeight / 2;
    inner.scrollTo({ top: center, behavior: 'smooth' });
  }
}
