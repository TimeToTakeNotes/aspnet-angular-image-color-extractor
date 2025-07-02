import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { UploadComponent } from './upload/upload.component';
import { ImageListComponent } from './image-list/image-list.component';
import { ImageDetailComponent } from './image-detail/image-detail.component';

// Defines which component to show for each url:
export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'upload', component: UploadComponent },
  { path: 'images', component: ImageListComponent },
  { path: 'images/:id', component: ImageDetailComponent },
  { path: '**', redirectTo: '' } // For unkown paths, redirect to Home
];