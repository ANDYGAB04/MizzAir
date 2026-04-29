export interface Passenger {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string | null;
  address: string;
  totalBookings: number;
  lastBookingDate: string | null;
}

export interface PassengerFilterRequest {
  searchTerm?: string;
  flightId?: number;
  sortBy?: string;
  isDescending?: boolean;
  pageNumber?: number;
  pageSize?: number;
}

