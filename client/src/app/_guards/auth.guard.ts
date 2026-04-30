import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
    const authService = inject(AuthService);
    const toastr = inject(ToastrService);

    if (authService.currentUser()) {
        return true;
    } else {
        toastr.error('You shall not pass');
        return false;
    }
};