import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Restaurant } from '../models/restaurant.model';
import { AppResponse } from '../models/app-response.model';
import { Dish } from '../models/dish.model';
import { environment } from '../../environments/environment';
import { PaginationParams, PagedResult } from '../types/pagination.interface';

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
  getAllRestaurants(pagination?: PaginationParams): Observable<AppResponse<PagedResult<Restaurant>>> {
    console.log(`Fetching restaurants from: ${this.baseUrl}/restaurants`);
    
    let params = new HttpParams();
    if (pagination) {
      params = params.set('pageNumber', pagination.pageNumber.toString());
      params = params.set('pageSize', pagination.pageSize.toString());
    }
    
    return this.http.get<AppResponse<PagedResult<Restaurant>>>(`${this.baseUrl}/restaurants`, { params })
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
  getAllDishes(pagination?: PaginationParams): Observable<AppResponse<PagedResult<Dish>>> {
    console.log(`Fetching dishes from: ${this.baseUrl}/dishes`);
    
    let params = new HttpParams();
    if (pagination) {
      params = params.set('pageNumber', pagination.pageNumber.toString());
      params = params.set('pageSize', pagination.pageSize.toString());
    }
    
    return this.http.get<AppResponse<PagedResult<Dish>>>(`${this.baseUrl}/dishes`, { params })
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
  getFeaturedRestaurants(pagination?: PaginationParams): Observable<AppResponse<PagedResult<Restaurant>>> {
    console.log(`Fetching featured restaurants from: ${this.baseUrl}/featured-restaurants`);
    
    let params = new HttpParams();
    if (pagination) {
      params = params.set('pageNumber', pagination.pageNumber.toString());
      params = params.set('pageSize', pagination.pageSize.toString());
    }
    
    return this.http.get<AppResponse<PagedResult<Restaurant>>>(`${this.baseUrl}/featured-restaurants`, { params })
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
  getPopularDishes(pagination?: PaginationParams): Observable<AppResponse<PagedResult<Dish>>> {
    console.log(`Fetching popular dishes from: ${this.baseUrl}/popular-dishes`);
    
    let params = new HttpParams();
    if (pagination) {
      params = params.set('pageNumber', pagination.pageNumber.toString());
      params = params.set('pageSize', pagination.pageSize.toString());
    }
    
    return this.http.get<AppResponse<PagedResult<Dish>>>(`${this.baseUrl}/popular-dishes`, { params })
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
