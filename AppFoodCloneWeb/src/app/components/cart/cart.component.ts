import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CartService, CartItem } from '../../services/cart.service';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css'
})
export class CartComponent implements OnInit {
  cartItems: CartItem[] = [];
  totalPrice = 0;
  deliveryFee = 3.99;
  tax = 0;
  promoCode: string = '';
  promoDiscount = 0;

  constructor(private cartService: CartService) {}
  ngOnInit(): void {
    this.cartService.cart$.subscribe(items => {
      this.cartItems = items;
      this.calculateTotals();
    });
  }

  updateQuantity(dishId: number, quantity: number): void {
    if (quantity <= 0) {
      this.removeItem(dishId);
    } else {
      this.cartService.updateQuantity(dishId, quantity);
      this.calculateTotals();
    }
  }

  removeItem(dishId: number): void {
    this.cartService.removeFromCart(dishId);
    // The subscription in ngOnInit will handle the updates
  }

  calculateTotals(): void {
    this.totalPrice = this.cartService.getTotalPrice();

    // Calculate tax (let's assume 8.5%)
    this.tax = this.totalPrice * 0.085;

    // Adjust delivery fee based on total price
    if (this.totalPrice > 50) {
      this.deliveryFee = 0; // Free delivery for orders over $50
    } else if (this.totalPrice > 30) {
      this.deliveryFee = 1.99; // Reduced fee for orders over $30
    } else {
      this.deliveryFee = 3.99; // Standard delivery fee
    }

    // Apply promo discount if available
    if (this.promoDiscount > 0) {
      // Apply discount logic here
      // For example: this.totalPrice = this.totalPrice - this.promoDiscount;
    }
  }

  applyPromoCode(): void {
    if (this.promoCode.trim().toLowerCase() === 'welcome10') {
      // 10% discount
      this.promoDiscount = this.totalPrice * 0.1;
      alert('Promo code applied! You got 10% off.');
    } else if (this.promoCode.trim().toLowerCase() === 'free') {
      // Free delivery
      this.deliveryFee = 0;
      alert('Promo code applied! Free delivery.');
    } else {
      alert('Invalid promo code');
      this.promoDiscount = 0;
    }
    this.calculateTotals();
  }

  clearCart(): void {
    if (confirm('Are you sure you want to clear your cart?')) {
      this.cartService.clearCart();
    }
  }

  getGrandTotal(): number {
    return this.totalPrice + this.deliveryFee + this.tax - this.promoDiscount;
  }
}
