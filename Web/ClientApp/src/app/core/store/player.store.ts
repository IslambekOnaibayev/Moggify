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
  readonly isLoading = signal(false);
  readonly progress = signal(0);
  readonly duration = signal(0);
  readonly volume = signal(0.8);
  readonly isShuffled = signal(false);
  readonly isRepeating = signal(false);
  readonly autoPlay = signal(true);

  private audio: HTMLAudioElement | null = null;
  private raf = 0;
  private fadeTimer: ReturnType<typeof setInterval> | null = null;
  private resolvingDuration = false;

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
    this.isLoading.set(true);

    this.teardown();

    const a = new Audio(this.api.getAudioUrl(seed, track.index));
    a.preload = 'auto';
    a.loop = this.isRepeating();
    a.volume = 0;
    a.addEventListener('canplay', () => this.isLoading.set(false));
    a.addEventListener('loadedmetadata', () => this.applyDuration(a));
    a.addEventListener('durationchange', () => this.applyDuration(a));
    a.addEventListener('ended', () => {
      if (this.resolvingDuration) return;
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
    this.isLoading.set(false);
    this.currentIndex.set(null);
    this.currentTitle.set('');
    this.currentArtist.set('');
    this.progress.set(0);
    this.duration.set(0);
  }

  next(): void {
    if (this.isShuffled()) this.shuffleNext();
    else this.step(1);
  }
  previous(): void { this.step(-1); }

  private step(delta: number): void {
    const idx = this.currentIndex();
    if (idx === null) return;
    const target = Math.max(1, idx + delta);
    if (target === idx) return;
    this.goToIndex(target);
  }

  // Pick a random track (other than the current one) from the loaded pool, so
  // shuffle stays responsive — those tracks are already preloaded/cached.
  private shuffleNext(): void {
    const idx = this.currentIndex();
    if (idx === null) return;
    const pool = this.songStore.loadedIndices().filter(i => i !== idx);
    if (pool.length === 0) { this.step(1); return; }
    const target = pool[Math.floor(Math.random() * pool.length)];
    this.goToIndex(target);
  }

  private goToIndex(target: number): void {
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

  private applyDuration(a: HTMLAudioElement): void {
    const d = a.duration;
    if (isFinite(d) && d > 0) this.duration.set(d);
    else if (!this.resolvingDuration) this.resolveDurationViaSeek(a);
  }

  // Ogg/Opus streams report duration === Infinity until the final page is read.
  // Seeking past the end forces the browser to resolve the real duration.
  private resolveDurationViaSeek(a: HTMLAudioElement): void {
    this.resolvingDuration = true;
    const onUpdate = () => {
      if (!isFinite(a.duration) || a.duration <= 0) return;
      a.removeEventListener('timeupdate', onUpdate);
      this.resolvingDuration = false;
      this.duration.set(a.duration);
      a.currentTime = 0;
    };
    a.addEventListener('timeupdate', onUpdate);
    a.currentTime = 1e101;
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
      if (!this.resolvingDuration) this.progress.set(this.audio.currentTime);
      const d = this.audio.duration;
      if (isFinite(d) && d > 0) this.duration.set(d);
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
    this.resolvingDuration = false;
    if (this.fadeTimer !== null) { clearInterval(this.fadeTimer); this.fadeTimer = null; }
    if (this.audio) {
      this.audio.pause();
      this.audio = null;
    }
  }
}
