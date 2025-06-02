declare namespace google {
  namespace maps {
    class Map {
      constructor(element: HTMLElement, options: MapOptions);
      setCenter(latLng: LatLng): void;
      setZoom(zoom: number): void;
      addListener(event: string, callback: Function): MapsEventListener;
    }

    class Marker {
      constructor(options: MarkerOptions);
      setPosition(latLng: LatLng): void;
      setMap(map: Map | null): void;
      addListener(event: string, callback: Function): MapsEventListener;
      setDraggable(draggable: boolean): void;
      getPosition(): LatLng;
    }

    class Geocoder {
      constructor();
      geocode(request: GeocoderRequest, callback: (results: GeocoderResult[], status: GeocoderStatus) => void): void;
    }

    class LatLng {
      constructor(lat: number, lng: number);
      lat(): number;
      lng(): number;
      toJSON(): { lat: number, lng: number };
      toString(): string;
    }

    // Allow LatLng to be either a class or a plain object
    type LatLngLiteral = { lat: number, lng: number };
    type LatLngType = LatLng | LatLngLiteral;

    interface MapOptions {
      center: LatLngType;
      zoom: number;
      mapTypeId?: MapTypeId;
      disableDefaultUI?: boolean;
      zoomControl?: boolean;
      mapTypeControl?: boolean;
    }

    interface MarkerOptions {
      position: LatLngType;
      map: Map | null;
      title?: string;
      draggable?: boolean;
    }

    interface GeocoderRequest {
      address?: string;
      location?: LatLngType;
    }

    interface GeocoderResult {
      formatted_address: string;
      geometry: {
        location: LatLng
      };
      types: string[];
      address_components: {
        long_name: string;
        short_name: string;
        types: string[];
      }[];
    }

    interface MapsEventListener {
      remove(): void;
    }

    interface MapMouseEvent {
      latLng: LatLngType;
    }

    enum GeocoderStatus {
      ERROR,
      INVALID_REQUEST,
      OK,
      OVER_QUERY_LIMIT,
      REQUEST_DENIED,
      UNKNOWN_ERROR,
      ZERO_RESULTS
    }

    enum MapTypeId {
      ROADMAP,
      SATELLITE,
      HYBRID,
      TERRAIN
    }
  }
}

interface Window {
  google?: typeof google;
}
