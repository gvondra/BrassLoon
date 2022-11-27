import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { HttpClientUtilService } from '../http-client-util.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TokenService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private httpClient: HttpClient) { }

  GetToken() : Promise<string> {
    return this.GetToken2().toPromise()
    .then(res => res as string);
  }

  GetToken2() : Observable<string> {
    return this.httpClient.post(`${this.httpClientUtil.GetAccountBaseAddress()}Token`, null, {headers: this.httpClientUtil.CreateUserTokenAuthHeader(), responseType: 'text'});
  }
}
