import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatSliderModule } from '@angular/material/slider';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { SongStore } from '../../core/store/song.store';
import { ThemeStore } from '../../core/store/theme.store';
import { PlayerStore } from '../../core/store/player.store';
import { SUPPORTED_LOCALES, ViewMode } from '../../core/models/song.model';

@Component({
  selector: 'app-toolbar',
  standalone: true,
  imports: [
    FormsModule, MatToolbarModule, MatFormFieldModule, MatSelectModule,
    MatInputModule, MatSliderModule, MatButtonModule, MatButtonToggleModule,
    MatIconModule, MatTooltipModule,
  ],
  templateUrl: './toolbar.component.html',
  styleUrl: './toolbar.component.scss',
})
export class ToolbarComponent {
  store = inject(SongStore);
  theme = inject(ThemeStore);
  private player = inject(PlayerStore);
  locales = SUPPORTED_LOCALES;

  get currentLocale() {
    return this.locales.find(l => l.code === this.store.locale()) ?? this.locales[0];
  }

  seedValue = this.store.seed();
  likesValue = this.store.likesAvg();

  onLocale(value: string): void {
    this.player.close();
    this.store.setLocale(value);
  }

  onSeed(value: number): void {
    this.seedValue = Number(value);
    this.player.close();
    this.store.setSeed(value);
  }

  onRandomSeed(): void {
    this.player.close();
    this.store.generateRandomSeed();
    this.seedValue = this.store.seed();
  }

  onLikes(value: number): void {
    this.likesValue = Number(value);
    this.player.close();
    this.store.setLikesAvg(value);
  }

  onLikesCommit(value: number): void {
    this.likesValue = Number(value);
    this.player.close();
    this.store.setLikesAvg(value, true);
  }

  onViewMode(mode: ViewMode | undefined): void {
    if (mode) this.store.setViewMode(mode);
  }
}
