import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Restaurant } from '../models/restaurant.model';
import { AppResponse } from '../models/app-response.model';
import { Dish } from '../models/dish.model';
import { environment } from '../../environments/environment';

export interface HomeStats {
  totalRestaurants: number;
  totalDishes: number;
  availableDishes: number;
  openRestaurants: number;
}

@Injectable({
  providedIn: 'root'
})
export class HomeService {
  private baseUrl: string;

  constructor(private http: HttpClient) {
    this.baseUrl = `${environment.apiUrl}/Home`;
  }

  /**
   * Get all restaurants for home page
   */
  getAllRestaurants(): Observable<AppResponse<Restaurant[]>> {
    console.log(`Fetching restaurants from: ${this.baseUrl}/restaurants`);
    return this.http.get<AppResponse<Restaurant[]>>(`${this.baseUrl}/restaurants`)
      .pipe(
        catchError((error: any) => {
          console.error('Error fetching restaurants:', error);
          throw error;
        })
      );
  }

  /**
   * Get all dishes for home page
   */
  getAllDishes(): Observable<AppResponse<Dish[]>> {
    console.log(`Fetching dishes from: ${this.baseUrl}/dishes`);
    return this.http.get<AppResponse<Dish[]>>(`${this.baseUrl}/dishes`)
      .pipe(
        catchError((error: any) => {
          console.error('Error fetching dishes:', error);
          throw error;
        })
      );
  }

  /**
   * Get featured restaurants (first 6)
   */
  getFeaturedRestaurants(): Observable<AppResponse<Restaurant[]>> {
    console.log(`Fetching featured restaurants from: ${this.baseUrl}/featured-restaurants`);
    return this.http.get<AppResponse<Restaurant[]>>(`${this.baseUrl}/featured-restaurants`)
      .pipe(
        catchError((error: any) => {
          console.error('Error fetching featured restaurants:', error);
          throw error;
        })
      );
  }

  /**
   * Get popular dishes (first 8 available)
   */
  getPopularDishes(): Observable<AppResponse<Dish[]>> {
    console.log(`Fetching popular dishes from: ${this.baseUrl}/popular-dishes`);
    return this.http.get<AppResponse<Dish[]>>(`${this.baseUrl}/popular-dishes`)
      .pipe(
        catchError((error: any) => {
          console.error('Error fetching popular dishes:', error);
          throw error;
        })
      );
  }

  /**
   * Get home page statistics
   */
  getHomeStats(): Observable<AppResponse<HomeStats>> {
    console.log(`Fetching home stats from: ${this.baseUrl}/stats`);
    return this.http.get<AppResponse<HomeStats>>(`${this.baseUrl}/stats`)
      .pipe(
        catchError((error: any) => {
          console.error('Error fetching home stats:', error);
          throw error;
        })
      );
  }
}
