import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Address } from '../../models/address.model';
import { AddressService } from '../../services/address.service';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-address-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './address-list.component.html',
  styleUrls: ['./address-list.component.css']
})
export class AddressListComponent implements OnInit {
  addresses$: Observable<Address[]>;
  isLoading = true;
  error: string | null = null;
  
  constructor(private addressService: AddressService) {
    this.addresses$ = this.loadAddresses();
  }
  
  ngOnInit(): void {
    // Initial load is done in constructor
  }
  
  private loadAddresses(): Observable<Address[]> {
    this.isLoading = true;
    this.error = null;
    
    return this.addressService.getMyAddresses().pipe(
      map(response => {
        this.isLoading = false;
        if (!response.success) {
          this.error = response.errorMessage || 'Failed to load addresses';
          return [];
        }
        return response.data;
      })
    );
  }
  
  refreshAddresses(): void {
    this.addresses$ = this.loadAddresses();
  }
  
  setDefaultAddress(addressId: number): void {
    this.addressService.setDefaultAddress(addressId).subscribe(
      response => {
        if (response.success) {
          this.refreshAddresses();
        } else {
          this.error = response.errorMessage || 'Failed to set default address';
        }
      },
      error => {
        this.error = 'An error occurred while setting default address';
      }
    );
  }
  
  deleteAddress(addressId: number): void {
    if (confirm('Are you sure you want to delete this address?')) {
      this.addressService.deleteAddress(addressId).subscribe(
        response => {
          if (response.success) {
            this.refreshAddresses();
          } else {
            this.error = response.errorMessage || 'Failed to delete address';
          }
        },
        error => {
          this.error = 'An error occurred while deleting the address';
        }
      );
    }
  }
}
