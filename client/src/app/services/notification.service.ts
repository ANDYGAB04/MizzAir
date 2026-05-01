import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { finalize } from 'rxjs';
import { environment } from '../../environments/environment';
import { AppNotification } from '../../models/notification';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

  notifications = signal<AppNotification[]>([]);
  unreadCount = signal<number>(0);
  loading = signal<boolean>(false);
  error = signal<string | null>(null);

  loadUnreadCount() {
    return this.http.get<number>(this.baseUrl + 'notification/unread-count').subscribe({
      next: (count) => this.unreadCount.set(count ?? 0),
      error: () => this.unreadCount.set(0),
    });
  }

  loadNotifications(take = 30) {
    this.loading.set(true);
    this.error.set(null);

    return this.http
      .get<AppNotification[]>(this.baseUrl + 'notification', { params: { take } as any })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (items) => {
          const list = Array.isArray(items) ? items : [];
          this.notifications.set(list);
          this.unreadCount.set(list.filter((n) => !n.isRead).length);
        },
        error: () => {
          this.error.set('Failed to load notifications');
        },
      });
  }

  markRead(id: number, isRead: boolean) {
    return this.http
      .patch<void>(this.baseUrl + `notification/${id}/read`, { isRead })
      .subscribe({
        next: () => {
          const updated = this.notifications().map((n) => (n.id === id ? { ...n, isRead } : n));
          this.notifications.set(updated);
          this.unreadCount.set(updated.filter((n) => !n.isRead).length);
        },
      });
  }
}

