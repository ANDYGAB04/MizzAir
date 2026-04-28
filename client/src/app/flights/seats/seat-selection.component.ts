import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { SeatSelectionService } from '../../services/seat-selection.service';
import { FlightService } from '../../services/flight.service';
import { BaggageService } from '../../services/baggage.service';
import { FlightStepperComponent } from '../stepper/flight-stepper.component';

@Component({
  selector: 'app-seat-selection',
  standalone: true,
  imports: [CommonModule, FlightStepperComponent],
  templateUrl: './seat-selection.component.html',
  styleUrl: './seat-selection.component.css'
})
export class SeatSelectionComponent {
  seatSelectionService = inject(SeatSelectionService);
  flightService = inject(FlightService);
  baggageService = inject(BaggageService);
  router = inject(Router);

  currentStep = signal<string>('seat');
  numberOfPassengers = signal<number>(1);
  isLoading = signal(false);
  error = signal<string | null>(null);

  // Seat grid
  seatsGrid = signal<any[]>([]);
  bookedSeatIds = signal<number[]>([]);

  selectedSeats = computed(() => {
    return this.seatSelectionService.getSelectedSeats();
  });

  sortedSelectedSeats = computed(() => {
    const selected = this.selectedSeats();
    return [...selected].sort((a, b) => {
      if (a.seatRow !== b.seatRow) {
        return a.seatRow - b.seatRow;
      }
      return a.seatNumber.localeCompare(b.seatNumber);
    });
  });

  isFormValid = computed(() => {
    return this.selectedSeats().length === this.numberOfPassengers();
  });

  totalCost = computed(() => {
    const flight = this.flightService.getSelectedFlight();
    if (!flight) return 0;
    
    const flightCost = flight.price * this.numberOfPassengers();
    const baggageCost = this.baggageService.getTotalBaggagePrice();
    return flightCost + baggageCost;
  });

  constructor() {
    effect(() => {
      const passengers = this.flightService.getNumberOfPassengers();
      this.numberOfPassengers.set(passengers);
      
      const flight = this.flightService.getSelectedFlight();
      if (flight) {
        this.loadSeats(flight.id);
      }
    });
  }

  loadSeats(flightId: number): void {
    this.isLoading.set(true);
    this.error.set(null);

    // Load available seats
    this.seatSelectionService.loadSeatsByFlight(flightId).subscribe({
      next: (seats) => {
        this.organizeSeatsGrid(seats);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set('Failed to load seats');
        this.isLoading.set(false);
      }
    });

    // Load booked seats
    this.seatSelectionService.loadBookedSeats(flightId).subscribe({
      next: (bookedIds) => {
        this.bookedSeatIds.set(bookedIds);
      },
      error: (err) => {
        console.error('Failed to load booked seats:', err);
      }
    });
  }

  organizeSeatsGrid(seats: any[]): void {
    // Group seats by row
    const grid: any[] = [];
    const seatsByRow = new Map<number, any[]>();

    seats.forEach(seat => {
      if (!seatsByRow.has(seat.seatRow)) {
        seatsByRow.set(seat.seatRow, []);
      }
      seatsByRow.get(seat.seatRow)!.push(seat);
    });

    // Sort and add to grid
    Array.from(seatsByRow.entries())
      .sort((a, b) => a[0] - b[0])
      .forEach(([row, rowSeats]) => {
        rowSeats.sort((a, b) => a.seatNumber.localeCompare(b.seatNumber));
        grid.push({ row, seats: rowSeats });
      });

    this.seatsGrid.set(grid);
  }

  selectSeat(seat: any): void {
    const isBooked = this.bookedSeatIds().includes(seat.id);
    const isSelected = this.seatSelectionService.isSeatSelected(seat.id);

    // If booked, do nothing
    if (isBooked) {
      return;
    }

    // Toggle selection
    if (isSelected) {
      this.seatSelectionService.deselectSeat(seat.id);
    } else {
      this.seatSelectionService.selectSeat(seat);
    }
  }

  isSeatBooked(seatId: number): boolean {
    return this.bookedSeatIds().includes(seatId);
  }

  isSeatSelected(seatId: number): boolean {
    return this.seatSelectionService.isSeatSelected(seatId);
  }

  getSelectedFlight() {
    return this.flightService.getSelectedFlight();
  }

  book(): void {
    if (this.isFormValid()) {
      this.router.navigate(['/confirmation']);
    }
  }
}
