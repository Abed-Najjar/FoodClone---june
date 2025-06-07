import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Restaurant } from '../../models/restaurant.model';
import { RestaurantService } from '../../services/restaurant.service';
import { HomeService } from '../../services/home.service';
import { PagedResult } from '../../types/pagination.interface';

@Component({
  selector: 'app-restaurant-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './restaurant-list.component.html',
  styleUrl: './restaurant-list.component.css'
})
export class RestaurantListComponent implements OnInit {
  restaurants: Restaurant[] = [];
  filteredRestaurants: Restaurant[] = [];
  loading = true;
  error = '';
  private _showOpenOnly = false;
  @ViewChild('searchInput') searchInput!: ElementRef;

  get showOpenOnly(): boolean {
    return this._showOpenOnly;
  }

  set showOpenOnly(value: boolean) {
    this._showOpenOnly = value;
    const searchValue = this.searchInput?.nativeElement?.value || '';
    this.applyFilters(searchValue);
  }
  constructor(
    private restaurantService: RestaurantService,
    private homeService: HomeService
  ) {}

  ngOnInit(): void {
    this.loadRestaurants();
  }

  loadRestaurants(): void {
    this.homeService.getAllRestaurants().subscribe({
      next: (response: any) => {
        if (response.success) {
          const pagedResult = response.data as PagedResult<Restaurant>;
          this.restaurants = pagedResult.data;
          this.filteredRestaurants = this.restaurants;
          this.loading = false;
        } else {
          this.error = response.errorMessage;
          this.loading = false;
        }
      },
      error: (err: any) => {
        this.error = 'Failed to load restaurants. Please try again later.';
        this.loading = false;
        console.error('Error fetching restaurants:', err);
      }
    });
  }  // Filter restaurants by name (could be expanded for more filtering options)
  filterRestaurants(filterValue: string): void {
    this.applyFilters(filterValue);
  }

  // Apply all filters (search + open status)
  private applyFilters(filterValue: string = ''): void {
    let filtered = [...this.restaurants];

    // Apply search filter
    if (filterValue.trim()) {
      const filterText = filterValue.toLowerCase().trim();
      filtered = filtered.filter(restaurant =>
        restaurant.name.toLowerCase().includes(filterText) ||
        restaurant.description.toLowerCase().includes(filterText)
      );
    }

    // Apply open status filter
    if (this._showOpenOnly) {
      filtered = filtered.filter(restaurant => restaurant.isOpen);
    }

    this.filteredRestaurants = filtered;
  }

  // Reset all filters and search
  resetFilters(): void {
    this._showOpenOnly = false;
    this.filteredRestaurants = this.restaurants;
    if (this.searchInput) {
      this.searchInput.nativeElement.value = '';
    }
  }
}
