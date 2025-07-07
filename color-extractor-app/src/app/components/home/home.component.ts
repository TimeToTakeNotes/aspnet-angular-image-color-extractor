import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common'; 
import { Router, RouterLink } from '@angular/router'; 
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './home.component.html', 
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  isLoggedIn = false;
  userName: string = '';

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit(): void {
    this.authService.isLoggedIn().subscribe(status => {
      this.isLoggedIn = status;
      if (status) {
        // Fetch user info if logged in
        this.authService.getMe().subscribe({
          next: user => {
            this.userName = user.name;
          },
          error: err => {
            console.error('Failed to fetch user info', err);
          }
          });
      }
    });
  }

  onLogout(): void {
    this.authService.logout().subscribe({
      next: res => {
        console.log(res.message);
        this.isLoggedIn = false;
        this.router.navigate(['/login']);
      },
      error: err => {
        console.error('Logout failed', err);
      }
    });
  }
}