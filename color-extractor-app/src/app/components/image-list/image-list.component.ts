import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ImageService, ImageListItem } from '../../services/image.service';

// Material Design:
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';


@Component({
  selector: 'app-image-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    // Material Design:
    MatIconModule,
    MatButtonModule,
    MatTooltipModule
  ],
  templateUrl: './image-list.component.html',
  styleUrls: ['./image-list.component.css']
})
export class ImageListComponent implements OnInit {
  images: ImageListItem[] = []; // Use ImageListItem arr
  isLoading = true;
  errorMessage: string | null = null;

  constructor(private imageService: ImageService) { }

  ngOnInit(): void {
    this.getImages();
  }

  getImages(): void {
    this.isLoading = true;
    this.imageService.getMyImages().subscribe({
      next: (data: ImageListItem[]) => { // Expect ImageListItem arr
        this.images = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error fetching images:', err);
        this.errorMessage = 'Failed to load images. Please try again later.';
        this.isLoading = false;
      }
    });
  }
}