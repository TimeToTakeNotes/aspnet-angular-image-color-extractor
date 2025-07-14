import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { environment } from '../../environment'; // load environment config

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
  private apiUrl = `${environment.apiBaseUrl}/user`; // Base url for user operations 

  constructor(private http: HttpClient) {}

  updateUserInfo(data: UserUpdateRequest): Observable<any> { // Backend [HttpPut] endpoint
    return this.http.put(`${this.apiUrl}/me`, data, {
        withCredentials: true
      }).pipe(catchError(this.handleError));
    }

  updatePassword(data: { currentPassword: string; newPassword: string; confirmPassword: string }): Observable<any> {
    return this.http.post(`${this.apiUrl}/update-password`, data, {
      withCredentials: true
    }).pipe(catchError(this.handleError));
  }

  deleteAccount(data: UserDeleteRequest): Observable<any> { // Backend [HttpDelete] endpoint
    return this.http.request('delete', `${this.apiUrl}/me`, {
      body: data,
      withCredentials: true
    }).pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse) { // Handle HTTP errors
    const errMsg = error.error?.message || 'An unknown error occurred';
    return throwError(() => new Error(errMsg));
  }
}