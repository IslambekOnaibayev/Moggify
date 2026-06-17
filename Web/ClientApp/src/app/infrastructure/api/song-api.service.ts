import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResult, Song, SongDetails } from '../../core/models/song.model';

const API_BASE = '/api';

@Injectable({ providedIn: 'root' })
export class SongApiService {
  constructor(private http: HttpClient) {}

  listSongs(
    locale: string,
    seed: number,
    page: number,
    pageSize: number,
    likes: number,
  ): Observable<PagedResult<Song>> {
    const params = new HttpParams()
      .set('locale', locale)
      .set('seed', seed.toString())
      .set('page', page.toString())
      .set('pageSize', pageSize.toString())
      .set('likes', likes.toString());
    return this.http.get<PagedResult<Song>>(`${API_BASE}/songs`, { params });
  }

  getSongDetails(
    index: number,
    locale: string,
    seed: number,
    likes: number,
  ): Observable<SongDetails> {
    const params = new HttpParams()
      .set('locale', locale)
      .set('seed', seed.toString())
      .set('likes', likes.toString());
    return this.http.get<SongDetails>(`${API_BASE}/songs/${index}/details`, { params });
  }

  getAudioUrl(seed: number, index: number): string {
    return `${API_BASE}/songs/${seed}/${index}/audio`;
  }

  getCoverUrl(seed: number, index: number, likes: number, locale: string, title: string, artist: string): string {
    const t = encodeURIComponent(title);
    const a = encodeURIComponent(artist);
    return `${API_BASE}/songs/${seed}/${index}/cover?likes=${likes}&locale=${locale}&title=${t}&artist=${a}`;
  }
}
