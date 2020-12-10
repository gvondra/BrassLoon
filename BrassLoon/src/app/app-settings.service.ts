import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AppSettingsService {

  private AppSettings;

  constructor(private httpClient: HttpClient) { }

  LoadSettings() {
    return this.httpClient.get(environment["AppSettingsPath"])
    .toPromise()
    .then(res => {
      this.AppSettings = res;
    });
  }

  GetSettings() : any {
    return this.AppSettings;
  }

}
