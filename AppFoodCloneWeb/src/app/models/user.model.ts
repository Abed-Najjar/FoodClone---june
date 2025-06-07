export interface User {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  token: string;
  rolename: string;
  address: string[];
  phoneNumber?: string;
  dateOfBirth?: string;
  gender?: string;
  bio?: string;
  profileImageUrl?: string;
  created?: Date;
  lastLogin?: Date;
  isActive?: boolean;
}

export interface UserLogin {
  email: string;
  password: string;
}

export interface UserRegister {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  address: string[];
}

export interface UserProfileUpdate {
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  dateOfBirth?: string;
  gender?: string;
  bio?: string;
  address?: string;
}

export interface PasswordChangeRequest {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}
