import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

import { AuthService } from '../../services/auth.service';
import { StaffService } from '../../services/staff.service';
import { CreateStaffAccountRequest, StaffAccount } from '../../../models/staff';

@Component({
  selector: 'app-staff-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './staff-list.component.html',
  styleUrl: './staff-list.component.css',
})
export class StaffListComponent {
  private staffService = inject(StaffService);
  private authService = inject(AuthService);
  private toastr = inject(ToastrService);
  private router = inject(Router);
  private fb = inject(FormBuilder);

  staff = signal<StaffAccount[]>([]);
  isLoading = signal(false);
  isSaving = signal(false);
  isDeleting = signal(false);
  error = signal<string | null>(null);
  isAddModalOpen = signal(false);
  staffToDelete = signal<StaffAccount | null>(null);

  staffCount = computed(() => this.staff().length);

  staffForm: FormGroup = this.fb.group({
    firstName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
    lastName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(100)]],
    city: ['', [Validators.maxLength(50)]],
    country: ['', [Validators.maxLength(50)]],
    address: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(100)]],
    password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
  });

  constructor() {
    if (!this.authService.currentUser()?.token) {
      this.toastr.error('Please login to manage staff.');
      this.router.navigate(['/login']);
      return;
    }

    if (!this.authService.getRoles().includes('Admin')) {
      this.toastr.error('Admin access is required.');
      this.router.navigate(['/']);
      return;
    }

    this.loadStaff();
  }

  loadStaff(): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.staffService.getStaffAccounts().subscribe({
      next: (staff) => {
        this.staff.set(staff);
        this.isLoading.set(false);
      },
      error: () => {
        this.error.set('Failed to load staff accounts.');
        this.isLoading.set(false);
      },
    });
  }

  openAddModal(): void {
    this.staffForm.reset({
      firstName: '',
      lastName: '',
      email: '',
      city: '',
      country: '',
      address: '',
      password: '',
    });
    this.isAddModalOpen.set(true);
  }

  closeAddModal(): void {
    if (this.isSaving()) return;
    this.isAddModalOpen.set(false);
  }

  createStaff(): void {
    if (this.staffForm.invalid) {
      this.staffForm.markAllAsTouched();
      return;
    }

    this.isSaving.set(true);
    const payload = this.buildCreatePayload();

    this.staffService.createStaffAccount(payload).subscribe({
      next: (createdStaff) => {
        this.staff.update((items) => [...items, createdStaff].sort(this.sortByName));
        this.isSaving.set(false);
        this.isAddModalOpen.set(false);
        this.toastr.success('Staff account created.');
      },
      error: (err) => {
        this.isSaving.set(false);
        this.toastr.error(this.extractError(err, 'Failed to create staff account.'));
      },
    });
  }

  requestDelete(staff: StaffAccount): void {
    this.staffToDelete.set(staff);
  }

  cancelDelete(): void {
    if (this.isDeleting()) return;
    this.staffToDelete.set(null);
  }

  deleteStaff(): void {
    const staff = this.staffToDelete();
    if (!staff) return;

    this.isDeleting.set(true);
    this.staffService.deleteStaffAccount(staff.id).subscribe({
      next: () => {
        this.staff.update((items) => items.filter((item) => item.id !== staff.id));
        this.isDeleting.set(false);
        this.staffToDelete.set(null);
        this.toastr.success('Staff account deleted.');
      },
      error: (err) => {
        this.isDeleting.set(false);
        this.toastr.error(this.extractError(err, 'Failed to delete staff account.'));
      },
    });
  }

  fullName(staff: StaffAccount): string {
    return `${staff.firstName} ${staff.lastName}`.trim();
  }

  private buildCreatePayload(): CreateStaffAccountRequest {
    const value = this.staffForm.getRawValue() as CreateStaffAccountRequest;

    return {
      firstName: value.firstName.trim(),
      lastName: value.lastName.trim(),
      email: value.email.trim(),
      city: value.city.trim(),
      country: value.country.trim(),
      address: value.address.trim(),
      password: value.password,
    };
  }

  private sortByName(a: StaffAccount, b: StaffAccount): number {
    const left = `${a.lastName} ${a.firstName}`.toLowerCase();
    const right = `${b.lastName} ${b.firstName}`.toLowerCase();
    return left.localeCompare(right);
  }

  private extractError(err: unknown, fallback: string): string {
    const error = (err as { error?: unknown })?.error;
    if (typeof error === 'string') return error;
    if (Array.isArray(error)) return error.join(' ');
    return fallback;
  }
}
