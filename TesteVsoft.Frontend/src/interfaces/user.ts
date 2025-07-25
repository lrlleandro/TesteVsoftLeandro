export interface UserFormModel {
  id?: string;
  name?: string;
  email?: string;
  userName: string;
  oldPassword?: string;
  newPassword?: string;
}

export interface UserModel {
  id?: string;
  name: string;
  email: string;
  userName: string;
  password?: string;
}

export interface AssignedUserModel {
  id: string;
  name: string;
}