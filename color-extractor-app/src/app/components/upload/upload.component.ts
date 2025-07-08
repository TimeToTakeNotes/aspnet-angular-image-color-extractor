import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ImageService, UploadResponse } from '../../services/image.service';

// Material Design:
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-upload',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    FormsModule,
    // Material Design:  
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatIconModule
  ],
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.css'],
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
       // Reset previous upload state to show new results:
      this.uploadedImageUrl = null;
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