import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, map, catchError, tap, BehaviorSubject } from 'rxjs';

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
  private apiUrl = 'http://localhost:5176/api/auth'; // Base URL for authentication

  //public loginStatus$ = new BehaviorSubject<boolean>(false); // Track login status 

  constructor(private http: HttpClient) {}

  login(data: LoginRequest): Observable<AuthResponse> {
    // Backend [HttpPost("login")] endpoint
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, data, {
      withCredentials: true
    }).pipe(
      tap(res => console.log('Login response:', res))
    );
  }

  register(data: RegisterRequest): Observable<AuthResponse> {
    // Backend [HttpPost("register")] endpoint
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, data, {
      withCredentials: true
    }).pipe(
      tap(res => console.log('Register response:', res))
    );
  }

  logout(): Observable<{ message: string }> {
    // Backend [HttpPost("logout")] endpoint
    return this.http.post<{ message: string }>(`${this.apiUrl}/logout`, {}, {
      withCredentials: true
    });
  }

  getMe(): Observable<any> {
    // Backend [HttpPost("me")] endpoint
    return this.http.get<any>(`${this.apiUrl}/me`, {
      withCredentials: true
    });
  }

  refresh(): Observable<{ message: string }> {
    // Backend [HttpPost("refresh")] endpoint
    return this.http.post<{ message: string }>(`${this.apiUrl}/refresh`, {}, {
      withCredentials: true
    });
  }

  // Returns an Observable<boolean> that emits true if logged in, false if not
  isLoggedIn(): Observable<boolean> {
    return this.getMe().pipe(
      map(user => !!user), // If getMe succeeds, user is logged in
      catchError(() => of(false)) // On error (401 etc), not logged in
    );
  }
}