export interface Dish {
  id: number;
  name: string;
  description: string;
  price: number;
  imageUrl: string;
  restaurantId: number;
  restaurantName: string;
  categoryId: number;
  categoryName?: string;
  isAvailable?: boolean;
}
