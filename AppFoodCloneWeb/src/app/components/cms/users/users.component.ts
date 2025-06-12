import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CmsService } from '../../../services/cms.service';
import { AuthService } from '../../../services/auth.service';
import { User, UserInputDto } from '../../../models/user.model';
import { PagedResult, PaginationParams } from '../../../types/pagination.interface';
import { PaginationComponent } from '../../shared/pagination/pagination.component';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, PaginationComponent],
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit {
  users: User[] = [];
  filteredUsers: User[] = [];
  loading: boolean = false;
  error: string | null = null;
  searchTerm: string = '';
  roleFilter: string = '';

  // Pagination properties
  usersPagedResult: PagedResult<User> | null = null;
  usersPagination: PaginationParams = { pageNumber: 1, pageSize: 4 };

  // Edit modal properties
  isEditModalOpen: boolean = false;
  editingUser: User | null = null;
  userForm: FormGroup;

  // Create modal properties
  isCreateModalOpen: boolean = false;

  // Available roles
  availableRoles = ['User', 'Employee', 'Provider', 'Admin'];

  constructor(
    private cmsService: CmsService,
    private authService: AuthService,
    private fb: FormBuilder
  ) {
    this.userForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
      lastName: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
      email: ['', [Validators.required, Validators.email]],
      role: ['User', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(100)]],
      address: [[]]
    });
  }

  ngOnInit() {
    this.loadUsers();
  }

  loadUsers() {
    this.loading = true;
    this.error = null;

    // Use getAllUsersWithAllRoles to include admin users
    this.cmsService.getAllUsersWithAllRoles(this.usersPagination).subscribe({
      next: (response) => {
        if (response.success) {
          this.usersPagedResult = response.data as PagedResult<User>;
          this.users = this.usersPagedResult.data;
          this.filteredUsers = [...this.users];
        } else {
          this.error = response.errorMessage || 'Failed to load users';
        }
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Error loading users. Please try again.';
        this.loading = false;
        console.error('Error loading users:', err);
      }
    });
  }

  filterUsers() {
    let filtered = [...this.users];

    // Apply search term filter
    if (this.searchTerm.trim()) {
      const term = this.searchTerm.toLowerCase().trim();
      filtered = filtered.filter(user =>
        user.id.toString().includes(term) ||
        (user.firstName && user.firstName.toLowerCase().includes(term)) ||
        (user.lastName && user.lastName.toLowerCase().includes(term)) ||
        (user.email && user.email.toLowerCase().includes(term)) ||
        (user.rolename && user.rolename.toLowerCase().includes(term))
      );
    }

    // Apply role filter
    if (this.roleFilter) {
      filtered = filtered.filter(user =>
        user.rolename.toLowerCase() === this.roleFilter.toLowerCase()
      );
    }

    this.filteredUsers = filtered;
  }

  onUsersPageChanged(page: number): void {
    this.usersPagination.pageNumber = page;
    this.loadUsers();
  }

  // Check if current user can edit/delete another user
  canModifyUser(targetUser: User): boolean {
    const currentUser = this.authService.getCurrentUser();
    if (!currentUser || !this.authService.isAdmin()) {
      return false;
    }

    // An admin can't modify another admin (including themselves for delete)
    return targetUser.rolename.toLowerCase() !== 'admin';
  }

  // Create user methods
  openCreateModal(): void {
    this.isCreateModalOpen = true;
    this.userForm.reset();
    this.userForm.patchValue({
      role: 'User',
      address: []
    });
  }

  closeCreateModal(): void {
    this.isCreateModalOpen = false;
    this.userForm.reset();
  }

  createUser(): void {
    if (this.userForm.invalid) {
      this.markFormGroupTouched(this.userForm);
      return;
    }

    const formValue = this.userForm.value;
    const userInputDto: UserInputDto = {
      firstName: formValue.firstName,
      lastName: formValue.lastName,
      email: formValue.email,
      role: formValue.role,
      password: formValue.password,
      address: formValue.address || []
    };

    this.cmsService.createUser(userInputDto).subscribe({
      next: (response) => {
        if (response.success) {
          this.closeCreateModal();
          this.loadUsers();
          alert('User created successfully!');
        } else {
          this.error = response.errorMessage || 'Failed to create user';
        }
      },
      error: (err) => {
        this.error = 'Error creating user. Please try again.';
        console.error('Error creating user:', err);
      }
    });
  }

  // Edit user methods
  editUser(user: User): void {
    if (!this.canModifyUser(user)) {
      alert('You cannot modify admin users.');
      return;
    }

    this.editingUser = user;
    this.isEditModalOpen = true;
    
    this.userForm.patchValue({
      firstName: user.firstName,
      lastName: user.lastName,
      email: user.email,
      role: user.rolename,
      password: '', // Don't pre-fill password
      address: user.address || []
    });
  }

  closeEditModal(): void {
    this.isEditModalOpen = false;
    this.editingUser = null;
    this.userForm.reset();
  }

  updateUser(): void {
    if (this.userForm.invalid || !this.editingUser) {
      this.markFormGroupTouched(this.userForm);
      return;
    }

    const formValue = this.userForm.value;
    const userInputDto: UserInputDto = {
      firstName: formValue.firstName,
      lastName: formValue.lastName,
      email: formValue.email,
      role: formValue.role,
      password: formValue.password,
      address: formValue.address || []
    };

    this.cmsService.updateUser(this.editingUser.id, userInputDto).subscribe({
      next: (response) => {
        if (response.success) {
          this.closeEditModal();
          this.loadUsers();
          alert('User updated successfully!');
        } else {
          this.error = response.errorMessage || 'Failed to update user';
        }
      },
      error: (err) => {
        this.error = 'Error updating user. Please try again.';
        console.error('Error updating user:', err);
      }
    });
  }

  // Toggle user status method
  toggleUserStatus(user: User): void {
    if (!this.canModifyUser(user)) {
      return; // Don't show alert, just ignore the click for disabled states
    }

    const action = user.isActive ? 'deactivate' : 'activate';
    const confirmMessage = `Are you sure you want to ${action} user "${user.firstName} ${user.lastName}"?`;
    
    if (confirm(confirmMessage)) {
      this.cmsService.toggleUserStatus(user.id).subscribe({
        next: (response) => {
          if (response.success) {
            this.loadUsers();
            alert(`User ${action}d successfully!`);
          } else {
            alert(`Failed to ${action} user: ${response.errorMessage}`);
            this.error = response.errorMessage || `Failed to ${action} user`;
          }
        },
        error: (err) => {
          const errorMessage = err.error?.errorMessage || err.message || `An unexpected error occurred while ${action}ing the user.`;
          alert(`Error ${action}ing user: ${errorMessage}`);
          this.error = errorMessage;
          console.error(`Error ${action}ing user:`, err);
        }
      });
    }
  }

  // Delete user method
  deleteUser(user: User): void {
    if (!this.canModifyUser(user)) {
      alert('You cannot delete admin users.');
      return;
    }

    const confirmMessage = `Are you sure you want to delete user "${user.firstName} ${user.lastName}"?\n\nWarning: This action cannot be undone. If this user has orders or other associated data, the deletion may fail.`;
    
    if (confirm(confirmMessage)) {
      this.cmsService.deleteUser(user.id).subscribe({
        next: (response) => {
          if (response.success) {
            this.loadUsers();
            alert('User deleted successfully!');
          } else {
            // Show specific error message from backend
            alert(`Failed to delete user: ${response.errorMessage}`);
            this.error = response.errorMessage || 'Failed to delete user';
          }
        },
        error: (err) => {
          const errorMessage = err.error?.errorMessage || err.message || 'An unexpected error occurred while deleting the user.';
          alert(`Error deleting user: ${errorMessage}`);
          this.error = errorMessage;
          console.error('Error deleting user:', err);
        }
      });
    }
  }

  // Helper method to mark form controls as touched
  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();
    });
  }

  // Helper method to check if a form field has errors and is touched
  isFieldInvalid(fieldName: string): boolean {
    const field = this.userForm.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }

  // Helper method to get form field error message
  getFieldError(fieldName: string): string {
    const field = this.userForm.get(fieldName);
    if (field?.errors && field.touched) {
      if (field.errors['required']) return `${fieldName} is required`;
      if (field.errors['email']) return 'Please enter a valid email';
      if (field.errors['minlength']) return `${fieldName} must be at least ${field.errors['minlength'].requiredLength} characters`;
      if (field.errors['maxlength']) return `${fieldName} cannot exceed ${field.errors['maxlength'].requiredLength} characters`;
    }
    return '';
  }
}
