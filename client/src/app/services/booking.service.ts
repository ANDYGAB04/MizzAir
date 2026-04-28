import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Booking } from '../../models/booking';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class BookingService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);
  private baseUrl = environment.apiUrl;

  getBookings(): Observable<Booking[]> {
    return this.http.get<Booking[]>(this.baseUrl + 'booking', {
      headers: this.getAuthHeaders()
    });
  }

  deleteBooking(id: number): Observable<void> {
    return this.http.delete<void>(this.baseUrl + `booking/${id}`, {
      headers: this.getAuthHeaders()
    });
  }

  private getAuthHeaders(): HttpHeaders {
    const token = this.authService.currentUser()?.token;

    return new HttpHeaders({
      Authorization: `Bearer ${token ?? ''}`
    });
  }
}
