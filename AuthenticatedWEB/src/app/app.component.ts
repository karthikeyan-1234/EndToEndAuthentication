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
  weatherInfo: WeatherForecast[] = [];

  constructor(private authService: AuthService, private weatherService: WeatherService) { 
    this.authService.loadUserProfile().then(profile => {
      this.username = profile ? profile.username : 'Guest';
      localStorage.setItem('username', this.username);
    });




  }
  
  logout(): void {
    this.authService.logout();
  }
}
