import { Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { HomeComponent } from './home/home.component';
import { FlightSearchComponent } from './flights/search/flight-search.component';
import { BaggageComponent } from './baggage/baggage.component';
import { SeatSelectionComponent } from './flights/seats/seat-selection.component';
import { BookingsListComponent } from './bookings/bookings-list.component';
import { PassengersListComponent } from './passengers/components/passengers-list.component';
import { PassengerProfileComponent } from './passengers/components/passenger-profile.component';
import { MyAccountComponent } from './account/my-account.component';
import { StaffListComponent } from './admin/staff/staff-list.component';
import { FlightsAdminComponent } from './admin/flights/flights-admin.component';
import { AircraftAdminComponent } from './admin/aircraft/aircraft-admin.component';
import { BroadcastComponent } from './admin/broadcast/broadcast.component';
import { authGuard } from './_guards/auth.guard';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [authGuard],
    children:
      [{ path: 'flights', component: FlightSearchComponent },
      { path: 'baggage', component: BaggageComponent },
      { path: 'seats', component: SeatSelectionComponent },
      { path: 'bookings', component: BookingsListComponent },
      { path: 'admin/passengers', component: PassengersListComponent },
      { path: 'admin/passengers/:id', component: PassengerProfileComponent },
      { path: 'admin/staff', component: StaffListComponent },
      { path: 'admin/flights', component: FlightsAdminComponent },
      { path: 'admin/aircraft', component: AircraftAdminComponent },
      { path: 'admin/broadcast', component: BroadcastComponent },
      { path: 'account', component: MyAccountComponent }]

  },
  { path: '**', component: HomeComponent, pathMatch: 'full' },
];
