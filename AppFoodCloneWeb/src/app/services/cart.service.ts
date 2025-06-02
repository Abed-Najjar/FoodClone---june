import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { Dish } from '../models/dish.model';
import { 
  CartCalculationRequest, 
  CartCalculationResponse, 
  ValidatePromoCodeRequest, 
  PromoCode, 
  AppResponse 
} from '../models/cart.model';
import { environment } from '../../environments/environment';

export interface CartItem {
  dish: Dish;
  quantity: number;
}

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private cart = new BehaviorSubject<CartItem[]>([]);
  public cart$ = this.cart.asObservable();
  private baseUrl = `${environment.apiUrl}/cart`;

  constructor(private http: HttpClient) {
    // Load cart from localStorage if available
    const savedCart = localStorage.getItem('cart');
    if (savedCart) {
      this.cart.next(JSON.parse(savedCart));
    }
  }

  addToCart(dish: Dish, quantity: number = 1): void {
    const currentCart = this.cart.getValue();
    const existingItem = currentCart.find(item => item.dish.id === dish.id);
    
    let updatedCart: CartItem[];
    
    if (existingItem) {
      // Update quantity of existing item
      updatedCart = currentCart.map(item => 
        item.dish.id === dish.id 
          ? { ...item, quantity: item.quantity + quantity } 
          : item
      );
    } else {
      // Add new item
      updatedCart = [...currentCart, { dish, quantity }];
    }
    
    this.cart.next(updatedCart);
    localStorage.setItem('cart', JSON.stringify(updatedCart));
  }

  removeFromCart(dishId: number): void {
    const currentCart = this.cart.getValue();
    const updatedCart = currentCart.filter(item => item.dish.id !== dishId);
    
    this.cart.next(updatedCart);
    localStorage.setItem('cart', JSON.stringify(updatedCart));
  }

  updateQuantity(dishId: number, quantity: number): void {
    const currentCart = this.cart.getValue();
    const updatedCart = currentCart.map(item => 
      item.dish.id === dishId ? { ...item, quantity } : item
    );
    
    this.cart.next(updatedCart);
    localStorage.setItem('cart', JSON.stringify(updatedCart));
  }

  clearCart(): void {
    this.cart.next([]);
    localStorage.removeItem('cart');
  }

  getCartItems(): CartItem[] {
    return this.cart.getValue();
  }

  getTotalItems(): number {
    return this.cart.getValue().reduce(
      (total, item) => total + item.quantity, 
      0
    );
  }

  // Get restaurant ID from cart items
  getRestaurantId(): number | null {
    const cartItems = this.cart.getValue();
    if (cartItems.length === 0) return null;
    return cartItems[0].dish.restaurantId;
  }

  // Check if cart has items from different restaurants
  hasItemsFromDifferentRestaurants(): boolean {
    const cartItems = this.cart.getValue();
    if (cartItems.length <= 1) return false;
    
    const firstRestaurantId = cartItems[0].dish.restaurantId;
    return cartItems.some(item => item.dish.restaurantId !== firstRestaurantId);
  }

  // Backend API calls for calculations (REPLACES ALL FRONTEND CALCULATIONS)
  calculateCartTotals(promoCode?: string, deliveryAddressId?: number): Observable<AppResponse<CartCalculationResponse>> {
    const cartItems = this.cart.getValue();
    const restaurantId = this.getRestaurantId();
    
    if (!restaurantId || cartItems.length === 0) {
      return throwError(() => new Error('Cart is empty or restaurant not found'));
    }

    // Check for items from different restaurants
    if (this.hasItemsFromDifferentRestaurants()) {
      return throwError(() => new Error('Cart contains items from different restaurants'));
    }

    const request: CartCalculationRequest = {
      cartItems: cartItems.map(item => ({
        dishId: item.dish.id,
        quantity: item.quantity
      })),
      restaurantId: restaurantId,
      promoCode: promoCode,
      deliveryAddressId: deliveryAddressId
    };

    console.log('🛒 Cart calculation request:', request);
    console.log('🌐 API URL:', `${this.baseUrl}/calculate`);

    return this.http.post<AppResponse<CartCalculationResponse>>(`${this.baseUrl}/calculate`, request)
      .pipe(
        tap(response => {
          console.log('✅ Cart calculation response:', response);
          if (response.success && response.data) {
            console.log(`💰 Totals - Subtotal: ${response.data.formattedSubtotal}, Delivery: ${response.data.formattedDeliveryFee}, Tax: ${response.data.formattedTaxAmount}, Total: ${response.data.formattedGrandTotal}`);
          }
        }),
        catchError(this.handleError)
      );
  }

  validatePromoCode(promoCode: string, subtotal: number): Observable<AppResponse<PromoCode>> {
    const request: ValidatePromoCodeRequest = {
      promoCode: promoCode,
      subtotal: subtotal
    };

    console.log('🎫 Promo validation request:', request);

    return this.http.post<AppResponse<PromoCode>>(`${this.baseUrl}/validate-promo`, request)
      .pipe(
        tap(response => {
          console.log('✅ Promo validation response:', response);
        }),
        catchError(this.handleError)
      );
  }

  getAvailablePromoCodes(subtotal: number = 0): Observable<AppResponse<PromoCode[]>> {
    console.log('🎟️ Getting available promo codes for subtotal:', subtotal);

    return this.http.get<AppResponse<PromoCode[]>>(`${this.baseUrl}/promo-codes?subtotal=${subtotal}`)
      .pipe(
        tap(response => {
          console.log('✅ Available promo codes:', response);
        }),
        catchError(this.handleError)
      );
  }

  // Health check for cart service
  checkCartServiceHealth(): Observable<AppResponse<any>> {
    return this.http.get<AppResponse<any>>(`${this.baseUrl}/health`)
      .pipe(
        tap(response => {
          console.log('❤️ Cart service health:', response);
        }),
        catchError(this.handleError)
      );
  }

  private handleError = (error: HttpErrorResponse) => {
    console.error('🚨 Cart service error:', error);
    
    let errorMessage = 'An error occurred while processing your request.';
    
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Client Error: ${error.error.message}`;
    } else {
      // Server-side error
      if (error.status === 0) {
        errorMessage = 'Unable to connect to the server. Please check your internet connection.';
      } else if (error.status === 404) {
        errorMessage = 'Cart service not found. Please try again later.';
      } else if (error.status >= 500) {
        errorMessage = 'Server error. Please try again later.';
      } else {
        errorMessage = `Error (${error.status}): ${error.message}`;
        
        // Try to get more specific error message from response
        if (error.error && typeof error.error === 'string') {
          errorMessage = error.error;
        } else if (error.error && error.error.errorMessage) {
          errorMessage = error.error.errorMessage;
        } else if (error.error && error.error.message) {
          errorMessage = error.error.message;
        }
      }
    }
    
    console.error('❌ Processed error message:', errorMessage);
    return throwError(() => new Error(errorMessage));
  };
}
