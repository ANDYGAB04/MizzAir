import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { User } from '../../models/user';
import { AuthService } from './auth.service';

export interface UpdateAccountPayload {
  city?: string;
  country?: string;
  address?: string;
  currentPassword?: string;
  newPassword?: string;
}

export interface DeleteAccountResult {
  userId: number;
  deletedReservationsCount: number;
  deletedSessionsCount: number;
  message: string;
}

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);
  private baseUrl = environment.apiUrl;

  getCurrentUser(): Observable<User> {
    return this.http.get<User>(this.baseUrl + 'account', {
      headers: this.getAuthHeaders()
    });
  }

  updateAccount(payload: UpdateAccountPayload): Observable<User> {
    return this.http.patch<User>(this.baseUrl + 'account', payload, {
      headers: this.getAuthHeaders()
    });
  }

  deleteAccount(): Observable<DeleteAccountResult> {
    return this.http.delete<DeleteAccountResult>(this.baseUrl + 'account', {
      headers: this.getAuthHeaders()
    });
  }

  private getAuthHeaders(): HttpHeaders {
    const token = this.authService.currentUser()?.token;

    return new HttpHeaders({
      Authorization: `Bearer ${token ?? ''}`
    });
  }
}
