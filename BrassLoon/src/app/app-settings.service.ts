import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AppSettingsService {

  private AppSettings: any;

  constructor(private httpClient: HttpClient) { }

  LoadSettings() : Promise<any> {
    return this.httpClient.get(environment["AppSettingsPath"])
    .toPromise()
    .then(res => {
      this.AppSettings = res;
      return res;
    });
  }

  GetSettings() : any {
    return this.AppSettings;
  }

}
