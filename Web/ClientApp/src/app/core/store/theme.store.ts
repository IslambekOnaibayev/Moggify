import { Injectable, signal } from '@angular/core';

const STORAGE_KEY = 'moggify-theme';

@Injectable({ providedIn: 'root' })
export class ThemeStore {
  readonly isDark = signal(true);

  constructor() {
    const saved = localStorage.getItem(STORAGE_KEY);
    const dark = saved ? saved === 'dark' : true;
    this.isDark.set(dark);
    this.apply(dark);
  }

  toggle(): void {
    const dark = !this.isDark();
    this.isDark.set(dark);
    this.apply(dark);
    localStorage.setItem(STORAGE_KEY, dark ? 'dark' : 'light');
  }

  private apply(dark: boolean): void {
    const root = document.documentElement;
    root.classList.toggle('dark', dark);
    root.classList.toggle('light', !dark);
  }
}
