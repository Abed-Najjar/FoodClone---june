import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CmsService } from '../../../services/cms.service';
import { Category } from '../../../models/category.model';
import { ImageUtilService } from '../../../services/image-util.service';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  template: `
    <div class="categories-manager">
      <h2>Categories Management</h2>

      <div class="action-bar">
        <button (click)="showAddForm()" class="add-btn">Add New Category</button>
        <div class="search-container">
          <input type="text" [(ngModel)]="searchTerm" placeholder="Search categories..." class="search-input" (input)="filterCategories()">
        </div>
      </div>

      <!-- Category Form -->
      <div *ngIf="showForm" class="category-form">
        <h3>{{ isEditing ? 'Edit' : 'Add' }} Category</h3>
        <form [formGroup]="categoryForm" (ngSubmit)="saveCategory()">
          <div class="form-group">
            <label for="name">Name</label>
            <input type="text" id="name" formControlName="name" class="form-control">
            <div *ngIf="categoryForm.get('name')?.errors && categoryForm.get('name')?.touched" class="error-message">
              Name is required
            </div>
          </div>

          <div class="form-group">
            <label for="description">Description</label>
            <textarea id="description" formControlName="description" class="form-control"></textarea>
          </div>

          <div class="form-group">
            <label for="imageUrl">Image URL</label>
            <input type="text" id="imageUrl" formControlName="imageUrl" class="form-control">
            <div *ngIf="categoryForm.get('imageUrl')?.errors && categoryForm.get('imageUrl')?.touched" class="error-message">
              Image URL is required
            </div>
          </div>

          <div class="form-group">
            <label for="restaurantId">Restaurant</label>
            <select id="restaurantId" formControlName="restaurantId" class="form-control">
              <option value="">Select Restaurant</option>
              <option *ngFor="let restaurant of restaurants" [value]="restaurant.id">
                {{ restaurant.name }}
              </option>
            </select>
            <div *ngIf="categoryForm.get('restaurantId')?.errors && categoryForm.get('restaurantId')?.touched" class="error-message">
              Restaurant is required
            </div>
          </div>

          <div class="form-actions">
            <button type="submit" [disabled]="categoryForm.invalid" class="save-btn">Save</button>
            <button type="button" (click)="cancelForm()" class="cancel-btn">Cancel</button>
          </div>
        </form>
      </div>

      <!-- Categories List -->
      <div *ngIf="!showForm" class="categories-list">
        <div *ngIf="loading" class="loading">Loading categories...</div>
        <div *ngIf="error" class="error-message">{{ error }}</div>

        <table *ngIf="!loading && !error && filteredCategories.length > 0">
          <thead>
            <tr>
              <th>ID</th>
              <th>Image</th>
              <th>Name</th>
              <th>Description</th>
              <th>Restaurant</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let category of filteredCategories">
              <td>{{ category.id }}</td>
              <td>
                <img [src]="category.imageUrl" alt="{{ category.name }}" class="category-image">
              </td>
              <td>{{ category.name }}</td>
              <td>{{ category.description || 'No description' }}</td>
              <td>{{ category.restaurantName }}</td>
              <td class="actions">
                <button (click)="editCategory(category)" class="edit-btn">Edit</button>
                <button (click)="deleteCategory(category)" class="delete-btn">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>

        <div *ngIf="!loading && !error && filteredCategories.length === 0" class="no-data">
          No categories found. Add a new category to get started.
        </div>
      </div>
    </div>
  `,
  styles: [`
    .categories-manager {
      padding: 1rem;
      max-width: 100%;
    }

    h2 {
      margin-bottom: 1.5rem;
      color: #333;
    }

    .action-bar {
      display: flex;
      justify-content: space-between;
      margin-bottom: 1.5rem;
    }

    .add-btn {
      background-color: #4caf50;
      color: white;
      border: none;
      padding: 0.5rem 1rem;
      border-radius: 4px;
      cursor: pointer;
    }

    .search-input {
      padding: 0.5rem;
      border: 1px solid #ddd;
      border-radius: 4px;
      width: 250px;
    }

    .category-form {
      background-color: #f9f9f9;
      padding: 1.5rem;
      border-radius: 8px;
      margin-bottom: 2rem;
      box-shadow: 0 2px 10px rgba(0,0,0,0.1);
    }

    .form-group {
      margin-bottom: 1rem;
    }

    label {
      display: block;
      margin-bottom: 0.5rem;
      font-weight: 500;
    }

    .form-control {
      width: 100%;
      padding: 0.5rem;
      border: 1px solid #ddd;
      border-radius: 4px;
      font-size: 1rem;
    }

    textarea.form-control {
      min-height: 100px;
      resize: vertical;
    }

    .error-message {
      color: #f44336;
      font-size: 0.875rem;
      margin-top: 0.25rem;
    }

    .form-actions {
      display: flex;
      gap: 1rem;
      margin-top: 1.5rem;
    }

    .save-btn {
      background-color: #2196f3;
      color: white;
      border: none;
      padding: 0.5rem 1rem;
      border-radius: 4px;
      cursor: pointer;
    }

    .save-btn:disabled {
      background-color: #b0bec5;
      cursor: not-allowed;
    }

    .cancel-btn {
      background-color: #f5f5f5;
      color: #333;
      border: 1px solid #ddd;
      padding: 0.5rem 1rem;
      border-radius: 4px;
      cursor: pointer;
    }

    table {
      width: 100%;
      border-collapse: collapse;
      margin-top: 1rem;
      background-color: white;
      box-shadow: 0 2px 10px rgba(0,0,0,0.1);
      border-radius: 8px;
      overflow: hidden;
    }

    th, td {
      padding: 1rem;
      text-align: left;
      border-bottom: 1px solid #eee;
    }

    th {
      background-color: #f5f5f5;
      font-weight: 600;
    }

    .category-image {
      width: 60px;
      height: 60px;
      object-fit: cover;
      border-radius: 4px;
    }

    .actions {
      display: flex;
      gap: 0.5rem;
    }

    .edit-btn {
      background-color: #2196f3;
      color: white;
      border: none;
      padding: 0.25rem 0.5rem;
      border-radius: 4px;
      cursor: pointer;
    }

    .delete-btn {
      background-color: #f44336;
      color: white;
      border: none;
      padding: 0.25rem 0.5rem;
      border-radius: 4px;
      cursor: pointer;
    }

    .loading, .no-data {
      text-align: center;
      padding: 2rem;
      color: #757575;
    }
  `]
})
export class CategoriesComponent implements OnInit {
  categories: Category[] = [];
  filteredCategories: Category[] = [];
  restaurants: any[] = [];
  loading: boolean = false;
  error: string | null = null;
  searchTerm: string = '';

