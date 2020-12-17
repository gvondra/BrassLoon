import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { HttpClientUtilService } from '../http-client-util.service';
import { TokenService } from './token.service';
import { Metric } from '../models/metric';

@Injectable({
  providedIn: 'root'
})
export class MetricService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private httpClient: HttpClient,
    private tokenService: TokenService) { }
   
  Search(domainId: string, maxTimestamp: string, eventCode: string) : Promise<Array<Metric>> {
    let params : HttpParams = new HttpParams()
    .append("maxTimestamp", maxTimestamp)
    .append("eventCode", eventCode);
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.get(`${this.httpClientUtil.GetLogBaseAddress()}Metric/${domainId}`, {headers: headers, params: params}).toPromise()
        .then(res => res as Metric[]);
    });      
  }

  GetEventCodes(domainId: string) : Promise<Array<string>> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.get(`${this.httpClientUtil.GetLogBaseAddress()}MetricEventCode/${domainId}`, {headers: headers}).toPromise()
        .then(res => res as string[]);
    });      
  }
}
