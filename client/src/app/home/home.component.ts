import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
    selector: 'app-home',
    standalone: true,
    imports: [CommonModule, RouterLink],
    templateUrl: './home.component.html',
    styleUrl: './home.component.css'
})
export class HomeComponent {
    authService = inject(AuthService);
    
    features = [
        {
            icon: '✈️',
            title: 'Easy Booking',
            description: 'Book your flights in just a few clicks'
        },
        {
            icon: '🌍',
            title: 'Worldwide Destinations',
            description: 'Fly to over 500 destinations globally'
        },
        {
            icon: '💰',
            title: 'Best Prices',
            description: 'Get the best deals on flights'
        },
        {
            icon: '🛡️',
            title: 'Secure',
            description: 'Your data is safe and secure with us'
        }
    ];
}
