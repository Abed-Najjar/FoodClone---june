import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Restaurant } from '../../models/restaurant.model';
import { RestaurantService } from '../../services/restaurant.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  featuredRestaurants: Restaurant[] = [];
  loading = true;
  error = '';

  constructor(private restaurantService: RestaurantService) {}

  ngOnInit(): void {
    this.restaurantService.getAllRestaurants().subscribe({
      next: (response) => {
        if (response.success) {
          // Just show a few restaurants on the homepage
          this.featuredRestaurants = response.data.slice(0, 4);
          this.loading = false;
        } else {
          this.error = response.message;
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
}
