export interface BaggageTypeDto {
  id: number;
  type: string;
  maxDimensions: string;
  maxWeight: number;
  price: number;
}

export interface SelectedBaggage {
  baggageTypeId: number;
  type: string;
  price: number;
}

export interface PassengerBaggage {
  passengerIndex: number;
  cabinBaggage: SelectedBaggage | null;
  checkedBaggage: SelectedBaggage | null;
}
