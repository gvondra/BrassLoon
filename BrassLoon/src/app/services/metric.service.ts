import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { HttpClientUtilService } from '../http-client-util.service';
import { Metric } from '../models/metric';
import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root'
})
export class MetricService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private tokenService: TokenService) { }
   
  Search(domainId: string, maxTimestamp: string, eventCode: string) : Promise<Array<Metric>> {
    let params : HttpParams = new HttpParams()
    .append("maxTimestamp", maxTimestamp)
    .append("eventCode", eventCode);
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetLogBaseAddress()
      .then(baseAddress => `${baseAddress}Metric/${domainId}`),
      params
    );     
  }

  GetEventCodes(domainId: string) : Promise<Array<string>> {
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetLogBaseAddress()
      .then(baseAddress => `${baseAddress}MetricEventCode/${domainId}`)
    );   
  }
}
