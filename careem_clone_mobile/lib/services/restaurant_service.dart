import 'dart:convert';
import 'package:http/http.dart' as http;
import '../models/restaurant.dart';
import '../services/auth_service.dart';
import '../utils/constants.dart';
import '../models/category.dart';
import '../models/dish.dart';

class RestaurantService {
  final AuthService authService = AuthService();
  
  
  Future<List<Restaurant>> getRestaurants() async {
    final token = await authService.getToken();
    
    final response = await http.get(
      Uri.parse('${AppConstants.baseUrl}${AppConstants.restaurants}'),
      headers: {
        'Authorization': 'Bearer $token',
        'Content-Type': 'application/json',
      },
    );
    
    if (response.statusCode == 200) {
      final jsonResponse = jsonDecode(response.body);
      final List<dynamic> restaurantsJson = jsonResponse['data'];
      return restaurantsJson
          .map((restaurant) => Restaurant.fromJson(restaurant))
          .toList();
    } else {
      throw Exception('Failed to load restaurants: ${response.body}');
    }
  }
  
  Future<Restaurant> getRestaurantDetails(int restaurantId) async {
    final token = await authService.getToken();
    
    final response = await http.get(
      Uri.parse('${AppConstants.baseUrl}${AppConstants.restaurants}/$restaurantId'),
      headers: {
        'Authorization': 'Bearer $token',
        'Content-Type': 'application/json',
      },
    );
    
    if (response.statusCode == 200) {
      final jsonResponse = jsonDecode(response.body);
      return Restaurant.fromJson(jsonResponse['data']);
    } else {
      throw Exception('Failed to load restaurant details: ${response.body}');
    }
  }

  Future<List<Category>> getCategories(int restaurantId) async {
    final token = await authService.getToken();
    final response = await http.get(
      Uri.parse('${AppConstants.baseUrl}${AppConstants.userGetCategoriesByRestaurantId}/$restaurantId'),
      headers: {
        'Authorization': 'Bearer $token',
        'Content-Type': 'application/json',
      },
    );
    if (response.statusCode == 200) {
      final jsonResponse = jsonDecode(response.body);
      final List<dynamic> categoriesJson = jsonResponse['data'];
      return categoriesJson.map((cat) => Category.fromJson(cat)).toList();
    } else {
      throw Exception('Failed to load categories: \\${response.body}');
    }
  }

  Future<List<Dish>> getDishes(int restaurantId) async {
    final token = await authService.getToken();
    final response = await http.get(
      Uri.parse('${AppConstants.baseUrl}${AppConstants.userGetDishesByRestaurantId}/$restaurantId'),
      headers: {
        'Authorization': 'Bearer $token',
        'Content-Type': 'application/json',
      },
    );
    if (response.statusCode == 200) {
      final jsonResponse = jsonDecode(response.body);
      final List<dynamic> dishesJson = jsonResponse['data'];
      return dishesJson.map((dish) => Dish.fromJson(dish)).toList();
    } else {
      throw Exception('Failed to load dishes: \\${response.body}');
    }
  }
}
