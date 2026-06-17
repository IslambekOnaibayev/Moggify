import { Component, inject } from '@angular/core';
import { SongStore } from './core/store/song.store';
import { ThemeStore } from './core/store/theme.store';
import { ToolbarComponent } from './features/toolbar/toolbar.component';
import { TableViewComponent } from './features/table-view/table-view.component';
import { GalleryViewComponent } from './features/gallery-view/gallery-view.component';
import { PlayerBarComponent } from './features/player-bar/player-bar.component';
import { CoverModalComponent } from './features/cover-modal/cover-modal.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [ToolbarComponent, TableViewComponent, GalleryViewComponent, PlayerBarComponent, CoverModalComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  store = inject(SongStore);
  private theme = inject(ThemeStore);

  get viewMode() { return this.store.viewMode(); }
}
