import { Injectable, signal, computed, inject } from '@angular/core';
import { Song, SongDetails, ViewMode } from '../models/song.model';
import { SongApiService } from '../../infrastructure/api/song-api.service';
import { AudioPreloadService } from '../services/audio-preload.service';

export const TABLE_PAGE_SIZE = 10;
export const GALLERY_BATCH_SIZE = 20;

@Injectable({ providedIn: 'root' })
export class SongStore {
  private api = inject(SongApiService);
  private preload = inject(AudioPreloadService);

  readonly locale = signal<string>('en-US');
  readonly seed = signal<number>(Math.floor(Math.random() * 99999999));
  readonly likesAvg = signal<number>(0);
  readonly viewMode = signal<ViewMode>('table');

  readonly currentPage = signal<number>(1);
  readonly tableSongs = signal<Song[]>([]);
  readonly tableLoading = signal<boolean>(false);

  readonly gallerySongs = signal<Song[]>([]);
  readonly galleryLoading = signal<boolean>(false);
  readonly galleryHasMore = signal<boolean>(true);
  private galleryBatch = 0;

  readonly expandedIndex = signal<number | null>(null);
  readonly detailRowIndex = signal<number | null>(null);
  readonly expandedDetails = signal<SongDetails | null>(null);
  readonly detailsLoading = signal<boolean>(false);

  readonly userLikes = signal<Record<number, number>>({});

  readonly loadedIndices = computed<number[]>(() => {
    const set = new Set<number>();
    for (const s of this.tableSongs()) set.add(s.index);
    for (const s of this.gallerySongs()) set.add(s.index);
    return [...set];
  });

  private reloadTimer: ReturnType<typeof setTimeout> | null = null;
  private collapseTimer: ReturnType<typeof setTimeout> | null = null;
  private tableReqId = 0;

  constructor() {
    this.fetchTablePage(this.locale(), this.seed(), 1);
  }

  setLocale(value: string): void {
    if (value === this.locale()) return;
    this.locale.set(value);
    this.resetAndReload();
  }

  setSeed(value: number): void {
    const n = Number(value);
    if (!Number.isFinite(n) || n === this.seed()) return;
    this.seed.set(n);
    this.debounce(() => this.resetAndReload(), 300);
  }

  generateRandomSeed(): void {
    this.seed.set(Math.floor(Math.random() * 99999999));
    this.resetAndReload();
  }

  setLikesAvg(value: number, immediate = false): void {
    this.likesAvg.set(Number(value));
    if (immediate) {
      if (this.reloadTimer !== null) { clearTimeout(this.reloadTimer); this.reloadTimer = null; }
      this.reloadForLikes();
    } else {
      this.debounce(() => this.reloadForLikes(), 200);
    }
  }

  setViewMode(mode: ViewMode): void {
    this.viewMode.set(mode);
    if (mode === 'gallery') this.loadInitialGallery();
  }

  likeSong(index: number, baseLikes: number): void {
    const liked = (this.userLikes()[index] ?? 0) > 0;
    if (!liked && baseLikes >= 10) return;
    this.userLikes.update(m => ({ ...m, [index]: liked ? 0 : 1 }));
  }

  displayLikes(index: number, baseLikes: number): number {
    return Math.min(10, baseLikes + (this.userLikes()[index] ?? 0));
  }

  goToPage(page: number): void {
    this.currentPage.set(page);
    this.clearExpansion();
    this.fetchTablePage(this.locale(), this.seed(), page);
  }

  loadNextGalleryBatch(): void {
    if (this.galleryLoading() || !this.galleryHasMore()) return;
    this.galleryBatch += 1;
    const batch = this.galleryBatch;
    this.galleryLoading.set(true);
    this.api
      .listSongs(this.locale(), this.seed(), batch, GALLERY_BATCH_SIZE, this.likesAvg())
      .subscribe({
        next: (result) => {
          this.gallerySongs.update((prev) => [...prev, ...result.items]);
          this.galleryHasMore.set(result.items.length === GALLERY_BATCH_SIZE);
          this.galleryLoading.set(false);
          this.preload.preload(result.items.map(s => this.api.getAudioUrl(this.seed(), s.index)));
        },
        error: () => this.galleryLoading.set(false),
      });
  }

