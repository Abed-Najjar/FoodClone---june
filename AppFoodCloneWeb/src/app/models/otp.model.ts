export enum OtpType {
  Registration = 1,
  ForgotPassword = 2,
  ResetPassword = 3,
  EmailVerification = 4,
  TwoFactorAuthentication = 5
}

export interface GenerateOtpRequest {
  email: string;
  type: OtpType;
  userId?: number;
}

export interface VerifyOtpRequest {
  email: string;
  code: string;
  type: OtpType;
}

export interface OtpResponse {
  email: string;
  type: OtpType;
  expiryDate: string;
  isSuccess: boolean;
  message: string;
}

export interface RegistrationWithOtpRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  address: string[];
  otpCode: string;
}

export interface PasswordResetWithOtpRequest {
  email: string;
  newPassword: string;
  otpCode: string;
}

export interface ResendOtpRequest {
  email: string;
  type: OtpType;
}
