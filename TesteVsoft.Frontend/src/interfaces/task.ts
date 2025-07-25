import { AssignedUserModel } from './user';

export interface TaskModel {
  id?: string;
  title: string;
  description: string;
  dueDate: string;
  status: 'Pending' | 'InProgress' | 'Completed';
  assignedUser?: AssignedUserModel | null;
}