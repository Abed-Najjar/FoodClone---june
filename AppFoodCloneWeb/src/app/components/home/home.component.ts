import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Restaurant } from '../../models/restaurant.model';
import { RestaurantService } from '../../services/restaurant.service';
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
    private cartService: CartService,
    private imageUtilService: ImageUtilService
  ) {}
  ngOnInit(): void {
    this.loadRestaurants();
    this.loadPopularDishes();
    this.heroImage = this.getRandomHeroImage();
  }

  loadRestaurants(): void {
    this.loading = true;
    this.restaurantService.getAllRestaurants().subscribe({
      next: (response) => {
        if (response.success) {
          this.allRestaurants = response.data;
          this.filteredRestaurants = [...this.allRestaurants];
          this.featuredRestaurants = this.allRestaurants.slice(0, 8);
          this.loading = false;
        } else {
          this.error = response.errorMessage;
          this.loading = false;
        }      },
      error: (err) => {
        this.error = 'Failed to load restaurants. Please try again later.';
        this.loading = false;
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
    this.featuredRestaurants = results.slice(0, 8);
  }

  resetFilters(): void {
    this.searchTerm = '';
    this.selectedCategory = null;
    this.filteredRestaurants = [...this.allRestaurants];
    this.featuredRestaurants = this.allRestaurants.slice(0, 8);
  }  // Load popular dishes from the database
  loadPopularDishes(): void {
    this.dishesLoading = true;
    
    // First get all restaurants, then fetch dishes from each restaurant
    this.restaurantService.getAllRestaurants().subscribe({
      next: (restaurantsResponse) => {
        if (restaurantsResponse.success && restaurantsResponse.data && restaurantsResponse.data.length > 0) {
          this.loadDishesFromRestaurants(restaurantsResponse.data);
        } else {
          this.dishesError = 'No restaurants available to load dishes from';
          this.dishesLoading = false;
        }
      },
      error: (err) => {
        this.dishesError = 'Failed to load restaurants for dishes. Please try again later.';
        this.dishesLoading = false;
        console.error('Error fetching restaurants for dishes:', err);
      }
    });
  }

  // Load dishes from multiple restaurants
  private loadDishesFromRestaurants(restaurants: Restaurant[]): void {
    const allDishes: Dish[] = [];
    let completedRequests = 0;
    const totalRequests = Math.min(restaurants.length, 3); // Limit to first 3 restaurants for performance

    if (totalRequests === 0) {
      this.dishesError = 'No restaurants available';
      this.dishesLoading = false;
      return;
    }

    // Get dishes from first few restaurants
    restaurants.slice(0, totalRequests).forEach(restaurant => {
      this.restaurantService.getRestaurantDishes(restaurant.id).subscribe({
        
        next: (dishesResponse) => {
          completedRequests++;            if (dishesResponse.success && dishesResponse.data) {            // Convert UserRestaurantDishesDto to Dish model
            const convertedDishes: Dish[] = dishesResponse.data
              .map(dish => ({
                id: dish.id,
                name: dish.name,
                description: dish.description,
                price: dish.price,
                imageUrl: dish.imageUrl || this.imageUtilService.getRandomDishImage(),
                restaurantId: restaurant.id,
                restaurantName: restaurant.name,
                categoryId: dish.categoryId,
                isAvailable: dish.isAvailable, // Make sure this is preserved from the backend
                restaurantLogoUrl: restaurant.logoUrl || this.getRestaurantLogoImage(restaurant)
              }));
            
            allDishes.push(...convertedDishes);
          }          // Check if all requests completed
          if (completedRequests === totalRequests) {
            // Only show available dishes
            this.popularDishes = allDishes.slice(0, 8);
            
            this.dishesLoading = false;
            
            if (this.popularDishes.length === 0) {
              this.dishesError = 'No available dishes at the moment';
            }
          }
        },
        error: (err) => {
          completedRequests++;
          console.error(`Error fetching dishes for restaurant ${restaurant.id}:`, err);
            // Check if all requests completed (including failed ones)
          if (completedRequests === totalRequests) {
            if (allDishes.length > 0) {
              this.popularDishes = allDishes.slice(0, 8);
              this.dishesLoading = false;
            } else {
              this.dishesError = 'Failed to load available dishes from restaurants';
              this.dishesLoading = false;
            }
          }
        }
      });
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
