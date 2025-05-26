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
  @Input() initialLatitude: number = 30.5852;  // Default to Amman, Jordan
  @Input() initialLongitude: number = 26.2384; // Default to Amman, Jordan
  @Input() allowSelection: boolean = true;
  @Input() markerTitle: string = 'Selected Location';

  @Output() locationSelected = new EventEmitter<Location>();

  map: google.maps.Map | null = null;
  marker: google.maps.Marker | null = null;
  geocoder: google.maps.Geocoder | null = null;

  constructor() { }

  ngOnInit(): void {
    // Load Google Maps API if not already loaded
    if (!window.google || !window.google.maps) {
      this.loadGoogleMapsApi();
    } else {
      this.initializeMap();
    }
  }

  private loadGoogleMapsApi(): void {
    const script = document.createElement('script');
    script.src = `https://maps.googleapis.com/maps/api/js?key=AIzaSyCdTIZGWLJ-VyNf73fv4My3WsG7-qVrWH8
&libraries=places`;
    script.async = true;
    script.defer = true;
    script.onload = () => {
      this.initializeMap();
    };
    document.head.appendChild(script);
  }

  private initializeMap(): void {
    const mapElement = document.getElementById('map');
    if (!mapElement) return;

    // Option 1: Use type assertion to bypass the type check
    const mapOptions = {
      zoom: 15,
      center: { lat: this.initialLatitude, lng: this.initialLongitude },
      mapTypeControl: true,
      fullscreenControl: true
    } as google.maps.MapOptions;

    // OR Option 2: Use the controlOptions property instead
    /* const mapOptions: google.maps.MapOptions = {
      zoom: 15,
      center: { lat: this.initialLatitude, lng: this.initialLongitude },
      mapTypeControl: true,
      fullscreenControlOptions: {
        position: google.maps.ControlPosition.RIGHT_TOP
      }
    }; */

    const extendedMapOptions: ExtendedMapOptions = {
      ...mapOptions,
      fullscreenControl: true
    };

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
}
