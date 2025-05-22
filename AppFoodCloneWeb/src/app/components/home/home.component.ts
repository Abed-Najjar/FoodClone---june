import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Restaurant } from '../../models/restaurant.model';
import { RestaurantService } from '../../services/restaurant.service';
import { CartService } from '../../services/cart.service';
import { Dish } from '../../models/dish.model';
import { ImageUtilService } from '../../services/image-util.service';

interface PopularDish {
  id: number;
  name: string;
  description: string;
  price: number;
  imageUrl: string;
  restaurantId: number;
}

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
  popularDishes: PopularDish[] = [];
  loading = true;
  error = '';
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
    this.generatePopularDishes();
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
          this.error = response.message;
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
  }

  // Generate mock popular dishes
  generatePopularDishes(): void {
    const dishNames = [
      'Classic Cheeseburger', 'Fresh Garden Salad', 'Pepperoni Pizza', 'Spaghetti Bolognese',
      'Grilled Salmon', 'Vegetarian Stir Fry', 'Chicken Teriyaki Bowl', 'Beef Tacos'
    ];

    const dishDescriptions = [
      'Juicy beef patty with melted cheese and special sauce',
      'Mix of fresh vegetables with our house vinaigrette',
      'Classic pizza with pepperoni, cheese and our special sauce',
      'Traditional pasta with rich meat sauce and parmesan',
      'Fresh salmon filet with lemon herb sauce',
      'Seasonal vegetables sautéed in teriyaki sauce',
      'Grilled chicken with teriyaki glaze over steamed rice',
      'Authentic corn tortillas filled with seasoned beef'
    ];

    this.popularDishes = Array(8).fill(0).map((_, idx) => {
      return {
        id: idx + 1,
        name: dishNames[idx % dishNames.length],
        description: dishDescriptions[idx % dishDescriptions.length],
        price: 6.99 + (idx * 2),
        imageUrl: this.getRandomDishImage(),
        restaurantId: (idx % 4) + 1
      };
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

  // Get a random dish image
  getRandomDishImage(): string {
    const dishImages = [
      'https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=600&q=80',
      'https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=600&q=80',
      'https://images.unsplash.com/photo-1567620905732-2d1ec7ab7445?w=600&q=80',
      'https://images.unsplash.com/photo-1565958011703-44f9829ba187?w=600&q=80',
      'https://images.unsplash.com/photo-1565299507177-b0ac66763828?w=600&q=80',
      'https://images.unsplash.com/photo-1540189549336-e6e99c3679fe?w=600&q=80',
      'https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=600&q=80',
      'https://images.unsplash.com/photo-1551782450-17144efb9c50?w=600&q=80',
      'https://images.unsplash.com/photo-1572802419224-296b0aeee0d9?w=600&q=80',
      'https://images.unsplash.com/photo-1586190848861-99aa4a171e90?w=600&q=80',
      'https://images.unsplash.com/photo-1559742811-822873691df8?w=600&q=80',
      'https://images.unsplash.com/photo-1563379091339-03b21ab4a4f8?w=600&q=80',
      'https://images.unsplash.com/photo-1569058242567-93de6f36f8e7?w=600&q=80',
      'https://images.unsplash.com/photo-1534080564583-6be75777b70a?w=600&q=80',
      'https://images.unsplash.com/photo-1558961363-fa8fdf82db35?w=600&q=80',
      'https://images.unsplash.com/photo-1562967916-eb82221dfb92?w=600&q=80'
    ];
    return dishImages[Math.floor(Math.random() * dishImages.length)];
  }

  // Add dish to cart
  addToCart(dish: PopularDish): void {
    // Find a restaurant for this dish
    const restaurant = this.allRestaurants.find(r => r.id === dish.restaurantId) ||
                      { id: dish.restaurantId, name: 'Restaurant ' + dish.restaurantId };

    // Convert PopularDish to Dish model
    const cartDish: Dish = {
      id: dish.id,
      name: dish.name,
      description: dish.description,
      price: dish.price,
      imageUrl: dish.imageUrl,
      restaurantId: dish.restaurantId,
      restaurantName: restaurant?.name || 'Restaurant',
      categoryId: 1
    };

    // Add to cart with a quantity of 1
    this.cartService.addToCart(cartDish, 1);

    // Show a notification (in a real app)
    alert(`Added ${dish.name} to your cart!`);
  }
}
