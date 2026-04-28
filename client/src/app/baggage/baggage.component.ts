import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { BaggageService } from '../services/baggage.service';
import { FlightService } from '../services/flight.service';
import { FlightStepperComponent } from '../flights/stepper/flight-stepper.component';
import { BaggageCardComponent } from './baggage-card/baggage-card.component';
import { BaggageTypeDto } from '../../models/baggage';

@Component({
  selector: 'app-baggage',
  standalone: true,
  imports: [CommonModule, FlightStepperComponent, BaggageCardComponent],
  templateUrl: './baggage.component.html',
  styleUrl: './baggage.component.css'
})
export class BaggageComponent {
  baggageService = inject(BaggageService);
  flightService = inject(FlightService);
  router = inject(Router);

  currentStep = signal<string>('baggage');
  numberOfPassengers = signal<number>(1);

  // Computed signals for categorized baggage
  cabinBaggages = computed(() => {
    return this.baggageService.baggageOptions().filter(b =>
      b.type.toLowerCase().includes('cabin')
    );
  });

  checkedBaggages = computed(() => {
    return this.baggageService.baggageOptions().filter(b =>
      b.type.toLowerCase().includes('checked')
    );
  });

  totalPrice = computed(() => {
    return this.baggageService.getTotalBaggagePrice();
  });

  constructor() {
    // Initialize passengers when component loads
    effect(() => {
      const passengers = this.flightService.getNumberOfPassengers();
      this.numberOfPassengers.set(passengers);
      this.baggageService.initializePassengers(passengers);
    });
  }

  onCabinBaggageSelect(baggage: BaggageTypeDto) {
    const currentIndex = this.baggageService.currentPassengerIndex();
    const isSelected = this.baggageService.isBaggageSelectedForPassenger(currentIndex, baggage.id, 'cabin');
    
    if (isSelected) {
      this.baggageService.deselectBaggageForPassenger(currentIndex, 'cabin');
    } else {
      this.baggageService.selectBaggageForPassenger(currentIndex, 'cabin', baggage);
    }
  }

  onCheckedBaggageSelect(baggage: BaggageTypeDto) {
    const currentIndex = this.baggageService.currentPassengerIndex();
    const isSelected = this.baggageService.isBaggageSelectedForPassenger(currentIndex, baggage.id, 'checked');
    
    if (isSelected) {
      this.baggageService.deselectBaggageForPassenger(currentIndex, 'checked');
    } else {
      this.baggageService.selectBaggageForPassenger(currentIndex, 'checked', baggage);
    }
  }

  isCabinBaggageSelected(baggageId: number): boolean {
    const currentIndex = this.baggageService.currentPassengerIndex();
    return this.baggageService.isBaggageSelectedForPassenger(currentIndex, baggageId, 'cabin');
  }

  isCheckedBaggageSelected(baggageId: number): boolean {
    const currentIndex = this.baggageService.currentPassengerIndex();
    return this.baggageService.isBaggageSelectedForPassenger(currentIndex, baggageId, 'checked');
  }

  selectPassenger(index: number): void {
    this.baggageService.setCurrentPassenger(index);
  }

  getCurrentPassenger(): number {
    return this.baggageService.currentPassengerIndex();
  }

  getSelectedFlight() {
    return this.flightService.getSelectedFlight();
  }

  continue(): void {
    this.router.navigate(['/seats']);
  }
}
