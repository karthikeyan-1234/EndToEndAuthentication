import { Routes } from '@angular/router';
import { AppComponent } from './app.component';
import { KeycloakGuard } from './auth/keycloak.guard';
import { HomeComponent } from './components/home/home.component';

export const routes: Routes = [
    { path: 'home', component: HomeComponent, canActivate: [KeycloakGuard] },
    { path: '**', redirectTo: 'home' }
];
