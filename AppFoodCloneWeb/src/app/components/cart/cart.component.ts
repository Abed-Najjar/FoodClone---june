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

  constructor(private cartService: CartService) {}

  ngOnInit(): void {
    this.cartService.cart$.subscribe(items => {
      this.cartItems = items;
      this.calculateTotal();
    });
  }

  updateQuantity(dishId: number, quantity: number): void {
    if (quantity <= 0) {
      this.removeItem(dishId);
    } else {
      this.cartService.updateQuantity(dishId, quantity);
      this.calculateTotal();
    }
  }

  removeItem(dishId: number): void {
    this.cartService.removeFromCart(dishId);
  }

  calculateTotal(): void {
    this.totalPrice = this.cartService.getTotalPrice();
  }

  clearCart(): void {
    this.cartService.clearCart();
  }
}
