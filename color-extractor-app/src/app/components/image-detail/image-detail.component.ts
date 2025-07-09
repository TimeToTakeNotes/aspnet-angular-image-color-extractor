import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink, Router, RouterModule } from '@angular/router';
import { ImageService, ImageDetail } from '../../services/image.service';
// Material Design:
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-image-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    RouterModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatButtonModule,
    MatDialogModule,
    MatTooltipModule
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
    private router: Router,
    private imageService: ImageService,
    private dialog: MatDialog
  ) {}

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

  deleteImage(): void {
    const dialogRef = this.dialog.open(DeleteConfirmDialog);

    dialogRef.afterClosed().subscribe(result => {
      if (result && this.image) {
        this.imageService.deleteImage(this.image.id).subscribe({
          next: () => {
            alert('Image deleted successfully.');
            this.router.navigate(['/images']);
          },
          error: err => {
            console.error('Failed to delete image:', err);
            alert('Deletion failed. Try again.');
          }
        });
      }
    });
  }
}

@Component({ // Custom component to prevent accidental deletion
  selector: 'app-delete-confirm-dialog',
  standalone: true,
  template: `
    <h2 mat-dialog-title>Delete Image?</h2>
    <mat-dialog-content>
      Are you sure you want to permanently delete this image?
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>No</button>
      <button mat-raised-button color="warn" [mat-dialog-close]="true">Yes, Delete</button>
    </mat-dialog-actions>
  `,
  imports: [MatDialogModule, MatButtonModule]
})
export class DeleteConfirmDialog {}