import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { HttpClientUtilService } from '../http-client-util.service';
import { TokenService } from './token.service';
import { Observable } from 'rxjs';
import { map, mergeMap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ConfigLookupService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private httpClient: HttpClient,
    private tokenService: TokenService) { }

  GetCodes(domainId: string) : Observable<string[]> {
    return this.httpClientUtil.CreateAuthHeader2(this.tokenService)
    .pipe(
      mergeMap(headers => {
        return this.httpClient.get(`${this.httpClientUtil.GetConfigBaseAddress()}LookupCode/${domainId}`, {headers: headers})
        .pipe(
          map(value => value as string[])
        );
    })
    );
  }
}
