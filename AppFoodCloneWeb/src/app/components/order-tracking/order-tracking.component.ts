import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Subscription, interval, forkJoin, startWith, switchMap, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { OrderService } from '../../services/order.service';
import { AuthService } from '../../services/auth.service';
import { HomeService } from '../../services/home.service';
import { RestaurantService } from '../../services/restaurant.service';
import { DeliveryTrackingService, DeliveryLocation } from '../../services/delivery-tracking.service';
import { DeliveryMapComponent } from '../delivery-map/delivery-map.component';
import { Order } from '../../models/order.model';
import { Dish } from '../../models/dish.model';
import { Restaurant } from '../../models/restaurant.model';

@Component({
  selector: 'app-order-tracking',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, DeliveryMapComponent],
  templateUrl: './order-tracking.component.html',
  styleUrl: './order-tracking.component.css'
})
export class OrderTrackingComponent implements OnInit, OnDestroy {
  orders: Order[] = [];
  isLoading = false;
  error = '';
  expandedOrders: Set<number> = new Set(); // Track which orders are expanded
  deliveryLocations: Map<number, DeliveryLocation> = new Map(); // Track delivery locations by order ID
  private refreshSubscription?: Subscription;
  private deliveryTrackingSubscriptions: Map<number, Subscription> = new Map(); // Track delivery subscriptions
  private dishesMap: Map<number, Dish> = new Map(); // Map to store dish details by ID
  private restaurantsMap: Map<number, Restaurant> = new Map(); // Map to store restaurant details by ID
  loadingDishIds: Set<number> = new Set(); // Track dishes being loaded (public for template access)
  loadingRestaurantIds: Set<number> = new Set(); // Track restaurants being loaded (public for template access)
  private dishLoadErrors: Set<number> = new Set(); // Track failed dish loads
  private restaurantLoadErrors: Set<number> = new Set(); // Track failed restaurant loads

  constructor(
    private orderService: OrderService,
    private authService: AuthService,
    private homeService: HomeService,
    private restaurantService: RestaurantService,
    private deliveryTrackingService: DeliveryTrackingService,
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) {}

  ngOnInit(): void {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/auth/login']);
      return;
    }
    
    // Load orders first, then load required data lazily
    this.loadOrders();
    
