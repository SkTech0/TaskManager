export interface Task {
  id: string;
  title: string;
  description: string;
  dueDate: string | null;
  status: 'Pending' | 'InProgress' | 'Completed';
  remarks: string | null;
  userId: string;
  createdAt: string;
  updatedAt: string;
}

export interface TaskCreateRequest {
  title: string;
  description: string;
  dueDate: string | null;
  status: 'Pending' | 'InProgress' | 'Completed';
  remarks: string;
}

export interface TaskUpdateRequest {
  title?: string;
  description?: string;
  dueDate?: string | null;
  status?: 'Pending' | 'InProgress' | 'Completed';
  remarks?: string;
}

export interface TaskSearchResult {
  tasks: Task[];
  totalCount: number;
}
