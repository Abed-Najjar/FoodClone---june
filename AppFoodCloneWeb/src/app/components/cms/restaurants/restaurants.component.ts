import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CmsService } from '../../../services/cms.service';
import { RestaurantService } from '../../../services/restaurant.service';
import { Restaurant } from '../../../models/restaurant.model';
import { ImageUtilService } from '../../../services/image-util.service';
import { ImageUploadService } from '../../../services/image-upload.service';
import { CmsNavigationService } from '../../../services/cms-navigation.service';

@Component({
  selector: 'app-restaurants',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './restaurants.component.html',
  styleUrls: ['./restaurants.component.css']
})
export class RestaurantsComponent implements OnInit {
  restaurants: Restaurant[] = [];
  filteredRestaurants: Restaurant[] = [];
  loading: boolean = false;
  error: string | null = null;
  searchTerm: string = '';

  // Form properties
  restaurantForm: FormGroup;
  showForm: boolean = false;
  isEditing: boolean = false;
  currentRestaurantId: number | null = null;
  logoFile: File | null = null;
  coverFile: File | null = null;
  logoPreviewUrl: string | null = null;
  coverPreviewUrl: string | null = null; constructor(
    private cmsService: CmsService,
    private restaurantService: RestaurantService,
    private router: Router,
    private fb: FormBuilder,
    private imageUtil: ImageUtilService,
    private imageUploadService: ImageUploadService,
    private navigationService: CmsNavigationService
  ) {
    this.restaurantForm = this.fb.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
      logoUrl: ['', Validators.required],
      coverImageUrl: ['', Validators.required],
      address: ['', Validators.required],
      phoneNumber: [''],
      openingHoursFrom: ['09:00'],
      openingHoursTo: ['22:00'],
      rating: [4.0, [Validators.required, Validators.min(0), Validators.max(5)]],
      reviewCount: [0],
      isOpen: [true],
      email: ['', [Validators.required, Validators.email]]
    });
  }

  ngOnInit() {
    this.loadRestaurants();
  }
  loadRestaurants() {
    this.loading = true;
    this.error = null;

    this.cmsService.getAllRestaurants().subscribe({
      next: (response) => {
        if (response.success) {
          console.log('Restaurants data from API:', response.data);
          this.restaurants = response.data;
          this.filteredRestaurants = [...this.restaurants];
        } else {
          this.error = response.errorMessage || 'Failed to load restaurants';
        }
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Error loading restaurants. Please try again.';
        this.loading = false;
        console.error('Error loading restaurants:', err);
      }
    });
  }

  filterRestaurants() {
    if (!this.searchTerm.trim()) {
      this.filteredRestaurants = [...this.restaurants];
      return;
    }

    const term = this.searchTerm.toLowerCase().trim();
    this.filteredRestaurants = this.restaurants.filter(restaurant =>
      restaurant.name.toLowerCase().includes(term) ||
      restaurant.description.toLowerCase().includes(term) ||
      restaurant.address.toLowerCase().includes(term)
    );
  }
  showAddForm() {
    this.isEditing = false;
    this.currentRestaurantId = null;
    this.restaurantForm.reset({
      rating: 4.0,
      reviewCount: 0,
      isOpen: true
    });
    this.showForm = true;
  } editRestaurant(restaurant: Restaurant) {
    this.isEditing = true;
    this.currentRestaurantId = restaurant.id;

    console.log('Original restaurant opening hours:', restaurant.openingHours);

    // Parse opening hours string into from and to times
    let openingHoursFrom = '09:00';
    let openingHoursTo = '22:00';

    if (restaurant.openingHours) {
      const hoursMatch = restaurant.openingHours.match(/(\d{1,2}:\d{2})\s*-\s*(\d{1,2}:\d{2})/);
      console.log('Regex match result:', hoursMatch);
      if (hoursMatch && hoursMatch.length === 3) {
        openingHoursFrom = hoursMatch[1];
        openingHoursTo = hoursMatch[2];
        console.log('Parsed opening hours - From:', openingHoursFrom, 'To:', openingHoursTo);
      } else {
        console.log('Failed to parse opening hours, using defaults');
      }
    }

    this.restaurantForm.patchValue({
      name: restaurant.name,
      description: restaurant.description,
      logoUrl: restaurant.logoUrl,
      coverImageUrl: restaurant.coverImageUrl,
      address: restaurant.address,
      phoneNumber: restaurant.phoneNumber,
      openingHoursFrom: openingHoursFrom,
      openingHoursTo: openingHoursTo,
      rating: restaurant.rating,
      reviewCount: restaurant.reviewCount,
      isOpen: restaurant.isOpen,
      email: restaurant.email
    });
    this.showForm = true;
  }

  cancelForm() {
    this.showForm = false;
    this.restaurantForm.reset();
  }
  onLogoFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.logoFile = file;
      const reader = new FileReader();
      reader.onload = (e: any) => this.logoPreviewUrl = e.target.result;
      reader.readAsDataURL(file);
    }
  }

  onCoverFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.coverFile = file;
      const reader = new FileReader();
      reader.onload = (e: any) => this.coverPreviewUrl = e.target.result;
      reader.readAsDataURL(file);
    }
  } async saveRestaurant() {
    if (this.restaurantForm.invalid) return;
    let logoUrl = this.restaurantForm.value.logoUrl;
    let coverImageUrl = this.restaurantForm.value.coverImageUrl;

    // Upload logo if a new file is selected
    if (this.logoFile) {
      try {
        const res = await this.imageUploadService.uploadImage(this.logoFile).toPromise();
        logoUrl = res?.url || logoUrl;
      } catch (err) {
        alert('Failed to upload logo image.');
        return;
      }
    }
    // Upload cover if a new file is selected
    if (this.coverFile) {
      try {
        const res = await this.imageUploadService.uploadImage(this.coverFile).toPromise();
        coverImageUrl = res?.url || coverImageUrl;
      } catch (err) {
        alert('Failed to upload cover image.');
        return;
      }
    }

    const formValue = this.restaurantForm.value;
    // Combine opening hours from and to times
    const openingHoursFrom = formValue.openingHoursFrom || '09:00';
    const openingHoursTo = formValue.openingHoursTo || '22:00';
    const openingHours = `${openingHoursFrom} - ${openingHoursTo}`;

    console.log('Opening hours combined value:', openingHours); const restaurantData = {
      id: this.isEditing ? this.currentRestaurantId : 0,
      name: formValue.name,
      description: formValue.description,
      logoUrl,
      coverImageUrl,
      address: formValue.address,
      phoneNumber: formValue.phoneNumber || '',
      openingHours: openingHours,
      rating: formValue.rating,
      reviewCount: formValue.reviewCount || 0,
      isOpen: formValue.isOpen,
      email: formValue.email,
      categories: [],
      deliveryFee: 0,
      suspended: false
    };

    console.log('Restaurant data being sent to API:', restaurantData); if (this.isEditing && this.currentRestaurantId) {
      this.cmsService.updateRestaurant(this.currentRestaurantId, restaurantData).subscribe({
        next: (response) => {
          console.log('Update restaurant response:', response);
          if (response.success) {
            const index = this.restaurants.findIndex(r => r.id === this.currentRestaurantId);
            if (index !== -1) {
              this.restaurants[index] = response.data;
              this.filterRestaurants();
            }
            this.showForm = false;
            this.restaurantForm.reset();
            this.logoFile = null;
            this.coverFile = null;
            this.logoPreviewUrl = null;
            this.coverPreviewUrl = null;
          } else {
            alert(response.errorMessage || 'Failed to update restaurant');
          }
        },
        error: (err) => {
          console.error('Error updating restaurant:', err);
          alert('Error updating restaurant. Please try again.');
        }
      });
    } else {
      this.cmsService.createRestaurant(restaurantData).subscribe({
        next: (response) => {
          if (response.success) {
            this.restaurants.push(response.data);
            this.filterRestaurants();
            this.showForm = false;
            this.restaurantForm.reset();
            this.logoFile = null;
            this.coverFile = null;
            this.logoPreviewUrl = null;
            this.coverPreviewUrl = null;
          } else {
            alert(response.errorMessage || 'Failed to create restaurant');
          }
        },
        error: (err) => {
          console.error('Error creating restaurant:', err);
          alert('Error creating restaurant. Please try again.');
        }
      });
    }
  }
  deleteRestaurant(restaurant: Restaurant) {
    if (confirm(`Are you sure you want to delete the restaurant "${restaurant.name}"? This will also delete all associated categories and dishes.`)) {
      this.cmsService.deleteRestaurant(restaurant.id).subscribe({
        next: (response) => {
          if (response.success) {
            // Remove from the local list
            this.restaurants = this.restaurants.filter(r => r.id !== restaurant.id);
            this.filterRestaurants();
          } else {
            alert(response.errorMessage || 'Failed to delete restaurant');
          }
        },
        error: (err) => {
          console.error('Error deleting restaurant:', err);
          alert('Error deleting restaurant. Please try again.');
        }
      });
    }
  }
  viewRestaurantCategories(restaurant: Restaurant) {


    this.router.navigate(['/admin/dashboard'], {
      queryParams: { restaurantId: restaurant.id }
    });
    this.navigationService.changeTab('categories');
    return

    // Store the restaurant info in local storage
    localStorage.setItem('selectedRestaurantId', restaurant.id.toString());
    localStorage.setItem('selectedRestaurantName', restaurant.name);

    // Switch to the categories tab using the navigation service
  }

  viewRestaurantDishes(restaurant: Restaurant) {
    // Store the restaurant info in local storage
    localStorage.setItem('selectedRestaurantId', restaurant.id.toString());
    localStorage.setItem('selectedRestaurantName', restaurant.name);

    // Switch to the dishes tab using the navigation service
    this.navigationService.changeTab('dishes');
  }
}
