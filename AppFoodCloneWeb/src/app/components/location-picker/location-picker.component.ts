import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Location } from '../../models/address.model';

// Define an interface extending Google Maps options
interface ExtendedMapOptions extends google.maps.MapOptions {
  fullscreenControl?: boolean;
}

@Component({
  selector: 'app-location-picker',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './location-picker.component.html',
  styleUrls: ['./location-picker.component.css']
})
export class LocationPickerComponent implements OnInit {
  @Input() initialLatitude: number = 31.9454;  // Default to center of Jordan
  @Input() initialLongitude: number = 35.9284; // Default to center of Jordan
  @Input() allowSelection: boolean = true;
  @Input() markerTitle: string = 'Selected Location';

  @Output() locationSelected = new EventEmitter<Location>();

  map: google.maps.Map | null = null;
  marker: google.maps.Marker | null = null;
  geocoder: google.maps.Geocoder | null = null;
  
  isLoading: boolean = true;
  hasError: boolean = false;
  errorMessage: string = '';

  constructor() { }
  ngOnInit(): void {
    console.log('LocationPickerComponent ngOnInit called');
    // Load Google Maps API if not already loaded
    if (!window.google || !window.google.maps) {
      console.log('Google Maps API not loaded, loading now...');
      this.loadGoogleMapsApi();
    } else {
      console.log('Google Maps API already loaded, initializing map...');
      this.initializeMap();
    }
  }  private loadGoogleMapsApi(): void {
    // Check if script is already loading
    const existingScript = document.querySelector('script[src*="maps.googleapis.com"]');
    if (existingScript) {
      console.log('Google Maps script already exists, waiting for load...');
      // If script exists but Google Maps is not available, set up a check
      if (!window.google?.maps) {
        this.waitForGoogleMaps();
      }
      return;
    }

    // Create a unique callback name for this instance
    const callbackName = `initLocationPicker_${Math.random().toString(36).substr(2, 9)}`;
    
    // Set up the global callback
    (window as any)[callbackName] = () => {
      console.log('Google Maps API loaded successfully via callback');
      this.initializeMap();
      // Clean up the callback
      delete (window as any)[callbackName];
    };    const script = document.createElement('script');
    script.src = `https://maps.googleapis.com/maps/api/js?key=AIzaSyB_meL0N-2zg3ZzCw22LcUrSiTAYtXVbBE&libraries=places&callback=${callbackName}`;
    script.async = true;
    script.defer = true;
    script.onerror = () => {
      console.error('Failed to load Google Maps API');
      this.hasError = true;
      this.isLoading = false;
      this.errorMessage = 'Failed to load Google Maps. Please check your internet connection and API key.';
      // Clean up the callback on error
      delete (window as any)[callbackName];
    };
    document.head.appendChild(script);
  }

