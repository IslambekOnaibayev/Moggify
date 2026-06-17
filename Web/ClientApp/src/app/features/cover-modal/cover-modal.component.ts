import { Component, HostListener, inject } from '@angular/core';
import { CoverModalStore } from '../../core/store/cover-modal.store';

@Component({
  selector: 'app-cover-modal',
  standalone: true,
  templateUrl: './cover-modal.component.html',
  styleUrl: './cover-modal.component.scss',
})
export class CoverModalComponent {
  modal = inject(CoverModalStore);

  @HostListener('document:keydown.escape')
  onEscape(): void {
    this.modal.close();
  }
}
