import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { Booking } from '../../models/booking';
import { Passenger, PassengerFilterRequest } from '../../models/passenger';
import { PaginatedResult } from '../../models/paginated-result';

@Injectable({
  providedIn: 'root',
})
export class PassengerService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

  getPassengers(filter: PassengerFilterRequest): Observable<PaginatedResult<Passenger>> {
    const params: Record<string, any> = {
      ...(filter.searchTerm !== undefined && { searchTerm: filter.searchTerm }),
      ...(filter.flightId !== undefined && { flightId: filter.flightId }),
      ...(filter.sortBy !== undefined && { sortBy: filter.sortBy }),
      ...(filter.isDescending !== undefined && { isDescending: filter.isDescending }),
      ...(filter.pageNumber !== undefined && { pageNumber: filter.pageNumber }),
      ...(filter.pageSize !== undefined && { pageSize: filter.pageSize }),
    };

    return this.http.get<PaginatedResult<Passenger>>(this.baseUrl + 'passenger', { params });
  }

  getPassengerById(passengerId: number): Observable<Passenger> {
    return this.http.get<Passenger>(this.baseUrl + `passenger/${passengerId}`);
  }

  getPassengerHistory(passengerId: number): Observable<Booking[]> {
    return this.http.get<Booking[]>(this.baseUrl + `passenger/${passengerId}/history`);
  }

  deletePassenger(passengerId: number): Observable<any> {
    return this.http.delete<any>(this.baseUrl + `admin/passengers/${passengerId}`);
  }
}

