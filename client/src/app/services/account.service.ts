import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { User } from '../../models/user';

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
  private baseUrl = environment.apiUrl;

  getCurrentUser(): Observable<User> {
    return this.http.get<User>(this.baseUrl + 'account');
  }

  updateAccount(payload: UpdateAccountPayload): Observable<User> {
    return this.http.patch<User>(this.baseUrl + 'account', payload);
  }

  deleteAccount(): Observable<DeleteAccountResult> {
    return this.http.delete<DeleteAccountResult>(this.baseUrl + 'account', {
      body: {
        confirm: true
      }
    });
  }
}
