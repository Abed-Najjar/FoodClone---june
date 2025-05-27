import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map, switchMap, forkJoin, of, catchError } from 'rxjs';
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
    console.log('Getting all categories using CategoryManagement endpoint');
    // First get all restaurants, then get categories for each restaurant
    return this.getAllRestaurants().pipe(
      switchMap((restaurantsResponse: AppResponse<Restaurant[]>) => {
        if (!restaurantsResponse.success || !restaurantsResponse.data) {
          return of({
            data: [],
            errorMessage: restaurantsResponse.errorMessage || 'Failed to load restaurants',
            statusCode: restaurantsResponse.statusCode || 500,
            success: false
          } as AppResponse<Category[]>);
        }

        // Get categories for each restaurant using CategoryManagement endpoint
        const categoryRequests = restaurantsResponse.data.map(restaurant =>
          this.http.get<AppResponse<Category[]>>(`${environment.apiUrl}/CategoryManagement/categories/${restaurant.id}`)
        );

        if (categoryRequests.length === 0) {
          return of({
            data: [],
            errorMessage: '',
            statusCode: 200,
            success: true
          } as AppResponse<Category[]>);
        }

        // Execute all requests in parallel and combine results
        return forkJoin(categoryRequests).pipe(
          map((responses: AppResponse<Category[]>[]) => {
            const allCategories: Category[] = [];

            responses.forEach(response => {
              if (response.success && response.data) {
                allCategories.push(...response.data);
              }
            });

            console.log('All categories with restaurant info loaded:', allCategories);
            return {
              data: allCategories,
              errorMessage: '',
              statusCode: 200,
              success: true
            } as AppResponse<Category[]>;
          }),
          catchError((error: any) => {
            console.error('Error loading categories:', error);
            return of({
              data: [],
              errorMessage: 'Failed to load categories',
              statusCode: 500,
              success: false
            } as AppResponse<Category[]>);
          })
        );
      })
    );
  }
  createCategory(category: Category): Observable<AppResponse<Category>> {
    // Use the CategoryManagement controller endpoint
    const categoryManagementUrl = `${environment.apiUrl}/CategoryManagement/categories`;

    // Format data to match the CreateCategoryDto expected by the API
    const createCategoryDto = {
      name: category.name,
      description: category.description || '', // Ensure description is never null
      imageUrl: category.imageUrl,
      restaurantId: category.restaurantId
    };

    console.log('Creating category using endpoint:', categoryManagementUrl);
    console.log('Request payload:', createCategoryDto);

    return this.http.post<AppResponse<Category>>(categoryManagementUrl, createCategoryDto);
  }

  updateCategory(id: number, category: Category): Observable<AppResponse<Category>> {
    return this.http.put<AppResponse<Category>>(`${this.baseUrl}/categories/${id}`, category);
  }

  deleteCategory(id: number): Observable<AppResponse<boolean>> {
    return this.http.delete<AppResponse<boolean>>(`${this.baseUrl}/categories/${id}`);
  }

  // Method to get categories for a specific restaurant
  getCategoriesByRestaurant(restaurantId: number): Observable<AppResponse<Category[]>> {
    console.log(`Getting categories for restaurant ID: ${restaurantId}`);
    return this.http.get<AppResponse<Category[]>>(`${this.baseUrl}/categories/${restaurantId}`);
  }

  getDishesByRestaurant(restaurantId: number): Observable<AppResponse<Dish[]>> {
    console.log(`Getting dishes for restaurant ID: ${restaurantId}`);
    // Using DishManagement controller endpoint which already exists
    return this.http.get<AppResponse<Dish[]>>(`${environment.apiUrl}/DishManagement/restaurant/dishes/${restaurantId}`);
  }
  // Restaurants
  getAllRestaurants(id?: number): Observable<AppResponse<Restaurant[]>> {
    return this.http.get<AppResponse<Restaurant[]>>(`${this.baseUrl}/restaurants${id ? `/${id}` : ''}`);
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
    console.log('Getting all dishes using DishManagement endpoint');
    // First get all restaurants, then get dishes for each restaurant
    return this.getAllRestaurants().pipe(
      switchMap((restaurantsResponse: AppResponse<Restaurant[]>) => {
        if (!restaurantsResponse.success || !restaurantsResponse.data) {
          return of({
            data: [],
            errorMessage: restaurantsResponse.errorMessage || 'Failed to load restaurants',
            statusCode: restaurantsResponse.statusCode || 500,
            success: false
          } as AppResponse<Dish[]>);
        }

        // Get dishes for each restaurant using DishManagement endpoint
        const dishRequests = restaurantsResponse.data.map(restaurant =>
          this.http.get<AppResponse<Dish[]>>(`${environment.apiUrl}/DishManagement/restaurant/dishes/${restaurant.id}`)
        );

        if (dishRequests.length === 0) {
          return of({
            data: [],
            errorMessage: '',
            statusCode: 200,
            success: true
          } as AppResponse<Dish[]>);
        }

        // Execute all requests in parallel and combine results
        return forkJoin(dishRequests).pipe(
          map((responses: AppResponse<Dish[]>[]) => {
            const allDishes: Dish[] = [];

            responses.forEach(response => {
              if (response.success && response.data) {
                allDishes.push(...response.data);
              }
            });

            console.log('All dishes with restaurant info loaded:', allDishes);
            return {
              data: allDishes,
              errorMessage: '',
              statusCode: 200,
              success: true
            } as AppResponse<Dish[]>;
          }),
          catchError((error: any) => {
            console.error('Error loading dishes:', error);
            return of({
              data: [],
              errorMessage: 'Failed to load dishes',
              statusCode: 500,
              success: false
            } as AppResponse<Dish[]>);
          })        );
      })
    );
  }

  createDish(dish: Dish): Observable<AppResponse<Dish>> {
    // Use the DishManagement controller endpoint
    const dishManagementUrl = `${environment.apiUrl}/DishManagement/dishes`;

    // Format data to match the CreateDishDto expected by the API
    const createDishDto = {
      name: dish.name,
      description: dish.description || '', // Ensure description is never null
      price: dish.price,
      imageUrl: dish.imageUrl,
      restaurantId: dish.restaurantId,
      categoryId: dish.categoryId
    };

    console.log('Creating dish using endpoint:', dishManagementUrl);
    console.log('Request payload:', createDishDto);

    return this.http.post<AppResponse<Dish>>(dishManagementUrl, createDishDto);
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
