import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { AddressService } from '../../services/address.service';
import { Address, AddressCreate, AddressUpdate, Location } from '../../models/address.model';
import { of, throwError } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import { LocationPickerComponent } from '../location-picker/location-picker.component';

@Component({
  selector: 'app-address-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, LocationPickerComponent],
  templateUrl: './address-form.component.html',
  styleUrls: ['./address-form.component.css']
})
export class AddressFormComponent implements OnInit {
  addressForm: FormGroup;
  isEditMode = false;
  addressId: number | null = null;
  isLoading = false;
  error: string | null = null;
  successMessage: string | null = null;
  
  constructor(
    private fb: FormBuilder,
    private addressService: AddressService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.addressForm = this.createAddressForm();
  }
  
  ngOnInit(): void {
    this.route.paramMap.pipe(
      switchMap(params => {
        const id = params.get('id');
        if (id && id !== 'new') {
          this.isEditMode = true;
          this.addressId = +id;
          this.isLoading = true;
          return this.addressService.getAddress(+id).pipe(
            catchError(err => {
              this.error = 'Failed to load address details';
              return throwError(err);
            })
          );
        }
        return of(null);
      })
    ).subscribe(response => {
      this.isLoading = false;
      if (response && response.success) {
        this.populateForm(response.data);
      }
    });
  }
  
  private createAddressForm(): FormGroup {
    return this.fb.group({
      name: ['', [Validators.required]],
      streetAddress: ['', [Validators.required]],
      city: ['', [Validators.required]],
      state: ['', [Validators.required]],
      country: ['', [Validators.required]],
      zipCode: [''],
      latitude: [0, [Validators.required]],
      longitude: [0, [Validators.required]],
      formattedAddress: ['', [Validators.required]],
      notes: [''],
      isDefault: [false]
    });
  }
  
  private populateForm(address: Address): void {
    this.addressForm.patchValue({
      name: address.name,
      streetAddress: address.streetAddress,
      city: address.city,
      state: address.state,
      country: address.country,
      zipCode: address.zipCode,
      latitude: address.latitude,      longitude: address.longitude,
      formattedAddress: address.formattedAddress,
      isDefault: address.isDefault
    });
  }
  
  onLocationSelected(location: Location): void {
    this.addressForm.patchValue({
      latitude: location.latitude,
      longitude: location.longitude,
      formattedAddress: location.formattedAddress
    });
    
    // Try to extract components from formatted address
    const addressParts = location.formattedAddress.split(',');
    if (addressParts.length >= 3) {
      // Simple parsing attempt - this is very basic and might need improvement
      this.addressForm.patchValue({
        streetAddress: addressParts[0].trim(),
        city: addressParts[1].trim(),
        state: addressParts[2].trim()
      });
      
      if (addressParts.length > 3) {
        this.addressForm.patchValue({
          country: addressParts[addressParts.length - 1].trim()
        });
      }
    }
  }
  
  onSubmit(): void {
    if (this.addressForm.invalid) {
      // Mark all fields as touched to trigger validation messages
      Object.keys(this.addressForm.controls).forEach(key => {
        const control = this.addressForm.get(key);
        control?.markAsTouched();
      });
      return;
    }
    
    this.isLoading = true;
    this.error = null;
    this.successMessage = null;
    
    if (this.isEditMode && this.addressId) {
      const addressUpdate: AddressUpdate = this.addressForm.value;
      this.addressService.updateAddress(this.addressId, addressUpdate).subscribe(
        response => {
          this.isLoading = false;
          if (response.success) {
            this.successMessage = 'Address updated successfully!';
            setTimeout(() => {
              this.router.navigate(['/addresses']);
            }, 1500);
          } else {
            this.error = response.errorMessage || 'Failed to update address';
          }
        },
        error => {
          this.isLoading = false;
          this.error = 'An error occurred while updating the address';
        }
      );
    } else {
      const addressCreate: AddressCreate = this.addressForm.value;
      this.addressService.createAddress(addressCreate).subscribe(
        response => {
          this.isLoading = false;
          if (response.success) {
            this.successMessage = 'Address added successfully!';
            setTimeout(() => {
              this.router.navigate(['/addresses']);
            }, 1500);
          } else {
            this.error = response.errorMessage || 'Failed to add address';
          }
        },
        error => {
          this.isLoading = false;
          this.error = 'An error occurred while adding the address';
        }
      );
    }
  }
  
  get formControls() {
    return this.addressForm.controls;
  }
}
