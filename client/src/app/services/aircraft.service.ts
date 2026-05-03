import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
    AircraftOption,
    CreateAircraftDto,
    UpdateAircraftDto,
    AircraftOperationResult
} from '../../models/aircraft';

@Injectable({
    providedIn: 'root'
})
export class AircraftService {
    private http = inject(HttpClient);
    baseUrl = environment.apiUrl;

    getAircraft(): Observable<AircraftOption[]> {
        return this.http.get<AircraftOption[]>(this.baseUrl + 'aircraft');
    }

    getAircraftById(id: number): Observable<AircraftOption> {
        return this.http.get<AircraftOption>(this.baseUrl + `aircraft/${id}`);
    }

    createAircraft(payload: CreateAircraftDto): Observable<AircraftOperationResult> {
        return this.http.post<AircraftOperationResult>(this.baseUrl + 'aircraft', payload);
    }

    updateAircraft(id: number, payload: UpdateAircraftDto): Observable<AircraftOperationResult> {
        return this.http.put<AircraftOperationResult>(this.baseUrl + `aircraft/${id}`, payload);
    }

    deleteAircraft(id: number): Observable<any> {
        return this.http.delete<any>(this.baseUrl + `aircraft/${id}`);
    }
}
