import 'package:flutter/foundation.dart';
import 'package:http/http.dart' as http;
import '../utils/constants.dart';

class ConnectionChecker {  // Use the same base URL as in the constants
  static String get baseUrl => AppConstants.baseUrl;
  
  static Future<Map<String, dynamic>> checkConnection() async {
    try {
      // Get the actual IP being used
      final uri = Uri.parse(baseUrl);
      final String ip = uri.host;
      final int port = uri.port;
      
      // Try to ping the server
      final response = await http.get(Uri.parse("$baseUrl/health"))
          .timeout(const Duration(seconds: 5));
      
      return {
        'connected': response.statusCode == 200,
        'ip': ip,
        'port': port,
        'statusCode': response.statusCode,
        'error': null
      };
    } catch (e) {
      return {
        'connected': false,
        'ip': Uri.parse(baseUrl).host,
        'port': Uri.parse(baseUrl).port,
        'statusCode': null,
        'error': e.toString()
      };
    }
  }
  
  // Call this during app startup to log connection info
  static void logConnectionInfo() async {
    if (kDebugMode) {
      final result = await checkConnection();
      print('🌐 Connection Info:');
      print('IP: ${result['ip']}');
      print('Port: ${result['port']}');
      print('Connected: ${result['connected']}');
      if (result['error'] != null) {
        print('Error: ${result['error']}');
      }
    }
  }
}
