import { Injectable } from '@angular/core';
import { KeycloakService } from 'keycloak-angular';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  constructor(private keycloakService: KeycloakService) {}

  async logout(): Promise<void> {
    await this.keycloakService.logout(window.location.origin);
  }

  async loadUserProfile(): Promise<any> {
    return this.keycloakService.loadUserProfile();
  }
}
