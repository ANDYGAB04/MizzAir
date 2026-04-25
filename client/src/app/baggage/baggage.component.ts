import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { BaggageService } from '../services/baggage.service';
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
  router = inject(Router);

  currentStep = signal<string>('baggage');

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

  onCabinBaggageSelect(baggage: BaggageTypeDto) {
    if (this.baggageService.selectedCabinBaggage()?.baggageTypeId === baggage.id) {
      this.baggageService.deselectBaggage('cabin');
    } else {
      this.baggageService.selectBaggage('cabin', baggage);
    }
  }

  onCheckedBaggageSelect(baggage: BaggageTypeDto) {
    if (this.baggageService.selectedCheckedBaggage()?.baggageTypeId === baggage.id) {
      this.baggageService.deselectBaggage('checked');
    } else {
      this.baggageService.selectBaggage('checked', baggage);
    }
  }

  isCabinBaggageSelected(baggageId: number): boolean {
    return this.baggageService.selectedCabinBaggage()?.baggageTypeId === baggageId;
  }

  isCheckedBaggageSelected(baggageId: number): boolean {
    return this.baggageService.selectedCheckedBaggage()?.baggageTypeId === baggageId;
  }

  continue(): void {
    this.router.navigate(['/seats']);
  }
}
