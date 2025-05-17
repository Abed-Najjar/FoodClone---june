import 'category.dart';

class Restaurant {
  final int id;
  final String name;
  final String? description;
  final String? image;
  final String? address;
  final double rating;
  final List<Category>? categories;
  
  Restaurant({
    required this.id,
    required this.name,
    this.description,
    this.image,
    this.address,
    required this.rating,
    this.categories,
  });
  
  factory Restaurant.fromJson(Map<String, dynamic> json) {
    List<Category>? categoriesList;
    if (json['categories'] != null) {
      categoriesList = (json['categories'] as List)
          .map((cat) => Category.fromJson(cat))
          .toList();
    }
    
    return Restaurant(
      id: json['id'],
      name: json['name'],
      description: json['description'],
      image: json['image'],
      address: json['address'],
      rating: json['rating']?.toDouble() ?? 0.0,
      categories: categoriesList,
    );
  }
}
