import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule, DatePipe, DecimalPipe } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { BookingService } from '../services/booking.service';
import { AuthService } from '../services/auth.service';
import { Booking } from '../../models/booking';

@Component({
  selector: 'app-bookings-list',
  standalone: true,
  imports: [CommonModule, RouterLink, DatePipe, DecimalPipe],
  templateUrl: './bookings-list.component.html',
  styleUrl: './bookings-list.component.css'
})
export class BookingsListComponent {
  private bookingService = inject(BookingService);
  private authService = inject(AuthService);
  private toastr = inject(ToastrService);
  private router = inject(Router);

  bookings = signal<Booking[]>([]);
  isLoading = signal(false);
  deletingBookingId = signal<number | null>(null);
  error = signal<string | null>(null);

  hasBookings = computed(() => this.bookings().length > 0);

  constructor() {
    if (!this.authService.currentUser()?.token) {
      this.toastr.error('Please login to view your bookings.');
      this.router.navigate(['/login']);
      return;
    }

    this.loadBookings();
  }

  loadBookings(): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.bookingService.getBookings().subscribe({
      next: (bookings) => {
        this.bookings.set(bookings);
        this.isLoading.set(false);
      },
      error: () => {
        this.error.set('Failed to load your bookings. Please try again.');
        this.isLoading.set(false);
      }
    });
  }

  deleteBooking(bookingId: number): void {
    if (this.deletingBookingId() !== null) {
      return;
    }

    this.deletingBookingId.set(bookingId);

    this.bookingService.deleteBooking(bookingId).subscribe({
      next: () => {
        this.bookings.update((bookings) => bookings.filter((booking) => booking.id !== bookingId));
        this.toastr.success('Booking deleted successfully.');
      },
      error: () => {
        this.deletingBookingId.set(null);
        this.toastr.error('Failed to delete booking.');
      },
      complete: () => {
        this.deletingBookingId.set(null);
      }
    });
  }

  getBaggageLabel(booking: Booking): string {
    if (booking.baggageTypes.length === 0) {
      return 'No baggage';
    }

    const baggageCounts = new Map<string, number>();

    booking.baggageTypes.forEach((baggageType) => {
      baggageCounts.set(baggageType, (baggageCounts.get(baggageType) ?? 0) + 1);
    });

    return Array.from(baggageCounts.entries())
      .map(([baggageType, count]) => count > 1 ? `${baggageType} x${count}` : baggageType)
      .join(', ');
  }

  getSeatLabel(booking: Booking): string {
    return booking.seatNumbers.length > 0 ? booking.seatNumbers.join(', ') : 'No seat selected';
  }
}
