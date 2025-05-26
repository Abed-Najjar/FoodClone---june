export interface AppResponse<T> {
  data: T;
  errorMessage: string;
  statusCode: number;
  success: boolean;
}
