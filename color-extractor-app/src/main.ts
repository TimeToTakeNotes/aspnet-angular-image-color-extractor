import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi  } from '@angular/common/http';
import { provideRouter } from '@angular/router'; 
import { routes } from './app/app.routes'; 
import { CsrfInterceptor } from './app/middleware/csrf-interceptor';

bootstrapApplication(AppComponent, {
  providers: [
    provideHttpClient(withInterceptorsFromDi()),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: CsrfInterceptor,
      multi: true
    },
    provideRouter(routes),
  ]
}).catch(err => console.error(err));