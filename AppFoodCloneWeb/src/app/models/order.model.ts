export interface OrderItem {
  id: number;
  dishId: number;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

export interface OrderCreate {
  restaurantId: number;
  paymentMethod: string;
  orderItems: {
    dishId: number;
    quantity: number;
  }[];
}

export interface Order {
  id: number;
  totalAmount: number;
  paymentMethod: string;
  status: string;
  orderDate: Date;
  userId: number;
  userName: string;
  restaurantId: number;
  restaurantName: string;
  orderItems: OrderItem[];
}
