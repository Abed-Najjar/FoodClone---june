import 'dish.dart';

class Order {
  final int id;
  final int userId;
  final int restaurantId;
  final String status;
  final double totalAmount;
  final DateTime createdAt;
  final List<OrderItem>? items;
  
  Order({
    required this.id,
    required this.userId,
    required this.restaurantId,
    required this.status,
    required this.totalAmount,
    required this.createdAt,
    this.items,
  });
  
  factory Order.fromJson(Map<String, dynamic> json) {
    List<OrderItem>? orderItems;
    if (json['items'] != null) {
      orderItems = (json['items'] as List)
          .map((item) => OrderItem.fromJson(item))
          .toList();
    }
    
    return Order(
      id: json['id'],
      userId: json['userId'],
      restaurantId: json['restaurantId'],
      status: json['status'],
      totalAmount: json['totalAmount']?.toDouble() ?? 0.0,
      createdAt: DateTime.parse(json['createdAt']),
      items: orderItems,
    );
  }
}

class OrderItem {
  final int dishId;
  final int quantity;
  final double unitPrice;
  final Dish? dish;
  
  OrderItem({
    required this.dishId,
    required this.quantity,
    required this.unitPrice,
    this.dish,
  });
  
  factory OrderItem.fromJson(Map<String, dynamic> json) {
    return OrderItem(
      dishId: json['dishId'],
      quantity: json['quantity'],
      unitPrice: json['unitPrice']?.toDouble() ?? 0.0,
      dish: json['dish'] != null ? Dish.fromJson(json['dish']) : null,
    );
  }
  
  Map<String, dynamic> toJson() {
    return {
      'dishId': dishId,
      'quantity': quantity,
    };
  }
}
