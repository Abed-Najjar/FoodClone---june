import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AppResponse } from '../models/app-response.model';
import { Order, OrderCreate } from '../models/order.model';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private baseUrl = 'http://localhost:5000/api/user';

  constructor(private http: HttpClient) { }

  createOrder(orderData: OrderCreate): Observable<AppResponse<Order>> {
    return this.http.post<AppResponse<Order>>(`${this.baseUrl}/orders`, orderData);
  }

  // You can add more methods for getting user orders history etc.
}
