import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { AppResponse } from '../models/app-response.model';
import { Address, AddressCreate, AddressUpdate } from '../models/address.model';

@Injectable({
  providedIn: 'root'
})
export class AddressService {
  private apiUrl = `${environment.apiUrl}/address`;

  constructor(private http: HttpClient) { }
  /**
   * Get all addresses for the current user
   * @returns Observable with addresses
   */
  getMyAddresses(): Observable<AppResponse<Address[]>> {
    return this.http.get<AppResponse<Address[]>>(this.apiUrl);
  }

  /**
   * Get a specific address by ID
   * @param id Address ID
   * @returns Observable with address
   */  getAddress(id: number): Observable<AppResponse<Address>> {
    return this.http.get<AppResponse<Address>>(`${this.apiUrl}/${id}`);
  }

  /**
   * Get the default address for the current user
   * @returns Observable with default address
   */
  getDefaultAddress(): Observable<AppResponse<Address>> {
    return this.http.get<AppResponse<Address>>(`${this.apiUrl}/default`);
  }

  /**
   * Create a new address
   * @param address Address data
   * @returns Observable with created address
   */
  createAddress(address: AddressCreate): Observable<AppResponse<Address>> {
    return this.http.post<AppResponse<Address>>(this.apiUrl, address);
  }

  /**
   * Update an existing address
   * @param id Address ID
   * @param address Address data
   * @returns Observable with updated address
   */
  updateAddress(id: number, address: AddressUpdate): Observable<AppResponse<Address>> {
    return this.http.put<AppResponse<Address>>(`${this.apiUrl}/${id}`, address);
  }

  /**
   * Delete an address
   * @param id Address ID
   * @returns Observable with status
   */
  deleteAddress(id: number): Observable<AppResponse<boolean>> {
    return this.http.delete<AppResponse<boolean>>(`${this.apiUrl}/${id}`);
  }

  /**
   * Set an address as default
   * @param id Address ID
   * @returns Observable with status
   */
  setDefaultAddress(id: number): Observable<AppResponse<boolean>> {
    return this.http.put<AppResponse<boolean>>(`${this.apiUrl}/setDefault/${id}`, {});
  }
}
