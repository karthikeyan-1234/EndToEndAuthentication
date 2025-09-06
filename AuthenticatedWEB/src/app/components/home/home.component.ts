import { Component } from '@angular/core';
import { WeatherForecast } from '../../models/weather';
import { WeatherService } from '../../services/weather.service';
import { MatTableModule } from '@angular/material/table';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [MatTableModule, CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {

  weatherInfo: WeatherForecast[] = [];

  constructor(private weatherService: WeatherService) {
    this.weatherService.getWeatherForecast().subscribe({
      next: (weatherInfo: WeatherForecast[]) => {
        this.weatherInfo = weatherInfo;
      }
    });
  }

}
