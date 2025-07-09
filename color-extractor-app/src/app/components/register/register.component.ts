import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService, RegisterRequest } from '../../services/auth.service';

// Material Design:
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-register',
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
    // Client-side input check
    const nameValid = /^[a-zA-Z\s\-]{2,30}$/.test(this.model.name);
    const surnameValid = /^[a-zA-Z\s\-]{2,30}$/.test(this.model.surname);
    const emailValid = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/.test(this.model.email);
    const passwordStrong = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&.])[A-Za-z\d@$!%*?&.]{8,}$/.test(this.model.password);

    if (!nameValid || !surnameValid || !emailValid || !passwordStrong || this.passwordMismatch) {
      this.errorMessage = 'Please fill in all fields.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = null;

    this.authService.register(this.model).subscribe({
      next: res => {
        this.isLoading = false;
        this.router.navigate(['/home']);
      },
      error: err => {
        this.errorMessage = err?.error?.message || 'Registration failed.';
        this.isLoading = false;
      }
    });
  }
}
