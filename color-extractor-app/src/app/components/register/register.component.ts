import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService, RegisterRequest } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  model: RegisterRequest = {
    name: '',
    surname: '',
    email: '',
    password: ''
  };

  confirmPassword: string = '';
  errorMessage: string | null = null;
  isLoading = false;

  constructor(private authService: AuthService, private router: Router) {}

  get passwordMismatch(): boolean { // Gives user immediate feedback if passwords don't match
    return this.model.password !== this.confirmPassword;
  }

  onSubmit(): void {
    if (!this.model.name || !this.model.surname || !this.model.email || !this.model.password) {
      this.errorMessage = 'Please fill in all fields.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = null;

    this.authService.register(this.model).subscribe({
      next: res => {
        console.log('Register successful:', res);
        this.isLoading = false;
        this.router.navigate(['/home']);
      },
      error: err => {
        console.error('Register failed:', err);
        this.errorMessage = (err.errorMessage && err.errorMessage.message) ? err.errorMessage.message : 'Registration failed.';
        this.isLoading = false;
      }
    });
  }
}
