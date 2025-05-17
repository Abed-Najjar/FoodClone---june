import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../providers/restaurant_provider.dart';
import '../providers/cart_provider.dart';
import '../models/dish.dart';
import '../models/category.dart';
import 'cart_screen.dart';

class RestaurantDetailScreen extends StatefulWidget {
  final int restaurantId;
  
  const RestaurantDetailScreen({super.key, required this.restaurantId});

  @override
  State<RestaurantDetailScreen> createState() => _RestaurantDetailScreenState();
}

class _RestaurantDetailScreenState extends State<RestaurantDetailScreen> {
  @override
  void initState() {
    super.initState();
    Future.microtask(() =>
      Provider.of<RestaurantProvider>(context, listen: false)
          .fetchRestaurantDetails(widget.restaurantId)
    );
  }

  @override
  void dispose() {
    Provider.of<RestaurantProvider>(context, listen: false).clearSelectedRestaurant();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final cartProvider = Provider.of<CartProvider>(context);
    
    return Scaffold(
      appBar: AppBar(
        title: const Text('Restaurant Details'),
        backgroundColor: Theme.of(context).colorScheme.primary,
        foregroundColor: Colors.white,
        actions: [
          Stack(
            alignment: Alignment.center,
            children: [
              IconButton(
                onPressed: cartProvider.itemCount > 0 
                  ? () {
                      Navigator.of(context).push(
                        MaterialPageRoute(
                          builder: (_) => const CartScreen(),
                        ),
                      );
                    }
                  : null,
                icon: const Icon(Icons.shopping_cart),
              ),
              if (cartProvider.itemCount > 0)
                Positioned(
                  top: 8,
                  right: 8,
                  child: Container(
                    padding: const EdgeInsets.all(2),
                    decoration: BoxDecoration(
                      color: Colors.red,
                      borderRadius: BorderRadius.circular(10),
                    ),
                    constraints: const BoxConstraints(
                      minWidth: 16,
                      minHeight: 16,
                    ),
                    child: Text(
                      '${cartProvider.itemCount}',
                      style: const TextStyle(
                        fontSize: 10,
                        color: Colors.white,
                      ),
                      textAlign: TextAlign.center,
                    ),
                  ),
                ),
            ],
          ),
        ],
      ),
      body: Consumer<RestaurantProvider>(
        builder: (ctx, provider, child) {
          if (provider.isLoading) {
            return const Center(child: CircularProgressIndicator());
          }
          
          if (provider.errorMessage.isNotEmpty) {
            return Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Text(
                    'Error: ${provider.errorMessage}',
                    style: const TextStyle(color: Colors.red),
                  ),
                  const SizedBox(height: 16),
                  ElevatedButton(
                    onPressed: () => provider.fetchRestaurantDetails(widget.restaurantId),
                    child: const Text('Try Again'),
                  ),
                ],
              ),
            );
          }
          
          if (provider.selectedRestaurant == null) {
            return const Center(child: Text('Restaurant not found'));
          }
          
          final restaurant = provider.selectedRestaurant!;
          
          return SingleChildScrollView(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // Restaurant Header
                if (restaurant.image != null && restaurant.image!.isNotEmpty)
                  SizedBox(
                    height: 200,
                    width: double.infinity,
                    child: Image.network(
                      restaurant.image!,
                      fit: BoxFit.cover,
                      errorBuilder: (_, __, ___) => Container(
                        height: 200,
                        color: Colors.grey.shade300,
                        child: const Icon(Icons.restaurant, size: 80),
                      ),
                    ),
                  )
                else
                  Container(
                    height: 200,
                    color: Colors.grey.shade300,
                    width: double.infinity,
                    child: const Icon(Icons.restaurant, size: 80),
                  ),
                
                // Restaurant Info
                Padding(
                  padding: const EdgeInsets.all(16.0),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          Expanded(
                            child: Text(
                              restaurant.name,
                              style: const TextStyle(
                                fontSize: 24,
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                          ),
                          Row(
                            children: [
                              const Icon(Icons.star, color: Colors.amber),
                              const SizedBox(width: 4),
                              Text(
                                restaurant.rating.toString(),
                                style: const TextStyle(
                                  fontWeight: FontWeight.bold,
                                  fontSize: 18,
                                ),
                              ),
                            ],
                          ),
                        ],
                      ),
                      const SizedBox(height: 8),
                      if (restaurant.address != null && restaurant.address!.isNotEmpty)
                        Row(
                          children: [
                            const Icon(Icons.location_on, color: Colors.grey),
                            const SizedBox(width: 4),
                            Expanded(
                              child: Text(
                                restaurant.address!,
                                style: TextStyle(color: Colors.grey.shade700, fontSize: 16),
                              ),
                            ),
                          ],
                        ),
                      const SizedBox(height: 12),
                      if (restaurant.description != null && restaurant.description!.isNotEmpty)
                        Text(
                          restaurant.description!,
                          style: TextStyle(color: Colors.grey.shade600),
                        ),
                      const SizedBox(height: 24),
                      const Text(
                        'Menu',
                        style: TextStyle(
                          fontSize: 20,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      const Divider(),
                    ],
                  ),
                ),
                
                // Menu Categories
                if (restaurant.categories != null && restaurant.categories!.isNotEmpty)
                  ListView.builder(
                    physics: const NeverScrollableScrollPhysics(),
                    shrinkWrap: true,
                    itemCount: restaurant.categories!.length,
                    itemBuilder: (ctx, index) {
                      final category = restaurant.categories![index];
                      return CategoryMenu(
                        category: category,
                        restaurantId: restaurant.id,
                      );
                    },
                  )
                else
                  const Padding(
                    padding: EdgeInsets.all(16.0),
                    child: Center(child: Text('No menu items available')),
                  ),
                
                const SizedBox(height: 20),
              ],
            ),
          );
        },
      ),
    );
  }
}

