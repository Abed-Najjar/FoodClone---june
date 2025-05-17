import 'package:flutter/material.dart';
import '../models/user.dart';
import '../services/auth_service.dart';

class AuthProvider extends ChangeNotifier {
  final AuthService _authService = AuthService();
  User? _user;
  bool _isLoading = false;
  String _errorMessage = '';
  
  User? get user => _user;
  bool get isLoading => _isLoading;
  String get errorMessage => _errorMessage;
  
  Future<bool> isLoggedIn() async {
    return await _authService.isLoggedIn();
  }
  
  Future<bool> login(String email, String password) async {
    _isLoading = true;
    _errorMessage = '';
    notifyListeners();
    
    try {
      _user = await _authService.login(email, password);
      _isLoading = false;
      notifyListeners();
      return true;
    } catch (e) {
      _isLoading = false;
      _errorMessage = e.toString();
      notifyListeners();
      return false;
    }
  }
  
  Future<bool> register(String username, String email, String password) async {
    _isLoading = true;
    _errorMessage = '';
    notifyListeners();
    
    try {
      _user = await _authService.register(username, email, password);
      _isLoading = false;
      notifyListeners();
      return true;
    } catch (e) {
      _isLoading = false;
      _errorMessage = e.toString();
      notifyListeners();
      return false;
    }
  }
  
  Future<void> logout() async {
    await _authService.logout();
    _user = null;
    notifyListeners();
  }
}
