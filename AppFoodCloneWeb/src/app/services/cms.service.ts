import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map, switchMap, forkJoin, of, catchError } from 'rxjs';
import { environment } from '../../environments/environment';
import { AppResponse } from '../models/app-response.model';
import { Category } from '../models/category.model';
import { Restaurant } from '../models/restaurant.model';
import { Dish } from '../models/dish.model';
import { User } from '../models/user.model';
import { Order } from '../models/order.model';
import { PaginationParams, PagedResult } from '../types/pagination.interface';

@Injectable({
  providedIn: 'root'
})
export class CmsService {
  private baseUrl = `${environment.apiUrl}/cms`;

  constructor(private http: HttpClient) { }
  // Categories
  getAllCategories(pagination?: PaginationParams): Observable<AppResponse<PagedResult<Category>>> {
    console.log('Getting all categories using CMS endpoint');
    
    let params = new HttpParams();
    if (pagination) {
      params = params.set('pageNumber', pagination.pageNumber.toString());
      params = params.set('pageSize', pagination.pageSize.toString());
    }
    
    return this.http.get<AppResponse<PagedResult<Category>>>(`${this.baseUrl}/categories`, { params });
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
    // The backend returns PagedResultDto but we want the data array directly
    return this.http.get<AppResponse<PagedResult<Category>>>(`${this.baseUrl}/categories/${restaurantId}`)
      .pipe(
        map(response => {
          if (response.success && response.data) {
            // Extract the data array from PagedResultDto
            return {
              ...response,
              data: response.data.data as any
            } as AppResponse<Category[]>;
          }
          return response as any;
        })
      );
  }

  getDishesByRestaurant(restaurantId: number): Observable<AppResponse<Dish[]>> {
    console.log(`Getting dishes for restaurant ID: ${restaurantId}`);
    // Using DishManagement controller endpoint which already exists - returns PagedResultDto
    return this.http.get<AppResponse<PagedResult<Dish>>>(`${environment.apiUrl}/DishManagement/restaurant/dishes/${restaurantId}`)
      .pipe(
        map(response => {
          if (response.success && response.data) {
            // Extract the data array from PagedResultDto
            return {
              ...response,
              data: response.data.data as any
            } as AppResponse<Dish[]>;
          }
          return response as any;
        })
      );
  }  // Restaurants
  getAllRestaurants(pagination?: PaginationParams): Observable<AppResponse<PagedResult<Restaurant>>> {
    let params = new HttpParams();
    if (pagination) {
      params = params.set('pageNumber', pagination.pageNumber.toString());
      params = params.set('pageSize', pagination.pageSize.toString());
    }
    
    return this.http.get<AppResponse<PagedResult<Restaurant>>>(`${this.baseUrl}/restaurants`, { params });
  }

  getRestaurantById(id: number): Observable<AppResponse<Restaurant>> {
    return this.http.get<AppResponse<Restaurant>>(`${environment.apiUrl}/RestaurantManagement/get/${id}`);
  }

  createRestaurant(restaurant: any): Observable<AppResponse<Restaurant>> {
    return this.http.post<AppResponse<Restaurant>>(`${environment.apiUrl}/RestaurantManagement/create`, restaurant);
  }
  updateRestaurant(id: number, restaurant: any): Observable<AppResponse<Restaurant>> {
    console.log(`Sending update to: ${environment.apiUrl}/RestaurantManagement/update/${id}`);
    console.log('Request data:', restaurant);
    return this.http.put<AppResponse<Restaurant>>(`${environment.apiUrl}/RestaurantManagement/update/${id}`, restaurant);
  }
  deleteRestaurant(id: number): Observable<AppResponse<boolean>> {
    return this.http.delete<AppResponse<boolean>>(`${environment.apiUrl}/RestaurantManagement/delete/${id}`);
  }

