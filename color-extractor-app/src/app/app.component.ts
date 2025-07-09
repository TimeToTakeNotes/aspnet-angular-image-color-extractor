import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from './services/auth.service';

// Angular Material modules:
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, MatToolbarModule, MatButtonModule, MatIconModule, MatTooltipModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Image Color Extractor';

  constructor(private authService: AuthService, private router: Router) {}

  isLoggedIn$!: Observable<boolean>;
  userName: string = '';

  ngOnInit(): void {
    this.authService.checkLoginStatus();
    this.isLoggedIn$ = this.authService.isLoggedIn$;

    this.authService.currentUser$.subscribe(user => {
      this.userName = user?.name ?? '';
    });
  }

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
