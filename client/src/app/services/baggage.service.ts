import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { BaggageTypeDto, SelectedBaggage } from '../../models/baggage';

@Injectable({
  providedIn: 'root'
})
export class BaggageService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;

  // Signals
  baggageOptions = signal<BaggageTypeDto[]>([]);
  selectedCabinBaggage = signal<SelectedBaggage | null>(null);
  selectedCheckedBaggage = signal<SelectedBaggage | null>(null);
  isLoading = signal(false);
  error = signal<string | null>(null);

  constructor() {
    this.loadBaggageOptions();
  }

  loadBaggageOptions() {
    this.isLoading.set(true);
    this.error.set(null);

    return this.http.get<BaggageTypeDto[]>(this.baseUrl + 'baggage/options').subscribe({
      next: (data) => {
        this.baggageOptions.set(data);
        // Auto-select cabin baggage (first one that contains "Cabin")
        const cabinBaggage = data.find(b => b.type.toLowerCase().includes('cabin'));
        if (cabinBaggage) {
          this.selectBaggage('cabin', cabinBaggage);
        }
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set('Failed to load baggage options');
        this.isLoading.set(false);
        console.error(err);
      }
    });
  }

  selectBaggage(category: 'cabin' | 'checked', baggage: BaggageTypeDto) {
    const selected: SelectedBaggage = {
      baggageTypeId: baggage.id,
      type: baggage.type,
      price: baggage.price
    };

    if (category === 'cabin') {
      this.selectedCabinBaggage.set(selected);
    } else {
      this.selectedCheckedBaggage.set(selected);
    }
  }

  deselectBaggage(category: 'cabin' | 'checked') {
    if (category === 'cabin') {
      this.selectedCabinBaggage.set(null);
    } else {
      this.selectedCheckedBaggage.set(null);
    }
  }

  getSelectedBaggages() {
    return {
      cabin: this.selectedCabinBaggage(),
      checked: this.selectedCheckedBaggage()
    };
  }

  getTotalBaggagePrice(): number {
    let total = 0;
    if (this.selectedCabinBaggage()) {
      total += this.selectedCabinBaggage()!.price;
    }
    if (this.selectedCheckedBaggage()) {
      total += this.selectedCheckedBaggage()!.price;
    }
    return total;
  }

  reset() {
    this.selectedCabinBaggage.set(null);
    this.selectedCheckedBaggage.set(null);
  }
}
