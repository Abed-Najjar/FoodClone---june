export interface User {
  id: number;
  username: string;
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
  username: string;
  email: string;
  password: string;
  address: string[];
}

export interface UserProfileUpdate {
  username: string;
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
