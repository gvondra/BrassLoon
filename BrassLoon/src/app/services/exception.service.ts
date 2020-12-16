import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { HttpClientUtilService } from '../http-client-util.service';
import { TokenService } from './token.service';
import { Exception } from '../models/exception';

@Injectable({
  providedIn: 'root'
})
export class ExceptionService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private httpClient: HttpClient,
    private tokenService: TokenService) { }

  Get(domainId: string, id: string) : Promise<Exception> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.get(`${this.httpClientUtil.GetLogBaseAddress()}Exception/${domainId}/${id}`, {headers: headers}).toPromise()
        .then(res => res as Exception);
    });      
  }

  Search(domainId: string, maxTimestamp: string) : Promise<Array<Exception>> {
    let params : HttpParams = new HttpParams().append("maxTimestamp", maxTimestamp);
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.get(`${this.httpClientUtil.GetLogBaseAddress()}Exception/${domainId}`, {headers: headers, params: params}).toPromise()
        .then(res => res as Exception[]);
    });      
  }
}
