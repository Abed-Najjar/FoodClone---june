import { Component, Input, OnInit, OnDestroy, AfterViewInit, ElementRef, ViewChild, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DeliveryLocation } from '../../services/delivery-tracking.service';

// Declare google as any to avoid type conflicts
declare const google: any;

@Component({
  selector: 'app-delivery-map',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './delivery-map.component.html',
  styleUrls: ['./delivery-map.component.css']
})
export class DeliveryMapComponent implements OnInit, OnDestroy, AfterViewInit, OnChanges {
  @Input() deliveryLocation: DeliveryLocation | null = null;
  @Input() destinationAddress: string = '';
  @Input() mapHeight: string = '200px';
  @ViewChild('mapContainer', { static: false }) mapContainer!: ElementRef;

  private map: any; // Google Maps Map instance
  private deliveryMarker: any; // Google Maps Marker
  private destinationMarker: any; // Google Maps Marker
  private directionsService: any; // Google Maps DirectionsService
  private directionsRenderer: any; // Google Maps DirectionsRenderer
  isMapLoaded = false;

  // Default location (Amman, Jordan)
  private defaultLat = 31.9566;
  private defaultLng = 35.9457;

  ngOnInit(): void {
    // Component initialization
  }

  ngAfterViewInit(): void {
    // Delay initialization to ensure container is rendered
    setTimeout(() => this.initializeMap(), 100);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['deliveryLocation'] && !changes['deliveryLocation'].firstChange && this.map) {
      this.updateMapContent();
    }
  }

  ngOnDestroy(): void {
    // Clean up all Google Maps resources
    if (this.deliveryMarker) {
      this.deliveryMarker.setMap(null);
      this.deliveryMarker = null;
    }
    
    if (this.destinationMarker) {
      this.destinationMarker.setMap(null);
      this.destinationMarker = null;
    }
    
    if (this.directionsRenderer) {
      this.directionsRenderer.setMap(null);
      this.directionsRenderer = null;
    }
    
    if (this.map) {
      // Clear all overlays and listeners
      google.maps?.event?.clearInstanceListeners(this.map);
      this.map = null;
    }
    
    this.directionsService = null;
    this.isMapLoaded = false;
  }

  private initializeMap(): void {
    // Prevent multiple initializations
    if (this.map) {
      return;
    }

    if (typeof google === 'undefined' || !google.maps) {
      console.error('Google Maps API not loaded');
      // Retry after a short delay, but only if we haven't already initialized
      setTimeout(() => this.initializeMap(), 500);
      return;
    }

    if (!this.mapContainer) {
      console.error('Map container not found');
      return;
    }

    try {
      // Initialize the map
      this.map = new google.maps.Map(this.mapContainer.nativeElement, {
        zoom: 13,
        center: { lat: this.defaultLat, lng: this.defaultLng },
        mapTypeControl: false,
        streetViewControl: false,
        fullscreenControl: false,
        zoomControl: true,
        zoomControlOptions: {
          position: google.maps.ControlPosition.RIGHT_CENTER
        }
      });

      // Initialize directions service and renderer
      this.directionsService = new google.maps.DirectionsService();
      this.directionsRenderer = new google.maps.DirectionsRenderer({
        map: this.map,
        suppressMarkers: true,
        polylineOptions: {
          strokeColor: '#007bff',
          strokeWeight: 4,
          strokeOpacity: 0.7
        }
      });

      this.isMapLoaded = true;

      // Set up markers and route
      this.updateMapContent();
    } catch (error) {
      console.error('Error initializing Google Maps:', error);
      this.map = null; // Reset if initialization failed
    }
  }

  private updateMapContent(): void {
    if (!this.map || !this.deliveryLocation || !this.directionsService || !this.directionsRenderer) return;

    const deliveryLat = this.deliveryLocation.currentLocation.latitude;
    const deliveryLng = this.deliveryLocation.currentLocation.longitude;

    // Destination coordinates (mock for now - in real app, get from address geocoding)
    const destLat = this.defaultLat + 0.01;
    const destLng = this.defaultLng + 0.01;

    // Create delivery marker with custom icon
    if (this.deliveryMarker) {
      this.deliveryMarker.setMap(null);
    }
    this.deliveryMarker = new google.maps.Marker({
      position: { lat: deliveryLat, lng: deliveryLng },
      map: this.map,
      icon: {
        url: 'data:image/svg+xml;charset=UTF-8,%3Csvg xmlns="http://www.w3.org/2000/svg" width="40" height="40" viewBox="0 0 40 40"%3E%3Ccircle cx="20" cy="20" r="18" fill="%23007bff" stroke="%23fff" stroke-width="3"/%3E%3Cpath d="M12 20l6 6l10-10" stroke="%23fff" stroke-width="3" fill="none"/%3E%3C/svg%3E',
        scaledSize: new google.maps.Size(40, 40),
        anchor: new google.maps.Point(20, 20)
      },
      title: this.deliveryLocation.employeeName,
      animation: google.maps.Animation.DROP
    });

    // Create destination marker with custom icon
    if (this.destinationMarker) {
      this.destinationMarker.setMap(null);
    }
    this.destinationMarker = new google.maps.Marker({
      position: { lat: destLat, lng: destLng },
      map: this.map,
      icon: {
        url: 'data:image/svg+xml;charset=UTF-8,%3Csvg xmlns="http://www.w3.org/2000/svg" width="40" height="40" viewBox="0 0 40 40"%3E%3Ccircle cx="20" cy="20" r="18" fill="%2328a745" stroke="%23fff" stroke-width="3"/%3E%3Cpath d="M12 12h16v16h-16z" fill="%23fff"/%3E%3C/svg%3E',
        scaledSize: new google.maps.Size(35, 35),
        anchor: new google.maps.Point(17.5, 17.5)
      },
      title: 'Delivery Address'
    });

    // Calculate and display route
    const request = {
      origin: { lat: deliveryLat, lng: deliveryLng },
      destination: { lat: destLat, lng: destLng },
      travelMode: google.maps.TravelMode.DRIVING
    };

    this.directionsService.route(request, (result: any, status: any) => {
      if (status === google.maps.DirectionsStatus.OK && result) {
        this.directionsRenderer.setDirections(result);
      }
    });

    // Fit map to show both markers
    const bounds = new google.maps.LatLngBounds();
    bounds.extend(new google.maps.LatLng(deliveryLat, deliveryLng));
    bounds.extend(new google.maps.LatLng(destLat, destLng));
    this.map.fitBounds(bounds);
  }

  getStatusClass(): string {
    if (!this.deliveryLocation) return 'status-unknown';
    
    switch (this.deliveryLocation.status) {
      case 'picked_up': return 'status-picked-up';
      case 'in_transit': return 'status-in-transit';
      case 'nearby': return 'status-nearby';
      case 'delivered': return 'status-delivered';
      default: return 'status-unknown';
    }
  }

  getStatusIcon(): string {
    if (!this.deliveryLocation) return 'fas fa-question';
    
    switch (this.deliveryLocation.status) {
      case 'picked_up': return 'fas fa-check-circle';
      case 'in_transit': return 'fas fa-route';
      case 'nearby': return 'fas fa-map-marker-alt';
      case 'delivered': return 'fas fa-check-double';
      default: return 'fas fa-clock';
    }
  }

  getStatusText(): string {
    if (!this.deliveryLocation) return 'Loading...';
    
    switch (this.deliveryLocation.status) {
      case 'picked_up': return 'Order Picked Up';
      case 'in_transit': return 'On the Way';
      case 'nearby': return 'Driver Nearby';
      case 'delivered': return 'Delivered';
      default: return 'Preparing';
    }
  }

  getDistanceText(): string {
    if (!this.deliveryLocation) return '';
    
    const distance = this.deliveryLocation.distanceToDestination;
    if (distance < 1) {
      return `${Math.round(distance * 1000)}m away`;
    }
    return `${distance.toFixed(1)}km away`;
  }

  getETAText(): string {
    if (!this.deliveryLocation) return '';
    
    const eta = new Date(this.deliveryLocation.estimatedArrival);
    const now = new Date();
    const diffMs = eta.getTime() - now.getTime();
    const diffMins = Math.round(diffMs / (1000 * 60));
    
    if (diffMins <= 0) {
      return 'Arriving now';
    } else if (diffMins < 60) {
      return `${diffMins} min`;
    } else {
      return eta.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    }
  }
}
