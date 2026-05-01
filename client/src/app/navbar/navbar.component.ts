import { Component, ElementRef, HostListener, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { AuthService } from '../services/auth.service';
import { NotificationService } from '../services/notification.service';

@Component({
    selector: 'app-navbar',
    standalone: true,
    imports: [RouterLink, RouterLinkActive, CommonModule, BsDropdownModule],
    templateUrl: './navbar.component.html',
    styleUrl: './navbar.component.css'
})
export class NavbarComponent {
    isMenuOpen = false;
    isNotificationsOpen = false;
    authService = inject(AuthService);
    notificationService = inject(NotificationService);
    private el = inject(ElementRef<HTMLElement>);

    toggleMenu() {
        this.isMenuOpen = !this.isMenuOpen;
    }

    closeMenu() {
        this.isMenuOpen = false;
    }

    toggleNotifications(event?: MouseEvent) {
        event?.stopPropagation();
        if (!this.authService.currentUser()) return;

        this.isNotificationsOpen = !this.isNotificationsOpen;
        if (this.isNotificationsOpen) {
            this.notificationService.loadNotifications();
        } else {
            this.notificationService.loadUnreadCount();
        }
    }

    closeNotifications() {
        if (!this.isNotificationsOpen) return;
        this.isNotificationsOpen = false;
        this.notificationService.loadUnreadCount();
    }

    @HostListener('document:click', ['$event'])
    onDocumentClick(event: MouseEvent) {
        if (!this.isNotificationsOpen) return;
        const target = event.target as Node | null;
        if (!target) return;
        if (this.el.nativeElement.contains(target)) return;
        this.closeNotifications();
    }

    logout() {
        this.authService.logout();
        this.closeMenu();
        this.closeNotifications();
    }
}