    // Auto-refresh every 30 seconds for real-time updates
    this.refreshSubscription = interval(30000).subscribe(() => {
      this.loadOrders(false); // Silent refresh
    });
  }

  ngOnDestroy(): void {
    if (this.refreshSubscription) {
      this.refreshSubscription.unsubscribe();
    }
    
    // Clean up delivery tracking subscriptions
    this.deliveryTrackingSubscriptions.forEach(subscription => {
      subscription.unsubscribe();
    });
    this.deliveryTrackingSubscriptions.clear();
  }

  loadOrders(showLoading: boolean = true): void {
    if (showLoading && this.orders.length === 0) {
      this.isLoading = true;
    }
    this.error = '';
    
    // Add pagination to limit data load - only get recent orders
    const pagination = {
      pageNumber: 1,
      pageSize: 20 // Limit to 20 most recent orders for better performance
    };
    
    this.orderService.getMyOrders(pagination).subscribe({
      next: (response: any) => {
        if (response.success) {
          this.orders = response.data.data.sort((a: Order, b: Order) => 
            new Date(b.orderDate).getTime() - new Date(a.orderDate).getTime()
          );
          
          // Check for orderId query parameter and auto-expand that order
          this.activatedRoute.queryParams.subscribe(params => {
            if (params['orderId']) {
              const targetOrderId = parseInt(params['orderId'], 10);
              if (this.orders.some(order => order.id === targetOrderId)) {
                // Auto-expand the target order
                this.expandedOrders.add(targetOrderId);
                
                // Scroll to the order after a short delay to ensure DOM is rendered
                setTimeout(() => {
                  this.scrollToOrder(targetOrderId);
                }, 500);
                
                // Remove the query parameter to clean up the URL
                this.router.navigate([], {
                  relativeTo: this.activatedRoute,
                  queryParams: {},
                  replaceUrl: true
                });
              }
            }
          });
          
          // Load required data only for visible/expanded orders
          this.loadRequiredDataLazily();
          
          // Start delivery tracking for orders that are out for delivery
          this.initializeDeliveryTracking();
        } else {
          this.error = response.errorMessage || 'Failed to load orders';
        }
        this.isLoading = false;
      },
      error: (err: any) => {
        console.error('Error fetching orders:', err);
        this.error = 'Failed to load orders. Please try again later.';
        this.isLoading = false;
      }
    });
  }

  // Load only the dishes and restaurants needed for current orders
  private loadRequiredDataLazily(): void {
    if (this.orders.length === 0) return;

    // Get unique restaurant IDs and dish IDs from orders
    const restaurantIds = new Set<number>();
    const dishIds = new Set<number>();

    this.orders.forEach(order => {
      if (order.restaurantId) {
        restaurantIds.add(order.restaurantId);
      }
      order.orderItems?.forEach(item => {
        if (item.dishId) {
          dishIds.add(item.dishId);
        }
      });
    });

    // Load restaurants that we don't have yet
    const restaurantsToLoad = Array.from(restaurantIds).filter(id => 
      !this.restaurantsMap.has(id) && 
      !this.loadingRestaurantIds.has(id) && 
      !this.restaurantLoadErrors.has(id)
    );

    // Load dishes that we don't have yet
    const dishesToLoad = Array.from(dishIds).filter(id => 
      !this.dishesMap.has(id) && 
      !this.loadingDishIds.has(id) && 
      !this.dishLoadErrors.has(id)
    );

    // Load restaurants and dishes with better error handling
    if (restaurantsToLoad.length > 0) {
      this.loadRestaurantsData(restaurantsToLoad);
    }

    if (dishesToLoad.length > 0) {
      this.loadDishesData(dishesToLoad);
    }
  }

  private loadRestaurantsData(restaurantIds: number[]): void {
    // Mark as loading
    restaurantIds.forEach(id => this.loadingRestaurantIds.add(id));

    this.restaurantService.getAllRestaurants().pipe(
      catchError(err => {
        console.warn('Failed to load restaurants:', err);
        restaurantIds.forEach(id => {
          this.loadingRestaurantIds.delete(id);
          this.restaurantLoadErrors.add(id);
        });
        return of({ success: false, data: [] });
      })
    ).subscribe(response => {
      if (response.success && response.data) {
        response.data.forEach((restaurant: Restaurant) => {
          if (restaurantIds.includes(restaurant.id)) {
            this.restaurantsMap.set(restaurant.id, restaurant);
            this.loadingRestaurantIds.delete(restaurant.id);
          }
        });
      }
      
      // Mark remaining as errors if not found
      restaurantIds.forEach(id => {
        if (this.loadingRestaurantIds.has(id)) {
          this.loadingRestaurantIds.delete(id);
          this.restaurantLoadErrors.add(id);
        }
      });
    });
  }

  private loadDishesData(dishIds: number[]): void {
    // Mark as loading
    dishIds.forEach(id => this.loadingDishIds.add(id));

    this.homeService.getAllDishes().pipe(
      catchError(err => {
        console.warn('Failed to load dishes:', err);
        dishIds.forEach(id => {
          this.loadingDishIds.delete(id);
          this.dishLoadErrors.add(id);
        });
        return of({ success: false, data: { data: [] } });
      })
    ).subscribe(response => {
      if (response.success && response.data?.data) {
        response.data.data.forEach((dish: Dish) => {
          if (dishIds.includes(dish.id)) {
            this.dishesMap.set(dish.id, dish);
            this.loadingDishIds.delete(dish.id);
          }
        });
      }
      
      // Mark remaining as errors if not found
      dishIds.forEach(id => {
        if (this.loadingDishIds.has(id)) {
          this.loadingDishIds.delete(id);
          this.dishLoadErrors.add(id);
        }
      });
    });
  }

  // Initialize delivery tracking for orders that need it
  private initializeDeliveryTracking(): void {
    this.orders.forEach(order => {
      const needsTracking = ['preparing', 'out for delivery', 'out_for_delivery'].includes(order.status.toLowerCase());
      
      if (needsTracking && !this.deliveryTrackingSubscriptions.has(order.id)) {
        // Start tracking this order with real-time updates every 10 seconds
        const subscription = interval(10000).pipe(
          startWith(0), // Start immediately
          switchMap(() => this.deliveryTrackingService.getMockDeliveryLocation(order.id))
        ).subscribe({
          next: (location) => {
            if (location) {
              this.deliveryLocations.set(order.id, location);
            }
          },
          error: (err) => {
            console.error('Error tracking delivery for order', order.id, err);
          }
        });
        
        this.deliveryTrackingSubscriptions.set(order.id, subscription);
      } else if (!needsTracking && this.deliveryTrackingSubscriptions.has(order.id)) {
        // Stop tracking orders that no longer need it
        const subscription = this.deliveryTrackingSubscriptions.get(order.id);
        if (subscription) {
          subscription.unsubscribe();
          this.deliveryTrackingSubscriptions.delete(order.id);
          this.deliveryLocations.delete(order.id);
        }
      }
    });
  }

  // Get order status color class
  getOrderStatusClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'pending':
        return 'status-pending';
      case 'confirmed':
        return 'status-confirmed';
      case 'preparing':
        return 'status-preparing';
      case 'out_for_delivery':
      case 'out for delivery':
        return 'status-delivery';
      case 'delivered':
        return 'status-delivered';
      case 'cancelled':
        return 'status-cancelled';
      default:
        return 'status-pending';
    }
  }

  // Get formatted order status text
  getFormattedOrderStatus(status: string): string {
    switch (status.toLowerCase()) {
      case 'out_for_delivery':
        return 'Out for Delivery';
      case 'out for delivery':
        return 'Out for Delivery';
      default:
        return status.charAt(0).toUpperCase() + status.slice(1).toLowerCase();
    }
  }

  // Get order status icon
  getOrderStatusIcon(status: string): string {
    switch (status.toLowerCase()) {
      case 'pending':
        return 'bi-clock';
      case 'confirmed':
        return 'bi-check-circle';
      case 'preparing':
        return 'bi-fire';
      case 'out_for_delivery':
      case 'out for delivery':
        return 'bi-truck';
      case 'delivered':
        return 'bi-check-circle-fill';
      case 'cancelled':
        return 'bi-x-circle';
      default:
        return 'bi-clock';
    }
  }

  // Get estimated delivery time based on order status
  getEstimatedDeliveryTime(order: Order): string {
    const orderTime = new Date(order.orderDate);
    const now = new Date();
    const minutesElapsed = Math.floor((now.getTime() - orderTime.getTime()) / (1000 * 60));
    
    switch (order.status.toLowerCase()) {
      case 'pending':
        return '5-10 min for confirmation';
      case 'confirmed':
        return '20-30 min preparation';
      case 'preparing':
        const prepTime = Math.max(15 - minutesElapsed, 5);
        return `${prepTime}-${prepTime + 10} min remaining`;
      case 'out_for_delivery':
      case 'out for delivery':
        return '10-15 min delivery';
      default:
        return 'Processing...';
    }
  }
  // Refresh orders
  refreshOrders(): void {
    this.loadOrders();
  }

  // Toggle order details expand/collapse
  toggleOrderDetails(orderId: number): void {
    if (this.expandedOrders.has(orderId)) {
      this.expandedOrders.delete(orderId);
    } else {
      this.expandedOrders.add(orderId);
      
      // Load data for this specific order when expanded
      const order = this.orders.find(o => o.id === orderId);
      if (order) {
        this.loadDataForOrder(order);
      }
    }
  }

  // Load data specifically for one order
  private loadDataForOrder(order: Order): void {
    const dishIds: number[] = [];
    const restaurantIds: number[] = [];

    if (order.restaurantId && !this.restaurantsMap.has(order.restaurantId) && 
        !this.loadingRestaurantIds.has(order.restaurantId) && 
        !this.restaurantLoadErrors.has(order.restaurantId)) {
      restaurantIds.push(order.restaurantId);
    }

    order.orderItems?.forEach(item => {
      if (item.dishId && !this.dishesMap.has(item.dishId) && 
          !this.loadingDishIds.has(item.dishId) && 
          !this.dishLoadErrors.has(item.dishId)) {
        dishIds.push(item.dishId);
      }
    });

    if (restaurantIds.length > 0) {
      this.loadRestaurantsData(restaurantIds);
    }

    if (dishIds.length > 0) {
      this.loadDishesData(dishIds);
    }
  }

  // Check if order is expanded
  isOrderExpanded(orderId: number): boolean {
    return this.expandedOrders.has(orderId);
  }

  // Prevent event bubbling for action buttons
  stopPropagation(event: Event): void {
    event.stopPropagation();
  }

  // Track order in real-time (placeholder for future implementation)
  trackOrderLive(orderId: number): void {
    console.log('Live tracking for order:', orderId);
    // TODO: Implement real-time tracking with maps/GPS
  }
  // Get order progress percentage
  getOrderProgress(status: string): number {
    switch (status.toLowerCase()) {
      case 'pending':
        return 10;
      case 'confirmed':
        return 25;
      case 'preparing':
        return 50;
      case 'out_for_delivery':
      case 'out for delivery':
        return 80;
      case 'delivered':
        return 100;
      default:
        return 0;
    }
  }

  // Track by function for ngFor performance
  trackByOrderId(index: number, order: Order): number {
    return order.id;
  }

  // Check if step is active
  isStepActive(step: string, status: string): boolean {
    return status.toLowerCase() === step.toLowerCase();
  }

  // Check if step is completed
  isStepCompleted(step: string, status: string): boolean {
    const steps = ['placed', 'confirmed', 'preparing', 'out for delivery', 'delivered'];
    const currentIndex = steps.findIndex(s => s === status.toLowerCase());
    const stepIndex = steps.findIndex(s => s === step.toLowerCase());
    return currentIndex > stepIndex;
  }

  // Get progress percentage for progress bar
  getProgressPercentage(status: string): number {
    switch (status.toLowerCase()) {
      case 'placed':
        return 20;
      case 'confirmed':
        return 40;
      case 'preparing':
        return 60;
      case 'out for delivery':
      case 'out_for_delivery':
        return 80;
      case 'delivered':
        return 100;
      default:
        return 0;
    }
  }

  // Check if order can be cancelled
  canCancelOrder(status: string): boolean {
    const cancellableStatuses = ['placed', 'confirmed'];
    return cancellableStatuses.includes(status.toLowerCase());
  }

  // Cancel order
  cancelOrder(orderId: number): void {
    if (confirm('Are you sure you want to cancel this order?')) {
      // TODO: Implement order cancellation API call
      console.log('Cancel order:', orderId);
    }
  }

  // Reorder items
  reorderItems(order: Order): void {
    // TODO: Implement reorder functionality
    console.log('Reorder items:', order);
  }
  // View order details
  viewOrderDetails(order: Order): void {
    // Navigate to detailed order view
    this.router.navigate(['/orders', order.id]);
  }
  // Get restaurant logo URL from actual restaurant data with improved error handling
  getRestaurantLogo(restaurantId: number): string {
    // If currently loading, return a loading placeholder
    if (this.loadingRestaurantIds.has(restaurantId)) {
      return 'https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=200&q=80';
    }

    const restaurant = this.restaurantsMap.get(restaurantId);
    if (restaurant && restaurant.logoUrl) {
      return restaurant.logoUrl;
    }
    
    // If we haven't loaded this restaurant yet and it's not in error state, trigger load
    if (!this.restaurantLoadErrors.has(restaurantId) && !this.restaurantsMap.has(restaurantId)) {
      this.loadRestaurantsData([restaurantId]);
    }
    
    // Fallback to a more reliable placeholder
    return 'https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=200&q=80';
  }

  // Get restaurant phone number with loading state handling
  getRestaurantPhone(restaurantId: number): string {
    if (this.loadingRestaurantIds.has(restaurantId)) {
      return 'Loading...';
    }
    
    const restaurant = this.restaurantsMap.get(restaurantId);
    if (restaurant && restaurant.phoneNumber) {
      return restaurant.phoneNumber;
    }
    
    // If we haven't loaded this restaurant yet and it's not in error state, trigger load
    if (!this.restaurantLoadErrors.has(restaurantId) && !this.restaurantsMap.has(restaurantId)) {
      this.loadRestaurantsData([restaurantId]);
    }
    
    return 'Contact Support';
  }

  // Get restaurant rating with improved fallback
  getRestaurantRating(restaurantId: number): string {
    const restaurant = this.restaurantsMap.get(restaurantId);
    if (restaurant && restaurant.rating) {
      return restaurant.rating.toString();
    }
    
    // Consistent rating based on restaurant ID
    const ratings = ['4.2', '4.5', '4.1', '4.7', '4.3', '4.6', '4.4', '4.8'];
    return ratings[restaurantId % ratings.length];
  }

  // Get dish image URL from actual dish data with improved error handling
  getDishImage(item: any): string {
    // If currently loading, return a loading placeholder
    if (this.loadingDishIds.has(item.dishId)) {
      return 'https://images.unsplash.com/photo-1565299624946-b28f40a0ca4b?w=80&q=80';
    }

    const dish = this.dishesMap.get(item.dishId);
    if (dish && dish.imageUrl) {
      return dish.imageUrl;
    }
    
    // If we haven't loaded this dish yet and it's not in error state, trigger load
    if (!this.dishLoadErrors.has(item.dishId) && !this.dishesMap.has(item.dishId)) {
      this.loadDishesData([item.dishId]);
    }
    
    // More reliable fallback image
    return 'https://images.unsplash.com/photo-1565299624946-b28f40a0ca4b?w=80&q=80';
  }

  // Get dish name from dish mapping with loading state
  getDishName(item: any): string {
    if (this.loadingDishIds.has(item.dishId)) {
      return 'Loading...';
    }
    
    const dish = this.dishesMap.get(item.dishId);
    if (dish && dish.name) {
      return dish.name;
    }
    
    // If we haven't loaded this dish yet and it's not in error state, trigger load
    if (!this.dishLoadErrors.has(item.dishId) && !this.dishesMap.has(item.dishId)) {
      this.loadDishesData([item.dishId]);
    }
    
    return `Dish ${item.dishId}`;
  }

  // Get dish description with loading state handling
  getDishDescription(item: any): string {
    if (this.loadingDishIds.has(item.dishId)) {
      return 'Loading dish details...';
    }
    
    const dish = this.dishesMap.get(item.dishId);
    if (dish && dish.description) {
      return dish.description;
    }
    
    // If we haven't loaded this dish yet and it's not in error state, trigger load
    if (!this.dishLoadErrors.has(item.dishId) && !this.dishesMap.has(item.dishId)) {
      this.loadDishesData([item.dishId]);
    }
    
    // Consistent fallback descriptions based on dish ID
    const descriptions = [
      'A delicious dish prepared with fresh ingredients and aromatic spices',
      'Perfectly cooked with traditional recipes and modern techniques',
      'Made with the finest ingredients sourced locally',
      'A chef\'s special with unique flavors and presentation',
      'Fresh, healthy, and bursting with flavor',
      'Traditional recipe with a modern twist',
      'Expertly crafted for the perfect taste experience',
      'Premium quality ingredients combined perfectly'
    ];
    return descriptions[item.dishId % descriptions.length];
  }

  // Calculate subtotal from order items with proper decimal formatting
  calculateSubtotal(order: Order): string {
    const subtotal = order.orderItems.reduce((sum, item) => sum + (item.unitPrice * item.quantity), 0);
    return subtotal.toFixed(2);
  }

  // Delivery tracking helper methods
  getDeliveryLocation(orderId: number): DeliveryLocation | null {
    return this.deliveryLocations.get(orderId) || null;
  }

  getDeliveryStatus(orderId: number): { message: string, icon: string, color: string } {
    const location = this.getDeliveryLocation(orderId);
    if (location) {
      return this.deliveryTrackingService.getStatusMessage(location.status);
    }
    return { message: 'Restaurant Open', icon: 'fas fa-check-circle', color: '#28a745' };
  }

  getDeliveryLocationText(orderId: number): string {
    const location = this.getDeliveryLocation(orderId);
    if (location) {
      return location.currentLocation.address;
    }
    return 'Open Now';
  }

  getDeliveryEmployeeName(orderId: number): string {
    const location = this.getDeliveryLocation(orderId);
    return location ? location.employeeName : '';
  }

  getDeliveryDistance(orderId: number): string {
    const location = this.getDeliveryLocation(orderId);
    if (location) {
      return this.deliveryTrackingService.formatDistance(location.distanceToDestination);
    }
    return '';
  }

  isOrderBeingDelivered(order: Order): boolean {
    return ['preparing', 'out for delivery', 'out_for_delivery'].includes(order.status.toLowerCase());
  }

  getEstimatedArrival(orderId: number): string {
    const location = this.getDeliveryLocation(orderId);
    if (location) {
      const eta = new Date(location.estimatedArrival);
      const now = new Date();
      const diffMs = eta.getTime() - now.getTime();
      const diffMins = Math.round(diffMs / (1000 * 60));
      
      if (diffMins <= 0) {
        return 'Arriving now';
      } else if (diffMins < 60) {
        return `${diffMins} min`;
      } else {
        return eta.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
      }
    }
    return '';
  }

  // Handle image loading errors
  handleImageError(event: any): void {
    const target = event.target;
    if (target.src.includes('restaurant')) {
      target.src = 'https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=200&q=80';
    } else {
      target.src = 'https://images.unsplash.com/photo-1565299624946-b28f40a0ca4b?w=80&q=80';
    }
    target.style.opacity = '0.8'; // Indicate it's a fallback image
  }

  // TEMPORARY: Method to change order status for testing Google Maps
  changeOrderStatusForTesting(orderId: number, newStatus: string): void {
    const order = this.orders.find(o => o.id === orderId);
    if (order) {
      order.status = newStatus;
      console.log(`Order ${orderId} status changed to: ${newStatus}`);
      // Reinitialize delivery tracking if needed
      this.initializeDeliveryTracking();
    }
  }

  // Scroll to a specific order card
  private scrollToOrder(orderId: number): void {
    const element = document.getElementById(`order-card-${orderId}`);
    if (element) {
      element.scrollIntoView({ 
        behavior: 'smooth', 
        block: 'start',
        inline: 'nearest'
      });
      
      // Add a highlight effect
      element.classList.add('highlight-order');
      setTimeout(() => {
        element.classList.remove('highlight-order');
      }, 3000);
    }
  }
}
