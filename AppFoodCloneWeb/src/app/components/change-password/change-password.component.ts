import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';
import { PasswordChangeRequest } from '../../models/user.model';

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './change-password.component.html',
  styleUrl: './change-password.component.css'
})
export class ChangePasswordComponent implements OnInit {
  passwordForm: FormGroup;
  isLoading = false;
  error = '';
  success = '';

  constructor(
    private formBuilder: FormBuilder,
    private userService: UserService,
    private authService: AuthService,
    private router: Router
  ) {
    this.passwordForm = this.formBuilder.group({
      currentPassword: ['', [Validators.required]],
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]]
    }, {
      validators: this.passwordMatchValidator
    });
  }

  ngOnInit(): void {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login']);
    }
  }

  passwordMatchValidator(form: FormGroup) {
    const newPassword = form.get('newPassword');
    const confirmPassword = form.get('confirmPassword');
    
    if (newPassword && confirmPassword && newPassword.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    
    if (confirmPassword?.errors?.['passwordMismatch']) {
      delete confirmPassword.errors['passwordMismatch'];
      if (Object.keys(confirmPassword.errors).length === 0) {
        confirmPassword.setErrors(null);
      }
    }
    
    return null;
  }

  onSubmit(): void {
    if (this.passwordForm.valid) {
      this.isLoading = true;
      this.error = '';
      this.success = '';

      const formData = this.passwordForm.value;
      const passwordChangeData: PasswordChangeRequest = {
        currentPassword: formData.currentPassword,
        newPassword: formData.newPassword,
        confirmPassword: formData.confirmPassword
      };

      this.userService.changePassword(passwordChangeData).subscribe({
        next: (response) => {
          if (response.success) {
            this.success = 'Password changed successfully!';
            this.passwordForm.reset();
            
            // Redirect to profile after 2 seconds
            setTimeout(() => {
              this.router.navigate(['/profile']);
            }, 2000);
          } else {
            this.error = response.errorMessage || 'Failed to change password.';
          }
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error changing password:', error);
          
          if (error.status === 400) {
            this.error = 'Current password is incorrect or new password is invalid.';
          } else if (error.status === 401) {
            this.error = 'You are not authorized. Please log in again.';
            this.authService.logout();
            this.router.navigate(['/login']);
          } else {
            this.error = 'Failed to change password. Please try again later.';
          }
          
          this.isLoading = false;
        }
      });
    } else {
      this.markFormGroupTouched(this.passwordForm);
    }
  }

  markFormGroupTouched(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      control?.markAsTouched({ onlySelf: true });
    });
  }

  goBack(): void {
    this.router.navigate(['/profile']);
  }

  // Getter methods for form validation
  get currentPassword() { return this.passwordForm.get('currentPassword'); }
  get newPassword() { return this.passwordForm.get('newPassword'); }
  get confirmPassword() { return this.passwordForm.get('confirmPassword'); }
}
