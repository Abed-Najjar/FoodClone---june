import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { OtpService } from '../../services/otp.service';
import { AuthService } from '../../services/auth.service';
import { OtpType, GenerateOtpRequest } from '../../models/otp.model';
import { OtpVerificationComponent } from '../otp-verification/otp-verification.component';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, OtpVerificationComponent],
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent implements OnInit {
  step: 'email' | 'otp' | 'password' = 'email';
  emailForm: FormGroup;
  resetPasswordForm: FormGroup;
  email: string = '';
  otpCode: string = '';
  loading = false;
  error: string = '';
  success: string = '';

  constructor(
    private formBuilder: FormBuilder,
    private otpService: OtpService,
    private authService: AuthService,
    private router: Router
  ) {
    this.emailForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]]
    });

    this.resetPasswordForm = this.formBuilder.group({
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required]
    }, { validators: this.passwordMatchValidator });
  }

  ngOnInit(): void {}

  passwordMatchValidator(group: FormGroup): { [key: string]: any } | null {
    const password = group.get('newPassword');
    const confirmPassword = group.get('confirmPassword');
    
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      return { passwordMismatch: true };
    }
    return null;
  }

  get ef() { return this.emailForm.controls; }
  get rf() { return this.resetPasswordForm.controls; }
  onRequestOtp(): void {
    if (this.emailForm.invalid) {
      return;
    }

    this.loading = true;
    this.error = '';
    this.email = this.emailForm.value.email;

    const request: GenerateOtpRequest = {
      email: this.email,
      type: OtpType.ForgotPassword
    };

    this.otpService.generateOtp(request).subscribe({
      next: (response) => {
        this.loading = false;
        this.success = 'OTP sent successfully! Please check your email.';
        this.step = 'otp';
      },
      error: (error) => {
        this.loading = false;
        this.error = error.error?.message || 'Failed to send OTP. Please try again.';
      }
    });
  }
  onOtpVerified(event: { verified: boolean, otpCode?: string }): void {
    if (event.verified && event.otpCode) {
      this.otpCode = event.otpCode;
      this.success = 'OTP verified successfully! Please set your new password.';
      this.error = '';
      this.step = 'password';
    } else {
      this.error = 'Invalid OTP. Please try again.';
      this.success = '';
    }
  }  onResetPassword(): void {
    if (this.resetPasswordForm.invalid) {
      return;
    }

    this.loading = true;
    this.error = '';

    console.log('Forgot Password - Attempting to reset password with:');
    console.log('  Email:', this.email);
    console.log('  OTP Code:', this.otpCode);
    console.log('  New Password:', this.resetPasswordForm.value.newPassword ? '[SET]' : '[EMPTY]');

    this.authService.resetPasswordWithOtp(
      this.email, 
      this.resetPasswordForm.value.newPassword, 
      this.otpCode
    ).subscribe({
      next: (response) => {
        this.loading = false;
        this.success = 'Password reset successfully! You can now login with your new password.';
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 2000);
      },
      error: (error) => {
        this.loading = false;
        this.error = error.error?.message || 'Failed to reset password. Please try again.';
      }
    });
  }

  goBack(): void {
    if (this.step === 'otp') {
      this.step = 'email';
    } else if (this.step === 'password') {
      this.step = 'otp';
    } else {
      this.router.navigate(['/login']);
    }
  }

  goToLogin(): void {
    this.router.navigate(['/login']);
  }
}
