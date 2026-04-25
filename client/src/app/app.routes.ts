import { Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { HomeComponent } from './home/home.component';
import { FlightSearchComponent } from './flights/search/flight-search.component';
import { BaggageComponent } from './baggage/baggage.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'flights', component: FlightSearchComponent },
  { path: 'baggage', component: BaggageComponent },
  { path: 'bookings', component: HomeComponent }
];
