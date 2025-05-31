import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Address } from '../../models/address.model';
import { AddressService } from '../../services/address.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-address-selection',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="address-selection-container">
      <h4>Select Delivery Address</h4>
      
      <div *ngIf="isLoading" class="text-center">
        <div class="spinner-border spinner-border-sm" role="status">
          <span class="visually-hidden">Loading addresses...</span>
        </div>
      </div>

      <div *ngIf="error" class="alert alert-danger">
        {{ error }}
      </div>

      <div *ngIf="!isLoading && addresses.length === 0" class="no-addresses">
        <p>No delivery addresses found.</p>
        <button class="btn btn-primary btn-sm" [routerLink]="['/addresses/new']">
          Add New Address
        </button>
      </div>

      <div *ngIf="addresses.length > 0" class="address-options">
        <div 
          *ngFor="let address of addresses" 
          class="address-option"
          [class.selected]="selectedAddressId === address.id"
          (click)="selectAddress(address)"
        >
          <div class="address-option-header">
            <input 
              type="radio" 
              [id]="'address-' + address.id"
              [value]="address.id"
              [checked]="selectedAddressId === address.id"
              (change)="selectAddress(address)"
            >
            <label [for]="'address-' + address.id" class="address-name">
              {{ address.name }}
              <span *ngIf="address.isDefault" class="badge bg-primary ms-2">Default</span>
            </label>
          </div>
          <div class="address-details">
            <p class="mb-1">{{ address.streetAddress }}</p>
            <p class="mb-0 text-muted">{{ address.city }}, {{ address.state }} {{ address.zipCode }}</p>
          </div>
        </div>
        
        <div class="add-new-address">
          <button class="btn btn-outline-primary btn-sm" [routerLink]="['/addresses/new']">
            <i class="bi bi-plus"></i> Add New Address
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .address-selection-container {
      margin-bottom: 1rem;
    }

    .address-options {
      max-height: 300px;
      overflow-y: auto;
    }

    .address-option {
      border: 1px solid #ddd;
      border-radius: 8px;
      padding: 12px;
      margin-bottom: 8px;
      cursor: pointer;
      transition: all 0.2s ease;
    }

    .address-option:hover {
      border-color: #007bff;
      background-color: #f8f9fa;
    }

    .address-option.selected {
      border-color: #007bff;
      background-color: #e7f3ff;
    }

    .address-option-header {
      display: flex;
      align-items: center;
      margin-bottom: 8px;
    }

    .address-option-header input[type="radio"] {
      margin-right: 8px;
    }

    .address-name {
      font-weight: 600;
      margin: 0;
      cursor: pointer;
      display: flex;
      align-items: center;
    }

    .address-details p {
      font-size: 0.9em;
      line-height: 1.3;
    }

    .no-addresses {
      text-align: center;
      padding: 20px;
      background-color: #f8f9fa;
      border-radius: 8px;
    }

    .add-new-address {
      margin-top: 10px;
      text-align: center;
    }

    .badge {
      font-size: 0.7em;
    }
  `]
})
export class AddressSelectionComponent implements OnInit {
  @Output() addressSelected = new EventEmitter<Address>();
  
  addresses: Address[] = [];
  selectedAddressId: number | null = null;
  isLoading = false;
  error: string | null = null;

  constructor(private addressService: AddressService) {}

  ngOnInit(): void {
    this.loadAddresses();
  }

  private loadAddresses(): void {
    this.isLoading = true;
    this.error = null;

    this.addressService.getMyAddresses().subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success) {
          this.addresses = response.data;
          // Auto-select default address if available
          const defaultAddress = this.addresses.find(addr => addr.isDefault);
          if (defaultAddress) {
            this.selectAddress(defaultAddress);
          }
        } else {
          this.error = response.errorMessage || 'Failed to load addresses';
        }
      },
      error: (err) => {
        this.isLoading = false;
        this.error = 'Error loading addresses. Please try again.';
        console.error('Error loading addresses:', err);
      }
    });
  }

  selectAddress(address: Address): void {
    this.selectedAddressId = address.id;
    this.addressSelected.emit(address);
  }

  get selectedAddress(): Address | null {
    return this.addresses.find(addr => addr.id === this.selectedAddressId) || null;
  }
}
