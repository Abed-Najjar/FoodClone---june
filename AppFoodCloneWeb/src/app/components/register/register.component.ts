import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { OtpService } from '../../services/otp.service';
import { UserRegister } from '../../models/user.model';
import { OtpType, GenerateOtpRequest, RegistrationWithOtpRequest } from '../../models/otp.model';
import { OtpVerificationComponent } from '../otp-verification/otp-verification.component';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, OtpVerificationComponent, RouterLink],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  step: 'form' | 'otp' = 'form';
  registerForm!: FormGroup;
  loading = false;
  error = '';
  success = '';
  email = '';
  showPassword = false;
  showConfirmPassword = false;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private otpService: OtpService,
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

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  toggleConfirmPasswordVisibility(): void {
    this.showConfirmPassword = !this.showConfirmPassword;
  }

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
    this.email = this.f['email'].value.trim().toLowerCase();

    // Generate OTP for registration
    const otpRequest: GenerateOtpRequest = {
      email: this.email,
      type: OtpType.Registration
    };

    this.otpService.generateOtp(otpRequest).subscribe({
      next: (response) => {
        this.loading = false;
        this.success = 'OTP sent successfully! Please check your email and verify to complete registration.';
        this.step = 'otp';
      },
      error: (err) => {
        console.error('OTP generation error:', err);
        this.loading = false;
        if (err.error && err.error.message) {
          this.error = err.error.message;
        } else if (err.status === 400) {
          this.error = 'Invalid email address. Please check and try again.';
        } else if (err.status === 409) {
          this.error = 'Email already in use. Please try a different email or login.';
        } else {
          this.error = 'Failed to send OTP. Please try again.';
        }
      }
    });
  }

  onOtpVerified(event: { verified: boolean, otpCode?: string }): void {
    if (event.verified && event.otpCode) {
      this.completeRegistration(event.otpCode);
    } else {
      this.error = 'Invalid OTP. Please try again.';
      this.success = '';
    }
  }

  completeRegistration(otpCode: string): void {
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

    const user: UserRegister = {
      username: this.f['username'].value.trim(),
      email: this.f['email'].value.trim().toLowerCase(),
      password: this.f['password'].value,
      address: addressArray
    };

    // Debug logging
    console.log('Registration data being sent:', {
      username: user.username,
      email: user.email,
      password: user.password ? '[PASSWORD SET]' : '[NO PASSWORD]',
      address: user.address,
      otpCode: otpCode
    });

    this.authService.registerWithOtp(user, otpCode).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success && response.data) {
          this.authService.setCurrentUser(response.data);
          this.success = 'Registration completed successfully! Redirecting...';
          setTimeout(() => {
            this.router.navigate(['/']).then(() => {
              window.location.reload();
            });
          }, 2000);
        } else {
          this.error = response.errorMessage || 'Registration failed. Please try again.';
        }
      },
      error: (err) => {
        console.error('Registration error:', err);
        this.loading = false;
        
        if (err.error && err.error.message) {
          this.error = err.error.message;
        } else if (err.error && typeof err.error === 'string') {
          this.error = err.error;
        } else if (err.error && err.error.errors) {
          const errors = err.error.errors;
          this.error = Object.values(errors).flat().join(' ') || 'Invalid form data. Please check your inputs.';
        } else if (err.status === 400) {
          this.error = 'Invalid form data or OTP. Please check your inputs.';
        } else if (err.status === 409) {
          this.error = 'Email already in use. Please try a different email.';
        } else if (err.status === 500) {
          this.error = 'Server error. Please try again later.';
        } else {
          this.error = 'An error occurred during registration. Please try again.';
        }
      }
    });
  }

  goBackToForm(): void {
    this.step = 'form';
    this.error = '';
    this.success = '';
  }
}
