import 'dart:convert';
import 'package:http/http.dart' as http;
import '../models/order.dart';
import '../services/auth_service.dart';
import '../utils/constants.dart';

class OrderService {
  final AuthService authService = AuthService();
  
  Future<List<Order>> getOrders() async {
    final token = await authService.getToken();
    
    final response = await http.get(
      Uri.parse('${AppConstants.baseUrl}${AppConstants.orders}'),
      headers: {
        'Authorization': 'Bearer $token',
        'Content-Type': 'application/json',
      },
    );
    
    if (response.statusCode == 200) {
      final jsonResponse = jsonDecode(response.body);
      final List<dynamic> ordersJson = jsonResponse['data'];
      return ordersJson
          .map((order) => Order.fromJson(order))
          .toList();
    } else {
      throw Exception('Failed to load orders: ${response.body}');
    }
  }
  
  Future<Order> createOrder(int restaurantId, List<OrderItem> items) async {
    final token = await authService.getToken();
    
    final response = await http.post(
      Uri.parse('${AppConstants.baseUrl}${AppConstants.orders}'),
      headers: {
        'Authorization': 'Bearer $token',
        'Content-Type': 'application/json',
      },
      body: jsonEncode({
        'restaurantId': restaurantId,
        'items': items.map((item) => item.toJson()).toList(),
      }),
    );
    
    if (response.statusCode == 200) {
      final jsonResponse = jsonDecode(response.body);
      return Order.fromJson(jsonResponse['data']);
    } else {
      throw Exception('Failed to create order: ${response.body}');
    }
  }
}
