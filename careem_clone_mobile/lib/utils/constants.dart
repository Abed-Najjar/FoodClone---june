import 'dart:io';
import 'package:http/http.dart' as http;
import 'package:flutter/foundation.dart' show kIsWeb;

class AppConstants {
  // Development server URLs - Try multiple ports for .NET Core
  static const List<String> _webUrls = [
    'http://localhost:5236/api',  // HTTP .NET port
    'https://localhost:7258/api', // HTTPS .NET port
    'http://localhost:5000/api',  // Common alternative HTTP port
    'https://localhost:5001/api', // Common alternative HTTPS port
    'http://127.0.0.1:5236/api',  // Using IP instead of localhost
  ];
  
  // Development server URLs
  static const List<String> _devUrls = [
    'http://10.0.2.2:5236/api',    // Android emulator -> host localhost
    'http://localhost:5236/api',   // iOS simulator -> host localhost
    'http://192.168.56.1:5236/api', // Current specified IP
    'http://127.0.0.1:5236/api',   // Direct localhost
  ];
  
  // Backend connection status
  static bool isBackendRunning = false;
  static String? workingBackendUrl;
  
  // Use kDebugMode to enable console logging in debug builds
  static const bool enableDebugLogs = true;
  
  // Use primary web URL
  static const String webBrowserUrl = 'http://localhost:5236/api';
  
  // Production URL
  // static const String _prodUrl = 'https://your-production-api.com/api';
  
  // For web testing, explicitly disable CORS in your http calls
  // Change this to true when testing in web browsers
  static const bool noCORSMode = true;
  
  // Determine which URL to use based on environment
  static String get baseUrl {
    // If we've already found a working backend URL, use it
    if (workingBackendUrl != null) {
      return workingBackendUrl!;
    }
    
    // Special case for web platforms - always use web URL
    if (kIsWeb) {
      return _webUrls[0]; // Default to first URL until we find a working one
    }
    
    // Otherwise use platform-specific URLs
    if (Platform.isAndroid) {
      return _devUrls[0]; // Android emulator
    } else if (Platform.isIOS) {
      return _devUrls[1]; // iOS simulator
    } else {
      return _devUrls[2]; // Other platforms
    }
  }

  // API endpoints - must match backend routes exactly
  static const String login = '/auth/login';
  static const String register = '/auth/register';
  static const String forgotPassword = '/auth/forgotpassword';
  static const String restaurants = '/restaurantmanagement';
  static const String categories = '/categorymanagement';
  static const String dishes = '/dishmanagement';
  static const String orders = '/order';
  static const String users = '/user';

  // User endpoints matching backend UserController
  static const String userGetAllRestaurants = '/user/restaurants';
  static const String userGetDishesByRestaurantId = '/user/dishes'; // Usage: '/user/dishes/{restaurantId}'
  static const String userGetCategoriesByRestaurantId = '/user/categories'; // Usage: '/user/categories/{restaurantId}'
  static const String userCreateOrder = '/user/createOrder';

  // Enhanced diagnostic connection test
  static Future<String?> findWorkingUrl() async {
    List<String> urlsToTry = kIsWeb ? _webUrls : _devUrls;
    
    // Log which URLs we're trying
    debugLog('Testing connection to backend...');
    debugLog('URLs to try: $urlsToTry');
    
    for (String baseUrl in urlsToTry) {
      try {
        final testUrl = '$baseUrl/auth/ping';
        debugLog('Trying to connect to: $testUrl');
        
        final response = await http.get(
          Uri.parse(testUrl),
          headers: {'Accept': 'application/json'},
        ).timeout(const Duration(seconds: 3));
        
        debugLog('Response status code: ${response.statusCode}');
        
        if (response.statusCode < 400) {
          debugLog('Connection successful to: $baseUrl');
          isBackendRunning = true;
          workingBackendUrl = baseUrl;
          return baseUrl;
        }
      } catch (e) {
        debugLog('Failed connecting to $baseUrl: $e');
      }
    }
    
    debugLog('CRITICAL ERROR: Could not connect to any backend URL!');
    debugLog('Please ensure your backend server is running and listening on one of these URLs: $urlsToTry');
    isBackendRunning = false;
    return null;
  }

  // Updated helper method to test connectivity for web
  static Future<bool> testConnection() async {
    final url = '$baseUrl/auth';
    print('Testing connection to: $url');
    
    try {
      final response = await http.get(
        Uri.parse(url),
        headers: {'Accept': 'application/json'},
      ).timeout(const Duration(seconds: 5));
      
      print('Connection test result: ${response.statusCode}');
      return response.statusCode < 400; // Any 2xx or 3xx status is considered success
    } catch (e) {
      print('Connection test failed: $e');
      return false;
    }
  }
  
  // Helper method for logging in debug mode
  static void debugLog(String message) {
    if (enableDebugLogs) {
      print('🔧 [API] $message');
    }
  }
  
  // Get backend status as a user-friendly message
  static String getBackendStatusMessage() {
    if (isBackendRunning) {
      return "Backend server is running at: $workingBackendUrl";
    } else {
      return "Backend server is not running or not accessible. Make sure to start your .NET API before using this app.";
    }
  }
}

// Enum to make environment switching more explicit and readable
enum ConnectionMode {
  production,
  androidEmulator,
  iOSSimulator,
  webBrowser,
  localNetwork,
  auto,
}
