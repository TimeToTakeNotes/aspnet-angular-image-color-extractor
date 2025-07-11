import { Component } from '@angular/core';
import { NgIf } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { throwError } from 'rxjs';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-delete-account-dialog',
  standalone: true,
  imports: [
    NgIf,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDialogModule,
  ],
  template: `
    <h2 mat-dialog-title>Delete Account?</h2>
    <mat-dialog-content [formGroup]="form">
      <p>Please enter your password to confirm account deletion.</p>
      <mat-form-field appearance="outline" class="full-width">
        <mat-label>Password</mat-label>
        <input
          matInput
          type="password"
          formControlName="password"
          autocomplete="current-password"
          required
        />
        <mat-error *ngIf="form.controls['password'].hasError('required')">
          Password is required
        </mat-error>
        <mat-error *ngIf="form.controls['password'].hasError('minlength')">
          Password must be at least 8 characters
        </mat-error>
        <mat-error *ngIf="form.controls['password'].hasError('backend')">
            {{ form.controls['password'].getError('backend') }}
        </mat-error>
      </mat-form-field>
    </mat-dialog-content>

    <mat-dialog-actions align="end">
      <button mat-button (click)="onCancel()" class="global-button" [disabled]="loading">Cancel</button>
      <button
        mat-raised-button
        color="warn"
        [disabled]="form.invalid || loading"
        (click)="onConfirm()"
        class="global-button"
      >
        Delete Account
      </button>
    </mat-dialog-actions>
  `,
  styles: [`.full-width { width: 100%; }`],
})
export class DeleteAccountDialog {
  form: FormGroup;
  loading = false;
  backendError: string | null = null;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<DeleteAccountDialog>,
    private userService: UserService,
    private snackBar: MatSnackBar
  ) {
    this.form = this.fb.group({
      password: ['', [Validators.required, Validators.minLength(8)]],
    });

    // Clear backend error whenever user edits password
    this.form.controls['password'].valueChanges.subscribe(() => {
        const control = this.form.controls['password'];
        if (control.hasError('backend')) {
            const errors = { ...control.errors };
            delete errors['backend'];
            control.setErrors(Object.keys(errors).length ? errors : null);
        }
    });
  }

  onCancel() {
    this.dialogRef.close(false);
  }

  onConfirm() {
    if (this.form.invalid) return;

    this.loading = true;
    this.backendError = null;

    this.userService.deleteAccount({ password: this.form.value.password }).subscribe({
        next: (res) => {
        this.snackBar.open(res.message, 'Close', { 
          duration: 3000,
          panelClass: 'snackbar-success' 
        });
        this.dialogRef.close(true);
        },
        error: (err: HttpErrorResponse) => {
        console.error('DELETE error', err);

        const passwordControl = this.form.get('password');

        // Clear backendError
        this.backendError = null;

        let errMsg = 'Password is incorrect.';
        if (err.status === 401 && err.error?.message) {
            errMsg = err.error.message;
        } else if (typeof err.error === 'string') {
            errMsg = err.error;
        }

        // ✅ Attach the error to the control itself
        passwordControl?.setErrors({ backend: errMsg });

        this.loading = false;
        }
    });
    }
}