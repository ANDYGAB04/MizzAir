import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

import { AuthService } from '../../services/auth.service';
import { PassengerService } from '../../services/passenger.service';
import { Passenger, PassengerFilterRequest } from '../../../models/passenger';
import { PaginatedResult } from '../../../models/paginated-result';

type SortField = 'fullname' | 'email' | 'phone';

@Component({
  selector: 'app-passengers-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './passengers-list.component.html',
  styleUrl: './passengers-list.component.css',
})
export class PassengersListComponent {
  private passengerService = inject(PassengerService);
  private authService = inject(AuthService);
  private toastr = inject(ToastrService);
  private router = inject(Router);

  passengers = signal<Passenger[]>([]);
  pagination = signal<PaginatedResult<Passenger> | null>(null);

  isLoading = signal(false);
  error = signal<string | null>(null);

  searchTerm = signal('');
  sortBy = signal<SortField>('fullname');
  isDescending = signal(false);

  pageNumber = signal(1);
  pageSize = signal(10);

  hasPrevious = computed(() => this.pagination()?.hasPrevious ?? false);
  hasNext = computed(() => this.pagination()?.hasNext ?? false);

  constructor() {
    if (!this.authService.currentUser()?.token) {
      this.toastr.error('Please login to view passengers.');
      this.router.navigate(['/login']);
      return;
    }

    this.loadPassengers();
  }

  private buildFilter(): PassengerFilterRequest {
    return {
      searchTerm: this.searchTerm().trim() ? this.searchTerm().trim() : undefined,
      sortBy: this.sortBy(),
      isDescending: this.isDescending(),
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
    };
  }

  loadPassengers(): void {
    this.isLoading.set(true);
    this.error.set(null);

    const filter = this.buildFilter();
    this.passengerService.getPassengers(filter).subscribe({
      next: (result) => {
        this.passengers.set(result.items);
        this.pagination.set(result);
        this.isLoading.set(false);
      },
      error: () => {
        this.error.set('Failed to load passengers. Please try again.');
        this.isLoading.set(false);
      },
    });
  }

  onSearch(): void {
    this.pageNumber.set(1);
    this.loadPassengers();
  }

  setSort(field: SortField): void {
    if (this.sortBy() === field) {
      this.isDescending.set(!this.isDescending());
      this.loadPassengers();
      return;
    }

    this.sortBy.set(field);
    this.isDescending.set(false);
    this.pageNumber.set(1);
    this.loadPassengers();
  }

  getSortIndicator(field: SortField): string {
    if (this.sortBy() !== field) return '';
    return this.isDescending() ? '▼' : '▲';
  }

  openPassenger(passengerId: number): void {
    this.router.navigate(['/admin/passengers', passengerId]);
  }

  prevPage(): void {
    if (!this.hasPrevious()) return;
    this.pageNumber.update((p) => p - 1);
    this.loadPassengers();
  }

  nextPage(): void {
    if (!this.hasNext()) return;
    this.pageNumber.update((p) => p + 1);
    this.loadPassengers();
  }

  passengerFullName(p: Passenger): string {
    return `${p.firstName} ${p.lastName}`.trim();
  }
}
