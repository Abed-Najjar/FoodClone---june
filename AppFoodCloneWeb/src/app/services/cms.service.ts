import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { AppResponse } from '../models/app-response.model';
import { Category } from '../models/category.model';
import { Restaurant } from '../models/restaurant.model';
import { Dish } from '../models/dish.model';
import { User } from '../models/user.model';
import { Order } from '../models/order.model';

@Injectable({
  providedIn: 'root'
})
export class CmsService {
  private baseUrl = `${environment.apiUrl}/cms`;

  constructor(private http: HttpClient) { }

  // Categories
  getAllCategories(): Observable<AppResponse<Category[]>> {
    return this.http.get<AppResponse<Category[]>>(`${this.baseUrl}/categories`);
  }

  createCategory(category: Category): Observable<AppResponse<Category>> {
    return this.http.post<AppResponse<Category>>(`${this.baseUrl}/categories`, category);
  }

  updateCategory(id: number, category: Category): Observable<AppResponse<Category>> {
    return this.http.put<AppResponse<Category>>(`${this.baseUrl}/categories/${id}`, category);
  }

  deleteCategory(id: number): Observable<AppResponse<boolean>> {
    return this.http.delete<AppResponse<boolean>>(`${this.baseUrl}/categories/${id}`);
  }

  // Restaurants
  getAllRestaurants(): Observable<AppResponse<Restaurant[]>> {
    return this.http.get<AppResponse<Restaurant[]>>(`${this.baseUrl}/restaurants`);
  }

  createRestaurant(restaurant: any): Observable<AppResponse<Restaurant>> {
    return this.http.post<AppResponse<Restaurant>>(`${this.baseUrl}/restaurants`, restaurant);
  }
  updateRestaurant(id: number, restaurant: any): Observable<AppResponse<Restaurant>> {
    console.log(`Sending update to: ${this.baseUrl}/restaurants/${id}`);
    console.log('Request data:', restaurant);
    return this.http.put<AppResponse<Restaurant>>(`${this.baseUrl}/restaurants/${id}`, restaurant);
  }

  deleteRestaurant(id: number): Observable<AppResponse<boolean>> {
    return this.http.delete<AppResponse<boolean>>(`${this.baseUrl}/restaurants/${id}`);
  }
  // Dishes
  getAllDishes(): Observable<AppResponse<Dish[]>> {
    return this.http.get<AppResponse<Dish[]>>(`${this.baseUrl}/dishes`);
  }
  createDish(dish: Dish): Observable<AppResponse<Dish>> {
    console.log('Sending request to:', `${this.baseUrl}/create-dish`);
    console.log('Request payload:', dish);
    return this.http.post<AppResponse<Dish>>(`${this.baseUrl}/create-dish`, dish);
  }

  updateDish(id: number, dish: Dish): Observable<AppResponse<Dish>> {
    return this.http.put<AppResponse<Dish>>(`${this.baseUrl}/dishes/${id}`, dish);
  }

  deleteDish(id: number): Observable<AppResponse<boolean>> {
    return this.http.delete<AppResponse<boolean>>(`${this.baseUrl}/dishes/${id}`);
  }

  // Users
  getAllUsers(): Observable<AppResponse<User[]>> {
    return this.http.get<AppResponse<User[]>>(`${this.baseUrl}/users`);
  }

  // Orders
  getAllOrders(): Observable<AppResponse<Order[]>> {
    return this.http.get<AppResponse<Order[]>>(`${this.baseUrl}/orders`);
  }
}
