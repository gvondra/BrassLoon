import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { HttpClientUtilService } from '../http-client-util.service';
import { firstValueFrom, from, Observable } from 'rxjs';
import { mergeMap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class TokenService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private httpClient: HttpClient) { }

  GetToken() : Promise<string> {
    return firstValueFrom(this.GetToken2())
    .then(res => res as string);
  }

  GetToken2() : Observable<string> {
    return this.httpClientUtil.CreateUserTokenAuthHeader()
    .pipe(mergeMap(headers => {
      return from(this.httpClientUtil.GetAccountBaseAddress()
      .then(baseAddress => firstValueFrom(this.httpClient.post(`${baseAddress}Token`, null, {headers: headers, responseType: 'text'}))))
    }))
    ;    
  }
}
