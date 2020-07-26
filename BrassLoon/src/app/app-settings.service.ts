import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class AppSettingsService {

  private AppSettings;

  constructor(private httpClient: HttpClient) { }

  LoadSettings() {
    return this.httpClient.get("assets/appSettings.json")
    .toPromise()
    .then(res => {
      this.AppSettings = res;
    });
  }

  GetSettings() : any {
    return this.AppSettings;
  }

}
