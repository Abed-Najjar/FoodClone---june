import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CmsService } from '../../../services/cms.service';
import { User } from '../../../models/user.model';
import { PagedResult, PaginationParams } from '../../../types/pagination.interface';
import { PaginationComponent } from '../../shared/pagination/pagination.component';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, FormsModule, PaginationComponent],
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit {
  users: User[] = [];
  filteredUsers: User[] = [];
  loading: boolean = false;
  error: string | null = null;
  searchTerm: string = '';
  roleFilter: string = '';

  // Pagination properties
  usersPagedResult: PagedResult<User> | null = null;
  usersPagination: PaginationParams = { pageNumber: 1, pageSize: 4 };

  constructor(private cmsService: CmsService) {}

  ngOnInit() {
    this.loadUsers();
  }

  loadUsers() {
    this.loading = true;
    this.error = null;

    this.cmsService.getAllUsers(this.usersPagination).subscribe({
      next: (response) => {
        if (response.success) {
          this.usersPagedResult = response.data as PagedResult<User>;
          this.users = this.usersPagedResult.data;
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
        user.id.toString().includes(term) ||
        (user.firstName && user.firstName.toLowerCase().includes(term)) ||
        (user.lastName && user.lastName.toLowerCase().includes(term)) ||
        (user.email && user.email.toLowerCase().includes(term)) ||
        (user.rolename && user.rolename.toLowerCase().includes(term))
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

  onUsersPageChanged(page: number): void {
    this.usersPagination.pageNumber = page;
    this.loadUsers();
  }
}
