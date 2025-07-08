import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ImageService, ImageDetail } from '../../services/image.service';
// Material Design:
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-image-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatButtonModule
  ],
  templateUrl: './image-detail.component.html',
  styleUrls: ['./image-detail.component.css']
})
export class ImageDetailComponent implements OnInit {
  image: ImageDetail | null = null; // Use ImageDetail interface
  isLoading = true;
  errorMessage: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private imageService: ImageService
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.getImageDetail(Number(id));
      } else {
        this.errorMessage = 'Image ID not found in route.';
        this.isLoading = false;
      }
    });
  }

  getImageDetail(id: number): void {
    this.isLoading = true;
    this.imageService.getImageDetail(id).subscribe({
      next: (data: ImageDetail) => { // Expect ImageDetail
        this.image = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error fetching image detail:', err);
        this.errorMessage = 'Failed to load image details. Image may not exist.';
        this.isLoading = false;
      }
    });
  }
}