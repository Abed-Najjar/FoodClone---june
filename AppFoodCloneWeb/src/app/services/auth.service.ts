import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { User, UserLogin, UserRegister } from '../models/user.model';
import { AppResponse } from '../models/app-response.model';
import { environment } from '../../environments/environment';
import { OtpService } from './otp.service';
import { OtpType } from '../models/otp.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = environment.apiUrl;
  private currentUser: User | null = null;
  public loginStatusChange = new Subject<boolean>();

  constructor(private http: HttpClient, private otpService: OtpService) {
    // Load user from localStorage if available
    const storedUser = localStorage.getItem('user');
    if (storedUser) {
      this.currentUser = JSON.parse(storedUser);
    }
  }

  login(credentials: UserLogin): Observable<AppResponse<User>> {
    return this.http.post<AppResponse<User>>(`${this.baseUrl}/auth/login`, credentials);
  }

  register(user: UserRegister): Observable<AppResponse<User>> {
    console.log('API URL:', `${this.baseUrl}/auth/register`);

    // Set proper headers
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });

    return this.http.post<AppResponse<User>>(
      `${this.baseUrl}/auth/register`,
      user,
      { headers }
    );
  }

  // Debug method to help diagnose registration issues
  debugRegister(user: UserRegister): Observable<any> {
    console.log('Debug API URL:', `${this.baseUrl}/auth/debug-register`);

    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });

    return this.http.post<any>(
      `${this.baseUrl}/auth/debug-register`,
      user,
      { headers }
    );
  }

  logout(): void {
    localStorage.removeItem('user');
    this.currentUser = null;
    this.loginStatusChange.next(false);
  }

  setCurrentUser(user: User): void {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUser = user;
    this.loginStatusChange.next(true);
  }

  getCurrentUser(): User | null {
    return this.currentUser;
  }

  isLoggedIn(): boolean {
    return !!this.currentUser;
  }

  getToken(): string | null {
    return this.currentUser?.token || null;
  }

  // Check if the current user is an admin
  isAdmin(): boolean {
    return this.currentUser?.rolename?.toLowerCase() === 'admin';
  }
  // Check if the current user has a specific role
  hasRole(role: string): boolean {
    return this.currentUser?.rolename?.toLowerCase() === role.toLowerCase();
  }

  // OTP-based authentication methods
  
  /**
   * Start forgot password process by sending OTP
   */
  requestPasswordReset(email: string): Observable<any> {
    return this.otpService.generateForgotPasswordOtp(email);
  }
  /**
   * Reset password using OTP
   */
  resetPasswordWithOtp(email: string, newPassword: string, otpCode: string): Observable<any> {
    console.log('Auth Service - Reset password with OTP:', {
      email,
      newPassword: newPassword ? '[SET]' : '[EMPTY]',
      otpCode
    });
    return this.otpService.resetPasswordWithOtp({
      email,
      newPassword,
      otpCode
    });
  }

  /**
   * Request OTP for registration
   */
  requestRegistrationOtp(email: string): Observable<any> {
    return this.otpService.generateRegistrationOtp(email);
  }

  /**
   * Register with OTP verification
   */
  registerWithOtp(user: UserRegister, otpCode: string): Observable<AppResponse<User>> {
    return this.otpService.registerWithOtp({
      username: user.username,
      email: user.email,
      password: user.password,
      address: user.address,
      otpCode
    });
  }

  /**
   * Verify email with OTP
   */
  verifyEmail(email: string, otpCode: string): Observable<any> {
    return this.otpService.verifyRegistrationOtp(email, otpCode);
  }

  /**
   * Resend OTP for any purpose
   */
  resendOtp(email: string, type: OtpType): Observable<any> {
    return this.otpService.resendOtp(email, type);
  }

  // Profile management methods
  updateCurrentUserProfile(updatedUser: User): void {
    this.currentUser = updatedUser;
    localStorage.setItem('user', JSON.stringify(updatedUser));
    this.loginStatusChange.next(true);
  }

  refreshCurrentUser(): Observable<AppResponse<User>> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${this.getToken()}`
    });
    
    return this.http.get<AppResponse<User>>(`${this.baseUrl}/user/profile`, { headers });
  }
}
