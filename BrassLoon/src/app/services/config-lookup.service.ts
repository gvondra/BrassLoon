import { Injectable } from '@angular/core';
import { HttpClientUtilService } from '../http-client-util.service';
import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root'
})
export class ConfigLookupService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private tokenService: TokenService) { }

  GetCodes(domainId: string) : Promise<string[]> {
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetConfigBaseAddress()
      .then(baseAddress => `${baseAddress}LookupCode/${domainId}`)
    );  
  }
}
