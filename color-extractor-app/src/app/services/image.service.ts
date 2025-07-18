import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../environment'; // load environment config

// For the list of imgs (GET /api/image/list)
export interface ImageListItem {
  id: number;
  thumbnailUrl: string;
  hexColor: string;
}

// For single img detail (GET /api/image/{id})
export interface ImageDetail {
  id: number;
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
  private apiUrl = `${environment.apiBaseUrl}/image`; // Base URL for images

  constructor(private http: HttpClient) { }

  uploadImage(file: File): Observable<UploadResponse> { // Backend [HttpPost("upload")] endpoint
    const formData = new FormData();
    formData.append('img', file, file.name);

    return this.http.post<UploadResponse>(`${this.apiUrl}/upload`, formData, {
      withCredentials: true
    });
  }

  getMyImages(): Observable<ImageListItem[]> { // Backend [HttpGet("my-images")] endpoint
    return this.http.get<ImageListItem[]>(`${this.apiUrl}/my-images`, {
      withCredentials: true
    });
  }

  getImageDetail(id: number): Observable<ImageDetail> { // Backend [HttpGet("{id}")] endpoint
    return this.http.get<ImageDetail>(`${this.apiUrl}/${id}`, {
      withCredentials: true
    });
  }

  deleteImage(id: number): Observable<{ message: string }> { // Backend [HttpDelete("{id}")] endpoint
    return this.http.delete<{ message: string }>(`${this.apiUrl}/${id}`, {
      withCredentials: true
    });
  }
}