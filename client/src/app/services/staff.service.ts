import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';
import {
  CreateStaffAccountRequest,
  DeleteStaffAccountResult,
  StaffAccount,
} from '../../models/staff';

@Injectable({
  providedIn: 'root',
})
export class StaffService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);
  private baseUrl = environment.apiUrl;

  getStaffAccounts(): Observable<StaffAccount[]> {
    return this.http.get<StaffAccount[]>(this.baseUrl + 'admin/staff', {
      headers: this.getAuthHeaders(),
    });
  }

  createStaffAccount(payload: CreateStaffAccountRequest): Observable<StaffAccount> {
    return this.http.post<StaffAccount>(this.baseUrl + 'admin/staff', payload, {
      headers: this.getAuthHeaders(),
    });
  }

  deleteStaffAccount(staffId: number): Observable<DeleteStaffAccountResult> {
    return this.http.delete<DeleteStaffAccountResult>(this.baseUrl + `admin/staff/${staffId}`, {
      headers: this.getAuthHeaders(),
    });
  }

  private getAuthHeaders(): HttpHeaders {
    const token = this.authService.currentUser()?.token;
    return new HttpHeaders({
      Authorization: `Bearer ${token ?? ''}`,
    });
  }
}