  private waitForGoogleMaps(): void {
    const checkInterval = setInterval(() => {
      if (window.google?.maps) {
        console.log('Google Maps API became available');
        clearInterval(checkInterval);
        this.initializeMap();
      }
    }, 100);

    // Stop checking after 10 seconds
    setTimeout(() => {
      clearInterval(checkInterval);
      if (!window.google?.maps) {
        console.error('Timeout waiting for Google Maps API');
        this.hasError = true;
        this.isLoading = false;
        this.errorMessage = 'Timeout waiting for Google Maps to load.';
      }
    }, 10000);
  }private initializeMap(): void {
    console.log('Initializing Google Maps...');
    
    const mapElement = document.getElementById('map');
    if (!mapElement) {
      console.error('Map element not found');
      this.hasError = true;
      this.isLoading = false;
      this.errorMessage = 'Map container not found.';
      return;
    }

    if (!window.google || !window.google.maps) {
      console.error('Google Maps API not loaded');
      this.hasError = true;
      this.isLoading = false;
      this.errorMessage = 'Google Maps API failed to load.';
      return;
    }

    try {
      // Option 1: Use type assertion to bypass the type check
      const mapOptions = {
        zoom: 15,
        center: { lat: this.initialLatitude, lng: this.initialLongitude },
        mapTypeControl: true,
        fullscreenControl: true
      } as google.maps.MapOptions;

      const extendedMapOptions: ExtendedMapOptions = {
        ...mapOptions,
        fullscreenControl: true
      };

      console.log('Creating map with options:', extendedMapOptions);
      this.map = new google.maps.Map(mapElement, extendedMapOptions);

      this.geocoder = new google.maps.Geocoder();

      this.marker = new google.maps.Marker({
        position: { lat: this.initialLatitude, lng: this.initialLongitude },
        map: this.map,
        title: this.markerTitle,
        draggable: this.allowSelection
      });

      // Only add click event if selection is allowed
      if (this.allowSelection) {
        this.map.addListener('click', (event: google.maps.MapMouseEvent) => {
          const latLng = event.latLng!;
          const latLngObj = (latLng instanceof google.maps.LatLng)
            ? latLng
            : new google.maps.LatLng(latLng.lat, latLng.lng);
          this.updateMarkerPosition(latLngObj);
        });

        this.marker.addListener('dragend', (event: google.maps.MapMouseEvent) => {
          const latLng = event.latLng!;
          const latLngObj = (latLng instanceof google.maps.LatLng)
            ? latLng
            : new google.maps.LatLng(latLng.lat, latLng.lng);
          this.updateMarkerPosition(latLngObj);
        });
      }

      // Initial geocoding to get the address of the initial position
      this.geocodePosition(new google.maps.LatLng(this.initialLatitude, this.initialLongitude));
      
      console.log('Google Maps initialized successfully');
      this.isLoading = false;
      this.hasError = false;
    } catch (error) {
      console.error('Error initializing Google Maps:', error);
      this.hasError = true;
      this.isLoading = false;
      this.errorMessage = 'Error initializing the map. Please try again.';
    }
  }

  private updateMarkerPosition(latLng: google.maps.LatLng): void {
    if (this.marker && latLng) {
      this.marker.setPosition(latLng);
      this.geocodePosition(latLng);
    }
  }

  private geocodePosition(latLng: google.maps.LatLng): void {
    if (!this.geocoder) return;

    this.geocoder.geocode({ location: latLng }, (results, status) => {
      if (status === google.maps.GeocoderStatus.OK && results && results[0]) {
        const location: Location = {
          latitude: latLng.lat(),
          longitude: latLng.lng(),
          formattedAddress: results[0].formatted_address
        };

        this.locationSelected.emit(location);
      }
    });
  }

  // Method to externally set the location (can be called from parent component)
  public setLocation(latitude: number, longitude: number): void {
    if (this.map && this.marker) {
      const latLng = new google.maps.LatLng(latitude, longitude);
      this.map.setCenter(latLng);
      this.updateMarkerPosition(latLng);
    }
  }
  // Search for an address and update the map
  public searchAddress(address: string): void {
    if (!this.geocoder) return;

    this.geocoder.geocode({ address }, (results, status) => {
      if (status === google.maps.GeocoderStatus.OK && results && results[0] && this.map) {
        const location = results[0].geometry.location;
        this.map.setCenter(location);
        this.updateMarkerPosition(location);
      }
    });
  }
  // Retry loading the map
  public retryMapLoad(): void {
    console.log('Retrying map load...');
    this.isLoading = true;
    this.hasError = false;
    this.errorMessage = '';
    this.map = null;
    this.marker = null;
    this.geocoder = null;
    
    // Remove existing Google Maps scripts
    const existingScripts = document.querySelectorAll('script[src*="maps.googleapis.com"]');
    existingScripts.forEach(script => script.remove());
    
    // Clear any existing Google Maps objects
    if ((window as any).google) {
      delete (window as any).google;
    }
    
    // Reload the API
    this.loadGoogleMapsApi();
  }
}
