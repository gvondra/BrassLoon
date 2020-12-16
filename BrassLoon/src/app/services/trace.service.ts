import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { HttpClientUtilService } from '../http-client-util.service';
import { TokenService } from './token.service';
import { Trace } from '../models/trace';

@Injectable({
  providedIn: 'root'
})
export class TraceService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private httpClient: HttpClient,
    private tokenService: TokenService) { }
   
  Search(domainId: string, maxTimestamp: string, eventCode: string) : Promise<Array<Trace>> {
    let params : HttpParams = new HttpParams()
    .append("maxTimestamp", maxTimestamp)
    .append("eventCode", eventCode);
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.get(`${this.httpClientUtil.GetLogBaseAddress()}Trace/${domainId}`, {headers: headers, params: params}).toPromise()
        .then(res => res as Trace[]);
    });      
  }

  GetEventCodes(domainId: string) : Promise<Array<string>> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.get(`${this.httpClientUtil.GetLogBaseAddress()}TraceEventCode/${domainId}`, {headers: headers}).toPromise()
        .then(res => res as string[]);
    });      
  }
}
