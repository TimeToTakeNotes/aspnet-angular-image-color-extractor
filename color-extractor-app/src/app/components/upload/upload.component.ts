import { Component, ViewChild, ElementRef } from '@angular/core';
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
import { environment } from '../../../environment';

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
  isDragging = false;

  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  constructor(private imageService: ImageService) { }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.setFile(input.files[0]);
    }
  }

  triggerFileInput(): void {
    this.fileInput.nativeElement.click();
  }

  onDrop(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;

    if (event.dataTransfer?.files?.length) {
      const file = event.dataTransfer.files[0];
      if (file.type.startsWith('image/')) {
        this.setFile(file);
      } else {
        this.errorMessage = 'Only image files are allowed.';
      }
    }
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    this.isDragging = true;
  }

  onDragLeave(event: DragEvent): void {
    event.preventDefault();
    this.isDragging = false;
  }

  private setFile(file: File): void {
    this.selectedFile = file;
    this.uploadedImageUrl = null;
    this.hexColor = null;
    this.errorMessage = null;
  }

  onUpload(): void {
    if (!this.selectedFile) {
      this.errorMessage = 'Please select a file to upload.';
      return;
    }

    this.isUploading = true;
    this.errorMessage = null;

    this.imageService.uploadImage(this.selectedFile).subscribe({
      next: (response: UploadResponse) => {
        this.uploadedImageUrl = `${environment.apiBaseUrl}/image/imagefile/${response.imageId}`;
        this.hexColor = response.hexColor;
        this.selectedFile = null;
        this.fileInput.nativeElement.value = '';
        this.isUploading = false;
      },
      error: (err) => {
        this.errorMessage = 'Failed to upload image. Please try again.';
        if (err.error && typeof err.error === 'string') {
          this.errorMessage = err.error;
        }
        this.isUploading = false;
      }
    });
  }
}