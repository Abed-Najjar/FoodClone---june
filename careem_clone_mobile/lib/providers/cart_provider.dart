import 'package:flutter/material.dart';
import '../models/dish.dart';
import '../models/order.dart';

class CartProvider extends ChangeNotifier {
  Map<int, OrderItem> _items = {};
  int? _restaurantId;
  
  Map<int, OrderItem> get items => _items;
  int? get restaurantId => _restaurantId;
  int get itemCount => _items.values.fold(0, (sum, item) => sum + item.quantity);
  double get total => _items.values.fold(0, (sum, item) => sum + (item.quantity * item.unitPrice));
  
  void addItem(Dish dish, int restaurantId) {
    // If cart has items from another restaurant, clear it
    if (_restaurantId != null && _restaurantId != restaurantId) {
      clear();
    }
    
    _restaurantId = restaurantId;
    
    if (_items.containsKey(dish.id)) {
      _items.update(
        dish.id,
        (existingItem) => OrderItem(
          dishId: existingItem.dishId,
          quantity: existingItem.quantity + 1,
          unitPrice: dish.price,
          dish: dish,
        ),
      );
    } else {
      _items.putIfAbsent(
        dish.id,
        () => OrderItem(
          dishId: dish.id,
          quantity: 1,
          unitPrice: dish.price,
          dish: dish,
        ),
      );
    }
    
    notifyListeners();
  }
  
  void removeItem(int dishId) {
    if (_items.containsKey(dishId)) {
      if (_items[dishId]!.quantity > 1) {
        _items.update(
          dishId,
          (existingItem) => OrderItem(
            dishId: existingItem.dishId,
            quantity: existingItem.quantity - 1,
            unitPrice: existingItem.unitPrice,
            dish: existingItem.dish,
          ),
        );
      } else {
        _items.remove(dishId);
      }
      
      if (_items.isEmpty) {
        _restaurantId = null;
      }
      
      notifyListeners();
    }
  }
  
  void clear() {
    _items = {};
    _restaurantId = null;
    notifyListeners();
  }
  
  List<OrderItem> getOrderItems() {
    return _items.values.toList();
  }
}
