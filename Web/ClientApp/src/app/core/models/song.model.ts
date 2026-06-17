export interface Song {
  index: number;
  title: string;
  artist: string;
  album: string;
  genre: string;
  likes: number;
}

export interface SongDetails extends Song {
  label: string;
  year: number;
  reviewText: string;
  lyrics: string;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  perPage: number;
  totalCount: number;
  totalPages: number;
}

export interface SongParams {
  locale: string;
  seed: number;
  likesAvg: number;
}

export type ViewMode = 'table' | 'gallery';

export const SUPPORTED_LOCALES = [
  { code: 'en-US', label: 'English (US)', flagClass: 'us' },
  { code: 'ru-RU', label: 'Русский', flagClass: 'ru' },
  { code: 'kk-KZ', label: 'Қазақша', flagClass: 'kz' },
  { code: 'be-BY', label: 'Беларуская', flagClass: 'by' },
] as const;
