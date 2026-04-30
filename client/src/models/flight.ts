export interface Flight {
  id: number;
  departureTime: Date;
  arrivalTime: Date;
  price: number;
  availableSeats: number;
  duration: number;
  status: string;
  departureAirportName: string;
  arrivalAirportName: string;
  aircraftType: string;
}

export interface AdminFlight extends Flight {
  aircraftId: number;
  departureAirportId: number;
  arrivalAirportId: number;
}

export interface FlightEditorRequest {
  departureTime: string;
  arrivalTime: string;
  price: number;
  availableSeats: number;
  status: string;
  aircraftId: number;
  departureAirportId: number;
  arrivalAirportId: number;
}

export interface DeleteFlightResult {
  succeeded: boolean;
  flightId: number;
  message: string;
  errors: string[];
}

export interface Airport {
  id: number;
  name: string;
  city: string;
  country: string;
  iataCode: string;
}

export interface SearchFlightRequest {
  departureAirportId: number;
  arrivalAirportId: number;
  departureTime: Date;
  numberOfPassengers: number;
  sortBy?: string;
}

export interface AircraftOption {
  id: number;
  model: string;
  registrationNumber?: string;
  totalSeats: number;
  seatRows: number;
  seatsPerRow: number;
}
