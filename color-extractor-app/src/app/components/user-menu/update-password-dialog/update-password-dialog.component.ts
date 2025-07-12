import { Component } from '@angular/core';
import { NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UserService } from '../../../services/user.service';

@Component({
  selector: 'app-update-password-dialog',
  standalone: true,
  imports: [
    NgIf,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDialogModule
  ],
  templateUrl: './update-password-dialog.component.html',
  styleUrls: ['./update-password-dialog.component.css']
})
export class UpdatePasswordDialog {
  currentPassword = '';
  newPassword = '';
  confirmPassword = '';
  loading = false;
  errorMessage: string | null = null;

  constructor(
    private dialogRef: MatDialogRef<UpdatePasswordDialog>,
    private userService: UserService,
    private snackBar: MatSnackBar
  ) {}

  get passwordMismatch(): boolean {
    return this.newPassword !== this.confirmPassword;
  }

  get passwordInvalid(): boolean {
    return !/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&.])[A-Za-z\d@$!%*?&.]{8,}$/.test(this.newPassword);
  }

  onCancel() {
    this.dialogRef.close();
  }

  onUpdatePassword(formValid: boolean) {
    if (!formValid || this.passwordMismatch || this.passwordInvalid) return;
    this.loading = true;

    this.userService.updatePassword({
      currentPassword: this.currentPassword.trim(),
      newPassword: this.newPassword.trim(),
      confirmPassword: this.confirmPassword.trim()
    }).subscribe({
      next: res => {
        this.snackBar.open(res.message, 'Close', { duration: 3000, panelClass: 'snackbar-success' });
        this.dialogRef.close();
      },
      error: err => {
        this.loading = false;
        this.snackBar.open(err.message, 'Close', { duration: 3000, panelClass: 'snackbar-error' });
      }
    });
  }
}