  // Dishes
  getAllDishes(pagination?: PaginationParams): Observable<AppResponse<PagedResult<Dish>>> {
    console.log('Getting all dishes using CMS endpoint');
    
    let params = new HttpParams();
    if (pagination) {
      params = params.set('pageNumber', pagination.pageNumber.toString());
      params = params.set('pageSize', pagination.pageSize.toString());
    }
    
    return this.http.get<AppResponse<PagedResult<Dish>>>(`${this.baseUrl}/dishes`, { params });
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
  }  updateDish(id: number, dish: Dish): Observable<AppResponse<Dish>> {
    // Use the DishManagement controller endpoint
    const dishManagementUrl = `${environment.apiUrl}/DishManagement/dishes/${id}`;
    
    // Format data to match the UpdateDishDto expected by the API
    const updateDishDto = {
      id: id, // Include the ID as required by UpdateDishDto
      name: dish.name,
      description: dish.description || '', // Ensure description is never null
      price: dish.price,
      imageUrl: dish.imageUrl,
      restaurantId: dish.restaurantId, // Include restaurantId as required by UpdateDishDto
      isAvailable: dish.isAvailable,
      categoryId: dish.categoryId
    };    console.log('Updating dish using endpoint:', dishManagementUrl);
    console.log('Request payload:', updateDishDto);
    console.log('Request payload JSON:', JSON.stringify(updateDishDto, null, 2));
    
    // Check if user is authenticated
    const authService = this.http; // We'll inject AuthService separately if needed
    console.log('Making authenticated request to update dish...');
    return this.http.put<AppResponse<Dish>>(dishManagementUrl, updateDishDto).pipe(
      catchError((error: any) => {
        console.error('HTTP error in updateDish:', error);
        console.error('Error status:', error.status);
        console.error('Error message:', error.message);
        console.error('Error response body:', error.error);
        console.error('Full error object:', JSON.stringify(error, null, 2));
        throw error; // Re-throw so component can handle it
      })
    );
  }
  deleteDish(id: number): Observable<AppResponse<boolean>> {
    // Use the DishManagement controller endpoint
    const dishManagementUrl = `${environment.apiUrl}/DishManagement/dishes/${id}`;
    console.log('Deleting dish using endpoint:', dishManagementUrl);
    return this.http.delete<AppResponse<boolean>>(dishManagementUrl);
  }

  // Users
  getAllUsers(pagination?: PaginationParams): Observable<AppResponse<PagedResult<User>>> {
    let params = new HttpParams();
    if (pagination) {
      params = params.set('pageNumber', pagination.pageNumber.toString());
      params = params.set('pageSize', pagination.pageSize.toString());
    }
    
    return this.http.get<AppResponse<PagedResult<User>>>(`${this.baseUrl}/users`, { params });
  }

  // Get all users including admins using UserManagement endpoint
  getAllUsersWithAllRoles(pagination?: PaginationParams): Observable<AppResponse<PagedResult<User>>> {
    let params = new HttpParams();
    if (pagination) {
      params = params.set('pageNumber', pagination.pageNumber.toString());
      params = params.set('pageSize', pagination.pageSize.toString());
    }
    
    return this.http.get<AppResponse<PagedResult<User>>>(`${environment.apiUrl}/UserManagement/getAllIncludingAdmins`, { params });
  }

  updateUser(id: number, user: any): Observable<AppResponse<User>> {
    return this.http.put<AppResponse<User>>(`${environment.apiUrl}/UserManagement/update/${id}`, user);
  }

  deleteUser(id: number): Observable<AppResponse<boolean>> {
    return this.http.delete<AppResponse<boolean>>(`${environment.apiUrl}/UserManagement/delete/${id}`);
  }

  getUserById(id: number): Observable<AppResponse<User>> {
    return this.http.get<AppResponse<User>>(`${environment.apiUrl}/UserManagement/get/${id}`);
  }

  createUser(user: any): Observable<AppResponse<User>> {
    return this.http.post<AppResponse<User>>(`${environment.apiUrl}/UserManagement/create`, user);
  }

  toggleUserStatus(userId: number): Observable<AppResponse<boolean>> {
    return this.http.patch<AppResponse<boolean>>(`${environment.apiUrl}/UserManagement/toggleStatus/${userId}`, {});
  }

  // Orders
  getAllOrders(pagination?: PaginationParams): Observable<AppResponse<PagedResult<Order>>> {
    let params = new HttpParams();
    if (pagination) {
      params = params.set('pageNumber', pagination.pageNumber.toString());
      params = params.set('pageSize', pagination.pageSize.toString());
    }
    
    return this.http.get<AppResponse<PagedResult<Order>>>(`${this.baseUrl}/orders`, { params });
  }

  updateOrderStatus(orderId: number, status: string): Observable<AppResponse<boolean>> {
    return this.http.put<AppResponse<boolean>>(`${this.baseUrl}/orders/${orderId}/status`, JSON.stringify(status), {
      headers: { 'Content-Type': 'application/json' }
    });
  }

  deleteOrder(orderId: number): Observable<AppResponse<boolean>> {
    return this.http.delete<AppResponse<boolean>>(`${this.baseUrl}/orders/${orderId}`);
  }

  getOrderById(orderId: number): Observable<AppResponse<Order>> {
    return this.http.get<AppResponse<Order>>(`${this.baseUrl}/orders/${orderId}`);
  }
}
