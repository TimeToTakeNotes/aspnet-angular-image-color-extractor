import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

export interface UserUpdateRequest {
  name: string;
  surname: string;
  email: string;
}

export interface UserDeleteRequest {
  password: string;
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private baseUrl = 'http://localhost:5176/api/user/me'; // Base url for user operations

  constructor(private http: HttpClient) {}

  updateUserInfo(data: UserUpdateRequest): Observable<any> { // Backend [HttpPut] endpoint
    return this.http.put(this.baseUrl, data, { withCredentials: true })
      .pipe(catchError(this.handleError));
  }

  deleteAccount(data: UserDeleteRequest): Observable<any> { // Backend [HttpDelete] endpoint
    return this.http.request('delete', this.baseUrl, {
      body: data,
      withCredentials: true
    }).pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse) { // Handle HTTP errors
    const errMsg = error.error?.message || 'An unknown error occurred';
    return throwError(() => new Error(errMsg));
  }
}