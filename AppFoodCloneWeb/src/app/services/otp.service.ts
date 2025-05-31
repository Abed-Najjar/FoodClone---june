import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { AppResponse } from '../models/app-response.model';
import { 
  GenerateOtpRequest, 
  VerifyOtpRequest, 
  OtpResponse, 
  RegistrationWithOtpRequest,
  PasswordResetWithOtpRequest,
  ResendOtpRequest,
  OtpType 
} from '../models/otp.model';
import { User } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class OtpService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  private getHeaders(): HttpHeaders {
    return new HttpHeaders({
      'Content-Type': 'application/json'
    });
  }

  /**
   * Generate OTP for various purposes (registration, forgot password, etc.)
   */
  generateOtp(request: GenerateOtpRequest): Observable<AppResponse<OtpResponse>> {
    return this.http.post<AppResponse<OtpResponse>>(
      `${this.baseUrl}/otp/generate`,
      request,
      { headers: this.getHeaders() }
    );
  }

  /**
   * Verify OTP code
   */
  verifyOtp(request: VerifyOtpRequest): Observable<AppResponse<OtpResponse>> {
    return this.http.post<AppResponse<OtpResponse>>(
      `${this.baseUrl}/otp/verify`,
      request,
      { headers: this.getHeaders() }
    );
  }
  /**
   * Register user with OTP verification
   */
  registerWithOtp(request: RegistrationWithOtpRequest): Observable<AppResponse<User>> {
    console.log('OTP Service - Sending registration request:', request);
    return this.http.post<AppResponse<User>>(
      `${this.baseUrl}/otp/register-with-otp`,
      request,
      { headers: this.getHeaders() }
    );
  }
  /**
   * Reset password with OTP verification
   */
  resetPasswordWithOtp(request: PasswordResetWithOtpRequest): Observable<AppResponse<boolean>> {
    console.log('OTP Service - Sending password reset request:', request);
    return this.http.post<AppResponse<boolean>>(
      `${this.baseUrl}/otp/reset-password`,
      request,
      { headers: this.getHeaders() }
    );
  }

  /**
   * Resend OTP code
   */
  resendOtp(email: string, type: OtpType): Observable<AppResponse<boolean>> {
    const request: ResendOtpRequest = { email, type };
    return this.http.post<AppResponse<boolean>>(
      `${this.baseUrl}/otp/resend`,
      request,
      { headers: this.getHeaders() }
    );
  }

  /**
   * Cleanup expired OTPs (admin function)
   */
  cleanupExpiredOtps(): Observable<AppResponse<boolean>> {
    return this.http.delete<AppResponse<boolean>>(
      `${this.baseUrl}/otp/cleanup-expired`,
      { headers: this.getHeaders() }
    );
  }

  /**
   * Generate OTP for registration
   */
  generateRegistrationOtp(email: string): Observable<AppResponse<OtpResponse>> {
    return this.generateOtp({
      email,
      type: OtpType.Registration
    });
  }

  /**
   * Generate OTP for forgot password
   */
  generateForgotPasswordOtp(email: string): Observable<AppResponse<OtpResponse>> {
    return this.generateOtp({
      email,
      type: OtpType.ForgotPassword
    });
  }

  /**
   * Generate OTP for email verification
   */
  generateEmailVerificationOtp(email: string): Observable<AppResponse<OtpResponse>> {
    return this.generateOtp({
      email,
      type: OtpType.EmailVerification
    });
  }

  /**
   * Verify registration OTP
   */
  verifyRegistrationOtp(email: string, code: string): Observable<AppResponse<OtpResponse>> {
    return this.verifyOtp({
      email,
      code,
      type: OtpType.Registration
    });
  }

  /**
   * Verify forgot password OTP
   */
  verifyForgotPasswordOtp(email: string, code: string): Observable<AppResponse<OtpResponse>> {
    return this.verifyOtp({
      email,
      code,
      type: OtpType.ForgotPassword
    });
  }
}
