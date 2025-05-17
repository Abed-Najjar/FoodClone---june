import 'dish.dart';

class Category {
  int id;
  String name;
  String? description;
  String? image;
  List<Dish>? dishes;
  
  Category({
    required this.id,
    required this.name,
    this.description,
    this.image,
    this.dishes,
  });
  
  factory Category.fromJson(Map<String, dynamic> json) {
    List<Dish>? dishesList;
    if (json['dishes'] != null) {
      dishesList = (json['dishes'] as List)
          .map((dish) => Dish.fromJson(dish))
          .toList();
    }
    
    return Category(
      id: json['id'],
      name: json['name'],
      description: json['description'],
      image: json['imageUrl'],
      dishes: dishesList,
    );
  }
}