  loadInitialGallery(): void {
    if (this.gallerySongs().length === 0 && !this.galleryLoading()) {
      this.loadNextGalleryBatch();
    }
  }

  toggleExpand(index: number): void {
    if (this.expandedIndex() === index) {
      this.collapseExpanded();
      return;
    }
    if (this.collapseTimer !== null) { clearTimeout(this.collapseTimer); this.collapseTimer = null; }
    this.expandedIndex.set(index);
    this.detailRowIndex.set(index);
    this.loadDetails(index);
  }

  private collapseExpanded(): void {
    this.expandedIndex.set(null);
    if (this.collapseTimer !== null) clearTimeout(this.collapseTimer);
    this.collapseTimer = setTimeout(() => {
      this.collapseTimer = null;
      this.detailRowIndex.set(null);
      this.expandedDetails.set(null);
    }, 260);
  }

  private clearExpansion(): void {
    if (this.collapseTimer !== null) { clearTimeout(this.collapseTimer); this.collapseTimer = null; }
    this.expandedIndex.set(null);
    this.detailRowIndex.set(null);
    this.expandedDetails.set(null);
  }

  revealSong(index: number): void {
    const page = Math.floor((index - 1) / TABLE_PAGE_SIZE) + 1;
    if (page !== this.currentPage() || this.tableSongs().length === 0) {
      this.currentPage.set(page);
      this.fetchTablePage(this.locale(), this.seed(), page);
    }
    if (this.expandedIndex() === index) return;
    if (this.collapseTimer !== null) { clearTimeout(this.collapseTimer); this.collapseTimer = null; }
    this.expandedIndex.set(index);
    this.detailRowIndex.set(index);
    this.loadDetails(index);
  }

  private resetAndReload(): void {
    this.currentPage.set(1);
    this.clearExpansion();
    this.userLikes.set({});
    this.gallerySongs.set([]);
    this.galleryBatch = 0;
    this.galleryHasMore.set(true);
    this.fetchTablePage(this.locale(), this.seed(), 1);
    if (this.viewMode() === 'gallery') this.loadNextGalleryBatch();
  }

  private reloadForLikes(): void {
    this.userLikes.set({});
    this.clearExpansion();
    this.fetchTablePage(this.locale(), this.seed(), this.currentPage());
    if (this.viewMode() === 'gallery') {
      this.gallerySongs.set([]);
      this.galleryBatch = 0;
      this.galleryHasMore.set(true);
      this.loadNextGalleryBatch();
    }
  }

  private loadDetails(index: number): void {
    this.expandedDetails.set(null);
    this.detailsLoading.set(true);
    this.api
      .getSongDetails(index, this.locale(), this.seed(), this.likesAvg())
      .subscribe({
        next: (d) => { this.expandedDetails.set(d); this.detailsLoading.set(false); },
        error: () => this.detailsLoading.set(false),
      });
  }

  private fetchTablePage(locale: string, seed: number, page: number): void {
    const reqId = ++this.tableReqId;
    this.tableLoading.set(true);
    this.api.listSongs(locale, seed, page, TABLE_PAGE_SIZE, this.likesAvg()).subscribe({
      next: (r) => {
        if (reqId !== this.tableReqId) return;
        this.tableSongs.set(r.items);
        this.tableLoading.set(false);
        this.preload.preload(r.items.map(s => this.api.getAudioUrl(seed, s.index)));
      },
      error: () => { if (reqId === this.tableReqId) this.tableLoading.set(false); },
    });
  }

  private debounce(fn: () => void, ms: number): void {
    if (this.reloadTimer !== null) clearTimeout(this.reloadTimer);
    this.reloadTimer = setTimeout(() => { this.reloadTimer = null; fn(); }, ms);
  }
}
