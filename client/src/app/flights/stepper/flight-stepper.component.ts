import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface Step {
  id: string;
  label: string;
}

@Component({
  selector: 'app-flight-stepper',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './flight-stepper.component.html',
  styleUrl: './flight-stepper.component.css'
})
export class FlightStepperComponent {
  steps = input<Step[]>([
    { id: 'flights', label: 'Flights' },
    { id: 'baggage', label: 'Baggage' },
    { id: 'seat', label: 'Seat' }
  ]);

  activeStep = input<string>('flights');

  isStepActive(stepId: string): boolean {
    return this.activeStep() === stepId;
  }
}
