import { Component, OnInit } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Image Color Extractor';
  isLoggedIn = false;

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit(): void {
    // Check login status on load
    this.authService.isLoggedIn().subscribe(status => {
      this.isLoggedIn = status;
    });
  }

  onLogout(): void {
    this.authService.logout().subscribe({
      next: (res) => {
        console.log(res.message);
        this.isLoggedIn = false; // Update UI
        this.router.navigate(['/login']); // Redirect
      },
      error: (err) => {
        console.error('Logout failed', err);
      }
    });
  }
}