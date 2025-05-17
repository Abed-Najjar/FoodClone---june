import { APP_INITIALIZER } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export function initializeApp(router: Router, authService: AuthService) {
  return () => {
    return new Promise<void>((resolve) => {
      // Check if user is authenticated
      if (!authService.isLoggedIn()) {
        // Redirect to login page
        router.navigate(['/login']);
      }
      resolve();
    });
  };
}

export const appInitializerProvider = {
  provide: APP_INITIALIZER,
  useFactory: initializeApp,
  deps: [Router, AuthService],
  multi: true
};
