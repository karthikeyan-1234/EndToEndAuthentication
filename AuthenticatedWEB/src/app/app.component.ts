import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthService } from './services/auth.service';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { CommonModule } from '@angular/common';
import { WeatherService } from './services/weather.service';
import { WeatherForecast } from './models/weather';
import {MatTableModule} from '@angular/material/table';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, MatIconModule, MatButtonModule, MatToolbarModule, CommonModule, MatTableModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'KeyCloakTest';
  username: string = localStorage.getItem('username') || 'Guest';
  roles: string[] = JSON.parse(localStorage.getItem('roles') || '[]');
  role: string = localStorage.getItem('role') || 'No Role';
  weatherInfo: WeatherForecast[] = [];

  constructor(private authService: AuthService, private weatherService: WeatherService) { 
    this.authService.loadUserProfile().then(profile => {

      // Fetch and store user roles
      this.roles = this.authService.getUserRoles();
      localStorage.setItem('roles', JSON.stringify(this.roles));

      // For simplicity, just take the first role if available
      this.role = this.roles.length > 0 ? this.roles[0] : 'No Role';
      localStorage.setItem('role', this.role);

      console.log('User Roles:', this.roles);

      this.username = profile ? profile.username : 'Guest';
      localStorage.setItem('username', this.username);
    });




  }
  
  logout(): void {
    this.authService.logout();
  }
}
