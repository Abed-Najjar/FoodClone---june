import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CmsService } from '../../../services/cms.service';
import { Category } from '../../../models/category.model';
import { ImageUtilService } from '../../../services/image-util.service';
import { ImageUploadService } from '../../../services/image-upload.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './categories.component.html',
  styleUrls: ['./categories.component.css']
})
export class CategoriesComponent implements OnInit {  categories: Category[] = [];
  filteredCategories: Category[] = [];
  restaurants: any[] = [];
  loading: boolean = false;
  error: string | null = null;
  searchTerm: string = '';
  selectedRestaurantId: number | null = null;
  selectedRestaurantName: string = '';

  // Form properties
  categoryForm: FormGroup;
  showForm: boolean = false;
  isEditing: boolean = false;
  currentCategoryId: number | null = null;
  imagePreview: string | null = null;
  categoryImageFile: File | null = null;
  constructor(
    private cmsService: CmsService,
    private fb: FormBuilder,
    private imageUtil: ImageUtilService,
    private imageUploadService: ImageUploadService,
    private activeRoute: ActivatedRoute
  ) {
    this.categoryForm = this.fb.group({
      name: ['', Validators.required],
      description: [''],
      imageUrl: ['', Validators.required],
      restaurantId: ['', Validators.required]
    });
  }  ngOnInit() {


    this.activeRoute.queryParams.subscribe((params) => {
      if (params['restaurantId']) {
        this.loadRestaurants(parseInt(params['restaurantId']));
      } else this.loadRestaurants();
    })



    // Check if we have a selected restaurant from localStorage
    const storedRestaurantId = localStorage.getItem('selectedRestaurantId');
    if (storedRestaurantId) {
      this.selectedRestaurantId = parseInt(storedRestaurantId);
      this.selectedRestaurantName = localStorage.getItem('selectedRestaurantName') || '';
      console.log('Found selected restaurant:', this.selectedRestaurantId, this.selectedRestaurantName);
    }

    // First load restaurants, then categories to ensure we have restaurant names


    // Debug: Log form value changes
    this.categoryForm.valueChanges.subscribe(values => {
      console.log('Form values changed:', values);
    });
  }  loadCategories() {
    this.loading = true;
    this.error = null;

    // If a restaurant ID is selected, use the filtered endpoint
    if (this.selectedRestaurantId) {
      console.log(`Loading categories for restaurant ID: ${this.selectedRestaurantId}`);
      this.cmsService.getCategoriesByRestaurant(this.selectedRestaurantId).subscribe({
        next: (response) => {
          if (response.success) {
            this.categories = response.data.map(category => ({
              ...category,
              description: category.description || '',
              restaurantName: category.restaurantName || this.selectedRestaurantName || 'Unknown restaurant'
            }));
            this.filteredCategories = [...this.categories];
            console.log(`Loaded ${this.categories.length} categories for restaurant ID ${this.selectedRestaurantId}`);
          } else {
            this.error = response.errorMessage || `Failed to load categories for restaurant ID: ${this.selectedRestaurantId}`;
          }
          this.loading = false;
        },
        error: (err) => {
          this.error = `Error loading categories for restaurant ID: ${this.selectedRestaurantId}`;
          this.loading = false;
          console.error('Error loading categories by restaurant:', err);
        }
      });
    } else {
      // Otherwise load all categories
      this.cmsService.getAllCategories().subscribe({
        next: (response) => {
          if (response.success) {
            // Ensure we have restaurant names
            this.categories = response.data.map(category => {
              // Find the restaurant name if available
              const restaurant = this.restaurants.find(r => r.id === category.restaurantId);

              // Debug output to help diagnose the issue
              if (!category.restaurantId) {
                console.log('Category without restaurantId:', category);
              }

              if (!category.description) {
                console.log('Category without description:', category);
              }

              // Create a new object with guaranteed properties
              return {
                ...category,
                description: category.description || '',
                restaurantName: restaurant?.name || 'Unknown restaurant'
              };
            });

            this.filteredCategories = [...this.categories];
            console.log('Loaded categories with restaurant details:', this.categories);

            // Debug the first few entries to see what we're working with
            if (this.categories.length > 0) {
              console.log('Sample category data:', {
                id: this.categories[0].id,
                name: this.categories[0].name,
                description: this.categories[0].description,
                restaurantId: this.categories[0].restaurantId,
                restaurantName: this.categories[0].restaurantName
              });
            }
          } else {
            this.error = response.errorMessage || 'Failed to load categories';
          }
          this.loading = false;
        },
        error: (err) => {
          this.error = 'Error loading categories. Please try again.';
          this.loading = false;
          console.error('Error loading categories:', err);
        }
      });
    }
  }
  loadRestaurants(id?: number) {
    this.cmsService.getAllRestaurants(id).subscribe({
      next: (response) => {
        if (response.success) {
          this.restaurants = response.data;
          console.log('Restaurants loaded successfully:', this.restaurants);
          // Now load categories since we have restaurant data
          this.loadCategories();
        } else {
          console.error('Failed to load restaurants:', response.errorMessage);
        }
      },
      error: (err) => {
        console.error('Error loading restaurants:', err);
      }
    });
  }  filterCategories() {
    // First apply restaurant filter if selected
    let filtered = [...this.categories];

    if (this.selectedRestaurantId) {
      filtered = filtered.filter(category =>
        category.restaurantId === this.selectedRestaurantId
      );
    }

    // Then apply search term filter
    if (this.searchTerm.trim()) {
      const term = this.searchTerm.toLowerCase().trim();
      filtered = filtered.filter(category =>
        category.name.toLowerCase().includes(term)
      );
    }

    this.filteredCategories = filtered;
  }  showAddForm() {
    // First set editing mode to false and clear the ID
    this.isEditing = false;
    this.currentCategoryId = null;

    // Prepare form values, pre-select restaurant if one is filtered
    const formValues: any = {
      name: '',
      description: '',
      imageUrl: '',
      restaurantId: this.selectedRestaurantId ? this.selectedRestaurantId.toString() : ''
    };

    // Clear form values and set default restaurant if selected
    this.categoryForm.reset(formValues);

    // Reset image preview and file
    this.imagePreview = null;
    this.categoryImageFile = null;

    // Now show the form
    this.showForm = true;
  }editCategory(category: Category) {
    console.log('Edit button clicked for category:', category); // Debug logging

  // First set the image preview and prepare data
    this.imagePreview = category.imageUrl;

    console.log('Setting form values for category:', category);    // Set values directly without resetting first - with a delay to ensure component is ready
    setTimeout(() => {
      console.log('Setting form values with category:', JSON.stringify(category));

      // Make sure we have a description value (empty string if null/undefined)
      const description = category.description || '';

      this.categoryForm.setValue({
        name: category.name || '',
        description: description,
        imageUrl: category.imageUrl || '',
        restaurantId: category.restaurantId ? category.restaurantId.toString() : ''
      });

      console.log('Form values directly set:', this.categoryForm.value); // Debug logging

      // Force change detection
      this.categoryForm.markAsDirty();
      this.categoryForm.updateValueAndValidity();
    }, 0);

    // Set editing mode
    this.isEditing = true;
    this.currentCategoryId = category.id;

    // Now show the form
    this.showForm = true;

    // Try to scroll to the form
    setTimeout(() => {
      try {
        const formElement = document.querySelector('.category-form');
        if (formElement) {
          formElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
        } else {
          console.error('Form element not found in DOM');
        }
      } catch (error) {
        console.error('Error scrolling to form:', error);
      }
    }, 100);
  }  cancelForm() {
    this.showForm = false;
    this.isEditing = false; // Make sure to clear editing mode
    this.currentCategoryId = null; // Clear the current category ID
    this.categoryForm.reset({
      name: '',
      description: '',
      imageUrl: '',
      restaurantId: ''
    });
    this.imagePreview = null;
    this.categoryImageFile = null;
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      // Preview immediately
      const reader = new FileReader();
      reader.onload = e => this.imagePreview = reader.result as string;
      reader.readAsDataURL(file);

      // Store the file for later upload
      this.categoryImageFile = file;

      // Set a temporary data URL from the file itself to make the form valid immediately
      // This ensures the form is valid even before the upload completes
      const tempUrl = URL.createObjectURL(file);
      this.categoryForm.patchValue({ imageUrl: tempUrl });
      this.categoryForm.get('imageUrl')?.markAsDirty();
      this.categoryForm.get('imageUrl')?.markAsTouched();
      this.categoryForm.get('imageUrl')?.updateValueAndValidity();

      console.log('Selected image:', file.name);
    }
  }  async saveCategory() {
    if (this.categoryForm.invalid) {
      console.error('Form is invalid:', this.categoryForm.errors);
      alert('Please fill in all required fields correctly.');
      return;
    }

    this.loading = true;
    console.log('Starting category save process...', this.categoryForm.value);
    console.log('Current form description value:', this.categoryForm.get('description')?.value);

    // Handle image upload if a new file is selected
    if (this.categoryImageFile) {
      try {
        console.log('Uploading image file:', this.categoryImageFile.name);
        await new Promise<void>((resolve, reject) => {
          this.imageUploadService.uploadImage(this.categoryImageFile!).subscribe({
            next: (response) => {
              console.log('Image upload successful:', response);
              if (response && response.url) {
                // Update form with the new cloud URL
                this.categoryForm.patchValue({ imageUrl: response.url });
                resolve();
              } else {
                reject('Invalid response from image upload service');
              }
            },
            error: (err) => {
              console.error('Error uploading image:', err);
              reject(err);
            }
          });
        });
      } catch (error) {
        console.error('Image upload failed:', error);
        alert('Failed to upload the image. Please try again.');
        this.loading = false;
        return;
      }
    }

    const formValue = this.categoryForm.value;
    console.log('Form values to be sent:', formValue);

    // Ensure all required fields are present and correctly typed
    const categoryData: Category = {
      id: this.isEditing ? this.currentCategoryId! : 0,
      name: formValue.name || '',
      description: formValue.description || '', // Make sure description is never undefined
      imageUrl: formValue.imageUrl || '',
      restaurantId: parseInt(formValue.restaurantId) || 0,
      restaurantName: '' // Will be updated after creation/update
    };

    console.log('Category data prepared:', categoryData);

    if (this.isEditing && this.currentCategoryId) {
      // Update existing category
      const restaurantName = this.restaurants.find(r => r.id === categoryData.restaurantId)?.name || '';
      categoryData.restaurantName = restaurantName;      this.cmsService.updateCategory(this.currentCategoryId, categoryData).subscribe({
        next: (response) => {
          this.loading = false;
          if (response.success) {
            // Find the category in the array and update it
            const index = this.categories.findIndex(c => c.id === this.currentCategoryId);
            if (index !== -1) {
              // Get the restaurant name from our restaurants list
              const restaurantName = this.restaurants.find(r => r.id === categoryData.restaurantId)?.name || '';
              console.log('Found restaurant name for update:', restaurantName, 'for ID:', categoryData.restaurantId);

              // Create a complete updated object
              const updatedCategory = {
                ...categoryData,
                restaurantName: restaurantName,
                description: categoryData.description || ''
              };

              console.log('Original category:', this.categories[index]);
              console.log('Updated category:', updatedCategory);

              // Replace the category in the array
              this.categories[index] = updatedCategory;

              // Create a new array to ensure change detection
              this.categories = [...this.categories];
              this.filterCategories();
            }

            this.showForm = false;
            this.categoryForm.reset();
            this.imagePreview = null;
            this.categoryImageFile = null;
            alert('Category updated successfully!');
          } else {
            alert(response.errorMessage || 'Failed to update category');
          }
        },
        error: (err) => {
          this.loading = false;
          console.error('Error updating category:', err);
          alert('Error updating category. Please try again.');
        }
      });
    } else {      // Create new category
      this.cmsService.createCategory(categoryData).subscribe({
        next: (response) => {
          if (response.success) {
            console.log('Category created API response:', response.data);

            // Get restaurant name from our loaded restaurants data
            const restaurantName = this.restaurants.find(r => r.id === categoryData.restaurantId)?.name || '';
            console.log('Found restaurant name:', restaurantName, 'for ID:', categoryData.restaurantId);

            // Create a fully populated category object
            const newCategory = {
              ...response.data,
              restaurantName: restaurantName,
              description: categoryData.description || '' // Prioritize our locally-entered description
            };

            console.log('New category created with all fields:', newCategory);
            this.categories.push(newCategory);
            this.filterCategories();
            this.showForm = false;
            this.categoryForm.reset();
            this.imagePreview = null;
            this.categoryImageFile = null;
            alert('Category created successfully!');
          } else {
            alert(response.errorMessage || 'Failed to create category');
          }
        },        error: (err) => {
          console.error('Error creating category:', err);
          console.error('Error details:', JSON.stringify(err, null, 2));
          alert('Error creating category: ' + (err.error?.message || err.message || 'Please try again.'));
        },
        complete: () => {
          this.loading = false;
        }
      });
    }
  }

  deleteCategory(category: Category) {
    if (confirm(`Are you sure you want to delete the category "${category.name}"?`)) {
      this.cmsService.deleteCategory(category.id).subscribe({
        next: (response) => {
          if (response.success) {
            // Remove from the local list
            this.categories = this.categories.filter(c => c.id !== category.id);
            this.filterCategories();
          } else {
            alert(response.errorMessage || 'Failed to delete category');
          }
        },
        error: (err) => {
          console.error('Error deleting category:', err);
          alert('Error deleting category. Please try again.');
        }
      });
    }
  }
  clearRestaurantFilter() {
    this.selectedRestaurantId = null;
    this.selectedRestaurantName = '';
    localStorage.removeItem('selectedRestaurantId');
    localStorage.removeItem('selectedRestaurantName');
    // Reload all categories when clearing the filter
    this.loadRestaurants();
  }
}
