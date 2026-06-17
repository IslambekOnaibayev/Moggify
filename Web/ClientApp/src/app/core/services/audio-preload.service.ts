import { Injectable } from '@angular/core';

const TTL_MS = 15 * 60 * 1000;
const MAX_ENTRIES = 30;

@Injectable({ providedIn: 'root' })
export class AudioPreloadService {
  private elements = new Map<string, HTMLAudioElement>();
  private timestamps = new Map<string, number>();

  preload(urls: string[]): void {
    this.evict();
    for (const url of urls) {
      if (this.elements.has(url)) continue;
      if (this.elements.size >= MAX_ENTRIES) break;
      const a = new Audio(url);
      a.preload = 'auto';
      this.elements.set(url, a);
      this.timestamps.set(url, Date.now());
    }
  }

  private evict(): void {
    const now = Date.now();
    for (const [url, ts] of this.timestamps) {
      if (now - ts > TTL_MS) {
        const a = this.elements.get(url);
        if (a) { a.src = ''; a.load(); }
        this.elements.delete(url);
        this.timestamps.delete(url);
      }
    }
  }
}
