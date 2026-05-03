export interface AircraftOption {
    id: number;
    model: string;
    registrationNumber: string;
    totalSeats: number;
    seatRows: number;
    seatsPerRow: number;
}

export interface CreateAircraftDto {
    model: string;
    registrationNumber: string;
    totalSeats: number;
    seatRows: number;
    seatsPerRow: number;
}

export interface UpdateAircraftDto {
    model: string;
    registrationNumber: string;
    totalSeats: number;
    seatRows: number;
    seatsPerRow: number;
}

export interface AircraftOperationResult {
    succeeded: boolean;
    aircraft: AircraftOption | null;
    errors: string[];
}
