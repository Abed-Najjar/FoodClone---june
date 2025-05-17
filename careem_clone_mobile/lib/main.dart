import 'package:careem_clone_mobile/utils/constants.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'providers/auth_provider.dart';
import 'providers/restaurant_provider.dart';
import 'providers/cart_provider.dart';
import 'screens/auth_wrapper.dart';
import 'package:flutter/foundation.dart';
import 'utils/connection_checker.dart';

void main() {
  // Print platform information during startup
  WidgetsFlutterBinding.ensureInitialized();
  
  // Check connection to server
  ConnectionChecker.logConnectionInfo();
  
  // Add error handling for null safety issues
  FlutterError.onError = (FlutterErrorDetails details) {
    FlutterError.presentError(details);
    if (kReleaseMode) {
      // In release mode, log to a service
    } else {
      // In debug mode, print to console
      print('Flutter error caught: ${details.exception}');
    }
  };
  
  // Add connection test at startup
  _testApiConnection();
  
  runApp(const MyApp());
}

// Add connection test at startup
void _testApiConnection() async {
  final connected = await AppConstants.testConnection();
  if (connected) {
    print('API connection successful to: ${AppConstants.baseUrl}');
  } else {
    print('API connection failed to: ${AppConstants.baseUrl}');
    // Show a warning dialog or banner that the app may not work properly
  }
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MultiProvider(
      providers: [
        ChangeNotifierProvider(create: (_) => AuthProvider()),
        ChangeNotifierProvider(create: (_) => RestaurantProvider()),
        ChangeNotifierProvider(create: (_) => CartProvider()),
      ],
      child: MaterialApp(
        title: 'Careem Clone',
        debugShowCheckedModeBanner: false,
        theme: ThemeData(
          colorScheme: ColorScheme.fromSeed(seedColor: Colors.green),
          useMaterial3: true,
          appBarTheme: const AppBarTheme(
            centerTitle: true,
            backgroundColor: Colors.green,
            foregroundColor: Colors.white,
          ),
          elevatedButtonTheme: ElevatedButtonThemeData(
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.green,
              foregroundColor: Colors.white,
              minimumSize: const Size(double.infinity, 50),
            ),
          ),
        ),
        home: const AuthWrapper(),
      ),
    );
  }
}
