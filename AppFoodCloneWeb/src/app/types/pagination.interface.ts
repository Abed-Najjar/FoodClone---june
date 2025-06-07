export interface PaginationParams {
  pageNumber: number;
  pageSize: number;
}

export interface PagedResult<T> {
  data: T[];
  totalItems: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface ApiResponse<T> {
  success: boolean;
  data: T;
  errorMessage?: string;
  statusCode: number;
} 