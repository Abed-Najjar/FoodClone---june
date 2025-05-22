import { Injectable } from '@angular/core';

/**
 * A service to handle image loading and randomization in the app
 */
@Injectable({
  providedIn: 'root'
})
export class ImageUtilService {
  private restaurantCovers: string[] = [
    '/assets/images/restaurant-cover-1.jpg',
    '/assets/images/restaurant-cover-2.jpg',
    '/assets/images/restaurant-cover-3.jpg',
    '/assets/images/restaurant-cover-4.jpg',
    '/assets/images/restaurant-cover-5.jpg',
    '/assets/images/restaurant-cover-6.jpg',
    '/assets/images/restaurant-cover-7.jpg',
    '/assets/images/restaurant-cover-8.jpg'
  ];

  private restaurantLogos: string[] = [
    '/assets/images/restaurant-logo-1.jpg',
    '/assets/images/restaurant-logo-2.jpg',
    '/assets/images/restaurant-logo-3.jpg',
    '/assets/images/restaurant-logo-4.jpg',
    '/assets/images/restaurant-logo-5.jpg',
    '/assets/images/restaurant-logo-6.jpg',
    '/assets/images/restaurant-logo-7.jpg',
    '/assets/images/restaurant-logo-8.jpg'
  ];

  private dishImages: string[] = [
    '/assets/images/dish-1.jpg',
    '/assets/images/dish-2.jpg',
    '/assets/images/dish-3.jpg',
    '/assets/images/dish-4.jpg',
    '/assets/images/dish-5.jpg',
    '/assets/images/dish-6.jpg',
    '/assets/images/dish-7.jpg',
    '/assets/images/dish-8.jpg',
    '/assets/images/dish-9.jpg',
    '/assets/images/dish-10.jpg',
    '/assets/images/dish-11.jpg',
    '/assets/images/dish-12.jpg',
    '/assets/images/dish-13.jpg',
    '/assets/images/dish-14.jpg',
    '/assets/images/dish-15.jpg',
    '/assets/images/dish-16.jpg',
  ];

  private heroImages: string[] = [
    '/assets/images/hero-banner.jpg',
    '/assets/images/hero-banner2.jpg'
  ];

  constructor() { }

  /**
   * Get a random restaurant cover image
   */
  getRandomRestaurantCover(): string {
    return this.restaurantCovers[Math.floor(Math.random() * this.restaurantCovers.length)];
  }

  /**
   * Get a random restaurant logo image
   */
  getRandomRestaurantLogo(): string {
    return this.restaurantLogos[Math.floor(Math.random() * this.restaurantLogos.length)];
  }

  /**
   * Get a random dish image
   */
  getRandomDishImage(): string {
    return this.dishImages[Math.floor(Math.random() * this.dishImages.length)];
  }

  /**
   * Get a random hero image
   */
  getRandomHeroImage(): string {
    return this.heroImages[Math.floor(Math.random() * this.heroImages.length)];
  }

  /**
   * Get a restaurant cover image based on an ID (for consistent display)
   */
  getRestaurantCoverById(id: number): string {
    const index = id % this.restaurantCovers.length;
    return this.restaurantCovers[index];
  }

  /**
   * Get a restaurant logo image based on an ID (for consistent display)
   */
  getRestaurantLogoById(id: number): string {
    const index = id % this.restaurantLogos.length;
    return this.restaurantLogos[index];
  }

  /**
   * Get a fallback image URL for a restaurant cover
   */
  getFallbackRestaurantCover(): string {
    return 'https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=800&q=80';
  }

  /**
   * Get a fallback image URL for a restaurant logo
   */
  getFallbackRestaurantLogo(): string {
    return 'https://images.unsplash.com/photo-1594041680534-e8c8cdebd659?w=200&q=80';
  }

  /**
   * Get a fallback image URL for a dish
   */
  getFallbackDishImage(): string {
    return 'https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=600&q=80';
  }

  /**
   * Get a fallback image URL for a user avatar
   */
  getFallbackUserAvatar(): string {
    return '/assets/images/restaurant-logo-1.jpg';
  }
}
