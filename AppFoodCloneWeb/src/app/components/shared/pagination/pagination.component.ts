import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PagedResult } from '../../../types/pagination.interface';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pagination.component.html',
  styleUrls: ['./pagination.component.css']
})
export class PaginationComponent {
  @Input() pagedResult: PagedResult<any> | null = null;
  @Output() pageChanged = new EventEmitter<number>();
  
  // Expose Math to template
  Math = Math;

  get pages(): number[] {
    if (!this.pagedResult) return [];
    
    const totalPages = this.pagedResult.totalPages;
    const currentPage = this.pagedResult.pageNumber;
    const pages: number[] = [];
    
    // Show up to 10 pages around current page
    const startPage = Math.max(1, currentPage - 5);
    const endPage = Math.min(totalPages, currentPage + 4);
    
    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }
    
    return pages;
  }

  onPageClick(page: number): void {
    if (page !== this.pagedResult?.pageNumber) {
      this.pageChanged.emit(page);
    }
  }

  onPreviousClick(): void {
    if (this.pagedResult?.hasPreviousPage) {
      this.pageChanged.emit(this.pagedResult.pageNumber - 1);
    }
  }

  onNextClick(): void {
    if (this.pagedResult?.hasNextPage) {
      this.pageChanged.emit(this.pagedResult.pageNumber + 1);
    }
  }
} 