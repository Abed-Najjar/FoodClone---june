import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Restaurant } from '../../models/restaurant.model';
import { RestaurantService } from '../../services/restaurant.service';
import { HomeService } from '../../services/home.service';
import { Dish } from '../../models/dish.model';
import { ImageUtilService } from '../../services/image-util.service';
import { OrderService } from '../../services/order.service';
import { AuthService } from '../../services/auth.service';
import { Order } from '../../models/order.model';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  featuredRestaurants: Restaurant[] = [];
  allRestaurants: Restaurant[] = [];
  filteredRestaurants: Restaurant[] = [];
  popularDishes: Dish[] = [];
  loading = true;
  dishesLoading = true;
  error = '';
  dishesError = '';
  searchTerm = '';
  selectedCategory: string | null = null;
  heroImage: string = '';
  // Current order tracking properties
  currentOrders: Order[] = [];
  ordersLoading = false;
  ordersError = '';  showOrderTracking = false;
  
  // Expose Math to template
  Math = Math;

  constructor(
    private restaurantService: RestaurantService,
    private homeService: HomeService,
    private imageUtilService: ImageUtilService,
    private orderService: OrderService,
    private authService: AuthService
  ) {}
  ngOnInit(): void {
    this.loadFeaturedRestaurants();
    this.loadAllRestaurants();
    this.loadPopularDishes();
    this.heroImage = this.getRandomHeroImage();    // Load current orders if user is logged in
    if (this.authService.isLoggedIn()) {
      this.loadCurrentOrders();
    }
  }

  loadFeaturedRestaurants(): void {
    this.loading = true;
    this.homeService.getFeaturedRestaurants().subscribe({
      next: (response: any) => {
        if (response.success) {
          this.featuredRestaurants = response.data;
        } else {
          this.error = response.errorMessage;
        }
        this.loading = false;
      },
      error: (err: any) => {
        this.error = 'Failed to load featured restaurants. Please try again later.';
        this.loading = false;
        console.error('Error fetching featured restaurants:', err);
      }
    });
  }

  loadAllRestaurants(): void {
    this.homeService.getAllRestaurants().subscribe({
      next: (response: any) => {
        if (response.success) {
          this.allRestaurants = response.data;
          this.filteredRestaurants = [...this.allRestaurants];
        } else {
          this.error = response.errorMessage;
        }
      },
      error: (err: any) => {
        this.error = 'Failed to load restaurants. Please try again later.';
        console.error('Error fetching restaurants:', err);
      }
    });
  }

  onSearch(event: any): void {
    this.searchTerm = event.target.value.toLowerCase();
    this.applyFilters();
  }

  filterByCategory(category: string | null): void {
    this.selectedCategory = category === 'all' ? null : category;
    this.applyFilters();
  }

  applyFilters(): void {
    let results = [...this.allRestaurants];

    // Apply search filter
    if (this.searchTerm) {
      results = results.filter(restaurant =>
        restaurant.name.toLowerCase().includes(this.searchTerm) ||
        restaurant.description.toLowerCase().includes(this.searchTerm)
      );
    }

    // Apply category filter (in a real app, you'd have actual category data)
    if (this.selectedCategory) {
      // This is just a simulation since we don't have real category data
      // In a real application, you would filter based on actual category values
      results = results.filter((restaurant, index) => {
        // Just for demonstration, assign restaurants to categories based on their index
        const mockCategories = ['burger', 'pizza', 'sushi', 'dessert', 'drinks', 'vegetarian'];
        const mockCategory = mockCategories[index % mockCategories.length];
        return mockCategory === this.selectedCategory;
      });
    }

    this.filteredRestaurants = results;
    // Update featured restaurants to reflect filtered results (first 8)
    this.featuredRestaurants = results.slice(0, 8);
  }

  resetFilters(): void {
    this.searchTerm = '';
    this.selectedCategory = null;
    this.filteredRestaurants = [...this.allRestaurants];
    this.featuredRestaurants = this.allRestaurants.slice(0, 8);
  }

  // Load popular dishes from the database
  loadPopularDishes(): void {
    this.dishesLoading = true;

    this.homeService.getPopularDishes().subscribe({
      next: (response: any) => {
        if (response.success) {
          this.popularDishes = response.data.map((dish: any) => ({
            ...dish,
            imageUrl: dish.imageUrl || this.imageUtilService.getRandomDishImage(),
            restaurantLogoUrl: dish.restaurantLogoUrl || this.getRestaurantLogoImage({ id: dish.restaurantId } as Restaurant)
          }));
          this.dishesLoading = false;

          if (this.popularDishes.length === 0) {
            this.dishesError = 'No popular dishes available at the moment';
          }
        } else {
          this.dishesError = response.errorMessage;
          this.dishesLoading = false;
        }
      },
      error: (err: any) => {
        this.dishesError = 'Failed to load popular dishes. Please try again later.';
        this.dishesLoading = false;
        console.error('Error fetching popular dishes:', err);
      }
    });
  }
  // Load current orders for the current user (orders that are in progress)
  loadCurrentOrders(): void {
    this.ordersLoading = true;
    this.ordersError = '';
      this.orderService.getMyOrders().subscribe({
      next: (response: any) => {
        if (response.success) {
          // Filter for current orders (not delivered or cancelled)
          const activeStatuses = ['pending', 'confirmed', 'preparing', 'out_for_delivery', 'out for delivery'];
          this.currentOrders = response.data
            .filter((order: Order) => activeStatuses.includes(order.status.toLowerCase()))
            .sort((a: Order, b: Order) => new Date(b.orderDate).getTime() - new Date(a.orderDate).getTime());
          
          // Only show order tracking section if there are current orders
          this.showOrderTracking = this.currentOrders.length > 0;
        } else {
          this.ordersError = response.errorMessage || 'Failed to load orders';
          this.showOrderTracking = false;
        }
        this.ordersLoading = false;
      },
      error: (err: any) => {
        this.ordersError = 'Failed to load current orders. Please try again later.';
        this.ordersLoading = false;
        this.showOrderTracking = false;
        console.error('Error fetching current orders:', err);
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

  // Refresh current orders
  refreshOrders(): void {
    this.loadCurrentOrders();
  }

  // Track order in real-time
  trackOrder(orderId: number): void {
    // Navigate to order tracking page - you can implement this route
    console.log('Track order in real-time:', orderId);
  }

  // Get a random hero image
  getRandomHeroImage(): string {
    const heroImages = [
      'https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=1200&q=80',
      'https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=1200&q=80'
    ];
    return heroImages[Math.floor(Math.random() * heroImages.length)];
  }

  // Get a restaurant cover image with fallback
  getRestaurantCoverImage(restaurant: Restaurant): string {
    if (restaurant.coverImageUrl) {
      return restaurant.coverImageUrl;
    }
    const imageIndex = (restaurant.id % 8) + 1;
    return `https://images.unsplash.com/photo-${1517248135467 + restaurant.id * 10000}-4c7edcad34c4?w=800&q=80`;
  }
  // Get a restaurant logo image with fallback
  getRestaurantLogoImage(restaurant: Restaurant): string {
    if (restaurant.logoUrl) {
      return restaurant.logoUrl;
    }
    const imageIndex = (restaurant.id % 8) + 1;
    return `https://images.unsplash.com/photo-${1594041680534 + restaurant.id * 10000}-e8c8cdebd659?w=200&q=80`;
  }
}
