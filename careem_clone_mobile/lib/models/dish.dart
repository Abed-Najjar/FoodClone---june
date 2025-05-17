class Dish {
  final int id;
  final String name;
  final String? description;
  final double price;
  final String? image;
  final int categoryId;
  
  Dish({
    required this.id,
    required this.name,
    this.description,
    required this.price,
    this.image,
    required this.categoryId,
  });
  
  factory Dish.fromJson(Map<String, dynamic> json) {
    return Dish(
      id: json['id'],
      name: json['name'],
      description: json['description'],
      price: json['price']?.toDouble() ?? 0.0,
      image: json['imageUrl'],
      categoryId: json['categoryId'],
    );
  }
}
