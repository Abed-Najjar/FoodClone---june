import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Dish } from '../models/dish.model';

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

  constructor() {
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

  getTotalPrice(): number {
    return this.cart.getValue().reduce(
      (total, item) => total + (item.dish.price * item.quantity), 
      0
    );
  }

  getTotalItems(): number {
    return this.cart.getValue().reduce(
      (total, item) => total + item.quantity, 
      0
    );
  }
}
