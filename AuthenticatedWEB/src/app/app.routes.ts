import { Routes } from '@angular/router';
import { AppComponent } from './app.component';
import { KeycloakGuard } from './auth/keycloak.guard';

export const routes: Routes = [
    { path: '', component: AppComponent, canActivate: [KeycloakGuard] }
];
