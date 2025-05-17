import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { UserRegister } from '../../models/user.model';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  registerForm!: FormGroup;
  loading = false;
  error = '';

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.registerForm = this.formBuilder.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required],
      address: ['', Validators.required]
    }, {
      validators: this.passwordMatchValidator
    });

    // Auto-navigate if already logged in
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/']);
    }
  }

  // Custom validator to check if passwords match
  passwordMatchValidator(group: FormGroup) {
    const password = group.get('password')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;
    
    if (password !== confirmPassword) {
      group.get('confirmPassword')?.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    
    return null;
  }

  // Getter for easy access to form fields
  get f() { return this.registerForm.controls; }  

  onSubmit(): void {
    // Mark all fields as touched to show validation errors
    if (this.registerForm.invalid) {
      Object.keys(this.registerForm.controls).forEach(key => {
        const control = this.registerForm.get(key);
        control?.markAsTouched();
      });
      this.error = 'Please fix the errors in the form before submitting.';
      return;
    }

    this.loading = true;
    this.error = '';

    // Always send address as an array (even if single value)
    let addressValue = this.f['address'].value;
    let addressArray: string[];
    if (Array.isArray(addressValue)) {
      addressArray = addressValue;
    } else if (typeof addressValue === 'string') {
      addressArray = [addressValue];
    } else {
      addressArray = [];
    }

    // Debug log to check payload
    console.log('Form data:', this.registerForm.value);

    const user: UserRegister = {
      username: this.f['username'].value,
      email: this.f['email'].value,
      password: this.f['password'].value,
      address: addressArray
    };
    // Debug log the API request
    console.log('Sending registration request:', user);
    
    this.authService.register(user).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.authService.setCurrentUser(response.data);
          this.router.navigate(['/']).then(() => {
            // Force reload to update the header
            window.location.reload();
          });
        } else {
          this.error = response.message || 'Registration failed. No specific error message provided.';
          console.error('Registration failed:', response.message || 'No specific error message provided.');
          this.loading = false;
        }
      },
      error: (err) => {
        console.error('Registration error:', err);
        console.error('Error details:', {
          status: err.status,
          statusText: err.statusText,
          error: err.error,
          message: err.message,
          name: err.name
        });
        
        if (err.error && err.error.message) {
          this.error = err.error.message;
          console.error('Error message from server:', err.error.message);
        } else if (err.error && typeof err.error === 'string') {
          this.error = err.error;
          console.error('Error string from server:', err.error);
        } else if (err.error && err.error.errors) {
          // ASP.NET Core model state errors
          const errors = err.error.errors;
          this.error = Object.values(errors).flat().join(' ') || 'Invalid form data. Please check your inputs.';
          console.error('Model state errors:', errors);
        } else if (err.status === 400) {
          this.error = 'Invalid form data. Please check your inputs.';
        } else if (err.status === 409) {
          this.error = 'Email already in use. Please try a different email or login.';
        } else if (err.status === 500) {
          this.error = 'Server error. Please try again later.';
        } else {
          this.error = 'An error occurred during registration. Please try again.';
        }
        this.loading = false;
      }
    });
  }
}
