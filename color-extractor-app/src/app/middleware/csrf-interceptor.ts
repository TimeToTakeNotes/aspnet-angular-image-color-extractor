import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class CsrfInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (['POST', 'PUT', 'DELETE'].includes(req.method)) {
      const modifiedReq = req.clone({
        setHeaders: {
          'X-XSRF-TOKEN': 'a-very-secure-token!' // For production store a secure token and read it in here
        }
      });
      return next.handle(modifiedReq);
    }
    return next.handle(req);
  }
}