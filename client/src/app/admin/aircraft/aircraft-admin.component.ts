import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

import { AuthService } from '../../services/auth.service';
import { AircraftService } from '../../services/aircraft.service';
import { AircraftOption, CreateAircraftDto, UpdateAircraftDto } from '../../../models/aircraft';

@Component({
    selector: 'app-aircraft-admin',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule],
    templateUrl: './aircraft-admin.component.html',
    styleUrl: './aircraft-admin.component.css',
})
export class AircraftAdminComponent {
    private aircraftService = inject(AircraftService);
    private authService = inject(AuthService);
    private toastr = inject(ToastrService);
    private router = inject(Router);
    private fb = inject(FormBuilder);

    aircraft = signal<AircraftOption[]>([]);
    selectedAircraft = signal<AircraftOption | null>(null);
    aircraftToDelete = signal<AircraftOption | null>(null);

    isLoading = signal(false);
    isSaving = signal(false);
    isDeleting = signal(false);
    isModalOpen = signal(false);
    error = signal<string | null>(null);

    aircraftCount = computed(() => this.aircraft().length);
    modalTitle = computed(() => this.selectedAircraft() ? 'Edit Aircraft' : 'Add Aircraft');

    aircraftForm: FormGroup = this.fb.group({
        model: ['', Validators.required],
        registrationNumber: ['', Validators.required],
        seatRows: [1, [Validators.required, Validators.min(1)]],
        seatsPerRow: [1, [Validators.required, Validators.min(1)]],
        totalSeats: [1, [Validators.required, Validators.min(1)]],
    });

    constructor() {
        if (!this.authService.currentUser()?.token) {
            this.toastr.error('Please login to manage aircraft.');
            this.router.navigate(['/login']);
            return;
        }

        if (!this.authService.hasAdmin()) {
            this.toastr.error('Admin access is required.');
            this.router.navigate(['/']);
            return;
        }

        this.loadAircraft();

        // Auto-calculate totalSeats when seatRows or seatsPerRow change
        this.aircraftForm.get('seatRows')?.valueChanges.subscribe(() => this.updateTotalSeats());
        this.aircraftForm.get('seatsPerRow')?.valueChanges.subscribe(() => this.updateTotalSeats());
    }

    loadAircraft(): void {
        this.isLoading.set(true);
        this.error.set(null);

        this.aircraftService.getAircraft().subscribe({
            next: (data) => {
                this.aircraft.set(data);
                this.isLoading.set(false);
            },
            error: () => {
                this.error.set('Failed to load aircraft data.');
                this.isLoading.set(false);
            },
        });
    }

    openAddModal(): void {
        this.selectedAircraft.set(null);
        this.aircraftForm.reset({
            model: '',
            registrationNumber: '',
            seatRows: 1,
            seatsPerRow: 1,
            totalSeats: 1,
        });
        this.isModalOpen.set(true);
    }

    openEditModal(selectedAircraft: AircraftOption): void {
        this.selectedAircraft.set(selectedAircraft);
        this.aircraftForm.patchValue({
            model: selectedAircraft.model,
            registrationNumber: selectedAircraft.registrationNumber,
            seatRows: selectedAircraft.seatRows,
            seatsPerRow: selectedAircraft.seatsPerRow,
            totalSeats: selectedAircraft.totalSeats,
        });
        this.isModalOpen.set(true);
    }

    closeModal(): void {
        this.isModalOpen.set(false);
        this.aircraftForm.reset();
        this.selectedAircraft.set(null);
    }

    updateTotalSeats(): void {
        const seatRows = this.aircraftForm.get('seatRows')?.value || 0;
        const seatsPerRow = this.aircraftForm.get('seatsPerRow')?.value || 0;
        const totalSeats = seatRows * seatsPerRow;
        this.aircraftForm.patchValue({ totalSeats }, { emitEvent: false });
    }

    saveAircraft(): void {
        if (this.aircraftForm.invalid) {
            this.toastr.error('Please fill in all required fields correctly.');
            return;
        }

        this.isSaving.set(true);

        const formValue = this.aircraftForm.value;
        const payload = {
            model: formValue.model,
            registrationNumber: formValue.registrationNumber,
            seatRows: parseInt(formValue.seatRows, 10),
            seatsPerRow: parseInt(formValue.seatsPerRow, 10),
            totalSeats: parseInt(formValue.totalSeats, 10),
        };

        if (this.selectedAircraft()) {
            // Update aircraft
            this.aircraftService.updateAircraft(this.selectedAircraft()!.id, payload as UpdateAircraftDto).subscribe({
                next: () => {
                    this.toastr.success('Aircraft updated successfully.');
                    this.isSaving.set(false);
                    this.closeModal();
                    this.loadAircraft();
                },
                error: (err) => {
                    const errors = err.error?.errors || ['Failed to update aircraft.'];
                    this.toastr.error(Array.isArray(errors) ? errors.join(' ') : errors);
                    this.isSaving.set(false);
                },
            });
        } else {
            // Create aircraft
            this.aircraftService.createAircraft(payload as CreateAircraftDto).subscribe({
                next: () => {
                    this.toastr.success('Aircraft created successfully.');
                    this.isSaving.set(false);
                    this.closeModal();
                    this.loadAircraft();
                },
                error: (err) => {
                    const errors = err.error?.errors || ['Failed to create aircraft.'];
                    this.toastr.error(Array.isArray(errors) ? errors.join(' ') : errors);
                    this.isSaving.set(false);
                },
            });
        }
    }

    openDeleteModal(selectedAircraft: AircraftOption): void {
        this.aircraftToDelete.set(selectedAircraft);
    }

    closeDeleteModal(): void {
        this.aircraftToDelete.set(null);
    }

    confirmDelete(): void {
        if (!this.aircraftToDelete()) return;

        this.isDeleting.set(true);

        this.aircraftService.deleteAircraft(this.aircraftToDelete()!.id).subscribe({
            next: (result) => {
                if (result.succeeded) {
                    this.toastr.success('Aircraft deleted successfully.');
                } else {
                    const errorMessage = result.errors && result.errors.length > 0
                        ? result.errors.join(' ')
                        : 'Failed to delete aircraft.';
                    this.toastr.error(errorMessage);
                }
                this.isDeleting.set(false);
                this.closeDeleteModal();
                this.loadAircraft();
            },
            error: (err) => {
                let errorMessage = 'Failed to delete aircraft.';

                if (err.error?.errors && Array.isArray(err.error.errors)) {
                    errorMessage = err.error.errors.join(' ');
                } else if (err.error?.errors && typeof err.error.errors === 'string') {
                    errorMessage = err.error.errors;
                } else if (Array.isArray(err.error)) {
                    errorMessage = err.error.join(' ');
                }

                this.toastr.error(errorMessage);
                this.isDeleting.set(false);
                this.closeDeleteModal();
            },
        });
    }
}
