import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';

@Component({
    selector: 'app-broadcast',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule],
    templateUrl: './broadcast.component.html',
    styleUrl: './broadcast.component.css',
})
export class BroadcastComponent {
    private http = inject(HttpClient);
    private toastr = inject(ToastrService);
    private authService = inject(AuthService);
    private router = inject(Router);
    private fb = inject(FormBuilder);

    broadcastForm: FormGroup = this.fb.group({
        message: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(1000)]],
        type: ['Announcement', Validators.required],
    });

    isSending = signal(false);
    notificationsCreated = signal<number | null>(null);
    lastBroadcastTime = signal<string | null>(null);

    constructor() {
        if (!this.authService.currentUser()?.token) {
            this.toastr.error('Please login to broadcast announcements.');
            this.router.navigate(['/login']);
            return;
        }

        if (!this.authService.hasAdmin()) {
            this.toastr.error('Admin access is required.');
            this.router.navigate(['/']);
            return;
        }
    }

    broadcast(): void {
        if (!this.broadcastForm.valid) {
            this.toastr.error('Please fill in all required fields.');
            return;
        }

        this.isSending.set(true);
        const { message, type } = this.broadcastForm.value;

        this.http
            .post<any>(`${environment.apiUrl}admin/broadcast-announcement`, {
                message: message.trim(),
                type: type.trim() || 'Announcement',
            })
            .subscribe({
                next: (result) => {
                    this.notificationsCreated.set(result.notificationsCreated);
                    this.lastBroadcastTime.set(new Date(result.createdAt).toLocaleString());
                    this.toastr.success(`Announcement sent to ${result.notificationsCreated} users!`);
                    this.broadcastForm.reset({ type: 'Announcement' });
                    this.isSending.set(false);
                },
                error: (error) => {
                    console.error('Broadcast error:', error);
                    this.toastr.error(error.error?.message || 'Failed to broadcast announcement.');
                    this.isSending.set(false);
                },
            });
    }

    resetForm(): void {
        this.broadcastForm.reset({ type: 'Announcement' });
        this.notificationsCreated.set(null);
    }

    get messageControl() {
        return this.broadcastForm.get('message');
    }

    get typeControl() {
        return this.broadcastForm.get('type');
    }

    get remainingChars(): number {
        const message = this.messageControl?.value || '';
        return 1000 - message.length;
    }
}
