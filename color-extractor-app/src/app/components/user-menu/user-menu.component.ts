import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';

// Material Design:
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';

import { UserService } from '../../services/user.service';
import { DeleteAccountDialog } from './delete-account-dialog.component';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    // Material Design:
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatSnackBarModule,
    MatDialogModule
  ],
  templateUrl: './user-menu.component.html',
  styleUrls: ['./user-menu.component.css']
})
export class UserMenuComponent implements OnInit {
  form!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {}

  ngOnInit() {
    this.form = this.fb.group({
      name: ['', Validators.required],
      surname: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
    });
  }

  onSubmit() {
    if (this.form.invalid) return;

    this.userService.updateUserInfo(this.form.value).subscribe({
      next: res => {
        this.snackBar.open(res.message, 'Close', { duration: 3000 });
      },
      error: err => {
        this.snackBar.open(err.message, 'Close', { duration: 4000 });
      }
    });
  }

  onDeleteAccount() {
    const dialogRef = this.dialog.open(DeleteAccountDialog, { width: '400px' });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        // User confirmed and deletion succeeded
        window.location.href = '/login';
      }
    });
  }
}