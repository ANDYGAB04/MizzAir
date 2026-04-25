import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FlightService } from '../../services/flight.service';
import { SearchFlightRequest } from '../../../models/flight';

@Component({
  selector: 'app-flight-search',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './flight-search.component.html',
  styleUrl: './flight-search.component.css'
})
export class FlightSearchComponent {
  flightService = inject(FlightService);

  // Form state signals
  originAirportId = signal<number | string>('');
  destinationAirportId = signal<number | string>('');
  departureDate = signal('');
  numberOfPassengers = signal(1);
  sortBy = signal('');

  // Computed signal for form validation
  isFormValid = computed(() => {
    return (
      this.originAirportId() !== '' &&
      this.destinationAirportId() !== '' &&
      this.departureDate() !== '' &&
      this.numberOfPassengers() > 0
    );
  });

  search(): void {
    if (!this.isFormValid()) {
      return;
    }

    const searchRequest: SearchFlightRequest = {
      departureAirportId: Number(this.originAirportId()),
      arrivalAirportId: Number(this.destinationAirportId()),
      departureTime: new Date(this.departureDate()),
      numberOfPassengers: this.numberOfPassengers(),
      sortBy: this.sortBy() || undefined
    };

    this.flightService.searchFlights(searchRequest);
  }
}
