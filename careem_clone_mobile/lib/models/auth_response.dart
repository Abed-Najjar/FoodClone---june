class AuthResponse {
  final int? userId;
  final String? token;
  final String? error;
  
  AuthResponse({this.userId, this.token, this.error});
  
  factory AuthResponse.fromJson(Map<String, dynamic> json) {
    // Safely parse JSON with null checks
    return AuthResponse(
      userId: json['userId'] != null ? int.tryParse(json['userId'].toString()) : null,
      token: json['token'],
      error: json['error'],
    );
  }
  
  bool get isSuccess => userId != null && token != null;
}
