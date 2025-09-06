import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class WeatherService {

  private baseUrl = 'https://localhost:7253';

  constructor(private http: HttpClient) { 
  }

  getWeather() {
    return this.http.get(`${this.baseUrl}/WeatherForecast`);
  }
}
