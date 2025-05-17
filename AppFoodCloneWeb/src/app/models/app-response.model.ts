export interface AppResponse<T> {
  data: T;
  message: string;
  statusCode: number;
  success: boolean;
}
