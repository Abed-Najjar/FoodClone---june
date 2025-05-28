import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Restaurant } from '../../models/restaurant.model';
import { RestaurantService } from '../../services/restaurant.service';
import { HomeService } from '../../services/home.service';
import { CartService } from '../../services/cart.service';
import { Dish } from '../../models/dish.model';
import { ImageUtilService } from '../../services/image-util.service';

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
  
  constructor(
    private restaurantService: RestaurantService,
    private homeService: HomeService,
    private cartService: CartService,
    private imageUtilService: ImageUtilService
  ) {}  ngOnInit(): void {
    this.loadFeaturedRestaurants();
    this.loadAllRestaurants();
    this.loadPopularDishes();
    this.heroImage = this.getRandomHeroImage();
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
  }  applyFilters(): void {
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
  }// Load popular dishes from the database
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


  // Add dish to cart
  addToCart(dish: Dish): void {
    // Check if dish is available before adding to cart
    if (!dish.isAvailable) {
      alert(`${dish.name} is currently unavailable and cannot be added to cart.`);
      return;
    }
    
    // Find a restaurant for this dish
    const restaurant = this.allRestaurants.find(r => r.id === dish.restaurantId) ||
                      { id: dish.restaurantId, name: dish.restaurantName || 'Restaurant ' + dish.restaurantId };

    // The dish already follows the Dish model, just ensure all required properties are present
    const cartDish: Dish = {
      ...dish,
      restaurantName: dish.restaurantName || restaurant?.name || 'Restaurant',
      isAvailable: dish.isAvailable // Keep the original availability status
    };

    // Add to cart with a quantity of 1
    this.cartService.addToCart(cartDish, 1);

    // Show a notification (in a real app)
    alert(`Added ${dish.name} to your cart!`);
  }
}