class CategoryMenu extends StatelessWidget {
  final Category category;
  final int restaurantId;
  
  const CategoryMenu({super.key, required this.category, required this.restaurantId});

  @override
  Widget build(BuildContext context) {
    if (category.dishes == null || category.dishes!.isEmpty) {
      return const SizedBox.shrink();
    }
    
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: 16.0, vertical: 8.0),
          child: Text(
            category.name,
            style: const TextStyle(
              fontSize: 18,
              fontWeight: FontWeight.bold,
            ),
          ),
        ),
        if (category.description != null && category.description!.isNotEmpty)
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 16.0),
            child: Text(
              category.description!,
              style: TextStyle(color: Colors.grey.shade600, fontSize: 14),
            ),
          ),
        ListView.builder(
          physics: const NeverScrollableScrollPhysics(),
          shrinkWrap: true,
          itemCount: category.dishes!.length,
          itemBuilder: (ctx, index) {
            final dish = category.dishes![index];
            return DishItem(
              dish: dish,
              restaurantId: restaurantId,
            );
          },
        ),
        const Divider(thickness: 1),
      ],
    );
  }
}

class DishItem extends StatelessWidget {
  final Dish dish;
  final int restaurantId;
  
  const DishItem({super.key, required this.dish, required this.restaurantId});

  @override
  Widget build(BuildContext context) {
    final cartProvider = Provider.of<CartProvider>(context);
    final dishInCart = cartProvider.items[dish.id]?.quantity ?? 0;
    
    return Card(
      elevation: 0,
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 4),
      child: Padding(
        padding: const EdgeInsets.all(8.0),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.center,
          children: [
            if (dish.image != null && dish.image!.isNotEmpty)
              ClipRRect(
                borderRadius: BorderRadius.circular(8),
                child: SizedBox(
                  width: 80,
                  height: 80,
                  child: Image.network(
                    dish.image!,
                    fit: BoxFit.cover,
                    errorBuilder: (_, __, ___) => Container(
                      width: 80,
                      height: 80,
                      color: Colors.grey.shade300,
                      child: const Icon(Icons.fastfood),
                    ),
                  ),
                ),
              )
            else
              ClipRRect(
                borderRadius: BorderRadius.circular(8),
                child: Container(
                  width: 80,
                  height: 80,
                  color: Colors.grey.shade300,
                  child: const Icon(Icons.fastfood),
                ),
              ),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    dish.name,
                    style: const TextStyle(
                      fontWeight: FontWeight.bold,
                      fontSize: 16,
                    ),
                  ),
                  if (dish.description != null && dish.description!.isNotEmpty)
                    Text(
                      dish.description!,
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                      style: TextStyle(color: Colors.grey.shade600, fontSize: 14),
                    ),
                  const SizedBox(height: 4),
                  Text(
                    '\$${dish.price.toStringAsFixed(2)}',
                    style: TextStyle(
                      color: Theme.of(context).colorScheme.primary,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ],
              ),
            ),
            Column(
              children: [
                IconButton(
                  onPressed: () {
                    cartProvider.addItem(dish, restaurantId);
                  },
                  icon: Icon(
                    Icons.add_circle,
                    color: Theme.of(context).colorScheme.primary,
                  ),
                ),
                if (dishInCart > 0)
                  Text(
                    dishInCart.toString(),
                    style: const TextStyle(fontWeight: FontWeight.bold),
                  ),
                if (dishInCart > 0)
                  IconButton(
                    onPressed: () {
                      cartProvider.removeItem(dish.id);
                    },
                    icon: Icon(
                      Icons.remove_circle,
                      color: Theme.of(context).colorScheme.error,
                    ),
                  ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
