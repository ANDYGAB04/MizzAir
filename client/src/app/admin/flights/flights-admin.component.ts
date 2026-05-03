import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { forkJoin } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

import { AuthService } from '../../services/auth.service';
import { FlightService } from '../../services/flight.service';
import { AdminFlight, AircraftOption, Airport, FlightEditorRequest } from '../../../models/flight';

const FLIGHT_STATUSES = ['Scheduled', 'Active', 'Delayed', 'Cancelled', 'Completed'];

@Component({
  selector: 'app-flights-admin',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './flights-admin.component.html',
  styleUrl: './flights-admin.component.css',
})
export class FlightsAdminComponent {
  private flightService = inject(FlightService);
  private authService = inject(AuthService);
  private toastr = inject(ToastrService);
  private router = inject(Router);
  private fb = inject(FormBuilder);

  flights = signal<AdminFlight[]>([]);
  airports = signal<Airport[]>([]);
  aircraft = signal<AircraftOption[]>([]);
  selectedFlight = signal<AdminFlight | null>(null);
  flightToDelete = signal<AdminFlight | null>(null);

  isLoading = signal(false);
  isSaving = signal(false);
  isDeleting = signal(false);
  isModalOpen = signal(false);
  error = signal<string | null>(null);

  readonly statuses = FLIGHT_STATUSES;
  flightCount = computed(() => this.flights().length);
  modalTitle = computed(() => this.selectedFlight() ? 'Edit Flight' : 'Add Flight');

  flightForm: FormGroup = this.fb.group({
    departureTime: ['', Validators.required],
    arrivalTime: ['', Validators.required],
    price: [0, [Validators.required, Validators.min(0.01)]],
    availableSeats: [0, [Validators.required, Validators.min(0)]],
    status: ['Scheduled', Validators.required],
    aircraftId: [null, Validators.required],
    departureAirportId: [null, Validators.required],
    arrivalAirportId: [null, Validators.required],
  });

  constructor() {
    if (!this.authService.currentUser()?.token) {
      this.toastr.error('Please login to manage flights.');
      this.router.navigate(['/login']);
      return;
    }

    if (!this.authService.hasAdmin()) {
      this.toastr.error('Admin access is required.');
      this.router.navigate(['/']);
      return;
    }

    this.loadData();
  }

  loadData(): void {
    this.isLoading.set(true);
    this.error.set(null);

    forkJoin({
      flights: this.flightService.getAdminFlights(),
      airports: this.flightService.getAirports(),
      aircraft: this.flightService.getAircraft(),
    }).subscribe({
      next: ({ flights, airports, aircraft }) => {
        this.flights.set(flights);
        this.airports.set(airports);
        this.aircraft.set(aircraft);
        this.isLoading.set(false);
      },
      error: () => {
        this.error.set('Failed to load flight management data.');
        this.isLoading.set(false);
      },
    });
  }

  openAddModal(): void {
    this.selectedFlight.set(null);
    this.flightForm.reset({
      departureTime: '',
      arrivalTime: '',
      price: 0,
      availableSeats: 0,
      status: 'Scheduled',
      aircraftId: null,
      departureAirportId: null,
      arrivalAirportId: null,
    });
    this.isModalOpen.set(true);
  }

  openEditModal(flight: AdminFlight): void {
    this.selectedFlight.set(flight);
    this.flightForm.reset({
      departureTime: this.toDateTimeLocal(flight.departureTime),
      arrivalTime: this.toDateTimeLocal(flight.arrivalTime),
      price: flight.price,
      availableSeats: flight.availableSeats,
      status: flight.status,
      aircraftId: flight.aircraftId,
      departureAirportId: flight.departureAirportId,
      arrivalAirportId: flight.arrivalAirportId,
    });
    this.isModalOpen.set(true);
  }

  closeModal(): void {
    if (this.isSaving()) return;
    this.isModalOpen.set(false);
  }

  saveFlight(): void {
    if (this.flightForm.invalid) {
      this.flightForm.markAllAsTouched();
      return;
    }

    const payload = this.buildPayload();
    const selected = this.selectedFlight();
    this.isSaving.set(true);

    const request = selected
      ? this.flightService.updateFlight(selected.id, payload)
      : this.flightService.createFlight(payload);

    request.subscribe({
      next: (savedFlight) => {
        this.flights.update((items) => {
          const withoutSaved = items.filter((item) => item.id !== savedFlight.id);
          return [...withoutSaved, savedFlight].sort(this.sortByDeparture);
        });
        this.isSaving.set(false);
        this.isModalOpen.set(false);
        this.toastr.success(selected ? 'Flight updated.' : 'Flight created.');
      },
      error: (err) => {
        this.isSaving.set(false);
        this.toastr.error(this.extractError(err, 'Failed to save flight.'));
      },
    });
  }

  requestDelete(flight: AdminFlight): void {
    this.flightToDelete.set(flight);
  }

  cancelDelete(): void {
    if (this.isDeleting()) return;
    this.flightToDelete.set(null);
  }

  deleteFlight(): void {
    const flight = this.flightToDelete();
    if (!flight) return;

    this.isDeleting.set(true);
    this.flightService.deleteFlight(flight.id).subscribe({
      next: () => {
        this.flights.update((items) => items.filter((item) => item.id !== flight.id));
        this.isDeleting.set(false);
        this.flightToDelete.set(null);
        this.toastr.success('Flight deleted.');
      },
      error: (err) => {
        this.isDeleting.set(false);
        this.toastr.error(this.extractError(err, 'Failed to delete flight.'));
      },
    });
  }

  routeLabel(flight: AdminFlight): string {
    return `${flight.departureAirportName} -> ${flight.arrivalAirportName}`;
  }

  aircraftLabel(aircraft: AircraftOption): string {
    return aircraft.registrationNumber
      ? `${aircraft.model} (${aircraft.registrationNumber})`
      : aircraft.model;
  }

  private buildPayload(): FlightEditorRequest {
    const value = this.flightForm.getRawValue();

    return {
      departureTime: value.departureTime,
      arrivalTime: value.arrivalTime,
      price: Number(value.price),
      availableSeats: Number(value.availableSeats),
      status: value.status,
      aircraftId: Number(value.aircraftId),
      departureAirportId: Number(value.departureAirportId),
      arrivalAirportId: Number(value.arrivalAirportId),
    };
  }

  private toDateTimeLocal(value: Date | string): string {
    const date = new Date(value);
    const offsetDate = new Date(date.getTime() - date.getTimezoneOffset() * 60000);
    return offsetDate.toISOString().slice(0, 16);
  }

  private sortByDeparture(a: AdminFlight, b: AdminFlight): number {
    return new Date(a.departureTime).getTime() - new Date(b.departureTime).getTime();
  }

  private extractError(err: unknown, fallback: string): string {
    const error = (err as { error?: unknown })?.error;
    if (typeof error === 'string') return error;
    if (Array.isArray(error)) return error.join(' ');
    return fallback;
  }
}
