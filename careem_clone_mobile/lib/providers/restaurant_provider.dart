import 'package:flutter/material.dart';
import '../models/restaurant.dart';
import '../services/restaurant_service.dart';

class RestaurantProvider extends ChangeNotifier {
  final RestaurantService _restaurantService = RestaurantService();
  List<Restaurant> _restaurants = [];
  Restaurant? _selectedRestaurant;
  bool _isLoading = false;
  String _errorMessage = '';
  
  List<Restaurant> get restaurants => _restaurants;
  Restaurant? get selectedRestaurant => _selectedRestaurant;
  bool get isLoading => _isLoading;
  String get errorMessage => _errorMessage;
  
  Future<void> fetchRestaurants() async {
    _isLoading = true;
    _errorMessage = '';
    notifyListeners();
    
    try {
      _restaurants = await _restaurantService.getRestaurants();
      _isLoading = false;
      notifyListeners();
    } catch (e) {
      _isLoading = false;
      // In Dart with null safety, exception objects are non-nullable,
      // so we don't need the ?. operator
      _errorMessage = (e is Error) ? e.toString() : 'An unknown error occurred while fetching restaurants';
      notifyListeners();
    }
  }
  
  Future<void> fetchRestaurantDetails(int restaurantId) async {
    _isLoading = true;
    _errorMessage = '';
    notifyListeners();

    try {
      // Get the restaurant info from the list (if already loaded)
      Restaurant? restaurant = _restaurants.firstWhere(
        (r) => r.id == restaurantId,
        orElse: () => Restaurant(
          id: restaurantId,
          name: '',
          rating: 0.0,
        ),
      );
      // Fetch categories and dishes
      final categories = await _restaurantService.getCategories(restaurantId);
      final dishes = await _restaurantService.getDishes(restaurantId);

      // Debug: print fetched categories and dishes
      print('Fetched categories:');
      for (var cat in categories) {
        print('Category: ${cat.id} - ${cat.name}');
      }
      print('Fetched dishes:');
      for (var dish in dishes) {
        print('Dish: ${dish.id} - ${dish.name} (categoryId: ${dish.categoryId})');
      }

      // Attach dishes to their categories (force int conversion)
      for (var category in categories) {
        final catId = int.tryParse(category.id.toString()) ?? -1;
        category.dishes = dishes.where((d) {
          final dCatId = int.tryParse(d.categoryId.toString()) ?? -2;
          return dCatId == catId;
        }).toList();
        print('Category ${category.name} has ${category.dishes?.length ?? 0} dishes');
      }
      // Attach categories to the restaurant
      restaurant = Restaurant(
        id: restaurant.id,
        name: restaurant.name,
        description: restaurant.description,
        image: restaurant.image,
        address: restaurant.address,
        rating: restaurant.rating,
        categories: categories,
      );
      _selectedRestaurant = restaurant;
      _isLoading = false;
      notifyListeners();
    } catch (e) {
      _isLoading = false;
      _errorMessage = (e is Error) ? e.toString() : 'An unknown error occurred while fetching restaurant details';
      notifyListeners();
    }
  }
  
  void clearSelectedRestaurant() {
    _selectedRestaurant = null;
    notifyListeners();
  }
}
