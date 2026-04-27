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
    this.http.get<Airport[]>(this.baseUrl + 'airport').subscribe({
      next: (data) => {
        this.airports.set(data);
      },
      error: (err) => {
        console.error('Failed to load airports:', err);
        // Fallback to empty array if API fails
        this.airports.set([]);
      }
    });
  }
}
