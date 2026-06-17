import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class CoverModalStore {
  readonly url = signal<string | null>(null);

  open(url: string): void {
    if (url) this.url.set(url);
  }

  close(): void {
    this.url.set(null);
  }
}
