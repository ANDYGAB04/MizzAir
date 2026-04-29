export interface StaffAccount {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  city: string;
  country: string;
  address: string;
  roles: string[];
}

export interface CreateStaffAccountRequest {
  email: string;
  firstName: string;
  lastName: string;
  city: string;
  country: string;
  address: string;
  password: string;
}

export interface DeleteStaffAccountResult {
  succeeded: boolean;
  staffId: number;
  deletedSessionsCount: number;
  deletedAt: string;
  message: string;
  errors: string[];
}
