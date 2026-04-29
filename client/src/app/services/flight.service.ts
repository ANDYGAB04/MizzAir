import { inject, Injectable, signal } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  AdminFlight,
  AircraftOption,
  DeleteFlightResult,
  Flight,
  FlightEditorRequest,
  SearchFlightRequest,
  Airport
} from '../../models/flight';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class FlightService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);
  baseUrl = environment.apiUrl;

  // Signals
  flights = signal<Flight[]>([]);
  selectedFlight = signal<Flight | null>(null);
  numberOfPassengers = signal<number>(1);
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

  getAdminFlights(): Observable<AdminFlight[]> {
    return this.http.get<AdminFlight[]>(this.baseUrl + 'flight', {
      headers: this.getAuthHeaders(),
    });
  }

  createFlight(payload: FlightEditorRequest): Observable<AdminFlight> {
    return this.http.post<AdminFlight>(this.baseUrl + 'flight', payload, {
      headers: this.getAuthHeaders(),
    });
  }

  updateFlight(flightId: number, payload: FlightEditorRequest): Observable<AdminFlight> {
    return this.http.put<AdminFlight>(this.baseUrl + `flight/${flightId}`, payload, {
      headers: this.getAuthHeaders(),
    });
  }

  deleteFlight(flightId: number): Observable<DeleteFlightResult> {
    return this.http.delete<DeleteFlightResult>(this.baseUrl + `flight/${flightId}`, {
      headers: this.getAuthHeaders(),
    });
  }

  getAirports(): Observable<Airport[]> {
    return this.http.get<Airport[]>(this.baseUrl + 'airport');
  }

  getAircraft(): Observable<AircraftOption[]> {
    return this.http.get<AircraftOption[]>(this.baseUrl + 'aircraft', {
      headers: this.getAuthHeaders(),
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

  setSelectedFlight(flight: Flight): void {
    this.selectedFlight.set(flight);
  }

  getSelectedFlight(): Flight | null {
    return this.selectedFlight();
  }

  setNumberOfPassengers(count: number): void {
    this.numberOfPassengers.set(count);
  }

  getNumberOfPassengers(): number {
    return this.numberOfPassengers();
  }

  private getAuthHeaders(): HttpHeaders {
    const token = this.authService.currentUser()?.token;

    return new HttpHeaders({
      Authorization: `Bearer ${token ?? ''}`
    });
  }
}
