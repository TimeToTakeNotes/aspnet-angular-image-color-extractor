import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, map, catchError, tap, BehaviorSubject, throwError } from 'rxjs';

import { environment } from '../../environment'; // load environment config

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  name: string;
  surname: string;
  email: string;
  password: string;
}

export interface AuthResponse {
  message: string;
  user?: {
    id: number;
    name: string;
    surname: string;
    email: string;
  };
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiBaseUrl}/auth` // Base URL for authentication

  private currentUserSubject = new BehaviorSubject<any | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable(); // Track logged in user
  private isLoggedInSubject = new BehaviorSubject<boolean>(false);
  public isLoggedIn$ = this.isLoggedInSubject.asObservable(); // Track login status 

  constructor(private http: HttpClient) {}

  login(data: LoginRequest): Observable<AuthResponse> {
    // Backend [HttpPost("login")] endpoint
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, data, {
      withCredentials: true
    }).pipe(
      tap(() => {
        this.isLoggedInSubject.next(true);
        this.getMe().subscribe(); // This populates currentUserSubject
      })
    );
  }

  register(data: RegisterRequest): Observable<AuthResponse> {
    // Backend [HttpPost("register")] endpoint
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, data, {
      withCredentials: true
    }).pipe(
      tap(() => {
        this.isLoggedInSubject.next(true);
        this.getMe().subscribe(); // This populates currentUserSubject
      })
    );
  }

  logout(): Observable<{ message: string }> {
    // Backend [HttpPost("logout")] endpoint
    return this.http.post<{ message: string }>(`${this.apiUrl}/logout`, {}, {
      withCredentials: true
    }).pipe(
      tap(() => {
        this.isLoggedInSubject.next(false);
        this.currentUserSubject.next(null);
      })
    );
  }

  // Returns an Observable<boolean> that emits true if logged in, false if not
  isLoggedIn(): Observable<boolean> {
    return this.getMe().pipe(
      map(user => !!user), // If getMe succeeds, user is logged in
      catchError(() => of(false)) // On error (401 etc), not logged in
    );
  }

  // Checks if user is logged in so app.component parts can display
  checkLoginStatus(): void {
    this.getMe().subscribe({
      next: () => this.isLoggedInSubject.next(true),
      error: () => this.isLoggedInSubject.next(false)
    });
  }

  getMe(): Observable<any> {
    // Backend [HttpPost("me")] endpoint
     return this.http.get<any>(`${this.apiUrl}/me`, {
      withCredentials: true
    }).pipe(
      tap(user => this.currentUserSubject.next(user)),
      catchError(err => {
        this.currentUserSubject.next(null);
        return throwError(() => err);
      })
    );
  }

  refresh(): Observable<{ message: string }> {
    // Backend [HttpPost("refresh")] endpoint
    return this.http.post<{ message: string }>(`${this.apiUrl}/refresh`, {}, {
      withCredentials: true
    });
  }
}