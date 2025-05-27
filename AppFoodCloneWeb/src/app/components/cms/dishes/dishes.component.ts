import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CmsService } from '../../../services/cms.service';
import { Dish } from '../../../models/dish.model';
import { Restaurant } from '../../../models/restaurant.model';
import { Category } from '../../../models/category.model';
import { ImageUtilService } from '../../../services/image-util.service';
import { ImageUploadService } from '../../../services/image-upload.service';
import { AuthService } from '../../../services/auth.service';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-dishes',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './dishes.component.html',
  styleUrls: ['./dishes.component.css']
})
export class DishesComponent implements OnInit {  dishes: any[] = [];
  filteredDishes: any[] = [];
  restaurants: Restaurant[] = [];
  categories: Category[] = [];
  restaurantCategories: Category[] = [];
  loading: boolean = false;
  error: string | null = null;
  searchTerm: string = '';
  selectedRestaurantId: number | null = null;
  selectedRestaurantName: string = '';

  // Form properties
  dishForm: FormGroup;
  showForm: boolean = false;
  isEditing: boolean = false;
  currentDishId: number | null = null;
  imagePreview: string | null = null;

  constructor(
    private cmsService: CmsService,
    private fb: FormBuilder,
    private imageUtil: ImageUtilService,
    private imageUploadService: ImageUploadService,
    private http: HttpClient,
    private authService: AuthService
  ) {
    this.dishForm = this.fb.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
      imageUrl: ['', Validators.required],
      price: [0, [Validators.required, Validators.min(0.01)]],
      isAvailable: [true],
      restaurantId: ['', Validators.required],
      categoryId: ['', Validators.required]
    });
  }
  ngOnInit() {
    // Check if we have a selected restaurant from localStorage
    const storedRestaurantId = localStorage.getItem('selectedRestaurantId');
    if (storedRestaurantId) {
      this.selectedRestaurantId = parseInt(storedRestaurantId);
      this.selectedRestaurantName = localStorage.getItem('selectedRestaurantName') || '';
      console.log('Found selected restaurant:', this.selectedRestaurantId, this.selectedRestaurantName);
    }

    this.loadDishes();
    this.loadRestaurants();
    this.loadCategories();
  }
  loadDishes() {
    this.loading = true;
    this.error = null;

    // If a restaurant ID is selected, use the filtered endpoint
    if (this.selectedRestaurantId) {
      console.log(`Loading dishes for restaurant ID: ${this.selectedRestaurantId}`);
      this.cmsService.getDishesByRestaurant(this.selectedRestaurantId).subscribe({
        next: (response) => {
          if (response.success) {
            this.dishes = response.data;
            this.filteredDishes = [...this.dishes];
            console.log(`Loaded ${this.dishes.length} dishes for restaurant ID ${this.selectedRestaurantId}`);
          } else {
            this.error = response.errorMessage || `Failed to load dishes for restaurant ID: ${this.selectedRestaurantId}`;
          }
          this.loading = false;
        },
        error: (err) => {
          this.error = `Error loading dishes for restaurant ID: ${this.selectedRestaurantId}`;
          this.loading = false;
          console.error('Error loading dishes by restaurant:', err);
        }
      });
    } else {
      // Otherwise load all dishes
      this.cmsService.getAllDishes().subscribe({
        next: (response) => {
          if (response.success) {
            this.dishes = response.data;
            this.filteredDishes = [...this.dishes];
          } else {
            this.error = response.errorMessage || 'Failed to load dishes';
          }
          this.loading = false;
        },
        error: (err) => {
          this.error = 'Error loading dishes. Please try again.';
          this.loading = false;
          console.error('Error loading dishes:', err);
        }
      });
    }
  }

  loadRestaurants() {
    this.cmsService.getAllRestaurants().subscribe({
      next: (response) => {
        if (response.success) {
          this.restaurants = response.data;
        } else {
          console.error('Failed to load restaurants:', response.errorMessage);
        }
      },
      error: (err) => {
        console.error('Error loading restaurants:', err);
      }
    });
  }

  loadCategories() {
    this.cmsService.getAllCategories().subscribe({
      next: (response) => {
        if (response.success) {
          this.categories = response.data;
        } else {
          console.error('Failed to load categories:', response.errorMessage);
        }
      },
      error: (err) => {
        console.error('Error loading categories:', err);
      }
    });
  }

  filterDishes() {
    // First apply restaurant filter if selected
    let filtered = [...this.dishes];

    if (this.selectedRestaurantId) {
      filtered = filtered.filter(dish =>
        dish.restaurantId === this.selectedRestaurantId
      );
    }

    // Then apply search term filter
    if (this.searchTerm.trim()) {
      const term = this.searchTerm.toLowerCase().trim();
      filtered = filtered.filter(dish =>
        dish.name.toLowerCase().includes(term) ||
        dish.description.toLowerCase().includes(term) ||
        dish.restaurantName.toLowerCase().includes(term) ||
        dish.categoryName.toLowerCase().includes(term)
      );
    }

    this.filteredDishes = filtered;
  }  onRestaurantChange() {
    const restaurantId = this.dishForm.get('restaurantId')?.value;
    if (restaurantId) {
      // Load categories specific to the selected restaurant
      console.log(`Loading categories for restaurant ID: ${restaurantId}`);
      this.cmsService.getCategoriesByRestaurant(parseInt(restaurantId)).subscribe({
        next: (response) => {
          if (response.success) {
            this.restaurantCategories = response.data;
            console.log(`Loaded ${this.restaurantCategories.length} categories for restaurant ID ${restaurantId}`);

            // Reset category selection since the available categories have changed
            this.dishForm.patchValue({ categoryId: '' });
          } else {
            console.error('Failed to load categories for restaurant:', response.errorMessage);
            this.restaurantCategories = [];
            this.dishForm.patchValue({ categoryId: '' });
          }
        },
        error: (err) => {
          console.error('Error loading categories for restaurant:', err);
          this.restaurantCategories = [];
          this.dishForm.patchValue({ categoryId: '' });
        }
      });
    } else {
      // No restaurant selected, clear categories
      this.restaurantCategories = [];
      this.dishForm.patchValue({ categoryId: '' });
    }
  }  showAddForm() {
    this.isEditing = false;
    this.currentDishId = null;

    // Prepare form values, pre-select restaurant if one is filtered
    const formValues: any = {
      price: 0.01,
      isAvailable: true,
      restaurantId: this.selectedRestaurantId ? this.selectedRestaurantId.toString() : ''
    };

    this.dishForm.reset(formValues);

    // Load categories based on pre-selected restaurant or clear categories
    if (this.selectedRestaurantId) {
      this.onRestaurantChange(); // This will load categories for the selected restaurant
    } else {
      this.restaurantCategories = []; // No restaurant selected, no categories to show
    }

    this.showForm = true;
  }  editDish(dish: any) {
    this.isEditing = true;
    this.currentDishId = dish.id;

    // Set form values first
    this.dishForm.patchValue({
      name: dish.name,
      description: dish.description,
      imageUrl: dish.imageUrl,
      price: dish.price,
      isAvailable: dish.isAvailable,
      restaurantId: dish.restaurantId.toString(),
      categoryId: dish.categoryId ? dish.categoryId.toString() : ''
    });

    // Load categories for the dish's restaurant
    if (dish.restaurantId) {
      console.log(`Loading categories for editing dish in restaurant ID: ${dish.restaurantId}`);
      this.cmsService.getCategoriesByRestaurant(dish.restaurantId).subscribe({
        next: (response) => {
          if (response.success) {
            this.restaurantCategories = response.data;
            console.log(`Loaded ${this.restaurantCategories.length} categories for restaurant ID ${dish.restaurantId}`);

            // Re-set the category ID after loading categories
            this.dishForm.patchValue({
              categoryId: dish.categoryId ? dish.categoryId.toString() : ''
            });
          } else {
            console.error('Failed to load categories for restaurant:', response.errorMessage);
            this.restaurantCategories = [];
          }
        },
        error: (err) => {
          console.error('Error loading categories for restaurant:', err);
          this.restaurantCategories = [];
        }
      });
    } else {
      this.restaurantCategories = [];
    }

    this.showForm = true;
  }

  cancelForm() {
    this.showForm = false;
    this.dishForm.reset();
    this.restaurantCategories = [];
  }  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      // Preview immediately
      const reader = new FileReader();
      reader.onload = e => this.imagePreview = reader.result as string;
      reader.readAsDataURL(file);

      // Set a temporary data URL from the file itself to make the form valid immediately
      // This ensures the form is valid even before the upload completes
      const tempUrl = URL.createObjectURL(file);
      this.dishForm.patchValue({ imageUrl: tempUrl });
      this.dishForm.get('imageUrl')?.markAsDirty();
      this.dishForm.get('imageUrl')?.markAsTouched();
      this.dishForm.get('imageUrl')?.updateValueAndValidity();

      // Show uploading message
      console.log('Uploading image:', file.name);

      // If we're editing an existing dish, use the dish-specific upload
      if (this.isEditing && this.currentDishId) {
        console.log('Uploading image for existing dish:', this.currentDishId);
        this.imageUploadService.uploadDishImage(file, this.currentDishId).subscribe({
          next: (response) => {
            console.log('Dish image uploaded successfully:', response);
            if (response.success && response.data) {
              this.dishForm.patchValue({ imageUrl: response.data });
              console.log('Form updated with dish-specific cloudinary URL');
            }
          },
          error: (err) => {
            console.error('Error uploading dish image:', err);
            console.error('Error message:', err.message);
            // Don't invalidate the form, as we already have a valid temporary URL
            alert('There was an error uploading your image to the cloud storage, but you can still save the form with a temporary image URL.');
          }
        });
      } else {
        // For new dishes, use the regular upload as we don't have a dish ID yet
        this.imageUploadService.uploadImage(file).subscribe({
          next: (res) => {
            console.log('Image uploaded successfully:', res);
            if (res && res.url) {
              this.dishForm.patchValue({ imageUrl: res.url });
              console.log('Form updated with cloudinary URL');
            }
          },
          error: (err) => {
            console.error('Error uploading image details:', err);
            console.error('Error message:', err.message);
            // Don't invalidate the form, as we already have a valid temporary URL
            alert('There was an error uploading your image to the cloud storage, but you can still save the form with a temporary image URL.');
          }
        });
      }
    }
  }

  // Helper method to update dish image after creation
  private updateNewDishImage(dishId: number, file: File): void {
    // Check if we have a file that was uploaded previously but is still using a temporary URL
    const currentImageUrl = this.dishForm.get('imageUrl')?.value;

    // If it's a blob URL (temporary), we need to upload the file to the dish-specific endpoint
    if (currentImageUrl && currentImageUrl.startsWith('blob:')) {
      // Get the file from input element
      const fileInput = document.querySelector('input[type="file"]') as HTMLInputElement;
      if (fileInput && fileInput.files && fileInput.files.length > 0) {
        const file = fileInput.files[0];
        console.log('Uploading image for newly created dish:', dishId);

        this.imageUploadService.uploadDishImage(file, dishId).subscribe({
          next: (response) => {
            console.log('Image updated for new dish:', response);
            if (response.success) {
              // Find the dish in the array and update its imageUrl
              const dishIndex = this.dishes.findIndex(d => d.id === dishId);
              if (dishIndex !== -1) {
                this.dishes[dishIndex].imageUrl = response.data;
                this.filterDishes();
              }
            }
          },
          error: (err) => {
            console.error('Error updating image for new dish:', err);
            // Dish is already created, so we just log the error
          }
        });
      }
    }
  }

  // Helper method to check form validity
  checkFormValidity() {
    console.log('Form valid:', this.dishForm.valid);
    console.log('Form values:', this.dishForm.value);
    console.log('Form errors:', this.dishForm.errors);

    // Check each control
    Object.keys(this.dishForm.controls).forEach(key => {
      const control = this.dishForm.get(key);
      console.log(`${key} valid:`, control?.valid);
      console.log(`${key} value:`, control?.value);
      console.log(`${key} errors:`, control?.errors);
    });

    // Force mark all fields as touched to show validation errors
    Object.keys(this.dishForm.controls).forEach(key => {
      this.dishForm.get(key)?.markAsTouched();
    });
  }

  saveDish() {
    if (this.dishForm.invalid) return;

    // Check if user has admin role
    if (!this.authService.isAdmin()) {
      alert('You need to be logged in with an Admin account to perform this action.');
      return;
    }

    const formValue = this.dishForm.value;
    const dishData = {
      name: formValue.name,
      description: formValue.description,
      imageUrl: formValue.imageUrl,
      price: parseFloat(formValue.price),
      isAvailable: formValue.isAvailable,
      restaurantId: parseInt(formValue.restaurantId),
      categoryId: parseInt(formValue.categoryId)
    };

    if (this.isEditing && this.currentDishId) {
      // Update existing dish
      const restaurantName = this.restaurants.find(r => r.id === dishData.restaurantId)?.name || '';
      const completeData = {
        ...dishData,
        id: this.currentDishId,
        restaurantName
      };
      this.cmsService.updateDish(this.currentDishId, completeData).subscribe({
        next: (response) => {
          if (response.success) {
            // Update the local list
            const index = this.dishes.findIndex(d => d.id === this.currentDishId);
            if (index !== -1) {
              const restaurantName = this.restaurants.find(r => r.id === dishData.restaurantId)?.name || '';
              const categoryName = this.categories.find(c => c.id === dishData.categoryId)?.name || '';

              this.dishes[index] = {
                ...response.data,
                restaurantName,
                categoryName
              };
              this.filterDishes();
            }
            this.showForm = false;
            this.dishForm.reset();
            this.restaurantCategories = [];
          } else {
            alert(response.errorMessage || 'Failed to update dish');
          }
        },
        error: (err) => {
          console.error('Error updating dish:', err);
          alert('Error updating dish. Please try again.');
        }
      });
    } else {
      // Create new dish
      const restaurantName = this.restaurants.find(r => r.id === dishData.restaurantId)?.name || '';
      const completeDishData = {
        ...dishData,
        id: 0, // This will be assigned by the server
        restaurantName
      };

      this.cmsService.createDish(completeDishData).subscribe({
        next: (response) => {
          if (response.success) {
            // Add to the local list
            const restaurantName = this.restaurants.find(r => r.id === dishData.restaurantId)?.name || '';
            const categoryName = this.categories.find(c => c.id === dishData.categoryId)?.name || '';

            const newDish = {
              ...response.data,
              restaurantName,
              categoryName
            };

            this.dishes.push(newDish);
            this.filterDishes();
            this.showForm = false;
            this.dishForm.reset();
            this.restaurantCategories = [];

            // If there's an image to upload, handle the upload after creation
            const fileInput = document.querySelector('input[type="file"]') as HTMLInputElement;
            if (fileInput && fileInput.files && fileInput.files.length > 0) {
              const file = fileInput.files[0];
              this.updateNewDishImage(response.data.id, file);
            }
          } else {
            alert(response.errorMessage || 'Failed to create dish');
          }
        },
        error: (err) => {
          console.error('Error creating dish:', err);
          let errorMessage = 'Error creating dish';

          if (err.status === 404) {
            errorMessage += ': API endpoint not found. Please check that the server is running and the route is correct.';
          } else if (err.status === 401 || err.status === 403) {
            errorMessage += ': You are not authorized to perform this action. Please check your login status.';
          } else if (err.status === 500) {
            errorMessage += ': Server error. Please check the server logs.';
          } else if (err.error && err.error.message) {
            errorMessage += `: ${err.error.message}`;
          }

          alert(errorMessage);
        }
      });
    }
  }

  deleteDish(dish: any) {
    if (confirm(`Are you sure you want to delete the dish "${dish.name}"?`)) {
      this.cmsService.deleteDish(dish.id).subscribe({
        next: (response) => {
          if (response.success) {
            // Remove from the local list
            this.dishes = this.dishes.filter(d => d.id !== dish.id);
            this.filterDishes();
          } else {
            alert(response.errorMessage || 'Failed to delete dish');
          }
        },
        error: (err) => {
          console.error('Error deleting dish:', err);
          alert('Error deleting dish. Please try again.');
        }
      });
    }
  }

  clearRestaurantFilter() {
    this.selectedRestaurantId = null;
    this.selectedRestaurantName = '';
    localStorage.removeItem('selectedRestaurantId');
    localStorage.removeItem('selectedRestaurantName');
    // Reload all dishes when clearing the filter
    this.loadDishes();
  }
}
