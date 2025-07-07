import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root' // Makes guard available app wide
})

// Guard to protect routes so only authenticated users can access them:
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(): Observable<boolean> {
    return this.authService.isLoggedIn().pipe(
      tap(loggedIn => {
        if (!loggedIn) {
          this.router.navigate(['/login']);
        }
      }),
      map(loggedIn => loggedIn)
    );
  }
}