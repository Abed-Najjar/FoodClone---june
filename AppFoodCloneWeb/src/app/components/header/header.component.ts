import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent implements OnInit {
  cartItemCount = 0;
  isLoggedIn = false;

  constructor(
    private authService: AuthService,
    private cartService: CartService
  ) {}  ngOnInit(): void {
    this.cartService.cart$.subscribe(cart => {
      this.cartItemCount = cart.reduce((total, item) => total + item.quantity, 0);
    });
    
    // Check login status
    this.isLoggedIn = this.authService.isLoggedIn();
    
    // Subscribe to route changes to update login status
    this.authService.loginStatusChange.subscribe((isLoggedIn: boolean) => {
      this.isLoggedIn = isLoggedIn;
    });
  }
  logout(): void {
    this.authService.logout();
    this.isLoggedIn = false;
    // Redirect to login page
    window.location.href = '/login';
  }
}
