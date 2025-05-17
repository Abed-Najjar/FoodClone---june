class User {
  final int? id;
  final String? username;
  final String? email;
  final String? token;
  final String? role;
  // Add other properties as needed

  User({
    this.id,
    this.username,
    this.email,
    this.token,
    this.role,
    // Initialize other properties here
  });

  factory User.fromJson(Map<String, dynamic> json) {
    // Handle different API response formats
    final userData = json.containsKey('user') ? json['user'] : json;
    
    return User(
      id: userData['id'] as int?,
      username: userData['username'] as String?,
      email: userData['email'] as String?,
      token: json['token'] as String? ?? userData['token'] as String?,
      // Map other properties
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'username': username,
      'email': email,
      'token': token,
      // Include other properties
    };
  }
}
