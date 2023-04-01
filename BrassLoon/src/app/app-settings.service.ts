import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';
import { firstValueFrom, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AppSettingsService {

  private AppSettings: any = null;
  private SettingsObservable: Observable<any> = null;

  constructor(private httpClient: HttpClient) { }

  LoadSettings() : Observable<any> {
    this.SettingsObservable = this.httpClient.get(environment["AppSettingsPath"]);
    this.SettingsObservable.subscribe(res => {
      this.AppSettings = res;
    });
    return this.SettingsObservable;
  }

  GetSettings() : Promise<any> {    
    if (this.AppSettings) {
      return Promise.resolve(this.AppSettings);
    }
    else {
      return firstValueFrom(this.SettingsObservable);
    }    
  }

}
