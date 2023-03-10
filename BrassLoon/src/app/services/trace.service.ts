import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { HttpClientUtilService } from '../http-client-util.service';
import { Trace } from '../models/trace';
import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root'
})
export class TraceService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private tokenService: TokenService) { }
   
  Search(domainId: string, maxTimestamp: string, eventCode: string) : Promise<Array<Trace>> {
    let params : HttpParams = new HttpParams()
    .append("maxTimestamp", maxTimestamp)
    .append("eventCode", eventCode);
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetLogBaseAddress()
      .then(baseAddress => `${baseAddress}Trace/${domainId}`),
      params
    ); 
  }

  GetEventCodes(domainId: string) : Promise<Array<string>> {
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetLogBaseAddress()
      .then(baseAddress => `${baseAddress}TraceEventCode/${domainId}`)
    );   
  }
}
