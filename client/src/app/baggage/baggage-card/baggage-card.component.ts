import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BaggageTypeDto } from '../../../models/baggage';

@Component({
  selector: 'app-baggage-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './baggage-card.component.html',
  styleUrl: './baggage-card.component.css'
})
export class BaggageCardComponent {
  baggage = input.required<BaggageTypeDto>();
  isSelected = input(false);
  category = input.required<'cabin' | 'checked'>();

  selected = output<void>();

  onSelect() {
    this.selected.emit();
  }
}
