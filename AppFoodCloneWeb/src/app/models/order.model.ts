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
  totalPrice: number;
  paymentMethod: string;
  status: string;
  orderDate: Date;
  userId: number;
  userName: string;
  userEmail?: string;
  phoneNumber?: string;
  deliveryAddress?: string;
  restaurantId: number;
  restaurantName: string;
  deliveryFee?: number;
  deliveryAddressId?: number;
  deliveryInstructions?: string;
  deliveryEmployee?: DeliveryEmployee; // Added delivery employee tracking
  orderItems: OrderItem[];
  dishes: {
    name: string;
    price: number;
    quantity: number;
  }[];
}
