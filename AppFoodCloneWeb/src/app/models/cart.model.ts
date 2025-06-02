// Frontend models that match backend DTOs exactly

export interface CartCalculationRequest {
  cartItems: CartItemRequest[];
  restaurantId: number;
  promoCode?: string;
  deliveryAddressId?: number;
}

export interface CartItemRequest {
  dishId: number;
  quantity: number;
}

export interface CartCalculationResponse {
  subtotal: number;
  deliveryFee: number;
  taxAmount: number;
  discountAmount: number;
  grandTotal: number;
  taxRate: number;
  freeDeliveryApplied: boolean;
  promoCodeApplied?: string;
  promoCodeMessage?: string;
  itemDetails: CartItemDetail[];
  currency: string;
  formattedSubtotal: string;
  formattedDeliveryFee: string;
  formattedTaxAmount: string;
  formattedGrandTotal: string;
}

export interface CartItemDetail {
  dishId: number;
  dishName: string;
  unitPrice: number;
  quantity: number;
  totalPrice: number;
  isAvailable: boolean;
  formattedUnitPrice: string;
  formattedTotalPrice: string;
}

export interface PromoCode {
  code: string;
  description: string;
  discountPercentage: number;
  discountAmount: number;
  freeDelivery: boolean;
  minimumOrderAmount: number;
  isActive: boolean;
  expiryDate?: Date;
}

export interface ValidatePromoCodeRequest {
  promoCode: string;
  subtotal: number;
}

// Response wrapper from backend
export interface AppResponse<T> {
  data: T;
  message: string;
  statusCode: number;
  success: boolean;
  errorMessage?: string;
} 