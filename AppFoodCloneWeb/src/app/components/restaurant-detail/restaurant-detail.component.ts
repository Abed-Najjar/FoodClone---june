import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { RestaurantService } from '../../services/restaurant.service';
import { Restaurant } from '../../models/restaurant.model';
import { Dish } from '../../models/dish.model';
import { Category } from '../../models/category.model';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-restaurant-detail',
  standalone: true,
  imports: [CommonModule, RouterModule],
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
          this.error = response.message;
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
          if (this.categories.length > 0) {
            this.activeCategory = this.categories[0].id;
          }
        } else {
          console.warn('Failed to get categories:', response.message);
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
          this.filterDishes();
          this.loading = false;
        } else {
          this.error = response.message;
          this.loading = false;
          console.warn('Failed to get dishes:', response.message);
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
    this.activeCategory = categoryId;
    this.filterDishes();
  }

  filterDishes(): void {
    if (this.activeCategory === null) {
      this.filteredDishes = this.dishes;
    } else {
      this.filteredDishes = this.dishes.filter(dish => dish.categoryId === this.activeCategory);
    }
  }

  addToCart(dish: Dish): void {
    this.cartService.addToCart(dish, 1);
  }
}
