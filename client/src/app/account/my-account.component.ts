import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../services/auth.service';
import { AccountService, DeleteAccountResult, UpdateAccountPayload } from '../services/account.service';

@Component({
  selector: 'app-my-account',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './my-account.component.html',
  styleUrl: './my-account.component.css'
})
export class MyAccountComponent implements OnInit {
  authService = inject(AuthService);
  private accountService = inject(AccountService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  private fb = inject(FormBuilder);

  accountForm: FormGroup = new FormGroup({});
  isLoading = signal(false);
  isSaving = signal(false);
  isDeleting = signal(false);
  isEditOpen = signal(false);
  showDeleteDialog = signal(false);
  error = signal<string | null>(null);

  constructor() {
    if (!this.authService.currentUser()?.token) {
      this.toastr.error('Please login to access your account.');
      this.router.navigate(['/login']);
    }
  }

  ngOnInit(): void {
    this.initializeForm();
    this.loadAccount();
  }

  initializeForm(): void {
    this.accountForm = this.fb.group({
      phoneNumber: ['', [Validators.pattern(/^\d{10,}$/)]],
      city: ['', [Validators.minLength(2), Validators.maxLength(50)]],
      country: ['', [Validators.minLength(2), Validators.maxLength(50)]],
      address: ['', [Validators.minLength(5), Validators.maxLength(100)]],
      currentPassword: ['', [Validators.minLength(4), Validators.maxLength(8)]],
      newPassword: ['', [Validators.minLength(4), Validators.maxLength(8)]]
    }, { validators: [this.requireFullAddress(), this.requirePasswordPair(), this.disallowSamePasswords()] });
  }

  loadAccount(): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.accountService.getCurrentUser().subscribe({
      next: user => {
        this.authService.setCurrentUser(user);
        this.resetForm();
        this.isLoading.set(false);
      },
      error: () => {
        this.error.set('We could not load your account details. Please try again.');
        this.isLoading.set(false);
      }
    });
  }

  openEdit(): void {
    this.resetForm();
    this.isEditOpen.set(true);
  }

  cancelEdit(): void {
    this.resetForm();
    this.isEditOpen.set(false);
  }

  saveChanges(): void {
    if (this.accountForm.invalid) {
      this.accountForm.markAllAsTouched();
      return;
    }

    const payload = this.buildPayload();
    if (!payload) {
      this.toastr.error('Please change your details before saving.');
      return;
    }

    this.isSaving.set(true);

    this.accountService.updateAccount(payload).subscribe({
      next: user => {
        this.authService.setCurrentUser(user);
        this.resetForm();
        this.isEditOpen.set(false);
        this.toastr.success('Account details updated successfully.');
      },
      error: error => {
        this.toastr.error(this.extractErrorMessage(error), 'Update failed');
        this.isSaving.set(false);
      },
      complete: () => this.isSaving.set(false)
    });
  }

  confirmDelete(): void {
    this.showDeleteDialog.set(true);
  }

  closeDeleteDialog(): void {
    if (this.isDeleting()) {
      return;
    }

    this.showDeleteDialog.set(false);
  }

  deleteAccount(): void {
    this.isDeleting.set(true);

    this.accountService.deleteAccount().subscribe({
      next: (result: DeleteAccountResult) => {
        this.authService.logout();
        this.showDeleteDialog.set(false);
        this.toastr.success(
          `${result.message}. Removed ${result.deletedReservationsCount} reservations and ${result.deletedSessionsCount} saved sessions.`
        );
        this.router.navigate(['/']);
      },
      error: error => {
        this.toastr.error(this.extractErrorMessage(error), 'Delete failed');
        this.isDeleting.set(false);
      },
      complete: () => this.isDeleting.set(false)
    });
  }

  private resetForm(): void {
    const user = this.authService.currentUser();

    this.accountForm.reset({
      phoneNumber: user?.phoneNumber ?? '',
      city: user?.city ?? '',
      country: user?.country ?? '',
      address: user?.address ?? '',
      currentPassword: '',
      newPassword: ''
    });
  }

  private buildPayload(): UpdateAccountPayload | null {
    const user = this.authService.currentUser();
    const formValue = this.accountForm.getRawValue();
    const payload: UpdateAccountPayload = {};

    const phoneNumber = formValue.phoneNumber?.trim();
    const city = formValue.city?.trim();
    const country = formValue.country?.trim();
    const address = formValue.address?.trim();
    const currentPassword = formValue.currentPassword?.trim();
    const newPassword = formValue.newPassword?.trim();

    if (phoneNumber !== user?.phoneNumber) {
      payload.phoneNumber = phoneNumber;
    }

    if (city !== user?.city || country !== user?.country || address !== user?.address) {
      payload.city = city;
      payload.country = country;
      payload.address = address;
    }

    if (currentPassword || newPassword) {
      payload.currentPassword = currentPassword;
      payload.newPassword = newPassword;
    }

    return Object.keys(payload).length > 0 ? payload : null;
  }

  private extractErrorMessage(error: any): string {
    if (Array.isArray(error?.error)) {
      return error.error.join(', ');
    }

    if (typeof error?.error === 'string') {
      return error.error;
    }

    return 'Something went wrong. Please try again.';
  }

  private requireFullAddress(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const city = control.get('city')?.value?.trim() ?? '';
      const country = control.get('country')?.value?.trim() ?? '';
      const address = control.get('address')?.value?.trim() ?? '';
      const hasAnyAddressValue = !!city || !!country || !!address;

      if (!hasAnyAddressValue) {
        return null;
      }

      return city && country && address ? null : { addressGroupIncomplete: true };
    };
  }

  private requirePasswordPair(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const currentPassword = control.get('currentPassword')?.value?.trim() ?? '';
      const newPassword = control.get('newPassword')?.value?.trim() ?? '';

      if (!currentPassword && !newPassword) {
        return null;
      }

      return currentPassword && newPassword ? null : { passwordPairRequired: true };
    };
  }

  private disallowSamePasswords(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const currentPassword = control.get('currentPassword')?.value?.trim() ?? '';
      const newPassword = control.get('newPassword')?.value?.trim() ?? '';

      if (!currentPassword || !newPassword) {
        return null;
      }

      return currentPassword !== newPassword ? null : { samePassword: true };
    };
  }
}
