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

  getRoles(): string[] {
    const token = this.currentUser()?.token;
    if (!token) return [];

    try {
      const payload = this.decodeJwtPayload(token);

      const roleKeys = [
        'role',
        'roles',
        'http://schemas.microsoft.com/ws/2008/06/identity/claims/role',
        'http://schemas.microsoft.com/ws/2005/05/identity/claims/role',
      ];

      const roles: string[] = [];

      for (const key of Object.keys(payload ?? {})) {
        // Fast path for typical role claims.
        if (!key.toLowerCase().includes('role') && !roleKeys.includes(key)) continue;

        const value = (payload as any)[key];
        if (typeof value === 'string') {
          roles.push(value);
        } else if (Array.isArray(value)) {
          roles.push(...value.filter((x) => typeof x === 'string'));
        }
      }

      // Normalize + dedupe.
      return Array.from(new Set(roles.map((r) => String(r))));
    } catch {
      return [];
    }
  }

  hasStaffOrAdmin(): boolean {
    const roles = this.getRoles();
    return roles.includes('Admin') || roles.includes('Staff');
  }

  private getUserFromStorage(): User | null {
    const storedUser = localStorage.getItem('user');
    return storedUser ? JSON.parse(storedUser) : null;
  }

  private decodeJwtPayload(token: string): any {
    // JWT = header.payload.signature
    const parts = token.split('.');
    if (parts.length < 2) throw new Error('Invalid JWT');

    const payloadPart = parts[1];
    // Base64url decode
    const base64 = payloadPart.replace(/-/g, '+').replace(/_/g, '/');
    const json = decodeURIComponent(
      atob(base64)
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );
    return JSON.parse(json);
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
