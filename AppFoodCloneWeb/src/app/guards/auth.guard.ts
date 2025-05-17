import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  
  if (authService.isLoggedIn()) {
    return true;
  } else {
    // Don't include returnUrl for the root path to prevent login loops
    const isRootPath = state.url === '/';
    if (!isRootPath) {
      router.navigate(['/login'], { queryParams: { returnUrl: state.url }});
    } else {
      router.navigate(['/login']);
    }
    return false;
  }
};
