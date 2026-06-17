import { Component, inject } from '@angular/core';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatChipsModule } from '@angular/material/chips';
import { SongStore } from '../../core/store/song.store';
import { PlayerStore } from '../../core/store/player.store';
import { Song } from '../../core/models/song.model';
import { SongDetailComponent } from '../song-detail/song-detail.component';

@Component({
  selector: 'app-table-view',
  standalone: true,
  imports: [
    MatTableModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule,
    MatProgressBarModule, MatChipsModule, SongDetailComponent,
  ],
  templateUrl: './table-view.component.html',
  styleUrl: './table-view.component.scss',
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0', opacity: 0 })),
      state('expanded', style({ height: '*', opacity: 1 })),
      transition('expanded <=> collapsed', animate('250ms cubic-bezier(0.4, 0, 0.2, 1)')),
    ]),
  ],
})
export class TableViewComponent {
  store = inject(SongStore);
  player = inject(PlayerStore);

  readonly columns = ['expand', 'index', 'title', 'artist', 'album', 'likes'];

  get songs() { return this.store.tableSongs(); }
  get loading() { return this.store.tableLoading(); }
  get page() { return this.store.currentPage(); }
  get expandedIndex() { return this.store.expandedIndex(); }
  get expandedDetails() { return this.store.expandedDetails(); }
  get detailsLoading() { return this.store.detailsLoading(); }

  isExpanded(song: Song) { return this.expandedIndex === song.index; }
  toggle(song: Song) { this.store.toggleExpand(song.index); }

  isPlaying(song: Song) {
    return this.player.isCurrent(song.index, this.store.seed()) && this.player.isPlaying();
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

  prevPage() { if (this.page > 1) this.store.goToPage(this.page - 1); }
  nextPage() { this.store.goToPage(this.page + 1); }
  goTo(p: number) { this.store.goToPage(p); }

  get pageNumbers(): number[] {
    const p = this.page;
    const pages: number[] = [];
    for (let i = Math.max(1, p - 2); i <= p + 2; i++) pages.push(i);
    return pages;
  }
}
