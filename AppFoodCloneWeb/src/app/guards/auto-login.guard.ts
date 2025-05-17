import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const autoLoginGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);
  
  // If not logged in, always redirect to login page
  if (!authService.isLoggedIn()) {
    router.navigate(['/login']);
    return false;
  }
  
  // Allow access to the route if logged in
  return true;
};
