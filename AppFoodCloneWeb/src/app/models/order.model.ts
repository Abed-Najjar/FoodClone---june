export interface OrderItem {
  id: number;
  dishId: number;
  dishName?: string; // Added dish name property
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

export interface OrderCreate {
  restaurantId: number;
  paymentMethod: string;
  deliveryAddressId?: number;
  deliveryInstructions?: string;
  orderItems: {
    dishId: number;
    quantity: number;
  }[];
}

export interface DeliveryEmployee {
  id: number;
  name: string;
  phone: string;
  vehicleType: string;
  currentLocation: {
    latitude: number;
    longitude: number;
    address: string;
    lastUpdated: Date;
  };
  estimatedArrival: Date;
}

export interface Order {
  id: number;
  totalAmount: number;
  totalPrice?: number; // Keep for backward compatibility
  paymentMethod: string;
  status: string;
  orderDate: Date;
  userId: number;
  userName: string; // Backend uses userName instead of separate first/last names
  userFirstName?: string; // Keep for backward compatibility
  userLastName?: string; // Keep for backward compatibility
  userEmail?: string;
  phoneNumber?: string;
  deliveryAddress?: string;
  restaurantId: number;
  restaurantName: string;
  employeeId: number;
  employeeName: string;
  deliveryFee?: number;
  deliveryAddressId?: number;
  deliveryInstructions?: string;
  deliveryEmployee?: DeliveryEmployee; // Added delivery employee tracking
  orderItems: OrderItem[];
  dishes?: { // Keep for backward compatibility
    name: string;
    price: number;
    quantity: number;
  }[];
}
