import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Flight, SearchFlightRequest, Airport } from '../../models/flight';

@Injectable({
  providedIn: 'root'
})
export class FlightService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;

  // Signals
  flights = signal<Flight[]>([]);
  airports = signal<Airport[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);

  constructor() {
    this.loadAirports();
  }

  searchFlights(searchRequest: SearchFlightRequest) {
    this.isLoading.set(true);
    this.error.set(null);

    const params = {
      departureAirportId: searchRequest.departureAirportId,
      arrivalAirportId: searchRequest.arrivalAirportId,
      departureTime: searchRequest.departureTime.toISOString(),
      numberOfPassengers: searchRequest.numberOfPassengers,
      ...(searchRequest.sortBy && { sortBy: searchRequest.sortBy })
    };

    return this.http.get<Flight[]>(this.baseUrl + 'flight/search', { params }).subscribe({
      next: (data) => {
        this.flights.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set('Failed to search flights. Please try again.');
        this.isLoading.set(false);
        console.error('Flight search error:', err);
      }
    });
  }

  private loadAirports() {
    // Mock airports data - replace with API call if endpoint exists
    this.airports.set([
      { id: 1, name: 'Bucharest Henri Coandă', city: 'Bucharest', code: 'OTP' },
      { id: 2, name: 'Iași International', city: 'Iași', code: 'IAS' },
      { id: 3, name: 'Constanța International', city: 'Constanța', code: 'CND' },
      { id: 4, name: 'Sibiu International', city: 'Sibiu', code: 'SBZ' },
      { id: 5, name: 'Timișoara Traian Vuia', city: 'Timișoara', code: 'TSR' },
      { id: 6, name: 'Cluj Napoca International', city: 'Cluj', code: 'CLJ' }
    ]);
  }
}
