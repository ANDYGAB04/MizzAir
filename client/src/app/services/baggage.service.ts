import { inject, Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { BaggageTypeDto, SelectedBaggage, PassengerBaggage } from '../../models/baggage';

@Injectable({
  providedIn: 'root'
})
export class BaggageService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;

  // Signals
  baggageOptions = signal<BaggageTypeDto[]>([]);
  passengerBaggages = signal<PassengerBaggage[]>([]);
  currentPassengerIndex = signal<number>(0);
  isLoading = signal(false);
  error = signal<string | null>(null);

  // Computed signal for current passenger baggages
  currentPassengerBaggage = computed(() => {
    return this.passengerBaggages()[this.currentPassengerIndex()] || null;
  });

  constructor() {
    this.loadBaggageOptions();
  }

  loadBaggageOptions() {
    this.isLoading.set(true);
    this.error.set(null);

    return this.http.get<BaggageTypeDto[]>(this.baseUrl + 'baggage/options').subscribe({
      next: (data) => {
        this.baggageOptions.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set('Failed to load baggage options');
        this.isLoading.set(false);
        console.error(err);
      }
    });
  }

  initializePassengers(numberOfPassengers: number): void {
    const passengers: PassengerBaggage[] = [];
    
    // Find default cabin baggage
    const defaultCabinBaggage = this.baggageOptions().find(b => 
      b.type.toLowerCase().includes('cabin')
    );
    
    for (let i = 0; i < numberOfPassengers; i++) {
      passengers.push({
        passengerIndex: i,
        cabinBaggage: defaultCabinBaggage ? {
          baggageTypeId: defaultCabinBaggage.id,
          type: defaultCabinBaggage.type,
          price: defaultCabinBaggage.price
        } : null,
        checkedBaggage: null
      });
    }
    
    this.passengerBaggages.set(passengers);
    this.currentPassengerIndex.set(0);
  }

  selectBaggageForPassenger(passengerIndex: number, category: 'cabin' | 'checked', baggage: BaggageTypeDto): void {
    const passengers = this.passengerBaggages();
    const passenger = passengers[passengerIndex];
    
    if (!passenger) return;
    
    const selected: SelectedBaggage = {
      baggageTypeId: baggage.id,
      type: baggage.type,
      price: baggage.price
    };
    
    if (category === 'cabin') {
      passenger.cabinBaggage = selected;
    } else {
      passenger.checkedBaggage = selected;
    }
    
    this.passengerBaggages.set([...passengers]);
  }

  deselectBaggageForPassenger(passengerIndex: number, category: 'cabin' | 'checked'): void {
    const passengers = this.passengerBaggages();
    const passenger = passengers[passengerIndex];
    
    if (!passenger) return;
    
    if (category === 'cabin') {
      passenger.cabinBaggage = null;
    } else {
      passenger.checkedBaggage = null;
    }
    
    this.passengerBaggages.set([...passengers]);
  }

  setCurrentPassenger(index: number): void {
    if (index >= 0 && index < this.passengerBaggages().length) {
      this.currentPassengerIndex.set(index);
    }
  }

  isBaggageSelectedForPassenger(passengerIndex: number, baggageId: number, category: 'cabin' | 'checked'): boolean {
    const passenger = this.passengerBaggages()[passengerIndex];
    if (!passenger) return false;
    
    if (category === 'cabin') {
      return passenger.cabinBaggage?.baggageTypeId === baggageId;
    } else {
      return passenger.checkedBaggage?.baggageTypeId === baggageId;
    }
  }

  getAllPassengerBaggages(): PassengerBaggage[] {
    return this.passengerBaggages();
  }

  getTotalBaggagePrice(): number {
    let total = 0;
    this.passengerBaggages().forEach(passenger => {
      if (passenger.cabinBaggage) {
        total += passenger.cabinBaggage.price;
      }
      if (passenger.checkedBaggage) {
        total += passenger.checkedBaggage.price;
      }
    });
    return total;
  }

  reset(): void {
    this.passengerBaggages.set([]);
    this.currentPassengerIndex.set(0);
  }
}
