import { HttpClient } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Restaurant } from '../models/restaurant.model';
import { AppResponse } from '../models/app-response.model';
import { Dish } from '../models/dish.model';
import { Category } from '../models/category.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RestaurantService {
  private baseUrl: string;

  constructor(private http: HttpClient) {
    this.baseUrl = `${environment.apiUrl}/user`;
  }  getAllRestaurants(): Observable<AppResponse<Restaurant[]>> {
    console.log(`Fetching restaurants from: ${this.baseUrl}/restaurants`);
    return this.http.get<AppResponse<Restaurant[]>>(`${this.baseUrl}/restaurants`)
      .pipe(
        catchError((error: any) => {
          console.error('Error fetching restaurants:', error);
          throw error;
        })
      );
  }
    getRestaurantDishes(restaurantId: number): Observable<AppResponse<Dish[]>> {
    console.log(`Fetching dishes from: ${this.baseUrl}/dishes/${restaurantId}`);
    return this.http.get<AppResponse<Dish[]>>(`${this.baseUrl}/dishes/${restaurantId}`)
      .pipe(
        catchError((error: any) => {
          console.error(`Error fetching dishes for restaurant ${restaurantId}:`, error);
          throw error;
        })
      );
  }

  getRestaurantCategories(restaurantId: number): Observable<AppResponse<Category[]>> {
    console.log(`Fetching categories from: ${this.baseUrl}/categories/${restaurantId}`);
    return this.http.get<AppResponse<Category[]>>(`${this.baseUrl}/categories/${restaurantId}`)
      .pipe(
        catchError((error: any) => {
          console.error(`Error fetching categories for restaurant ${restaurantId}:`, error);
          throw error;
        })
      );
  }
}
