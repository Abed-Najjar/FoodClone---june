import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AppResponse } from '../models/app-response.model';
import { Order, OrderCreate } from '../models/order.model';
import { CartItem } from './cart.service';
import { environment } from '../../environments/environment';
import { PaginationParams, PagedResult } from '../types/pagination.interface';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private baseUrl = `${environment.apiUrl}/order`;

  constructor(private http: HttpClient) { }

  createOrder(orderData: OrderCreate): Observable<AppResponse<Order>> {
    return this.http.post<AppResponse<Order>>(`${this.baseUrl}`, orderData);
  }

  getMyOrders(pagination?: PaginationParams): Observable<AppResponse<PagedResult<Order>>> {
    let params = new HttpParams();
    if (pagination) {
      params = params.set('pageNumber', pagination.pageNumber.toString());
      params = params.set('pageSize', pagination.pageSize.toString());
    }
    
    return this.http.get<AppResponse<PagedResult<Order>>>(`${this.baseUrl}/my-orders`, { params });
  }

  getOrderById(orderId: number): Observable<AppResponse<Order>> {
    return this.http.get<AppResponse<Order>>(`${this.baseUrl}/${orderId}`);
  }

  updateOrderStatus(orderId: number, status: string): Observable<AppResponse<boolean>> {
    return this.http.put<AppResponse<boolean>>(`${this.baseUrl}/${orderId}/status`, { status });
  }
  // Convert cart items to order create DTO
  cartToOrderCreate(
    cartItems: CartItem[], 
    restaurantId: number, 
    paymentMethod: string = 'Cash',
    deliveryAddressId?: number,
    deliveryInstructions?: string
  ): OrderCreate {
    return {
      restaurantId: restaurantId,
      paymentMethod: paymentMethod,
      deliveryAddressId: deliveryAddressId,
      deliveryInstructions: deliveryInstructions,
      orderItems: cartItems.map(item => ({
        dishId: item.dish.id,
        quantity: item.quantity
      }))
    };
  }

  // Calculate order totals (client-side preview)
  calculateOrderTotals(cartItems: CartItem[], deliveryFee: number = 5.00, taxRate: number = 0.15) {
    const subtotal = cartItems.reduce((total, item) => total + (item.dish.price * item.quantity), 0);
    const tax = subtotal * taxRate;
    const total = subtotal + deliveryFee + tax;

    return {
      subtotal: Number(subtotal.toFixed(2)),
      tax: Number(tax.toFixed(2)),
      deliveryFee: Number(deliveryFee.toFixed(2)),
      total: Number(total.toFixed(2))
    };
  }
}
