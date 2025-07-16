import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class CsrfInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (['POST', 'PUT', 'DELETE'].includes(req.method)) {
      const csrfToken = this.getCookie('XSRF-TOKEN');
      if (csrfToken) {
        const modifiedReq = req.clone({
          setHeaders: {
            'X-XSRF-TOKEN': csrfToken
          }
        });
        return next.handle(modifiedReq);
      }
    }

    return next.handle(req);
  }

  private getCookie(name: string): string | null {
    const cookies = document.cookie.split(';');
    for (const c of cookies) {
      const [key, value] = c.trim().split('=');
      if (key === name) return decodeURIComponent(value);
    }
    return null;
  }
}