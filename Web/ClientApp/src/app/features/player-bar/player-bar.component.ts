import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSliderModule } from '@angular/material/slider';
import { MatTooltipModule } from '@angular/material/tooltip';
import { PlayerStore } from '../../core/store/player.store';
import { CoverModalStore } from '../../core/store/cover-modal.store';

@Component({
  selector: 'app-player-bar',
  standalone: true,
  imports: [MatButtonModule, MatIconModule, MatSliderModule, MatTooltipModule],
  templateUrl: './player-bar.component.html',
  styleUrl: './player-bar.component.scss',
})
export class PlayerBarComponent {
  player = inject(PlayerStore);
  private coverModal = inject(CoverModalStore);

  openCover(): void {
    this.coverModal.open(this.player.coverUrl());
  }

  formatTime = (s: number): string => {
    if (!isFinite(s) || s < 0) return '0:00';
    return `${Math.floor(s / 60)}:${Math.floor(s % 60).toString().padStart(2, '0')}`;
  };
}
