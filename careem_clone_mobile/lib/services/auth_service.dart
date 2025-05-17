import 'dart:convert';
import 'dart:async';
import 'package:http/http.dart' as http;
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import '../models/user.dart';
import '../utils/constants.dart';

class AuthService {
  final storage = const FlutterSecureStorage();
  String? _workingUrl;

  // Test connection before attempting login
  Future<bool> verifyBackendConnection() async {
    try {
      final workingUrl = await AppConstants.findWorkingUrl();
      return workingUrl != null;
    } catch (e) {
      AppConstants.debugLog('Backend verification error: $e');
      return false;
    }
  }

  // Before making any API call, try to find a working URL
  Future<String> getApiBaseUrl() async {
    if (_workingUrl != null) {
      return _workingUrl!;
    }

    _workingUrl = await AppConstants.findWorkingUrl();
    return _workingUrl ?? AppConstants.baseUrl;
  }

  Future<User> login(String email, String password) async {
    // First verify backend is accessible
    final isConnected = await verifyBackendConnection();
    if (!isConnected) {
      throw Exception(
        'Backend server is not running or not accessible.\n\n'
        'Make sure your .NET backend is running at one of these addresses:\n'
        '- http://localhost:5236\n'
        '- https://localhost:7258\n\n'
        'To fix this issue:\n'
        '1. Make sure you\'ve started your .NET API project\n'
        '2. Check that it\'s running on the expected port\n'
        '3. Try reopening your browser or using a different browser'
      );
    }

    final baseUrl = AppConstants.workingBackendUrl ?? AppConstants.baseUrl;
    final url = '$baseUrl${AppConstants.login}';
    AppConstants.debugLog('Attempting login at: $url');

    try {
      final Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      };

      final response = await http.post(
        Uri.parse(url),
        headers: headers,
        body: jsonEncode({
          'email': email,
          'password': password,
        }),
      ).timeout(const Duration(seconds: 15));

      AppConstants.debugLog('Login response code: ${response.statusCode}');

      if (response.statusCode == 200) {
        final jsonResponse = jsonDecode(response.body);
        AppConstants.debugLog('Raw API response: $jsonResponse');
        
        // Check for data field in API response
        final data = jsonResponse is Map<String, dynamic> && jsonResponse.containsKey('data') 
            ? jsonResponse['data'] 
            : jsonResponse;
            
        // Check for null or empty response
        if (data == null) {
          throw Exception('Invalid API response: Response data is null');
        }
        
        try {
          final user = User.fromJson(data);
          
          // Verify token exists before saving
          if (user.token == null) {
            throw Exception('Authentication failed: No token received from server');
          }
          
          // Save token to secure storage
          await storage.write(key: 'token', value: user.token);
          await storage.write(key: 'userId', value: user.id?.toString() ?? '');
          
          return user;
        } catch (e) {
          AppConstants.debugLog('Error parsing User from JSON: $e');
          AppConstants.debugLog('User JSON: $data');
          throw Exception('Failed to parse user data: $e');
        }
      } else {
        AppConstants.debugLog('Login failed with status: ${response.statusCode}');
        AppConstants.debugLog('Response body: ${response.body}');
        throw Exception('Login failed: ${response.body}');
      }
    } catch (e) {
      AppConstants.debugLog('Login exception: $e');
      rethrow;
    }
  }

  // Add a method to test the connection before attempting login/register
  Future<bool> testConnection() async {
    return await AppConstants.testConnection();
  }

  Future<User> register(String username, String email, String password) async {
    final baseUrl = await getApiBaseUrl();
    final url = '$baseUrl${AppConstants.register}';
    print('Attempting to register at: $url');

    try {
      final response = await http.post(
        Uri.parse(url),
        headers: {
          'Content-Type': 'application/json',
        },
        body: jsonEncode({
          'username': username,
          'email': email,
          'password': password,
        }),
      ).timeout(const Duration(seconds: 15));

      print('Register response status: ${response.statusCode}');
      print('Register response body: ${response.body}');

      if (response.statusCode == 200) {
        return login(email, password);
      } else {
        throw Exception('Failed to register: ${response.body}');
      }
    } on http.ClientException catch (e) {
      print('ClientException during register: ${e.message}');
      throw Exception('Connection error: Cannot reach server at $url. ${e.message}');
    } on TimeoutException {
      throw Exception('Connection timeout: Server took too long to respond');
    } catch (e) {
      throw Exception('Registration failed: $e');
    }
  }

  Future<void> logout() async {
    await storage.delete(key: 'token');
    await storage.delete(key: 'userId');
  }

  Future<String?> getToken() async {
    return await storage.read(key: 'token');
  }

  Future<bool> isLoggedIn() async {
    final token = await storage.read(key: 'token');
    return token != null;
  }
}
