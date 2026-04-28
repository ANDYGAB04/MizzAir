import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

export interface SeatDto {
  id: number;
  seatNumber: string;
  seatRow: number;
  aircraftId: number;
}

export interface SelectedSeat extends SeatDto {
}

@Injectable({
  providedIn: 'root'
})
export class SeatSelectionService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;

  // Signals
  selectedSeats = signal<SelectedSeat[]>([]);

  loadSeatsByFlight(flightId: number) {
    return this.http.get<SeatDto[]>(this.baseUrl + `seat/flight/${flightId}`);
  }

  loadBookedSeats(flightId: number) {
    return this.http.get<number[]>(this.baseUrl + `seat/flight/${flightId}/booked`);
  }

  selectSeat(seat: SeatDto): void {
    const seats = this.selectedSeats();
    if (!seats.find(s => s.id === seat.id)) {
      this.selectedSeats.set([...seats, seat]);
    }
  }

  deselectSeat(seatId: number): void {
    const seats = this.selectedSeats();
    this.selectedSeats.set(seats.filter(s => s.id !== seatId));
  }

  isSeatSelected(seatId: number): boolean {
    return this.selectedSeats().some(s => s.id === seatId);
  }

  getSelectedSeats(): SelectedSeat[] {
    return this.selectedSeats();
  }

  getSelectedSeatIds(): number[] {
    return this.selectedSeats().map(s => s.id);
  }

  reset(): void {
    this.selectedSeats.set([]);
  }
}
