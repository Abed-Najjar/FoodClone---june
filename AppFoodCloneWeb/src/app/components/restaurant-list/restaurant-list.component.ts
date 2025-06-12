import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Restaurant } from '../../models/restaurant.model';
import { RestaurantService } from '../../services/restaurant.service';
import { HomeService } from '../../services/home.service';
import { PagedResult, PaginationParams } from '../../types/pagination.interface';
import { PaginationComponent } from '../shared/pagination/pagination.component';

@Component({
  selector: 'app-restaurant-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, PaginationComponent],
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

  // Pagination properties
  restaurantsPagedResult: PagedResult<Restaurant> | null = null;
  pagination: PaginationParams = { pageNumber: 1, pageSize: 6 };
  currentSearchTerm = '';

  // Expose Math to template
  Math = Math;

  get showOpenOnly(): boolean {
    return this._showOpenOnly;
  }

  set showOpenOnly(value: boolean) {
    this._showOpenOnly = value;
    this.pagination.pageNumber = 1; // Reset to first page when filter changes
    this.loadRestaurants();
  }

  constructor(
    private restaurantService: RestaurantService,
    private homeService: HomeService
  ) {}

  ngOnInit(): void {
    this.loadRestaurants();
  }

  loadRestaurants(): void {
    this.loading = true;
    this.homeService.getAllRestaurants(this.pagination).subscribe({
      next: (response: any) => {
        if (response.success) {
          this.restaurantsPagedResult = response.data as PagedResult<Restaurant>;
          this.restaurants = this.restaurantsPagedResult.data;
          this.applyClientSideFilters();
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
  }

  // Handle pagination page changes
  onPageChanged(page: number): void {
    this.pagination.pageNumber = page;
    this.loadRestaurants();
  }

  // Filter restaurants by name
  filterRestaurants(filterValue: string): void {
    this.currentSearchTerm = filterValue;
    this.applyClientSideFilters();
  }

  // Apply client-side filters (search + open status) to the current page data
  private applyClientSideFilters(): void {
    let filtered = [...this.restaurants];

    // Apply search filter
    if (this.currentSearchTerm.trim()) {
      const filterText = this.currentSearchTerm.toLowerCase().trim();
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

  // Apply all filters (search + open status) - deprecated, keeping for compatibility
  private applyFilters(filterValue: string = ''): void {
    this.currentSearchTerm = filterValue;
    this.applyClientSideFilters();
  }

  // Reset all filters and search
  resetFilters(): void {
    this._showOpenOnly = false;
    this.currentSearchTerm = '';
    this.pagination.pageNumber = 1;
    if (this.searchInput) {
      this.searchInput.nativeElement.value = '';
    }
    this.loadRestaurants();
  }
}
