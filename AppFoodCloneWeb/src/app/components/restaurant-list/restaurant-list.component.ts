import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Restaurant } from '../../models/restaurant.model';
import { RestaurantService } from '../../services/restaurant.service';

@Component({
  selector: 'app-restaurant-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './restaurant-list.component.html',
  styleUrl: './restaurant-list.component.css'
})
export class RestaurantListComponent implements OnInit {
  restaurants: Restaurant[] = [];
  filteredRestaurants: Restaurant[] = [];
  loading = true;
  error = '';
  @ViewChild('searchInput') searchInput!: ElementRef;

  constructor(private restaurantService: RestaurantService) {}

  ngOnInit(): void {
    this.loadRestaurants();
  }

  loadRestaurants(): void {
    this.restaurantService.getAllRestaurants().subscribe({
      next: (response) => {
        if (response.success) {
          this.restaurants = response.data;
          this.filteredRestaurants = this.restaurants;
          this.loading = false;
        } else {
          this.error = response.errorMessage;
          this.loading = false;
        }
      },
      error: (err) => {
        this.error = 'Failed to load restaurants. Please try again later.';
        this.loading = false;
        console.error('Error fetching restaurants:', err);
      }
    });
  }

  // Filter restaurants by name (could be expanded for more filtering options)
  filterRestaurants(filterValue: string): void {
    if (!filterValue.trim()) {
      this.filteredRestaurants = this.restaurants;
      return;
    }

    const filterText = filterValue.toLowerCase().trim();
    this.filteredRestaurants = this.restaurants.filter(restaurant =>
      restaurant.name.toLowerCase().includes(filterText) ||
      restaurant.description.toLowerCase().includes(filterText)
    );
  }

  // Reset all filters and search
  resetFilters(): void {
    this.filteredRestaurants = this.restaurants;
    if (this.searchInput) {
      this.searchInput.nativeElement.value = '';
    }
  }
}
