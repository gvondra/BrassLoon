import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { HttpClientUtilService } from '../http-client-util.service';
import { Exception } from '../models/exception';
import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root'
})
export class ExceptionService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private tokenService: TokenService) { }

  Get(domainId: string, id: string) : Promise<Exception> {
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetLogBaseAddress()
      .then(baseAddress => `${baseAddress}Exception/${domainId}/${id}`)
    );  
  }

  Search(domainId: string, maxTimestamp: string) : Promise<Array<Exception>> {
    let params : HttpParams = new HttpParams().append("maxTimestamp", maxTimestamp);
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetLogBaseAddress()
      .then(baseAddress => `${baseAddress}Exception/${domainId}`),
      params
    );   
  }
}
