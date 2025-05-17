import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../providers/restaurant_provider.dart';
import '../models/restaurant.dart';
import 'restaurant_detail_screen.dart';

class RestaurantsScreen extends StatefulWidget {
  const RestaurantsScreen({super.key});

  @override
  State<RestaurantsScreen> createState() => _RestaurantsScreenState();
}

class _RestaurantsScreenState extends State<RestaurantsScreen> {
  @override
  void initState() {
    super.initState();
    Future.microtask(() => 
      Provider.of<RestaurantProvider>(context, listen: false).fetchRestaurants()
    );
  }  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Restaurants'),
        backgroundColor: Theme.of(context).colorScheme.primary,
        foregroundColor: Colors.white,
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
                    onPressed: () => provider.fetchRestaurants(),
                    child: const Text('Try Again'),
                  ),
                ],
              ),
            );
          }
          
          if (provider.restaurants.isEmpty) {
            return const Center(child: Text('No restaurants available'));
          }
          
          return RestaurantList(restaurants: provider.restaurants);
        },
      ),
    );
  }
}

class RestaurantList extends StatelessWidget {
  final List<Restaurant> restaurants;
  
  const RestaurantList({super.key, required this.restaurants});

  @override
  Widget build(BuildContext context) {
    return ListView.builder(
      padding: const EdgeInsets.all(8),
      itemCount: restaurants.length,
      itemBuilder: (ctx, index) {
        final restaurant = restaurants[index];
        return RestaurantCard(restaurant: restaurant);
      },
    );
  }
}

class RestaurantCard extends StatelessWidget {
  final Restaurant restaurant;
  
  const RestaurantCard({super.key, required this.restaurant});

  @override
  Widget build(BuildContext context) {
    return Card(
      elevation: 4,
      margin: const EdgeInsets.symmetric(vertical: 8, horizontal: 4),
      child: InkWell(
        onTap: () {
          Navigator.of(context).push(
            MaterialPageRoute(
              builder: (_) => RestaurantDetailScreen(restaurantId: restaurant.id),
            ),
          );
        },
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            if (restaurant.image != null && restaurant.image!.isNotEmpty)
              SizedBox(
                height: 150,
                width: double.infinity,
                child: Image.network(
                  restaurant.image!,
                  fit: BoxFit.cover,
                  errorBuilder: (_, __, ___) => Container(
                    height: 150,
                    color: Colors.grey.shade300,
                    child: const Icon(Icons.restaurant, size: 50),
                  ),
                ),
              )
            else
              Container(
                height: 150,
                color: Colors.grey.shade300,
                width: double.infinity,
                child: const Icon(Icons.restaurant, size: 50),
              ),
            Padding(
              padding: const EdgeInsets.all(12.0),
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
                            fontSize: 18,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                      ),
                      Row(
                        children: [
                          const Icon(Icons.star, color: Colors.amber, size: 20),
                          const SizedBox(width: 4),
                          Text(
                            restaurant.rating.toString(),
                            style: const TextStyle(
                              fontWeight: FontWeight.bold,
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
                        const Icon(Icons.location_on, size: 16, color: Colors.grey),
                        const SizedBox(width: 4),
                        Expanded(
                          child: Text(
                            restaurant.address!,
                            style: TextStyle(color: Colors.grey.shade700),
                          ),
                        ),
                      ],
                    ),
                  const SizedBox(height: 8),
                  if (restaurant.description != null && restaurant.description!.isNotEmpty)
                    Text(
                      restaurant.description!,
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                      style: TextStyle(color: Colors.grey.shade600),
                    ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}
