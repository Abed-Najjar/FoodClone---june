import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { RestaurantService } from '../../services/restaurant.service';
import { Restaurant } from '../../models/restaurant.model';
import { Dish } from '../../models/dish.model';
import { Category } from '../../models/category.model';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-restaurant-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './restaurant-detail.component.html',
  styleUrl: './restaurant-detail.component.css'
})
export class RestaurantDetailComponent implements OnInit {
  restaurantId!: number;
  restaurant: Restaurant | null = null;
  dishes: Dish[] = [];
  categories: Category[] = [];
  loading = true;
  error = '';
  activeCategory: number | null = null;
  filteredDishes: Dish[] = [];
  showAvailableOnly = false;

  constructor(
    private route: ActivatedRoute,
    private restaurantService: RestaurantService,
    private cartService: CartService
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.restaurantId = +params['id'];
      this.loadRestaurantData();
    });
  }

  loadRestaurantData(): void {
    // Get restaurant details (in a real app we would have an endpoint for this)
    this.restaurantService.getAllRestaurants().subscribe({
      next: (response) => {
        if (response.success) {
          this.restaurant = response.data.find(r => r.id === this.restaurantId) || null;
          if (!this.restaurant) {
            this.error = 'Restaurant not found';
            this.loading = false;
            return;
          }
        } else {
          this.error = response.errorMessage;
          this.loading = false;
        }
      },
      error: (err) => {
        this.error = 'Failed to load restaurant. Please try again later.';
        this.loading = false;
        console.error('Error fetching restaurant:', err);
      }
    });    // Get restaurant categories
    this.restaurantService.getRestaurantCategories(this.restaurantId).subscribe({
      next: (response) => {
        console.log('Categories response:', response);
        if (response.success) {
          this.categories = response.data;
          console.log('Loaded categories:', this.categories);
          // Start with "All Items" view instead of pre-selecting first category
          this.activeCategory = null;
        } else {
          console.warn('Failed to get categories:', response.errorMessage);
        }
      },
      error: (err) => {
        console.error('Error fetching categories:', err);
      }
    });    // Get restaurant dishes
    this.restaurantService.getRestaurantDishes(this.restaurantId).subscribe({
      next: (response) => {
        console.log('Dishes response:', response);
        if (response.success) {
          this.dishes = response.data;
          console.log('Loaded dishes:', this.dishes);

          // Debug: Log category IDs in dishes vs available categories
          const dishCategoryIds = [...new Set(this.dishes.map(d => d.categoryId))];
          const availableCategoryIds = this.categories.map(c => c.id);
          console.log('Dish category IDs:', dishCategoryIds);
          console.log('Available category IDs:', availableCategoryIds);

          this.filterDishes();
          this.loading = false;
        } else {
          this.error = response.errorMessage;
          this.loading = false;
          console.warn('Failed to get dishes:', response.errorMessage);
        }
      },
      error: (err) => {
        this.error = 'Failed to load dishes. Please try again later.';
        this.loading = false;
        console.error('Error fetching dishes:', err);
        console.error('Error details:', err.error);
      }
    });
  }
  setActiveCategory(categoryId: number | null): void {
    console.log('Setting active category to:', categoryId);
    this.activeCategory = categoryId;
    this.filterDishes();
  }  filterDishes(): void {
    console.log('Filtering dishes, activeCategory:', this.activeCategory, 'showAvailableOnly:', this.showAvailableOnly);
    
    let filtered = [...this.dishes];
    
    // Filter by category
    if (this.activeCategory !== null) {
      filtered = filtered.filter(dish => dish.categoryId === this.activeCategory);
    }
    
    // Filter by availability
    if (this.showAvailableOnly) {
      filtered = filtered.filter(dish => dish.isAvailable !== false); // Handle undefined as available
    }
    
    this.filteredDishes = filtered;
    console.log(`Showing ${this.filteredDishes.length} dishes after filtering`);
  }
  addToCart(dish: Dish): void {
    // Check if dish is available
    if (!dish.isAvailable) {
      alert('This dish is currently unavailable and cannot be added to cart.');
      return;
    }

    const quantity = this.getQuantity(dish);
    if (quantity > 0) {
      this.cartService.addToCart(dish, quantity);
      this.resetQuantity(dish);
    } else {
      this.cartService.addToCart(dish, 1);
    }
  }
  getCategoryName(categoryId: number | null): string {
    if (categoryId === null) {
      return 'Uncategorized';
    }
    const category = this.categories.find(c => c.id === categoryId);
    return category ? category.name : 'Uncategorized';
  }

  // Quantity management for dishes
  private dishQuantities: { [dishId: number]: number } = {};

  getQuantity(dish: Dish): number {
    return this.dishQuantities[dish.id] || 1;
  }

  increaseQuantity(dish: Dish): void {
    const currentQuantity = this.getQuantity(dish);
    this.dishQuantities[dish.id] = currentQuantity + 1;
  }

  decreaseQuantity(dish: Dish): void {
    const currentQuantity = this.getQuantity(dish);
    if (currentQuantity > 1) {
      this.dishQuantities[dish.id] = currentQuantity - 1;
    }
  }
  resetQuantity(dish: Dish): void {
    this.dishQuantities[dish.id] = 1;
  }

  // Availability statistics
  getAvailableDishesCount(): number {
    return this.filteredDishes.filter(dish => dish.isAvailable).length;
  }

  getUnavailableDishesCount(): number {
    return this.filteredDishes.filter(dish => !dish.isAvailable).length;
  }

  getTotalDishesCount(): number {
    return this.filteredDishes.length;
  }

  getAvailabilityPercentage(): number {
    if (this.getTotalDishesCount() === 0) return 0;
    return Math.round((this.getAvailableDishesCount() / this.getTotalDishesCount()) * 100);
  }
}
