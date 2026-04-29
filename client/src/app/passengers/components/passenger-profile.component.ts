import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule, DatePipe, DecimalPipe } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { forkJoin } from 'rxjs';

import { AuthService } from '../../services/auth.service';
import { PassengerService } from '../../services/passenger.service';
import { Passenger } from '../../../models/passenger';
import { Booking } from '../../../models/booking';

@Component({
  selector: 'app-passenger-profile',
  standalone: true,
  imports: [CommonModule, RouterLink, DatePipe, DecimalPipe],
  templateUrl: './passenger-profile.component.html',
  styleUrl: './passenger-profile.component.css',
})
export class PassengerProfileComponent {
  private authService = inject(AuthService);
  private passengerService = inject(PassengerService);
  private toastr = inject(ToastrService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  passenger = signal<Passenger | null>(null);
  history = signal<Booking[]>([]);

  isLoading = signal(false);
  error = signal<string | null>(null);

  passengerId: number | null = null;

  hasHistory = computed(() => this.history().length > 0);

  constructor() {
    if (!this.authService.currentUser()?.token) {
      this.toastr.error('Please login to view passenger details.');
      this.router.navigate(['/login']);
      return;
    }

    const idParam = this.route.snapshot.paramMap.get('id');
    const parsed = idParam ? Number(idParam) : NaN;
    if (!Number.isFinite(parsed) || parsed <= 0) {
      this.toastr.error('Invalid passenger id.');
      this.router.navigate(['/admin/passengers']);
      return;
    }

    this.passengerId = parsed;
    this.load();
  }

  private load(): void {
    if (this.passengerId == null) return;

    this.isLoading.set(true);
    this.error.set(null);

    forkJoin({
      passenger: this.passengerService.getPassengerById(this.passengerId),
      history: this.passengerService.getPassengerHistory(this.passengerId),
    }).subscribe({
      next: ({ passenger, history }) => {
        this.passenger.set(passenger);
        this.history.set(history);
        this.isLoading.set(false);
      },
      error: () => {
        this.error.set('Failed to load passenger details.');
        this.isLoading.set(false);
      },
    });
  }

  passengerFullName(p: Passenger): string {
    return `${p.firstName} ${p.lastName}`.trim();
  }

  getSeatLabel(booking: Booking): string {
    return booking.seatNumbers.length > 0 ? booking.seatNumbers.join(', ') : 'No seat selected';
  }

  getBaggageLabel(booking: Booking): string {
    if (booking.baggageTypes.length === 0) return 'No baggage';
    const baggageCounts = new Map<string, number>();
    booking.baggageTypes.forEach((baggageType) => {
      baggageCounts.set(baggageType, (baggageCounts.get(baggageType) ?? 0) + 1);
    });
    return Array.from(baggageCounts.entries())
      .map(([baggageType, count]) => (count > 1 ? `${baggageType} x${count}` : baggageType))
      .join(', ');
  }
}

