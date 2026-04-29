import { inject, Injectable, signal } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { User } from '../../models/user';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  currentUser = signal<User | null>(this.getUserFromStorage());

  private getUserFromStorage(): User | null {
    const storedUser = localStorage.getItem('user');
    return storedUser ? JSON.parse(storedUser) : null;
  }

  login(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      tap(user => this.setCurrentUser(user))
    )
  }

  register(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      tap(user => this.setCurrentUser(user))
    )
  }

  getCurrentUser(): Observable<User> {
    return this.http.get<User>(this.baseUrl + 'account', {
      headers: this.getAuthHeaders()
    }).pipe(
      tap(user => this.setCurrentUser(user))
    );
  }

  setCurrentUser(user: User): void {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUser.set(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUser.set(null);
  }

  private getAuthHeaders(): HttpHeaders {
    const token = this.currentUser()?.token;

    return new HttpHeaders({
      Authorization: `Bearer ${token ?? ''}`
    });
  }
}
