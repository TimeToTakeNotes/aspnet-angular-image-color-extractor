import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService, LoginRequest, AuthResponse } from '../../services/auth.service';

// Material Design:
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    RouterModule,
    //Material Design:
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  email = '';
  password = '';
  errorMessage: string | null = null;
  isLoading = false;

  constructor(private authService: AuthService, private router: Router) {}

  onLogin(): void {
    if (!this.email || !this.password) {
      this.errorMessage = 'Please enter valid credentials.';
      return;
    }

    // Trim values before submission
    const emailTrimmed = this.email.trim();
    const passwordTrimmed = this.password.trim();
    
    const loginData: LoginRequest = {
      email: emailTrimmed,
      password: passwordTrimmed
    };

    this.isLoading = true;
    this.errorMessage = null;

    this.authService.login(loginData).subscribe({
      next: (res: AuthResponse) => {
        this.isLoading = false;
        this.router.navigate(['/home']); // Navigate to home after login
      },
      error: (err) => {
        this.errorMessage = err?.error?.message || 'Login failed. Please try again.';
        this.isLoading = false;
      }
    });
  }

  ngOnInit(): void {
    this.authService.isLoggedIn().subscribe(isLoggedIn => {
      if (isLoggedIn) {
        this.router.navigate(['/home']);
      }
    });
  }
}