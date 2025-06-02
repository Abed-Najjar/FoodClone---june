import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { UserLogin } from '../../models/user.model';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  loading = false;
  error = '';
  returnUrl = '';
  showPassword = false;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });

    // Get return url from route parameters or default to '/'
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';

    // Auto-navigate if already logged in
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/']);
    }
  }

  // Getter for easy access to form fields
  get f() { return this.loginForm.controls; }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      // Mark all fields as touched to show validation errors
      Object.keys(this.loginForm.controls).forEach(key => {
        this.loginForm.get(key)?.markAsTouched();
      });
      return;
    }

    this.loading = true;
    this.error = '';

    const credentials: UserLogin = {
      email: this.f['email'].value.trim().toLowerCase(),
      password: this.f['password'].value
    };

    this.authService.login(credentials).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.authService.setCurrentUser(response.data);
          this.router.navigate(['/']).then(() => {
            // Force reload to update the header
            window.location.reload();
          });
        } else {
          this.error = response.errorMessage || 'Login failed. Please check your credentials.';
          this.loading = false;
        }
      },
      error: (err) => {
        console.error('Login error:', err);
        this.error = err.error?.message || 'An error occurred during login. Please try again.';
        this.loading = false;
      }
    });
  }
}
