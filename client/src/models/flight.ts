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