  // Form properties
  categoryForm: FormGroup;
  showForm: boolean = false;
  isEditing: boolean = false;
  currentCategoryId: number | null = null;

  constructor(
    private cmsService: CmsService,
    private fb: FormBuilder,
    private imageUtil: ImageUtilService
  ) {
    this.categoryForm = this.fb.group({
      name: ['', Validators.required],
      description: [''],
      imageUrl: ['', Validators.required],
      restaurantId: ['', Validators.required]
    });
  }

  ngOnInit() {
    this.loadCategories();
    this.loadRestaurants();
  }

  loadCategories() {
    this.loading = true;
    this.error = null;

    this.cmsService.getAllCategories().subscribe({
      next: (response) => {
        if (response.success) {
          this.categories = response.data;
          this.filteredCategories = [...this.categories];
        } else {
          this.error = response.message || 'Failed to load categories';
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

  loadRestaurants() {
    this.cmsService.getAllRestaurants().subscribe({
      next: (response) => {
        if (response.success) {
          this.restaurants = response.data;
        } else {
          console.error('Failed to load restaurants:', response.message);
        }
      },
      error: (err) => {
        console.error('Error loading restaurants:', err);
      }
    });
  }

  filterCategories() {
    if (!this.searchTerm.trim()) {
      this.filteredCategories = [...this.categories];
      return;
    }

    const term = this.searchTerm.toLowerCase().trim();
    this.filteredCategories = this.categories.filter(category =>
      category.name.toLowerCase().includes(term) ||
      (category.description && category.description.toLowerCase().includes(term)) ||
      category.restaurantName.toLowerCase().includes(term)
    );
  }

  showAddForm() {
    this.isEditing = false;
    this.currentCategoryId = null;
    this.categoryForm.reset();
    this.showForm = true;
  }

  editCategory(category: Category) {
    this.isEditing = true;
    this.currentCategoryId = category.id;
    this.categoryForm.patchValue({
      name: category.name,
      description: category.description || '',
      imageUrl: category.imageUrl,
      restaurantId: category.restaurantId.toString()
    });
    this.showForm = true;
  }

  cancelForm() {
    this.showForm = false;
    this.categoryForm.reset();
  }

  saveCategory() {
    if (this.categoryForm.invalid) return;

    const formValue = this.categoryForm.value;      const categoryData: Category = {
        id: 0, // Temporary ID, will be replaced by the server
        name: formValue.name,
        description: formValue.description,
        imageUrl: formValue.imageUrl,
        restaurantId: Number(formValue.restaurantId),
        restaurantName: '' // Temporary, will be updated after creation
      };

    if (this.isEditing && this.currentCategoryId) {
      // Update existing category
      const restaurantName = this.restaurants.find(r => r.id === categoryData.restaurantId)?.name || '';
      const categoryToUpdate: Category = {
        id: this.currentCategoryId,
        name: formValue.name,
        description: formValue.description,
        imageUrl: formValue.imageUrl,
        restaurantId: categoryData.restaurantId,
        restaurantName: restaurantName
      };

      this.cmsService.updateCategory(this.currentCategoryId, categoryToUpdate).subscribe({
        next: (response) => {
          if (response.success) {
            // Update the local list
            const index = this.categories.findIndex(c => c.id === this.currentCategoryId);
            if (index !== -1) {
              const restaurantName = this.restaurants.find(r => r.id === categoryData.restaurantId)?.name || '';
              this.categories[index] = {
                ...response.data,
                restaurantName
              };
              this.filterCategories();
            }
            this.showForm = false;
            this.categoryForm.reset();
          } else {
            alert(response.message || 'Failed to update category');
          }
        },
        error: (err) => {
          console.error('Error updating category:', err);
          alert('Error updating category. Please try again.');
        }
      });
    } else {
      // Create new category
      this.cmsService.createCategory(categoryData).subscribe({
        next: (response) => {
          if (response.success) {
            // Add to the local list
            const restaurantName = this.restaurants.find(r => r.id === categoryData.restaurantId)?.name || '';
            const newCategory = {
              ...response.data,
              restaurantName
            };
            this.categories.push(newCategory);
            this.filterCategories();
            this.showForm = false;
            this.categoryForm.reset();
          } else {
            alert(response.message || 'Failed to create category');
          }
        },
        error: (err) => {
          console.error('Error creating category:', err);
          alert('Error creating category. Please try again.');
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
            alert(response.message || 'Failed to delete category');
          }
        },
        error: (err) => {
          console.error('Error deleting category:', err);
          alert('Error deleting category. Please try again.');
        }
      });
    }
  }
}
