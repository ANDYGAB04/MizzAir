export interface Booking {
  id: number;
  bookingReference: string;
  bookingDate: string;
  status: string;
  totalPrice: number;
  flightId: number;
  departureAirportName: string;
  arrivalAirportName: string;
  departureTime: string;
  arrivalTime: string;
  seatNumbers: string[];
  baggageTypes: string[];
}
