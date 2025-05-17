export interface Category {
  id: number;
  name: string;
  description?: string; // Added to match API model
  imageUrl: string;
  restaurantId: number;
  restaurantName: string;
}
