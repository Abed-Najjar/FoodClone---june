import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { CmsService } from '../../../services/cms.service';
import { Order, OrderItem } from '../../../models/order.model';
import { PagedResult, PaginationParams } from '../../../types/pagination.interface';
import { PaginationComponent } from '../../shared/pagination/pagination.component';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, PaginationComponent],
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
  modalMode: 'view' | 'edit' | null = null;
  orderForm: FormGroup;

  // Pagination properties
  ordersPagedResult: PagedResult<Order> | null = null;
  ordersPagination: PaginationParams = { pageNumber: 1, pageSize: 4 };

  // Available payment methods and statuses
  availablePaymentMethods = ['Cash', 'Credit Card', 'Debit Card', 'Digital Wallet'];
  availableStatuses = ['pending', 'processing', 'delivering', 'completed', 'cancelled'];

  constructor(
    private cmsService: CmsService,
    private fb: FormBuilder
  ) {
    this.orderForm = this.fb.group({
      status: ['', Validators.required]
    });
  }

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
    this.modalMode = 'view';
  }

  closeOrderDetails() {
    this.selectedOrder = null;
    this.modalMode = null;
    this.orderForm.reset();
  }
  
  calculateSubtotal(order: Order): number {
    if (!order.orderItems || order.orderItems.length === 0) return 0;

    return order.orderItems.reduce((total: number, item: any): number => {
      return total + (item.totalPrice || 0);
    }, 0);
  }

  updateOrderStatus(order: Order, newStatus: string) {
    const statusCapitalized = newStatus.charAt(0).toUpperCase() + newStatus.slice(1);
    if (confirm(`Are you sure you want to change the order status to ${statusCapitalized}?`)) {
      this.loading = true;
      this.error = null;

      console.log(`Updating order ${order.id} status to: ${newStatus}`);

      this.cmsService.updateOrderStatus(order.id, newStatus).subscribe({
        next: (response) => {
          this.loading = false;
          console.log('Update response:', response);
          
          if (response.success) {
            // Update in the main orders array
            const orderIndex = this.orders.findIndex(o => o.id === order.id);
            if (orderIndex !== -1) {
              this.orders[orderIndex].status = statusCapitalized;
            }

            // Update in the filtered orders array
            const filteredOrderIndex = this.filteredOrders.findIndex(o => o.id === order.id);
            if (filteredOrderIndex !== -1) {
              this.filteredOrders[filteredOrderIndex].status = statusCapitalized;
            }

            // Update the selected order if it's open
            if (this.selectedOrder && this.selectedOrder.id === order.id) {
              this.selectedOrder.status = statusCapitalized;
              // Update the form as well if in edit mode
              if (this.modalMode === 'edit') {
                this.orderForm.patchValue({ status: newStatus });
              }
            }

            alert(`Order #${order.id} status has been updated to ${statusCapitalized}`);
          } else {
            this.error = response.errorMessage || 'Failed to update order status';
            alert(`Failed to update order status: ${this.error}`);
          }
        },
        error: (err) => {
          this.loading = false;
          const errorMessage = err.error?.errorMessage || err.message || 'Unknown error occurred';
          this.error = `Error updating order status: ${errorMessage}`;
          console.error('Error updating order status:', err);
          alert(`Error updating order status: ${errorMessage}`);
        }
      });
    }
  }

  editOrder(order: Order) {
    this.selectedOrder = { ...order };
    this.modalMode = 'edit';
    this.populateEditForm(order);
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

  // Edit form methods
  populateEditForm(order: Order) {
    // Only populate the status field since that's all we can edit
    this.orderForm.patchValue({
      status: order.status.toLowerCase()
    });
  }

  saveOrderChanges() {
    if (this.orderForm.invalid || !this.selectedOrder) {
      this.markFormGroupTouched(this.orderForm);
      return;
    }

    const formValue = this.orderForm.value;
    
    // For now, we can only update the status since the backend doesn't have a full order update endpoint
    // If the status changed, update it first
    if (formValue.status !== this.selectedOrder.status.toLowerCase()) {
      this.updateOrderStatus(this.selectedOrder, formValue.status);
    } else {
      // If no status change, just show a message about limited edit capabilities
      alert('Note: Currently only order status can be updated through the system. Other order details require manual database updates.');
      this.closeOrderDetails();
    }
  }

  // Helper method to mark form controls as touched
  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      } else if (control instanceof FormArray) {
        control.controls.forEach(arrayControl => {
          if (arrayControl instanceof FormGroup) {
            this.markFormGroupTouched(arrayControl);
          } else {
            arrayControl.markAsTouched();
          }
        });
      } else {
        control?.markAsTouched();
      }
    });
  }

  // Helper method to check if a form field has errors and is touched
  isFieldInvalid(fieldName: string): boolean {
    const field = this.orderForm.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }

  // Helper method to get form field error message
  getFieldError(fieldName: string): string {
    const field = this.orderForm.get(fieldName);
    if (field?.errors && field.touched) {
      if (field.errors['required']) return `${fieldName} is required`;
      if (field.errors['min']) return `${fieldName} must be greater than ${field.errors['min'].min}`;
    }
    return '';
  }
}
