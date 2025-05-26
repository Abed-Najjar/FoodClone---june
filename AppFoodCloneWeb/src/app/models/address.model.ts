export interface Address {
  id: number;
  name: string;
  streetAddress: string;
  city: string;
  state: string;
  country: string;
  zipCode: string;
  latitude: number;
  longitude: number;
  formattedAddress: string;  isDefault: boolean;
  createdAt: Date;
  updatedAt?: Date;
}

export interface AddressCreate {
  name: string;
  streetAddress: string;
  city: string;
  state: string;
  country: string;
  zipCode?: string;
  latitude: number;
  longitude: number;  formattedAddress: string;
  isDefault?: boolean;
}

export interface AddressUpdate {
  name?: string;
  streetAddress?: string;
  city?: string;
  state?: string;
  country?: string;
  zipCode?: string;
  latitude?: number;
  longitude?: number;  formattedAddress?: string;
  isDefault?: boolean;
}

export interface Location {
  latitude: number;
  longitude: number;
  formattedAddress: string;
}
