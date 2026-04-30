import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Booking } from '../../models/booking';

@Injectable({
  providedIn: 'root'
})
export class BookingService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

  getBookings(): Observable<Booking[]> {
    return this.http.get<Booking[]>(this.baseUrl + 'booking');
  }

  deleteBooking(id: number): Observable<void> {
    return this.http.delete<void>(this.baseUrl + `booking/${id}`);
  }
}
