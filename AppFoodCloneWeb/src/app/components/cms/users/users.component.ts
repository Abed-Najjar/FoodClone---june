import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CmsService } from '../../../services/cms.service';
import { User } from '../../../models/user.model';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="users-manager">
      <h2>Users Management</h2>

      <div class="action-bar">
        <div class="search-container">
          <input type="text" [(ngModel)]="searchTerm" placeholder="Search users..." class="search-input" (input)="filterUsers()">
        </div>
        <div class="filter-container">
          <select [(ngModel)]="roleFilter" (change)="filterUsers()" class="role-filter">
            <option value="">All Roles</option>
            <option value="Admin">Admin</option>
            <option value="User">User</option>
            <option value="Employee">Employee</option>
            <option value="Provider">Provider</option>
          </select>
        </div>
      </div>

      <!-- Users List -->
      <div class="users-list">
        <div *ngIf="loading" class="loading">Loading users...</div>
        <div *ngIf="error" class="error-message">{{ error }}</div>

        <table *ngIf="!loading && !error && filteredUsers.length > 0">
          <thead>
            <tr>
              <th>ID</th>
              <th>Username</th>
              <th>Email</th>
              <th>Role</th>
              <th>Created</th>
              <th>Last Login</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let user of filteredUsers">
              <td>{{ user.id }}</td>
              <td>{{ user.username }}</td>
              <td>{{ user.email }}</td>
              <td>
                <span class="role-badge" [ngClass]="'role-' + user.rolename.toLowerCase()">
                  {{ user.rolename }}
                </span>
              </td>
              <td>{{ user.created | date:'medium' }}</td>
              <td>{{ user.lastLogin ? (user.lastLogin | date:'medium') : 'Never' }}</td>
              <td>
                <span class="status-badge" [ngClass]="user.isActive ? 'status-active' : 'status-inactive'">
                  {{ user.isActive ? 'Active' : 'Inactive' }}
                </span>
              </td>
            </tr>
          </tbody>
        </table>

        <div *ngIf="!loading && !error && filteredUsers.length === 0" class="no-data">
          No users found matching your criteria.
        </div>
      </div>
    </div>
  `,
  styles: [`
    .users-manager {
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

    .role-filter {
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

    .role-badge {
      display: inline-block;
      padding: 0.25rem 0.5rem;
      border-radius: 20px;
      font-size: 0.75rem;
      font-weight: 600;
      text-transform: uppercase;
    }

    .role-admin {
      background-color: #673ab7;
      color: white;
    }

    .role-user {
      background-color: #2196f3;
      color: white;
    }

    .role-restaurant {
      background-color: #ff9800;
      color: white;
    }

    .status-badge {
      display: inline-block;
      padding: 0.25rem 0.5rem;
      border-radius: 20px;
      font-size: 0.75rem;
      font-weight: 600;
    }

    .status-active {
      background-color: #4caf50;
      color: white;
    }

    .status-inactive {
      background-color: #f44336;
      color: white;
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
  `]
})
export class UsersComponent implements OnInit {
  users: User[] = [];
  filteredUsers: User[] = [];
  loading: boolean = false;
  error: string | null = null;
  searchTerm: string = '';
  roleFilter: string = '';

  constructor(private cmsService: CmsService) {}

  ngOnInit() {
    this.loadUsers();
  }

  loadUsers() {
    this.loading = true;
    this.error = null;

    this.cmsService.getAllUsers().subscribe({
      next: (response) => {
        if (response.success) {
          this.users = response.data;
          this.filteredUsers = [...this.users];
        } else {
          this.error = response.errorMessage || 'Failed to load users';
        }
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Error loading users. Please try again.';
        this.loading = false;
        console.error('Error loading users:', err);
      }
    });
  }

  filterUsers() {
    let filtered = [...this.users];

    // Apply search term filter
    if (this.searchTerm.trim()) {
      const term = this.searchTerm.toLowerCase().trim();
      filtered = filtered.filter(user =>
        user.username.toLowerCase().includes(term) ||
        user.email.toLowerCase().includes(term)
      );
    }

    // Apply role filter
    if (this.roleFilter) {
      filtered = filtered.filter(user =>
        user.rolename.toLowerCase() === this.roleFilter.toLowerCase()
      );
    }

    this.filteredUsers = filtered;
  }
}
