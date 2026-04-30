import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
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
  private baseUrl = environment.apiUrl;

  getStaffAccounts(): Observable<StaffAccount[]> {
    return this.http.get<StaffAccount[]>(this.baseUrl + 'admin/staff');
  }

  createStaffAccount(payload: CreateStaffAccountRequest): Observable<StaffAccount> {
    return this.http.post<StaffAccount>(this.baseUrl + 'admin/staff', payload);
  }

  deleteStaffAccount(staffId: number): Observable<DeleteStaffAccountResult> {
    return this.http.delete<DeleteStaffAccountResult>(this.baseUrl + `admin/staff/${staffId}`);
  }
}
