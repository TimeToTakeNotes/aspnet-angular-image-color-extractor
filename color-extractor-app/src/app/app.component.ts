import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
// Services:
import { AuthService } from './services/auth.service';
// Components:
import { SidebarComponent } from './components/sidebar/sidebar.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet, 
    CommonModule,
    SidebarComponent
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Image Color Extractor';

  constructor(private authService: AuthService, private router: Router) {}

  isLoggedIn$!: Observable<boolean>;
  userName: string = '';
  sidebarOpen = false;

  ngOnInit(): void {
    this.authService.checkLoginStatus();
    this.isLoggedIn$ = this.authService.isLoggedIn$;

    this.authService.currentUser$.subscribe(user => {
      this.userName = user?.name ?? '';
    });
  }

  // Global sidebar toggle
  onToggleSidebar(): void {
    this.sidebarOpen = !this.sidebarOpen;
  }

  // Logout btn logic here so app.component is single source of truth (Logout btn is now in sidebar html)
  onLogout(): void {
    this.authService.logout().subscribe({
      next: res => {
        console.log(res.message);
        this.router.navigate(['/login']);
      },
      error: err => {
        console.error('Logout failed', err);
      }
    });
  }
}
