import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { KeycloakService } from 'keycloak-angular';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CommonModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'AuthenticatedWeb';

  constructor(private keycloakService: KeycloakService) { }

  isLoggedIn(): boolean {
    return this.keycloakService.isLoggedIn();
  }

async logout(): Promise<void> {
    try {
      // Call Keycloak logout with redirect back to your app
      await this.keycloakService.logout(window.location.origin);
    } catch (err) {
      console.error('Logout failed', err);
    }
  }
}
