export interface User {
  id: number;
  username: string;
  email: string;
  token: string;
  rolename: string;
  address: string[];
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
