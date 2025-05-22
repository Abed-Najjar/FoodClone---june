import { Injectable } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(): boolean | UrlTree {
    if (this.authService.isLoggedIn() && this.authService.isAdmin()) {
      return true;
    }

    // If user is logged in but not an admin, redirect to home page
    // Otherwise redirect to login
    const redirectUrl = this.authService.isLoggedIn() ? '/' : '/login';
    return this.router.parseUrl(redirectUrl);
  }
}
