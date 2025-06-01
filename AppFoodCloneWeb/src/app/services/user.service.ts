import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User, UserProfileUpdate, PasswordChangeRequest } from '../models/user.model';
import { AppResponse } from '../models/app-response.model';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private baseUrl = environment.apiUrl;

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {}

  private getAuthHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    });
  }

  // Get current user profile
  getUserProfile(): Observable<AppResponse<User>> {
    const headers = this.getAuthHeaders();
    return this.http.get<AppResponse<User>>(`${this.baseUrl}/user/profile`, { headers });
  }

  // Update user profile
  updateProfile(profileData: UserProfileUpdate): Observable<AppResponse<User>> {
    const headers = this.getAuthHeaders();
    return this.http.put<AppResponse<User>>(`${this.baseUrl}/user/profile`, profileData, { headers });
  }

  // Upload profile image
  uploadProfileImage(formData: FormData): Observable<AppResponse<{imageUrl: string}>> {
    const token = this.authService.getToken();
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
      // Don't set Content-Type for FormData, let the browser set it with boundary
    });
    return this.http.post<AppResponse<{imageUrl: string}>>(`${this.baseUrl}/user/profile/image`, formData, { headers });
  }

  // Change password
  changePassword(passwordData: PasswordChangeRequest): Observable<AppResponse<any>> {
    const headers = this.getAuthHeaders();
    return this.http.post<AppResponse<any>>(`${this.baseUrl}/user/change-password`, passwordData, { headers });
  }

  // Delete user account
  deleteAccount(): Observable<AppResponse<any>> {
    const headers = this.getAuthHeaders();
    return this.http.delete<AppResponse<any>>(`${this.baseUrl}/user/account`, { headers });
  }

  // Get user statistics
  getUserStatistics(): Observable<AppResponse<{
    totalOrders: number;
    favoriteRestaurants: number;
    reviewsWritten: number;
    savedAddresses: number;
  }>> {
    const headers = this.getAuthHeaders();
    return this.http.get<AppResponse<{
      totalOrders: number;
      favoriteRestaurants: number;
      reviewsWritten: number;
      savedAddresses: number;
    }>>(`${this.baseUrl}/user/statistics`, { headers });
  }
}
