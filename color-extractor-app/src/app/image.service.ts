// Cnetral way for Angular frontend to communicate with .NET backend

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

// For the list of imgs (GET /api/image/list)
export interface ImageListItem {
  id: number;
  thumbnailUrl: string;
  hexColor: string;
}

// For single img detail (GET /api/image/{id})
export interface ImageDetail {
  imageId: number;
  imageUrl: string;
  hexColor: string;
}

// For image upload response (POST /api/image/upload)
export interface UploadResponse {
  imageId: number;
  imageUrl: string;
  thumbnailUrl: string;
  hexColor: string;
}

@Injectable({
  providedIn: 'root'
})
export class ImageService {
  private apiUrl = 'http://localhost:5176/api/image'; // Base URL

  constructor(private http: HttpClient) { }

  uploadImage(file: File): Observable<UploadResponse> {
    const formData = new FormData();
    formData.append('img', file, file.name);
    return this.http.post<UploadResponse>(`${this.apiUrl}/upload`, formData);
  }

  getImages(): Observable<ImageListItem[]> {
    // Backend [HttpGet("list")] endpoint
    return this.http.get<ImageListItem[]>(`${this.apiUrl}/list`);
  }

  getImageDetail(id: number): Observable<ImageDetail> {
    // Backend [HttpGet("{id}")] endpoint
    return this.http.get<ImageDetail>(`${this.apiUrl}/${id}`);
  }
}