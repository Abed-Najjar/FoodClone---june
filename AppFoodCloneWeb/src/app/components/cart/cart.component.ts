import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CartService, CartItem } from '../../services/cart.service';
import { OrderService } from '../../services/order.service';
import { AddressSelectionComponent } from '../address-selection/address-selection.component';
import { Address } from '../../models/address.model';
import { CartCalculationResponse, PromoCode } from '../../models/cart.model';
import { Subject, takeUntil, debounceTime, distinctUntilChanged } from 'rxjs';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, AddressSelectionComponent],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css'
})
export class CartComponent implements OnInit, OnDestroy {
  cartItems: CartItem[] = [];
  isCheckingOut = false;
  selectedPaymentMethod = 'Cash';
  paymentMethods = ['Cash', 'Credit Card', 'Debit Card', 'Online Payment'];
  
  // Address-related properties
  selectedAddress: Address | null = null;
  deliveryInstructions: string = '';
  showAddressSelection = true;

  // Backend calculation response (REPLACES ALL FRONTEND CALCULATIONS)
  cartCalculation: CartCalculationResponse | null = null;
  isLoadingCalculation = false;
  calculationError: string | null = null;

  // Promo code handling
  promoCode: string = '';
  isValidatingPromo = false;
  promoError: string | null = null;
  promoCodeSubject = new Subject<string>();

  private destroy$ = new Subject<void>();

  constructor(
    private cartService: CartService,
    private orderService: OrderService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Subscribe to cart changes
    this.cartService.cart$.pipe(
      takeUntil(this.destroy$)
    ).subscribe(items => {
      this.cartItems = items;
      this.calculateCartTotals(); // Call backend calculation
    });

    // Handle promo code validation with debouncing
    this.promoCodeSubject.pipe(
      debounceTime(500),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(promoCode => {
      if (promoCode && promoCode.trim()) {
        this.validatePromoCode(promoCode);
      } else {
        this.calculateCartTotals(); // Recalculate without promo
      }
    });

    // Initial calculation
    this.calculateCartTotals();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  updateQuantity(dishId: number, quantity: number): void {
    if (quantity <= 0) {
      this.removeItem(dishId);
    } else {
      this.cartService.updateQuantity(dishId, quantity);
      // Cart subscription will trigger recalculation
    }
  }

  removeItem(dishId: number): void {
    this.cartService.removeFromCart(dishId);
    // The subscription in ngOnInit will handle the updates
  }

  // REMOVED ALL FRONTEND CALCULATIONS - Now calls backend
  public calculateCartTotals(): void {
    if (this.cartItems.length === 0) {
      this.cartCalculation = null;
      this.calculationError = null;
      return;
    }

    this.isLoadingCalculation = true;
    this.calculationError = null;

    console.log('🔄 Calculating cart totals via backend...');

    this.cartService.calculateCartTotals(
      this.promoCode ? this.promoCode.trim() : undefined,
      this.selectedAddress?.id
    ).pipe(
      takeUntil(this.destroy$)
    ).subscribe({
      next: (response) => {
        this.isLoadingCalculation = false;
        if (response.success && response.data) {
          this.cartCalculation = {
            ...response.data,
            // Add formatted versions for display
            formattedSubtotal: `${response.data.subtotal.toFixed(2)} ${response.data.currency}`,
            formattedDeliveryFee: `${response.data.deliveryFee.toFixed(2)} ${response.data.currency}`,
            formattedTaxAmount: `${response.data.taxAmount.toFixed(2)} ${response.data.currency}`,
            formattedGrandTotal: `${response.data.grandTotal.toFixed(2)} ${response.data.currency}`,
            itemDetails: response.data.itemDetails.map(item => ({
              ...item,
              formattedUnitPrice: `${item.unitPrice.toFixed(2)} ${response.data.currency}`,
              formattedTotalPrice: `${item.totalPrice.toFixed(2)} ${response.data.currency}`
            }))
          };
          console.log('✅ Cart calculation successful:', this.cartCalculation);
        } else {
          this.calculationError = response.errorMessage || 'Failed to calculate cart totals';
          console.error('❌ Cart calculation failed:', response.errorMessage);
        }
      },
      error: (error) => {
        this.isLoadingCalculation = false;
        this.calculationError = error.message || 'Error calculating cart totals';
        console.error('🚨 Cart calculation error:', error);
      }
    });
  }

  // Promo code handling
  onPromoCodeChange(): void {
    this.promoError = null;
    this.promoCodeSubject.next(this.promoCode);
  }

  private validatePromoCode(promoCode: string): void {
    if (!this.cartCalculation) {
      this.calculateCartTotals();
      return;
    }

    this.isValidatingPromo = true;
    this.promoError = null;

    console.log('🎫 Validating promo code:', promoCode);

    this.cartService.validatePromoCode(promoCode, this.cartCalculation.subtotal)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.isValidatingPromo = false;
          if (response.success) {
            console.log('✅ Promo code valid, recalculating totals...');
            this.calculateCartTotals(); // Recalculate with promo
          } else {
            this.promoError = response.errorMessage || 'Invalid promo code';
            console.warn('⚠️ Promo validation failed:', response.errorMessage);
          }
        },
        error: (error) => {
          this.isValidatingPromo = false;
          this.promoError = error.message || 'Error validating promo code';
          console.error('🚨 Promo validation error:', error);
        }
      });
  }

  clearPromoCode(): void {
    this.promoCode = '';
    this.promoError = null;
    this.calculateCartTotals(); // Recalculate without promo
  }

  clearCart(): void {
    if (confirm('Are you sure you want to clear your cart?')) {
      this.cartService.clearCart();
    }
  }

  // Getters for display (using backend calculation data)
  get subtotal(): number {
    return this.cartCalculation?.subtotal || 0;
  }

  get deliveryFee(): number {
    return this.cartCalculation?.deliveryFee || 0;
  }

  get tax(): number {
    return this.cartCalculation?.taxAmount || 0;
  }

  get discountAmount(): number {
    return this.cartCalculation?.discountAmount || 0;
  }

  get grandTotal(): number {
    return this.cartCalculation?.grandTotal || 0;
  }

  get hasDiscount(): boolean {
    return (this.cartCalculation?.discountAmount || 0) > 0;
  }

  get hasFreeDelivery(): boolean {
    return this.cartCalculation?.freeDeliveryApplied || false;
  }

  get appliedPromoCode(): string | null {
    return this.cartCalculation?.promoCodeApplied || null;
  }

  get promoMessage(): string | null {
    return this.cartCalculation?.promoCodeMessage || null;
  }

  onAddressSelected(address: Address): void {
    this.selectedAddress = address;
    // Recalculate totals with new address (delivery fee might change)
    this.calculateCartTotals();
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

    // Ensure we have valid cart calculation
    if (!this.cartCalculation) {
      alert('Unable to calculate order total. Please try again.');
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
