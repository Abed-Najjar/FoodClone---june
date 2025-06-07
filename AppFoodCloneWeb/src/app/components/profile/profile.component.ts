import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { UserService } from '../../services/user.service';
import { User, UserProfileUpdate } from '../../models/user.model';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent implements OnInit {
  profileForm: FormGroup;
  currentUser: User | null = null;
  isEditing = false;
  isLoading = false;
  error = '';
  success = '';
  profileImageUrl = '';
  selectedFile: File | null = null;
  imagePreview: string | null = null;
  userStatistics = {
    totalOrders: 0,
    favoriteRestaurants: 0,
    reviewsWritten: 0,
    savedAddresses: 0
  };

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private userService: UserService,
    private router: Router
  ) {
    this.profileForm = this.formBuilder.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', [Validators.pattern(/^\+?[\d\s\-\(\)]+$/)]],
      dateOfBirth: [''],
      gender: [''],
      address: [''],
      bio: ['', [Validators.maxLength(500)]]
    });
  }

  ngOnInit(): void {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login']);
      return;
    }

    this.currentUser = this.authService.getCurrentUser();
    if (this.currentUser) {
      this.loadUserProfile();
      this.loadUserStatistics();
    }
  }

  loadUserProfile(): void {
    this.isLoading = true;
    this.error = '';

    // Try to get fresh data from backend first
    this.userService.getUserProfile().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.currentUser = response.data;
          this.authService.updateCurrentUserProfile(response.data);
          this.populateForm();
        } else {
          // Fall back to local data if API fails
          this.populateForm();
        }
        this.isLoading = false;
      },
      error: (error) => {
        console.warn('Failed to load profile from backend:', error);
        // Fall back to local data
        this.populateForm();
        this.isLoading = false;
      }
    });
  }

  populateForm(): void {
    if (!this.currentUser) return;

    // Load profile data into form
    this.profileForm.patchValue({
      firstName: this.currentUser.firstName || '',
      lastName: this.currentUser.lastName || '',
      email: this.currentUser.email || '',
      phone: this.currentUser.phoneNumber || '',
      dateOfBirth: this.currentUser.dateOfBirth || '',
      gender: this.currentUser.gender || '',
      address: this.getAddressString() === 'Not provided' ? '' : this.getAddressString(),
      bio: this.currentUser.bio || ''
    });

    // Set profile image
    this.profileImageUrl = this.getProfileImageUrl();
  }

  loadUserStatistics(): void {
    this.userService.getUserStatistics().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.userStatistics = response.data;
        }
      },
      error: (error) => {
        console.warn('Failed to load user statistics:', error);
        // Keep default values
      }
    });
  }

  getProfileImageUrl(): string {
    // Return user's profile image from backend if available
    if (this.currentUser?.profileImageUrl) {
      return this.currentUser.profileImageUrl;
    }
    
    // Fallback to generated avatar if no profile image
    if (this.currentUser && this.currentUser.firstName) {
      // Generate a consistent avatar based on first and last name
      const avatarColors = [
        '#3498db', '#e74c3c', '#2ecc71', '#f39c12', '#9b59b6',
        '#1abc9c', '#34495e', '#e67e22', '#95a5a6', '#27ae60'
      ];
      const fullName = `${this.currentUser.firstName} ${this.currentUser.lastName}`;
      const colorIndex = this.currentUser.firstName.charCodeAt(0) % avatarColors.length;
      const initials = this.getInitials(fullName);
      
      return `https://ui-avatars.com/api/?name=${encodeURIComponent(initials)}&background=${avatarColors[colorIndex].substring(1)}&color=fff&size=200&font-size=0.6`;
    }
    return 'https://ui-avatars.com/api/?name=User&background=3498db&color=fff&size=200&font-size=0.6';
  }

  getInitials(name: string): string {
    return name
      .split(' ')
      .map(word => word.charAt(0))
      .join('')
      .toUpperCase()
      .substring(0, 2);
  }

  toggleEdit(): void {
    this.isEditing = !this.isEditing;
    this.error = '';
    this.success = '';

    if (!this.isEditing) {
      // Reset form when canceling edit
      this.loadUserProfile();
      this.selectedFile = null;
      this.imagePreview = null;
    }
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      // Validate file type
      if (!file.type.startsWith('image/')) {
        this.error = 'Please select a valid image file.';
        return;
      }

      // Validate file size (max 5MB)
      if (file.size > 5 * 1024 * 1024) {
        this.error = 'Image size should be less than 5MB.';
        return;
      }

      this.selectedFile = file;
      this.error = '';

      // Create preview
      const reader = new FileReader();
      reader.onload = () => {
        this.imagePreview = reader.result as string;
      };
      reader.readAsDataURL(file);
    }
  }

  removeImage(): void {
    this.selectedFile = null;
    this.imagePreview = null;
    // Reset file input
    const fileInput = document.getElementById('profileImage') as HTMLInputElement;
    if (fileInput) {
      fileInput.value = '';
    }
  }

  onSubmit(): void {
    if (this.profileForm.valid) {
      this.isLoading = true;
      this.error = '';
      this.success = '';

      const formData = this.profileForm.value;
      
      // Prepare profile update data
      const profileUpdateData: UserProfileUpdate = {
        firstName: formData.firstName,
        lastName: formData.lastName,
        email: formData.email,
        phone: formData.phone,
        dateOfBirth: formData.dateOfBirth,
        gender: formData.gender,
        bio: formData.bio,
        address: formData.address
      };

      // If there's a new image, upload it first
      if (this.selectedFile) {
        this.uploadImageAndUpdateProfile(profileUpdateData);
      } else {
        this.updateProfile(profileUpdateData);
      }
    } else {
      // Mark all fields as touched to show validation errors
      this.markFormGroupTouched(this.profileForm);
    }
  }

  private uploadImageAndUpdateProfile(profileData: UserProfileUpdate): void {
    const formData = new FormData();
    formData.append('image', this.selectedFile!, this.selectedFile!.name);

    this.userService.uploadProfileImage(formData).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          // Update profile image URL in the current user data
          if (this.currentUser) {
            this.currentUser.profileImageUrl = response.data.imageUrl;
          }
          // Continue with profile update
          this.updateProfile(profileData);
        } else {
          this.error = response.errorMessage || 'Failed to upload profile image.';
          this.isLoading = false;
        }
      },
      error: (error) => {
        console.error('Error uploading profile image:', error);
        this.error = 'Failed to upload profile image. Please try again.';
        this.isLoading = false;
      }
    });
  }

  private updateProfile(profileData: UserProfileUpdate): void {
    this.userService.updateProfile(profileData).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          // Update current user with new data
          this.currentUser = response.data;
          this.authService.updateCurrentUserProfile(response.data);
          
          // Update profile image if we have one
          if (this.imagePreview) {
            this.profileImageUrl = this.imagePreview;
          }

          this.success = 'Profile updated successfully!';
          this.isEditing = false;
          this.selectedFile = null;
          this.imagePreview = null;

          // Clear success message after 3 seconds
          setTimeout(() => {
            this.success = '';
          }, 3000);

          // Reload statistics in case they changed
          this.loadUserStatistics();
        } else {
          this.error = response.errorMessage || 'Failed to update profile.';
        }
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error updating profile:', error);
        
        // Handle specific error cases
        if (error.status === 400) {
          this.error = 'Invalid profile data. Please check your inputs.';
        } else if (error.status === 401) {
          this.error = 'You are not authorized to perform this action. Please log in again.';
          this.authService.logout();
          this.router.navigate(['/login']);
        } else if (error.status === 409) {
          this.error = 'Username or email already exists. Please choose different values.';
        } else {
          this.error = 'Failed to update profile. Please try again later.';
        }
        
        this.isLoading = false;
      }
    });
  }

  markFormGroupTouched(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      control?.markAsTouched({ onlySelf: true });
    });
  }

  // Getter methods for form validation
  get firstName() { return this.profileForm.get('firstName'); }
  get lastName() { return this.profileForm.get('lastName'); }
  get email() { return this.profileForm.get('email'); }
  get phone() { return this.profileForm.get('phone'); }
  get bio() { return this.profileForm.get('bio'); }

  // Helper method to check if field has error
  hasError(fieldName: string, errorType: string): boolean {
    const field = this.profileForm.get(fieldName);
    return !!(field && field.errors && field.errors[errorType] && field.touched);
  }

  // Method to get error message
  getErrorMessage(fieldName: string): string {
    const field = this.profileForm.get(fieldName);
    if (field && field.errors && field.touched) {
      if (field.errors['required']) return `${fieldName} is required`;
      if (field.errors['email']) return 'Please enter a valid email address';
      if (field.errors['minlength']) return `${fieldName} must be at least ${field.errors['minlength'].requiredLength} characters`;
      if (field.errors['maxlength']) return `${fieldName} cannot exceed ${field.errors['maxlength'].requiredLength} characters`;
      if (field.errors['pattern']) return 'Please enter a valid phone number';
    }
    return '';
  }

  changePassword(): void {
    // Navigate to change password page or show modal
    this.router.navigate(['/change-password']);
  }

  deleteAccount(): void {
    if (confirm('Are you sure you want to delete your account? This action cannot be undone.')) {
      this.isLoading = true;
      this.error = '';

      this.userService.deleteAccount().subscribe({
        next: (response) => {
          if (response.success) {
            // Account deleted successfully
            alert('Your account has been deleted successfully.');
            this.authService.logout();
            this.router.navigate(['/login']);
          } else {
            this.error = response.errorMessage || 'Failed to delete account.';
            this.isLoading = false;
          }
        },
        error: (error) => {
          console.error('Error deleting account:', error);
          if (error.status === 401) {
            this.error = 'You are not authorized to perform this action. Please log in again.';
            this.authService.logout();
            this.router.navigate(['/login']);
          } else {
            this.error = 'Failed to delete account. Please try again later.';
          }
          this.isLoading = false;
        }
      });
    }
  }

  // Helper methods for template
  isArray(value: any): boolean {
    return Array.isArray(value);
  }

  getFormattedMemberSince(): string {
    if (this.currentUser?.created) {
      const date = new Date(this.currentUser.created);
      return date.toLocaleDateString('en-US', { month: 'short', year: 'numeric' });
    }
    return 'Recently';
  }

  getAddressCount(): number {
    if (!this.currentUser?.address) return 0;
    return Array.isArray(this.currentUser.address) ? this.currentUser.address.length : 1;
  }

  getAddressString(): string {
    if (!this.currentUser?.address) return 'Not provided';
    return Array.isArray(this.currentUser.address) ? 
      this.currentUser.address.join(', ') : 
      this.currentUser.address;
  }
}
