import 'package:flutter/material.dart';
import '../models/order.dart';
import '../services/order_service.dart';

class OrdersScreen extends StatefulWidget {
  const OrdersScreen({super.key});

  @override
  State<OrdersScreen> createState() => _OrdersScreenState();
}

class _OrdersScreenState extends State<OrdersScreen> {
  final OrderService _orderService = OrderService();
  List<Order> _orders = [];
  bool _isLoading = false;
  String _errorMessage = '';

  @override
  void initState() {
    super.initState();
    _fetchOrders();
  }

  Future<void> _fetchOrders() async {
    setState(() {
      _isLoading = true;
      _errorMessage = '';
    });
    
    try {
      final orders = await _orderService.getOrders();
      setState(() {
        _orders = orders;
        _isLoading = false;
      });
    } catch (e) {
      setState(() {
        _errorMessage = e.toString();
        _isLoading = false;
      });
    }
  }  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Your Orders'),
        backgroundColor: Theme.of(context).colorScheme.primary,
        foregroundColor: Colors.white,
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : _errorMessage.isNotEmpty
              ? Center(
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Text(
                        'Error: $_errorMessage',
                        style: const TextStyle(color: Colors.red),
                      ),
                      const SizedBox(height: 16),
                      ElevatedButton(
                        onPressed: _fetchOrders,
                        child: const Text('Try Again'),
                      ),
                    ],
                  ),
                )
              : _orders.isEmpty
                  ? const Center(
                      child: Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          Icon(Icons.receipt_long, size: 80, color: Colors.grey),
                          SizedBox(height: 16),
                          Text(
                            'You have no orders yet',
                            style: TextStyle(fontSize: 18, color: Colors.grey),
                          ),
                        ],
                      ),
                    )
                  : ListView.builder(
                      itemCount: _orders.length,
                      itemBuilder: (ctx, index) {
                        final order = _orders[index];
                        return OrderCard(order: order);
                      },
                    ),
    );
  }
}

class OrderCard extends StatelessWidget {
  final Order order;
  
  const OrderCard({super.key, required this.order});

  String _getStatusIcon(String status) {
    switch (status.toLowerCase()) {
      case 'pending':
        return '⏳';
      case 'preparing':
        return '👨‍🍳';
      case 'ready':
        return '✅';
      case 'delivered':
        return '🚚';
      case 'cancelled':
        return '❌';
      default:
        return '📦';
    }
  }

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.all(8),
      child: ExpansionTile(
        title: Text(
          'Order #${order.id}',
          style: const TextStyle(fontWeight: FontWeight.bold),
        ),
        subtitle: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const SizedBox(height: 4),
            Text('Date: ${order.createdAt.toString().split('.')[0]}'),
            Text(
              'Status: ${_getStatusIcon(order.status)} ${order.status}',
              style: TextStyle(
                color: order.status.toLowerCase() == 'cancelled'
                    ? Colors.red
                    : Theme.of(context).colorScheme.primary,
              ),
            ),
            Text(
              'Total: \$${order.totalAmount.toStringAsFixed(2)}',
              style: const TextStyle(fontWeight: FontWeight.bold),
            ),
          ],
        ),
        children: [
          if (order.items != null && order.items!.isNotEmpty)
            ListView.builder(
              shrinkWrap: true,
              physics: const NeverScrollableScrollPhysics(),
              itemCount: order.items!.length,
              itemBuilder: (ctx, index) {
                final item = order.items![index];
                final dish = item.dish;
                
                if (dish == null) {
                  return const SizedBox.shrink();
                }
                
                return ListTile(
                  leading: Container(
                    width: 40,
                    height: 40,
                    decoration: BoxDecoration(
                      color: Colors.grey.shade200,
                      borderRadius: BorderRadius.circular(4),
                    ),
                    child: dish.image != null && dish.image!.isNotEmpty
                        ? Image.network(
                            dish.image!,
                            fit: BoxFit.cover,
                            errorBuilder: (_, __, ___) => const Icon(Icons.fastfood),
                          )
                        : const Icon(Icons.fastfood),
                  ),
                  title: Text(dish.name),
                  subtitle: Text('${item.quantity} x \$${item.unitPrice.toStringAsFixed(2)}'),
                  trailing: Text('\$${(item.quantity * item.unitPrice).toStringAsFixed(2)}'),
                );
              },
            )
          else
            const Padding(
              padding: EdgeInsets.all(16.0),
              child: Text('No items in this order'),
            ),
        ],
      ),
    );
  }
}
