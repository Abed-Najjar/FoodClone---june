import { HttpClient } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { Restaurant } from '../models/restaurant.model';
import { AppResponse } from '../models/app-response.model';
import { Dish } from '../models/dish.model';
import { Category } from '../models/category.model';
import { PagedResult } from '../types/pagination.interface';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RestaurantService {
  private baseUrl: string;
  constructor(private http: HttpClient) {
    this.baseUrl = environment.apiUrl;
  }

  // Get all restaurants (this can use HomeController for consistency)
  getAllRestaurants(): Observable<AppResponse<Restaurant[]>> {
    console.log(`Fetching restaurants from: ${this.baseUrl}/Home/restaurants`);
    return this.http.get<AppResponse<PagedResult<Restaurant>>>(`${this.baseUrl}/Home/restaurants`)
      .pipe(
        map(response => {
          if (response.success && response.data) {
            // Extract the data array from PagedResult
            return {
              ...response,
              data: response.data.data
            } as AppResponse<Restaurant[]>;
          }
          return response as any;
        }),
        catchError((error: any) => {
          console.error('Error fetching restaurants:', error);
          throw error;
        })
      );
  }
  // Get dishes for a specific restaurant (uses public Home controller endpoint)
  getRestaurantDishes(restaurantId: number): Observable<AppResponse<Dish[]>> {
    const dishUrl = `${this.baseUrl}/Home/restaurants/${restaurantId}/dishes`;
    console.log(`Fetching dishes from: ${dishUrl}`);
    return this.http.get<AppResponse<PagedResult<Dish>>>(dishUrl)
      .pipe(
        map(response => {
          if (response.success && response.data) {
            // Extract the data array from PagedResult
            return {
              ...response,
              data: response.data.data
            } as AppResponse<Dish[]>;
          }
          return response as any;
        }),
        catchError((error: any) => {
          console.error(`Error fetching dishes for restaurant ${restaurantId}:`, error);
          throw error;
        })
      );
  }
  // Get categories for a specific restaurant (uses public Home controller endpoint)
  getRestaurantCategories(restaurantId: number): Observable<AppResponse<Category[]>> {
    const categoryUrl = `${this.baseUrl}/Home/restaurants/${restaurantId}/categories`;
    console.log(`Fetching categories from: ${categoryUrl}`);
    return this.http.get<AppResponse<PagedResult<Category>>>(categoryUrl)
      .pipe(
        map(response => {
          if (response.success && response.data) {
            // Extract the data array from PagedResult
            return {
              ...response,
              data: response.data.data
            } as AppResponse<Category[]>;
          }
          return response as any;
        }),
        catchError((error: any) => {
          console.error(`Error fetching categories for restaurant ${restaurantId}:`, error);
          throw error;
        })
      );
  }
}
