import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CmsService } from '../../../services/cms.service';
import { Order } from '../../../models/order.model';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="orders-manager">
      <h2>Orders Management</h2>

      <div class="action-bar">
        <div class="search-container">
          <input type="text" [(ngModel)]="searchTerm" placeholder="Search by order ID or customer..." class="search-input" (input)="filterOrders()">
        </div>

        <div class="filter-container">
          <select [(ngModel)]="statusFilter" (change)="filterOrders()" class="status-filter">
            <option value="">All Statuses</option>
            <option value="pending">Pending</option>
            <option value="processing">Processing</option>
            <option value="delivering">Delivering</option>
            <option value="completed">Completed</option>
            <option value="cancelled">Cancelled</option>
          </select>
        </div>
      </div>

      <!-- Orders List -->
      <div class="orders-list">
        <div *ngIf="loading" class="loading">Loading orders...</div>
        <div *ngIf="error" class="error-message">{{ error }}</div>

        <table *ngIf="!loading && !error && filteredOrders.length > 0">
          <thead>
            <tr>
              <th>Order ID</th>
              <th>Customer</th>
              <th>Restaurant</th>
              <th>Items</th>
              <th>Total</th>
              <th>Date</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let order of filteredOrders">              <td>#{{ order.id }}</td>
              <td>{{ order.userName }}</td>
              <td>{{ order.restaurantName }}</td>
              <td>{{ order.dishes.length || 0 }}</td>
              <td>{{ order.totalPrice | currency }}</td>
              <td>{{ order.orderDate | date:'medium' }}</td>
              <td>
                <span class="status-badge" [ngClass]="'status-' + order.status.toLowerCase()">
                  {{ order.status }}
                </span>
              </td>
              <td class="actions">
                <button (click)="viewOrderDetails(order)" class="view-btn">View</button>
                <button *ngIf="order.status === 'Pending'" (click)="updateOrderStatus(order, 'Processing')" class="process-btn">Process</button>
                <button *ngIf="order.status === 'Processing'" (click)="updateOrderStatus(order, 'Delivering')" class="deliver-btn">Deliver</button>
                <button *ngIf="order.status === 'Delivering'" (click)="updateOrderStatus(order, 'Completed')" class="complete-btn">Complete</button>
                <button *ngIf="order.status !== 'Completed' && order.status !== 'Cancelled'" (click)="updateOrderStatus(order, 'Cancelled')" class="cancel-btn">Cancel</button>
              </td>
            </tr>
          </tbody>
        </table>

        <div *ngIf="!loading && !error && filteredOrders.length === 0" class="no-data">
          No orders found matching your criteria.
        </div>
      </div>

      <!-- Order Details Modal -->
      <div *ngIf="selectedOrder" class="order-details-modal">
        <div class="modal-content">
          <div class="modal-header">
            <h3>Order #{{ selectedOrder.id }} Details</h3>
            <button (click)="closeOrderDetails()" class="close-btn">&times;</button>
          </div>

          <div class="modal-body">
            <div class="order-info">
              <div class="info-group">
                <h4>Customer Information</h4>
                <p><strong>Name:</strong> {{ selectedOrder.userName }}</p>
                <p><strong>Email:</strong> {{ selectedOrder.userEmail }}</p>
                <p><strong>Phone:</strong> {{ selectedOrder.phoneNumber || 'Not provided' }}</p>
              </div>

              <div class="info-group">
                <h4>Order Information</h4>
                <p><strong>Restaurant:</strong> {{ selectedOrder.restaurantName }}</p>
                <p><strong>Order Date:</strong> {{ selectedOrder.orderDate | date:'medium' }}</p>
                <p><strong>Status:</strong> <span class="status-badge" [ngClass]="'status-' + selectedOrder.status.toLowerCase()">{{ selectedOrder.status }}</span></p>
              </div>

              <div class="info-group">
                <h4>Delivery Address</h4>
                <p>{{ selectedOrder.deliveryAddress || 'Not provided' }}</p>
              </div>
            </div>

            <div class="order-items">
              <h4>Order Items</h4>
              <table class="items-table">
                <thead>
                  <tr>
                    <th>Item</th>
                    <th>Price</th>
                    <th>Quantity</th>
                    <th>Subtotal</th>
                  </tr>
                </thead>                <tbody>
                  <tr *ngFor="let item of selectedOrder.dishes">
                    <td>{{ item.name }}</td>
                    <td>{{ item.price | currency }}</td>
                    <td>{{ item.quantity }}</td>
                    <td>{{ (item.price * item.quantity) | currency }}</td>
                  </tr>
                </tbody>                <tfoot>
                  <tr>
                    <td colspan="3" class="total-label">Subtotal</td>
                    <td>{{ calculateSubtotal(selectedOrder) | currency }}</td>
                  </tr>
                  <tr>
                    <td colspan="3" class="total-label">Delivery Fee</td>
                    <td>{{ selectedOrder.deliveryFee | currency }}</td>
                  </tr>
                  <tr class="total-row">
                    <td colspan="3" class="total-label">Total</td>
                    <td>{{ selectedOrder.totalPrice | currency }}</td>
                  </tr>
                </tfoot>
              </table>
            </div>

            <div class="status-update">
              <h4>Update Status</h4>
              <div class="status-buttons">
                <button *ngIf="selectedOrder.status !== 'Pending'" (click)="updateOrderStatus(selectedOrder, 'Pending')" class="status-btn status-pending">Pending</button>
                <button *ngIf="selectedOrder.status !== 'Processing'" (click)="updateOrderStatus(selectedOrder, 'Processing')" class="status-btn status-processing">Processing</button>
                <button *ngIf="selectedOrder.status !== 'Delivering'" (click)="updateOrderStatus(selectedOrder, 'Delivering')" class="status-btn status-delivering">Delivering</button>
                <button *ngIf="selectedOrder.status !== 'Completed'" (click)="updateOrderStatus(selectedOrder, 'Completed')" class="status-btn status-completed">Completed</button>
                <button *ngIf="selectedOrder.status !== 'Cancelled'" (click)="updateOrderStatus(selectedOrder, 'Cancelled')" class="status-btn status-cancelled">Cancelled</button>
              </div>
            </div>
          </div>

          <div class="modal-footer">
            <button (click)="closeOrderDetails()" class="close-btn-secondary">Close</button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .orders-manager {
      padding: 1rem;
      max-width: 100%;
    }

    h2 {
      margin-bottom: 1.5rem;
      color: #333;
    }

    .action-bar {
      display: flex;
      justify-content: space-between;
      margin-bottom: 1.5rem;
    }

    .search-input {
      padding: 0.5rem;
      border: 1px solid #ddd;
      border-radius: 4px;
      width: 250px;
    }

    .status-filter {
      padding: 0.5rem;
      border: 1px solid #ddd;
      border-radius: 4px;
      background-color: white;
      min-width: 150px;
    }

    table {
      width: 100%;
      border-collapse: collapse;
      margin-top: 1rem;
      background-color: white;
      box-shadow: 0 2px 10px rgba(0,0,0,0.1);
      border-radius: 8px;
      overflow: hidden;
    }

    th, td {
      padding: 1rem;
      text-align: left;
      border-bottom: 1px solid #eee;
    }

    th {
      background-color: #f5f5f5;
      font-weight: 600;
    }

    .status-badge {
      display: inline-block;
      padding: 0.25rem 0.5rem;
      border-radius: 20px;
      font-size: 0.75rem;
      font-weight: 600;
      text-transform: uppercase;
    }

    .status-pending {
      background-color: #ff9800;
      color: white;
    }

    .status-processing {
      background-color: #2196f3;
      color: white;
    }

    .status-delivering {
      background-color: #9c27b0;
      color: white;
    }

    .status-completed {
      background-color: #4caf50;
      color: white;
    }

    .status-cancelled {
      background-color: #f44336;
      color: white;
    }

    .actions {
      display: flex;
      gap: 0.5rem;
      flex-wrap: wrap;
    }

    .view-btn, .process-btn, .deliver-btn, .complete-btn, .cancel-btn {
      padding: 0.25rem 0.5rem;
      border: none;
      border-radius: 4px;
      font-size: 0.75rem;
      cursor: pointer;
      color: white;
    }

    .view-btn {
      background-color: #607d8b;
    }

    .process-btn {
      background-color: #2196f3;
    }

    .deliver-btn {
      background-color: #9c27b0;
    }

    .complete-btn {
      background-color: #4caf50;
    }

    .cancel-btn {
      background-color: #f44336;
    }

    .loading, .no-data {
      text-align: center;
      padding: 2rem;
      color: #757575;
    }

    .error-message {
      color: #f44336;
      padding: 1rem;
      background-color: #ffebee;
      border-radius: 4px;
      margin-bottom: 1rem;
    }

    /* Modal Styles */
    .order-details-modal {
      position: fixed;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      background-color: rgba(0, 0, 0, 0.5);
      display: flex;
      justify-content: center;
      align-items: center;
      z-index: 1000;
    }

    .modal-content {
      background-color: white;
      border-radius: 8px;
      width: 80%;
      max-width: 900px;
      max-height: 90vh;
      overflow-y: auto;
      box-shadow: 0 5px 15px rgba(0, 0, 0, 0.3);
    }

    .modal-header {
      padding: 1rem 1.5rem;
      border-bottom: 1px solid #eee;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .modal-header h3 {
      margin: 0;
      color: #333;
    }

    .close-btn {
      background: none;
      border: none;
      font-size: 1.5rem;
      cursor: pointer;
      color: #757575;
    }

    .modal-body {
      padding: 1.5rem;
    }

    .modal-footer {
      padding: 1rem 1.5rem;
      border-top: 1px solid #eee;
      display: flex;
      justify-content: flex-end;
    }

    .close-btn-secondary {
      background-color: #f5f5f5;
      color: #333;
      border: 1px solid #ddd;
      padding: 0.5rem 1rem;
      border-radius: 4px;
      cursor: pointer;
    }

    .order-info {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 1.5rem;
      margin-bottom: 2rem;
    }

    .info-group {
      background-color: #f9f9f9;
      padding: 1rem;
      border-radius: 8px;
    }

    .info-group h4 {
      margin-top: 0;
      margin-bottom: 0.5rem;
      color: #333;
      border-bottom: 1px solid #eee;
      padding-bottom: 0.5rem;
    }

    .info-group p {
      margin: 0.5rem 0;
    }

    .order-items {
      margin-bottom: 2rem;
    }

    .order-items h4 {
      margin-top: 0;
      margin-bottom: 1rem;
      color: #333;
    }

    .items-table {
      width: 100%;
      border-collapse: collapse;
      margin-top: 0.5rem;
    }

    .items-table th {
      background-color: #f5f5f5;
      text-align: left;
      padding: 0.75rem;
      border-bottom: 1px solid #ddd;
    }

    .items-table td {
      padding: 0.75rem;
      border-bottom: 1px solid #eee;
    }

    .items-table tfoot {
      font-weight: 500;
    }

    .items-table .total-label {
      text-align: right;
    }

    .items-table .total-row {
      font-weight: 700;
      background-color: #f9f9f9;
    }

    .status-update {
      margin-top: 2rem;
    }

    .status-update h4 {
      margin-top: 0;
      margin-bottom: 1rem;
      color: #333;
    }

    .status-buttons {
      display: flex;
      flex-wrap: wrap;
      gap: 0.5rem;
    }

    .status-btn {
      padding: 0.5rem 1rem;
      border: none;
      border-radius: 4px;
      font-size: 0.875rem;
      cursor: pointer;
      color: white;
    }
  `]
})
export class OrdersComponent implements OnInit {
  orders: Order[] = [];
  filteredOrders: Order[] = [];
  loading: boolean = false;
  error: string | null = null;
  searchTerm: string = '';
  statusFilter: string = '';
  selectedOrder: Order | null = null;

  constructor(private cmsService: CmsService) {}

  ngOnInit() {
    this.loadOrders();
  }

  loadOrders() {
    this.loading = true;
    this.error = null;

    this.cmsService.getAllOrders().subscribe({
      next: (response) => {
        if (response.success) {
          this.orders = response.data;
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
    if (!order.dishes || order.dishes.length === 0) return 0;

    return order.dishes.reduce((total: number, item: any): number => {
      return total + ((item.price || 0) * (item.quantity || 1));
    }, 0);
  }

  updateOrderStatus(order: Order, newStatus: string) {
    if (confirm(`Are you sure you want to change the order status to ${newStatus}?`)) {
      // In a real application, you would call an API endpoint to update the order status
      // For this example, we'll just update it locally

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

      // In a real application, you would show feedback based on the API response
      alert(`Order #${order.id} status has been updated to ${newStatus}`);
    }
  }
}
