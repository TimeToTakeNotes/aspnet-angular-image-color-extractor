import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ImageService, UploadResponse } from '../../services/image.service'; // Import UploadResponse

@Component({
  selector: 'app-upload',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.css']
})
export class UploadComponent {
  selectedFile: File | null = null;
  uploadedImageUrl: string | null = null;
  hexColor: string | null = null;
  errorMessage: string | null = null;
  isUploading = false;

  constructor(private imageService: ImageService) { }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
      this.uploadedImageUrl = null; // Reset previous upload
      this.hexColor = null;
      this.errorMessage = null;
    }
  }

  onUpload(): void {
    if (!this.selectedFile) {
      this.errorMessage = 'Please select a file to upload.';
      return;
    }

    this.isUploading = true;
    this.errorMessage = null;

    this.imageService.uploadImage(this.selectedFile).subscribe({
      next: (response: UploadResponse) => { // Use UploadResponse interface
        this.uploadedImageUrl = response.imageUrl;
        this.hexColor = response.hexColor;
        this.selectedFile = null;
        this.isUploading = false;
      },
      error: (err) => {
        console.error('Upload error:', err);
        this.errorMessage = 'Failed to upload image. Please try again.';
        if (err.error && typeof err.error === 'string') {
          this.errorMessage = err.error;
        } else if (err.status === 400 && err.error) {
            this.errorMessage = 'Bad request: ' + JSON.stringify(err.error);
        }
        this.isUploading = false;
      }
    });
  }
}