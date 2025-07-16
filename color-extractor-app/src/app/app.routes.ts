import { Routes } from '@angular/router';
import { RegisterComponent } from './components/register/register.component';
import { LoginComponent } from './components/login/login.component';
import { HomeComponent } from './components/home/home.component';
import { UploadComponent } from './components/upload/upload.component';
import { ImageListComponent } from './components/image-list/image-list.component';
import { ImageDetailComponent } from './components/image-detail/image-detail.component';
import { UserMenuComponent } from './components/user-menu/user-menu.component';
import { AuthGuard } from './guards/auth.guard';

// Defines which component to show for each url:
export const routes: Routes = [

  { path: 'register', component: RegisterComponent },
  { path: 'login', component: LoginComponent },

  // Protected routes:
  { path: 'home', component: HomeComponent, canActivate: [AuthGuard] },
  { path: 'upload', component: UploadComponent, canActivate: [AuthGuard] },
  { path: 'images', component: ImageListComponent, canActivate: [AuthGuard] },
  { path: 'images/:id', component: ImageDetailComponent, canActivate: [AuthGuard] },

  { path: 'user', component: UserMenuComponent, canActivate: [AuthGuard]},

  // Empty path -> redirect to login
  { path: '', redirectTo: 'login', pathMatch: 'full' },

  // Wildcard path -> redirect to login
  { path: '**', redirectTo: 'login', pathMatch: 'full' } // Catch-all redirects to root (which triggers redirect guard)
];