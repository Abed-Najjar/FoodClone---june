/**
 * Header Component
 * 
 * Updated to fetch real user profile data from the database instead of using hardcoded values.
 * 
 * Key Features:
 * - Fetches user profile including username and profile image from backend API
 * - Shows loading state while fetching profile data
 * - Falls back to generated avatar if no profile image is uploaded
 * - Updates automatically when user profile is modified from other components
 * - Handles errors gracefully with fallback user data
 * 
 * Dependencies:
 * - UserService: for fetching profile data from API
 * - AuthService: for authentication and profile update notifications
 */

import { Component, OnInit, HostListener, AfterViewInit, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service';
import { ImageUtilService } from '../../services/image-util.service';
import { UserService } from '../../services/user.service';
import { User } from '../../models/user.model';

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
  currentUser: User | null = null;
  isLoadingProfile = false;

  @ViewChild('navbarDropdown') dropdownEl!: ElementRef;

  constructor(
    private authService: AuthService,
    private cartService: CartService,
    private router: Router,
    private imageUtilService: ImageUtilService,
    private userService: UserService
  ) {
    // Set default images
    this.logoImage = 'https://images.unsplash.com/photo-1594041680534-e8c8cdebd659?w=100&q=80';
    this.userAvatar = this.getDefaultAvatar();
  }

  @HostListener('window:scroll')
  onWindowScroll() {
    this.isScrolled = window.scrollY > 50;
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: Event) {
    // Close dropdown when clicking outside
    const target = event.target as HTMLElement;
    const dropdown = document.querySelector('.dropdown');
    const dropdownMenu = document.querySelector('.dropdown-menu');
    
    if (dropdown && !dropdown.contains(target) && dropdownMenu?.classList.contains('show')) {
      dropdownMenu.classList.remove('show');
      const dropdownToggle = document.querySelector('.dropdown-toggle');
      dropdownToggle?.setAttribute('aria-expanded', 'false');
    }
  }

  ngOnInit(): void {
    this.cartService.cart$.subscribe(cart => {
      this.cartItemCount = cart.reduce((total, item) => total + item.quantity, 0);
    });

    // Check login status
    this.isLoggedIn = this.authService.isLoggedIn();

    // Get user details if logged in
    if (this.isLoggedIn) {
      this.loadUserProfile();
    }

    // Subscribe to route changes to update login status
    this.authService.loginStatusChange.subscribe((isLoggedIn: boolean) => {
      this.isLoggedIn = isLoggedIn;
      if (isLoggedIn) {
        this.loadUserProfile();
        // We need to initialize dropdown after Angular updates the DOM
        setTimeout(() => this.initDropdown(), 0);
      } else {
        this.username = null;
        this.currentUser = null;
        this.userAvatar = this.getDefaultAvatar();
      }
    });

    // Listen for profile updates from other components
    this.authService.loginStatusChange.subscribe((isLoggedIn: boolean) => {
      if (isLoggedIn && this.isLoggedIn) {
        // If user is still logged in, reload their profile (this catches profile updates)
        const updatedUser = this.authService.getCurrentUser();
        if (updatedUser && updatedUser !== this.currentUser) {
          this.currentUser = updatedUser;
          this.username = updatedUser.username;
          
          // Update avatar
          if (updatedUser.profileImageUrl) {
            this.userAvatar = updatedUser.profileImageUrl;
          } else {
            this.userAvatar = this.generateUserAvatar(updatedUser.username);
          }
        }
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
      const dropdownToggle = document.getElementById('userDropdown');
      if (dropdownToggle) {
        const dropdown = bootstrap.Dropdown.getOrCreateInstance(dropdownToggle);
        dropdown.toggle();
      }
    } catch (err) {
      console.error('Error toggling dropdown:', err);
      // Fallback: manually toggle dropdown visibility
      this.toggleDropdownFallback();
    }
  }

  // Fallback dropdown toggle using CSS classes
  private toggleDropdownFallback() {
    const dropdownMenu = document.querySelector('.dropdown-menu');
    const dropdownToggle = document.querySelector('.dropdown-toggle');
    
    if (dropdownMenu && dropdownToggle) {
      const isShown = dropdownMenu.classList.contains('show');
      
      if (isShown) {
        dropdownMenu.classList.remove('show');
        dropdownToggle.setAttribute('aria-expanded', 'false');
      } else {
        dropdownMenu.classList.add('show');
        dropdownToggle.setAttribute('aria-expanded', 'true');
      }
    }
  }

  // Load user profile from database
  loadUserProfile(): void {
    this.isLoadingProfile = true;
    
    this.userService.getUserProfile().subscribe({
      next: (response) => {
        this.isLoadingProfile = false;
        if (response.success && response.data) {
          this.currentUser = response.data;
          this.username = response.data.username;
          
          // Use profile image from database if available
          if (response.data.profileImageUrl) {
            this.userAvatar = response.data.profileImageUrl;
          } else {
            // Generate a consistent avatar based on username
            this.userAvatar = this.generateUserAvatar(response.data.username);
          }
        } else {
          console.error('Failed to load user profile:', response.errorMessage);
          this.setFallbackUserData();
        }
      },
      error: (error) => {
        this.isLoadingProfile = false;
        console.error('Error loading user profile:', error);
        this.setFallbackUserData();
      }
    });
  }

  // Set fallback user data when profile loading fails
  private setFallbackUserData(): void {
    const user = this.authService.getCurrentUser();
    this.username = user?.username || localStorage.getItem('username') || 'User';
    this.userAvatar = this.generateUserAvatar(this.username);
  }

  // Generate a consistent avatar based on username
  private generateUserAvatar(username: string): string {
    if (!username) return this.getDefaultAvatar();
    
    // Generate consistent colors based on username
    const avatarColors = [
      '#3498db', '#e74c3c', '#2ecc71', '#f39c12', '#9b59b6',
      '#1abc9c', '#34495e', '#e67e22', '#95a5a6', '#27ae60'
    ];
    
    const colorIndex = username.charCodeAt(0) % avatarColors.length;
    const initials = this.getInitials(username);
    
    return `https://ui-avatars.com/api/?name=${encodeURIComponent(initials)}&background=${avatarColors[colorIndex].substring(1)}&color=fff&size=200&font-size=0.6`;
  }

  // Get initials from username
  private getInitials(username: string): string {
    if (!username) return 'U';
    
    const parts = username.split(' ');
    if (parts.length === 1) {
      return username.substring(0, 2).toUpperCase();
    } else {
      return parts.map(part => part.charAt(0)).join('').substring(0, 2).toUpperCase();
    }
  }

  // Get default avatar
  private getDefaultAvatar(): string {
    return 'https://ui-avatars.com/api/?name=User&background=3498db&color=fff&size=200&font-size=0.6';
  }

  logout(): void {
    this.authService.logout();
    this.isLoggedIn = false;
    this.cartService.clearCart();

    // Reset user data
    this.currentUser = null;
    this.username = null;
    this.userAvatar = this.getDefaultAvatar();

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

  // Get user avatar (now fetched from database)
  getUserAvatar(): string {
    return this.userAvatar;
  }
}
