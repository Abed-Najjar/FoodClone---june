import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule, NgForm } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-admin-login',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './admin-login.component.html',
  styleUrls: ['./admin-login.component.css']
})
export class AdminLoginComponent {
  credentials = {
    email: '',
    password: ''
  };
  error: string = '';
  loading: boolean = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {
    // Redirect if already logged in as admin
    if (this.authService.isLoggedIn() && this.authService.isAdmin()) {
      this.router.navigate(['/admin/dashboard']);
    }
  }

  onSubmit(form: NgForm): void {
    if (form.invalid) {
      return;
    }

    this.loading = true;
    this.error = '';    this.authService.login(this.credentials).subscribe({
      next: (response) => {
        if (response.statusCode && response.data) {
          this.authService.setCurrentUser(response.data);

          if (this.authService.isAdmin()) {
            this.router.navigate(['/admin/dashboard']);
          } else {
            // Not an admin
            this.error = 'Unauthorized access. Admin privileges required.';
            this.authService.logout();
          }
        } else {
          this.error = response.errorMessage || 'Login failed';
        }
        this.loading = false;
      },
      error: (err) => {
        console.error('Login error:', err);
        this.error = 'Invalid credentials or server error';
        this.loading = false;
      }
    });
  }
}
