import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CartService, CartItem } from '../../services/cart.service';
import { OrderService } from '../../services/order.service';
import { AddressSelectionComponent } from '../address-selection/address-selection.component';
import { Address } from '../../models/address.model';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, AddressSelectionComponent],
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
  isCheckingOut = false;
  selectedPaymentMethod = 'Cash';
  paymentMethods = ['Cash', 'Credit Card', 'Debit Card', 'Online Payment'];
  
  // Address-related properties
  selectedAddress: Address | null = null;
  deliveryInstructions: string = '';
  showAddressSelection = true;

  constructor(
    private cartService: CartService,
    private orderService: OrderService,
    private router: Router
  ) {}

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

  onAddressSelected(address: Address): void {
    this.selectedAddress = address;
  }
  checkout(): void {
    if (this.cartItems.length === 0) {
      alert('Your cart is empty');
      return;
    }

    // Check if address is selected
    if (!this.selectedAddress) {
      alert('Please select a delivery address');
      return;
    }

    // Get restaurant ID from the first item (assuming all items are from the same restaurant)
    const restaurantId = this.cartItems[0].dish.restaurantId;
    
    // Validate that all items are from the same restaurant
    const differentRestaurant = this.cartItems.some(item => item.dish.restaurantId !== restaurantId);
    if (differentRestaurant) {
      alert('All items must be from the same restaurant');
      return;
    }

    this.isCheckingOut = true;

    // Convert cart to order with address information
    const orderData = this.orderService.cartToOrderCreate(
      this.cartItems, 
      restaurantId, 
      this.selectedPaymentMethod,
      this.selectedAddress.id,
      this.deliveryInstructions || undefined
    );

    this.orderService.createOrder(orderData).subscribe({
      next: (response) => {
        if (response.success) {
          alert('Order placed successfully!');
          this.cartService.clearCart();
          this.router.navigate(['/orders', response.data?.id]);
        } else {
          alert('Failed to place order: ' + response.errorMessage);
        }
        this.isCheckingOut = false;
      },
      error: (error) => {
        console.error('Error placing order:', error);
        alert('Failed to place order. Please try again.');
        this.isCheckingOut = false;
      }
    });
  }
}
