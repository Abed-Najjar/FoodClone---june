import { Component, OnInit, HostListener, AfterViewInit, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service';
import { ImageUtilService } from '../../services/image-util.service';

declare var bootstrap: any;

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent implements OnInit, AfterViewInit {
  cartItemCount = 0;
  isLoggedIn = false;
  username: string | null = null;
  isScrolled = false;
  userAvatar: string = '';
  logoImage: string = '';
  dropdownInstance: any;

  @ViewChild('navbarDropdown') dropdownEl!: ElementRef;

  constructor(
    private authService: AuthService,
    private cartService: CartService,
    private router: Router,
    private imageUtilService: ImageUtilService
  ) {
    // Set default images
    this.userAvatar = this.getRandomUserAvatar();
    this.logoImage = 'https://images.unsplash.com/photo-1594041680534-e8c8cdebd659?w=100&q=80';
  }

  @HostListener('window:scroll')
  onWindowScroll() {
    this.isScrolled = window.scrollY > 50;
  }

  ngOnInit(): void {
    this.cartService.cart$.subscribe(cart => {
      this.cartItemCount = cart.reduce((total, item) => total + item.quantity, 0);
    });

    // Check login status
    this.isLoggedIn = this.authService.isLoggedIn();

    // Get user details if logged in
    if (this.isLoggedIn) {
      this.getUserDetails();
    }

    // Subscribe to route changes to update login status
    this.authService.loginStatusChange.subscribe((isLoggedIn: boolean) => {
      this.isLoggedIn = isLoggedIn;
      if (isLoggedIn) {
        this.getUserDetails();
        // We need to initialize dropdown after Angular updates the DOM
        setTimeout(() => this.initDropdown(), 0);
      } else {
        this.username = null;
      }
    });
  }

  ngAfterViewInit() {
    // Initialize dropdown after view init if user is logged in
    if (this.isLoggedIn) {
      this.initDropdown();
    }
  }

  // Initialize Bootstrap dropdown
  initDropdown() {
    try {
      const dropdownElementList = document.querySelectorAll('.dropdown-toggle');
      dropdownElementList.forEach((dropdownToggleEl) => {
        new bootstrap.Dropdown(dropdownToggleEl, {
          autoClose: true
        });
      });
    } catch (err) {
      console.error('Error initializing dropdown:', err);
    }
  }

  // Handle dropdown toggle manually
  toggleDropdown(event: Event) {
    event.preventDefault();
    event.stopPropagation();

    try {
      const dropdownToggle = document.getElementById('navbarDropdown');
      if (dropdownToggle) {
        const dropdown = bootstrap.Dropdown.getOrCreateInstance(dropdownToggle);
        dropdown.toggle();
      }
    } catch (err) {
      console.error('Error toggling dropdown:', err);
    }
  }

  getUserDetails(): void {
    // In a real app, this would come from your user profile or auth service
    // For now, we're just setting a default value or getting from localStorage
    const user = this.authService.getCurrentUser();
    this.username = user?.username || localStorage.getItem('username') || 'User';

    // Set a random avatar for the user
    this.userAvatar = this.getRandomUserAvatar();
  }

  logout(): void {
    this.authService.logout();
    this.isLoggedIn = false;
    this.cartService.clearCart();

    // Show logout toast notification
    this.showLogoutToast();

    // Navigate to login page
    this.router.navigate(['/login']);
  }

  // Show a toast notification for successful logout
  private showLogoutToast(): void {
    try {
      // Initialize and show toast using Bootstrap
      const toastElement = document.getElementById('logoutToast');
      if (toastElement) {
        const toast = new bootstrap.Toast(toastElement);
        toast.show();
      }
    } catch (error) {
      console.error('Failed to show logout toast:', error);
    }
  }

  // Check if current user is an admin
  isAdmin(): boolean {
    return this.authService.isAdmin();
  }

  // Get a random logo image
  getLogoImage(): string {
    return this.logoImage;
  }

  // Get a random user avatar
  getUserAvatar(): string {
    return this.userAvatar;
  }

  // Generate a random user avatar from available logo images
  private getRandomUserAvatar(): string {
    const avatars = [
      'https://images.unsplash.com/photo-1594041680534-e8c8cdebd659?w=100&q=80',
      'https://images.unsplash.com/photo-1608031003385-c3992f10dfb5?w=100&q=80',
      'https://images.unsplash.com/photo-1566478989037-eec170784d0b?w=100&q=80',
      'https://images.unsplash.com/photo-1555992457-b8fefdd70bef?w=100&q=80',
      'https://images.unsplash.com/photo-1577219491135-ce391730fb2c?w=100&q=80',
      'https://images.unsplash.com/photo-1622419341006-bca64904e936?w=100&q=80'
    ];
    return avatars[Math.floor(Math.random() * avatars.length)];
  }
}
