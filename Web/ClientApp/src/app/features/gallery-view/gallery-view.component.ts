import { Component, inject, OnInit, HostListener } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { SongStore } from '../../core/store/song.store';
import { PlayerStore } from '../../core/store/player.store';
import { CoverModalStore } from '../../core/store/cover-modal.store';
import { SongApiService } from '../../infrastructure/api/song-api.service';
import { Song } from '../../core/models/song.model';
import { SongDetailComponent } from '../song-detail/song-detail.component';

@Component({
  selector: 'app-gallery-view',
  standalone: true,
  imports: [MatCardModule, MatIconModule, MatProgressSpinnerModule, SongDetailComponent],
  templateUrl: './gallery-view.component.html',
  styleUrl: './gallery-view.component.scss',
})
export class GalleryViewComponent implements OnInit {
  store = inject(SongStore);
  player = inject(PlayerStore);
  api = inject(SongApiService);
  private coverModal = inject(CoverModalStore);

  get songs() { return this.store.gallerySongs(); }
  get loading() { return this.store.galleryLoading(); }
  get hasMore() { return this.store.galleryHasMore(); }
  get selectedIndex() { return this.store.expandedIndex(); }
  get selectedDetails() { return this.store.expandedDetails(); }
  get detailsLoading() { return this.store.detailsLoading(); }

  ngOnInit(): void { this.store.loadInitialGallery(); }

  @HostListener('window:scroll')
  onScroll(): void {
    const scrolled = window.scrollY + window.innerHeight;
    const total = document.documentElement.scrollHeight;
    if (scrolled >= total - 300) this.store.loadNextGalleryBatch();
  }

  coverUrl(seed: number, index: number, title: string, artist: string): string {
    return this.api.getCoverUrl(seed, index, this.store.likesAvg(), this.store.locale(), title, artist);
  }

  seed() { return this.store.seed(); }

  select(song: Song) { this.store.toggleExpand(song.index); }
  isSelected(song: Song) { return this.store.expandedIndex() === song.index; }

  openCover(song: Song, event: MouseEvent) {
    event.stopPropagation();
    this.coverModal.open(this.coverUrl(this.store.seed(), song.index, song.title, song.artist));
  }

  isPlaying(song: Song) {
    return this.player.isCurrent(song.index, this.store.seed()) && this.player.isPlaying() && !this.player.isLoading();
  }
  isLoadingTrack(song: Song) {
    return this.player.isCurrent(song.index, this.store.seed()) && this.player.isLoading();
  }
  playSong(song: Song, event: MouseEvent) {
    event.stopPropagation();
    this.player.play(song, this.store.seed());
  }

  displayLikes(song: Song) { return this.store.displayLikes(song.index, song.likes); }
  like(song: Song, event: MouseEvent) {
    event.stopPropagation();
    this.store.likeSong(song.index, song.likes);
  }
}
