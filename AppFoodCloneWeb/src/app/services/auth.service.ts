import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { User, UserLogin, UserRegister } from '../models/user.model';
import { AppResponse } from '../models/app-response.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = environment.apiUrl;
  private currentUser: User | null = null;
  public loginStatusChange = new Subject<boolean>();

  constructor(private http: HttpClient) {
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
}
