import 'dart:async';
import 'package:http/http.dart' as http;

class ConnectionTester {
  static const List<String> _urlsToTest = [
    'http://10.0.2.2:5236/api',
    'http://localhost:5236/api',
    'http://192.168.56.1:5236/api',
    'http://127.0.0.1:5236/api',
  ];
  
  static Future<String?> findWorkingUrl() async {
    for (String url in _urlsToTest) {
      try {
        final response = await http.get(
          Uri.parse('$url/health'), // Assuming you have a health endpoint
        ).timeout(const Duration(seconds: 3));
        
        if (response.statusCode == 200) {
          print('Connection successful to: $url');
          return url;
        }
      } catch (e) {
        print('Failed to connect to $url: $e');
      }
    }
    return null;
  }
}
