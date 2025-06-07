import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CmsService } from '../../../services/cms.service';
import { Order } from '../../../models/order.model';
import { PagedResult, PaginationParams } from '../../../types/pagination.interface';
import { PaginationComponent } from '../../shared/pagination/pagination.component';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule, FormsModule, PaginationComponent],
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.css']
})
export class OrdersComponent implements OnInit {
  orders: Order[] = [];
  filteredOrders: Order[] = [];
  loading: boolean = false;
  error: string | null = null;
  searchTerm: string = '';
  statusFilter: string = '';
  selectedOrder: Order | null = null;

  // Pagination properties
  ordersPagedResult: PagedResult<Order> | null = null;
  ordersPagination: PaginationParams = { pageNumber: 1, pageSize: 4 };

  constructor(private cmsService: CmsService) {}

  ngOnInit() {
    this.loadOrders();
  }

  loadOrders() {
    this.loading = true;
    this.error = null;

    this.cmsService.getAllOrders(this.ordersPagination).subscribe({
      next: (response) => {
        if (response.success) {
          this.ordersPagedResult = response.data as PagedResult<Order>;
          this.orders = this.ordersPagedResult.data;
          this.filteredOrders = [...this.orders];
        } else {
          this.error = response.errorMessage || 'Failed to load orders';
        }
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Error loading orders. Please try again.';
        this.loading = false;
        console.error('Error loading orders:', err);
      }
    });
  }

  filterOrders() {
    let filtered = [...this.orders];

    // Apply search term filter
    if (this.searchTerm.trim()) {
      const term = this.searchTerm.toLowerCase().trim();
      filtered = filtered.filter(order =>
        order.id.toString().includes(term) ||
        (order.userName && order.userName.toLowerCase().includes(term)) ||
        (order.restaurantName && order.restaurantName.toLowerCase().includes(term))
      );
    }

    // Apply status filter
    if (this.statusFilter) {
      filtered = filtered.filter(order =>
        order.status.toLowerCase() === this.statusFilter.toLowerCase()
      );
    }

    this.filteredOrders = filtered;
  }

  viewOrderDetails(order: Order) {
    this.selectedOrder = { ...order };
  }

  closeOrderDetails() {
    this.selectedOrder = null;
  }
  
  calculateSubtotal(order: Order): number {
    if (!order.orderItems || order.orderItems.length === 0) return 0;

    return order.orderItems.reduce((total: number, item: any): number => {
      return total + (item.totalPrice || 0);
    }, 0);
  }

  updateOrderStatus(order: Order, newStatus: string) {
    if (confirm(`Are you sure you want to change the order status to ${newStatus}?`)) {
      this.loading = true;
      this.error = null;

      this.cmsService.updateOrderStatus(order.id, newStatus).subscribe({
        next: (response) => {
          this.loading = false;
          if (response.success) {
            // Update in the main orders array
            const orderIndex = this.orders.findIndex(o => o.id === order.id);
            if (orderIndex !== -1) {
              this.orders[orderIndex].status = newStatus;
            }

            // Update in the filtered orders array
            const filteredOrderIndex = this.filteredOrders.findIndex(o => o.id === order.id);
            if (filteredOrderIndex !== -1) {
              this.filteredOrders[filteredOrderIndex].status = newStatus;
            }

            // Update the selected order if it's open
            if (this.selectedOrder && this.selectedOrder.id === order.id) {
              this.selectedOrder.status = newStatus;
            }

            alert(`Order #${order.id} status has been updated to ${newStatus}`);
          } else {
            this.error = response.errorMessage || 'Failed to update order status';
          }
        },
        error: (err) => {
          this.loading = false;
          this.error = 'Error updating order status. Please try again.';
          console.error('Error updating order status:', err);
        }
      });
    }
  }

  editOrder(order: Order) {
    this.selectedOrder = { ...order };
    // The edit functionality is handled through the modal view and status updates
  }

  deleteOrder(order: Order) {
    if (confirm(`Are you sure you want to delete Order #${order.id}? This action cannot be undone.`)) {
      this.loading = true;
      this.error = null;

      this.cmsService.deleteOrder(order.id).subscribe({
        next: (response) => {
          this.loading = false;
          if (response.success) {
            // Remove from orders array
            this.orders = this.orders.filter(o => o.id !== order.id);
            this.filteredOrders = this.filteredOrders.filter(o => o.id !== order.id);
            
            // Close modal if this order is selected
            if (this.selectedOrder && this.selectedOrder.id === order.id) {
              this.selectedOrder = null;
            }
            
            alert(`Order #${order.id} has been deleted successfully.`);
          } else {
            this.error = response.errorMessage || 'Failed to delete order';
          }
        },
        error: (err) => {
          this.loading = false;
          this.error = 'Error deleting order. Please try again.';
          console.error('Error deleting order:', err);
        }
      });
    }
  }

  onOrdersPageChanged(page: number): void {
    this.ordersPagination.pageNumber = page;
    this.loadOrders();
  }
}
