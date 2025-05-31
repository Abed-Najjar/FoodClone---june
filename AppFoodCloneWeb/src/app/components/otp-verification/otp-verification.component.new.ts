import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { OtpService } from '../../services/otp.service';
import { OtpType } from '../../models/otp.model';

@Component({
  selector: 'app-otp-verification',
  imports: [CommonModule, FormsModule],
  templateUrl: './otp-verification.component.html',
  styleUrl: './otp-verification.component.css'
})
export class OtpVerificationComponent implements OnInit {
  @Input() email: string = '';
  @Input() otpType: OtpType = OtpType.Registration;
  @Input() title: string = 'Email Verification';
  @Input() subtitle: string = 'Please enter the verification code sent to your email';
  
  @Output() otpVerified = new EventEmitter<{ verified: boolean, otpCode?: string }>();
  @Output() otpFailed = new EventEmitter<string>();
  @Output() resendRequested = new EventEmitter<void>();

  loading = false;
  resending = false;
  error = '';
  success = '';
  timeLeft = 0;
  timerInterval: any;

  // Single OTP input field
  otpCode: string = '';

  constructor(private otpService: OtpService) {}

  ngOnInit(): void {
    this.startTimer();
  }

  ngOnDestroy(): void {
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
    }
  }

  onOtpInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    let value = input.value;
    
    // Remove any non-digit characters
    const numericValue = value.replace(/\D/g, '');
    
    // Limit to 6 digits
    const limitedValue = numericValue.slice(0, 6);
    
    // Update the input value and model
    input.value = limitedValue;
    this.otpCode = limitedValue;
    
    // Clear any previous errors
    if (this.error) {
      this.error = '';
    }
  }

  onOtpPaste(event: ClipboardEvent): void {
    // Prevent default paste behavior
    event.preventDefault();
    
    // Get the pasted text
    const pasteData = event.clipboardData?.getData('text') || '';
    
    // Extract only digits and limit to 6
    const digits = pasteData.replace(/\D/g, '').slice(0, 6);
    
    if (digits.length > 0) {
      this.otpCode = digits;
      // Clear any previous errors
      if (this.error) {
        this.error = '';
      }
    }
  }

  isOtpComplete(): boolean {
    return this.otpCode.length === 6 && /^\d{6}$/.test(this.otpCode);
  }

  verifyOtp(): void {
    if (!this.isOtpComplete()) {
      this.error = 'Please enter the complete 6-digit code';
      return;
    }

    this.loading = true;
    this.error = '';

    this.otpService.verifyOtp({
      email: this.email,
      code: this.otpCode,
      type: this.otpType
    }).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success) {
          this.success = 'Verification successful!';
          this.otpVerified.emit({ verified: true, otpCode: this.otpCode });
        } else {
          this.error = response.errorMessage || 'Verification failed';
          this.otpFailed.emit(this.error);
          this.otpVerified.emit({ verified: false });
          this.clearInput();
        }
      },
      error: (error) => {
        this.loading = false;
        this.error = error.error?.errorMessage || 'Verification failed. Please try again.';
        this.otpFailed.emit(this.error);
        this.otpVerified.emit({ verified: false });
        this.clearInput();
      }
    });
  }

  resendOtp(): void {
    this.resending = true;
    this.error = '';
    
    this.otpService.resendOtp(this.email, this.otpType).subscribe({
      next: (response) => {
        this.resending = false;
        if (response.success) {
          this.success = 'Verification code sent successfully!';
          this.resendRequested.emit();
          this.startTimer();
          this.clearInput();
        } else {
          this.error = response.errorMessage || 'Failed to resend code';
        }
      },
      error: (error) => {
        this.resending = false;
        this.error = error.error?.errorMessage || 'Failed to resend code. Please try again.';
      }
    });
  }

  private startTimer(): void {
    this.timeLeft = 120; // 2 minutes
    
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
    }
    
    this.timerInterval = setInterval(() => {
      this.timeLeft--;
      if (this.timeLeft <= 0) {
        clearInterval(this.timerInterval);
      }
    }, 1000);
  }

  private clearInput(): void {
    this.otpCode = '';
  }

  formatTime(seconds: number): string {
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes}:${remainingSeconds.toString().padStart(2, '0')}`;
  }

  get canResend(): boolean {
    return this.timeLeft <= 0 && !this.resending;
  }
}
