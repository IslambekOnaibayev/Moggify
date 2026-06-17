import { Injectable, computed, inject, signal } from '@angular/core';
import { SongApiService } from '../../infrastructure/api/song-api.service';
import { SongStore } from './song.store';

export interface PlayableTrack {
  index: number;
  title: string;
  artist: string;
}

@Injectable({ providedIn: 'root' })
export class PlayerStore {
  private api = inject(SongApiService);
  private songStore = inject(SongStore);

  readonly currentIndex = signal<number | null>(null);
  readonly currentTitle = signal<string>('');
  readonly currentArtist = signal<string>('');
  readonly currentSeed = signal<number>(0);

  readonly isPlaying = signal(false);
  readonly progress = signal(0);
  readonly duration = signal(0);
  readonly volume = signal(0.8);
  readonly isShuffled = signal(false);
  readonly isRepeating = signal(false);
  readonly autoPlay = signal(true);

  private audio: HTMLAudioElement | null = null;
  private raf = 0;
  private fadeTimer: ReturnType<typeof setInterval> | null = null;

  readonly hasTrack = computed(() => this.currentIndex() !== null);
  readonly coverUrl = computed(() => {
    const idx = this.currentIndex();
    if (idx === null) return '';
    return this.api.getCoverUrl(
      this.currentSeed(), idx, this.songStore.likesAvg(), this.songStore.locale(),
      this.currentTitle(), this.currentArtist());
  });

  readonly audioUrl = computed(() => {
    const idx = this.currentIndex();
    return idx === null ? '' : this.api.getAudioUrl(this.currentSeed(), idx);
  });
  readonly downloadName = computed(() => {
    const idx = this.currentIndex();
    if (idx === null) return 'track.wav';
    return `${this.currentTitle()} - ${this.currentArtist()}.wav`.replace(/[\\/:*?"<>|]/g, '_');
  });

  isCurrent(index: number, seed: number): boolean {
    return this.currentIndex() === index && this.currentSeed() === seed;
  }

  play(track: PlayableTrack, seed: number): void {
    if (this.isCurrent(track.index, seed)) {
      this.togglePlay();
      return;
    }
    if (this.audio && this.isPlaying()) {
      this.fade(this.audio.volume, 0, 250, () => this.startTrack(track, seed));
    } else {
      this.startTrack(track, seed);
    }
  }

  private startTrack(track: PlayableTrack, seed: number): void {
    this.currentIndex.set(track.index);
    this.currentTitle.set(track.title);
    this.currentArtist.set(track.artist);
    this.currentSeed.set(seed);
    this.progress.set(0);
    this.duration.set(0);

    this.teardown();

    const a = new Audio(this.api.getAudioUrl(seed, track.index));
    a.preload = 'auto';
    a.loop = this.isRepeating();
    a.volume = 0;
    a.addEventListener('loadedmetadata', () => this.duration.set(a.duration || 0));
    a.addEventListener('durationchange', () => this.duration.set(a.duration || 0));
    a.addEventListener('ended', () => {
      this.stopLoop();
      if (this.isRepeating()) return;
      if (this.autoPlay()) this.next();
      else this.isPlaying.set(false);
    });
    this.audio = a;

    void a.play();
    this.isPlaying.set(true);
    this.startLoop();
    this.fade(0, this.volume(), 600);

    this.songStore.revealSong(track.index);
  }

  togglePlay(): void {
    if (!this.audio) return;
    if (this.isPlaying()) {
      this.audio.pause();
      this.isPlaying.set(false);
      this.stopLoop();
    } else {
      void this.audio.play();
      this.isPlaying.set(true);
      this.startLoop();
    }
  }

  close(): void {
    this.teardown();
    this.isPlaying.set(false);
    this.currentIndex.set(null);
    this.currentTitle.set('');
    this.currentArtist.set('');
    this.progress.set(0);
    this.duration.set(0);
  }

  next(): void { this.step(1); }
  previous(): void { this.step(-1); }

  private step(delta: number): void {
    const idx = this.currentIndex();
    if (idx === null) return;
    const target = Math.max(1, idx + delta);
    if (target === idx) return;
    const seed = this.currentSeed();
    this.api
      .getSongDetails(target, this.songStore.locale(), seed, this.songStore.likesAvg())
      .subscribe(d => this.play(d, seed));
  }

  seekTo(seconds: number): void {
    if (!this.audio) return;
    this.audio.currentTime = seconds;
    this.progress.set(seconds);
  }

  setVolume(v: number): void {
    this.volume.set(v);
    if (this.audio && this.fadeTimer === null) this.audio.volume = v;
  }

  toggleShuffle(): void { this.isShuffled.update(x => !x); }
  toggleAutoPlay(): void { this.autoPlay.update(x => !x); }

  toggleRepeat(): void {
    this.isRepeating.update(x => !x);
    if (this.audio) this.audio.loop = this.isRepeating();
  }

  restart(): void {
    if (this.audio) {
      this.audio.currentTime = 0;
      this.progress.set(0);
    }
  }

  private startLoop(): void {
    cancelAnimationFrame(this.raf);
    const tick = () => {
      if (!this.audio) return;
      this.progress.set(this.audio.currentTime);
      if (this.audio.duration) this.duration.set(this.audio.duration);
      if (this.isPlaying()) this.raf = requestAnimationFrame(tick);
    };
    this.raf = requestAnimationFrame(tick);
  }

  private stopLoop(): void {
    cancelAnimationFrame(this.raf);
  }

  private fade(from: number, to: number, ms: number, onDone?: () => void): void {
    if (!this.audio) { onDone?.(); return; }
    if (this.fadeTimer !== null) clearInterval(this.fadeTimer);
    const steps = Math.max(1, Math.round(ms / 30));
    let step = 0;
    this.audio.volume = Math.max(0, Math.min(1, from));
    this.fadeTimer = setInterval(() => {
      step++;
      const v = from + (to - from) * (step / steps);
      if (this.audio) this.audio.volume = Math.max(0, Math.min(1, v));
      if (step >= steps) {
        if (this.fadeTimer !== null) clearInterval(this.fadeTimer);
        this.fadeTimer = null;
        onDone?.();
      }
    }, 30);
  }

  private teardown(): void {
    this.stopLoop();
    if (this.fadeTimer !== null) { clearInterval(this.fadeTimer); this.fadeTimer = null; }
    if (this.audio) {
      this.audio.pause();
      this.audio = null;
    }
  }
}
