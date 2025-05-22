import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError, tap, throwError } from 'rxjs';
import { environment } from '../../environments/environment';
import { AppResponse } from '../models/app-response.model';

@Injectable({ providedIn: 'root' })
export class ImageUploadService {
  // Using a base API URL that can be configured in environment
  private baseUrl = environment.apiUrl || 'http://localhost:5236'; // Default to local API if not defined

  constructor(private http: HttpClient) {}

  uploadImage(file: File): Observable<{ url: string }> {
    const formData = new FormData();
    formData.append('file', file);    // Use proper URL construction and add error handling
    // Remove extra /api if baseUrl already includes it
    return this.http.post<{ url: string }>(`${this.baseUrl}/cloudinary/upload-image`, formData)
      .pipe(
        tap(response => console.log('Upload successful:', response)),
        catchError(this.handleError)
      );
  }

  uploadDishImage(file: File, dishId: number): Observable<AppResponse<string>> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<AppResponse<string>>(`${this.baseUrl}/api/cloudinary/upload-dish-image/${dishId}`, formData)
      .pipe(
        tap(response => console.log('Dish image upload successful:', response)),
        catchError(this.handleError)
      );
  }

  private handleError(error: HttpErrorResponse) {
    console.error('Upload error details:', error);

    let errorMessage = 'Unknown error occurred during upload';
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // Server-side error
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }

    return throwError(() => new Error(errorMessage));
  }
}
