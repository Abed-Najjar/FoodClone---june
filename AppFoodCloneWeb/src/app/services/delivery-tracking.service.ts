import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, interval, map, switchMap, catchError, of } from 'rxjs';
import { environment } from '../../environments/environment';
import { DeliveryEmployee } from '../models/order.model';

export interface DeliveryLocation {
  orderId: number;
  employeeId: number;
  employeeName: string;
  vehicleType: string;
  currentLocation: {
    latitude: number;
    longitude: number;
    address: string;
    lastUpdated: Date;
  };
  estimatedArrival: Date;
  distanceToDestination: number; // in kilometers
  status: 'picked_up' | 'in_transit' | 'nearby' | 'delivered';
}

@Injectable({
  providedIn: 'root'
})
export class DeliveryTrackingService {
  private apiUrl = `${environment.apiUrl}/api/delivery`;

  constructor(private http: HttpClient) {}

  // Get current delivery location for an order
  getDeliveryLocation(orderId: number): Observable<DeliveryLocation | null> {
    return this.http.get<DeliveryLocation>(`${this.apiUrl}/location/${orderId}`).pipe(
      catchError(() => of(null))
    );
  }

  // Get live tracking updates every 10 seconds
  getLiveDeliveryTracking(orderId: number): Observable<DeliveryLocation | null> {
    return interval(10000).pipe( // Update every 10 seconds
      switchMap(() => this.getDeliveryLocation(orderId)),
      catchError(() => of(null))
    );
  }

  // Mock delivery data for demonstration (remove when API is ready)
  getMockDeliveryLocation(orderId: number): Observable<DeliveryLocation | null> {
    // Simulate different delivery statuses and locations
    const mockLocations = [
      'Downtown Amman, near 1st Circle',
      'Abdali Mall area',
      'Rainbow Street vicinity',
      'University of Jordan area',
      'Sweifieh commercial district',
      'Dabouq residential area',
      'Khalda neighborhood',
      'Jabal Amman',
      'Your delivery area'
    ];

    const mockData: DeliveryLocation = {
      orderId,
      employeeId: Math.floor(Math.random() * 100) + 1,
      employeeName: this.getRandomDeliveryName(),
      vehicleType: Math.random() > 0.5 ? 'Motorcycle' : 'Car',
      currentLocation: {
        latitude: 31.9566 + (Math.random() - 0.5) * 0.1, // Around Amman
        longitude: 35.9457 + (Math.random() - 0.5) * 0.1,
        address: mockLocations[Math.floor(Math.random() * mockLocations.length)],
        lastUpdated: new Date()
      },
      estimatedArrival: new Date(Date.now() + Math.random() * 30 * 60 * 1000), // Random time up to 30 mins
      distanceToDestination: Math.random() * 10, // Random distance up to 10km
      status: this.getRandomStatus()
    };

    return of(mockData);
  }

  private getRandomDeliveryName(): string {
    const names = ['Ahmed', 'Omar', 'Mohammed', 'Khalil', 'Sami', 'Fadi', 'Rami', 'Nader'];
    return names[Math.floor(Math.random() * names.length)];
  }

  private getRandomStatus(): 'picked_up' | 'in_transit' | 'nearby' | 'delivered' {
    const statuses: ('picked_up' | 'in_transit' | 'nearby' | 'delivered')[] = 
      ['picked_up', 'in_transit', 'in_transit', 'nearby']; // More likely to be in transit
    return statuses[Math.floor(Math.random() * statuses.length)];
  }

  // Calculate distance between two coordinates (Haversine formula)
  calculateDistance(lat1: number, lon1: number, lat2: number, lon2: number): number {
    const R = 6371; // Earth's radius in kilometers
    const dLat = this.deg2rad(lat2 - lat1);
    const dLon = this.deg2rad(lon2 - lon1);
    const a = 
      Math.sin(dLat/2) * Math.sin(dLat/2) +
      Math.cos(this.deg2rad(lat1)) * Math.cos(this.deg2rad(lat2)) * 
      Math.sin(dLon/2) * Math.sin(dLon/2);
    const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1-a));
    return R * c; // Distance in kilometers
  }

  private deg2rad(deg: number): number {
    return deg * (Math.PI/180);
  }

  // Format distance for display
  formatDistance(distance: number): string {
    if (distance < 1) {
      return `${Math.round(distance * 1000)}m away`;
    }
    return `${distance.toFixed(1)}km away`;
  }

  // Get status message with icon
  getStatusMessage(status: string): { message: string, icon: string, color: string } {
    switch (status) {
      case 'picked_up':
        return { 
          message: 'Order picked up from restaurant', 
          icon: 'fas fa-check-circle', 
          color: '#28a745' 
        };
      case 'in_transit':
        return { 
          message: 'On the way to your location', 
          icon: 'fas fa-route', 
          color: '#007bff' 
        };
      case 'nearby':
        return { 
          message: 'Delivery person is nearby', 
          icon: 'fas fa-map-marker-alt', 
          color: '#ffc107' 
        };
      case 'delivered':
        return { 
          message: 'Order delivered successfully', 
          icon: 'fas fa-check-double', 
          color: '#28a745' 
        };
      default:
        return { 
          message: 'Preparing for delivery', 
          icon: 'fas fa-clock', 
          color: '#6c757d' 
        };
    }
  }
}
