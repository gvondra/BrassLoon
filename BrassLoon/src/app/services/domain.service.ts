import { Injectable } from '@angular/core';
import { HttpClientUtilService } from '../http-client-util.service';
import { Domain } from '../models/domain';
import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root'
})
export class DomainService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private tokenService: TokenService) { }

  Get(id: string) : Promise<Domain> {
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetAccountBaseAddress()
      .then(baseAddress => `${baseAddress}Domain/${id}`)
    );  
  }

  Create(domain: Domain) : Promise<Domain> {
    return this.httpClientUtil.PostRequest(this.tokenService,
      this.httpClientUtil.GetAccountBaseAddress()
      .then(baseAddress => `${baseAddress}Domain`),
      domain
    );   
  }

  Update(id: string, domain: Domain) : Promise<Domain> {
    return this.httpClientUtil.PutRequest(this.tokenService,
      this.httpClientUtil.GetAccountBaseAddress()
      .then(baseAddress => `${baseAddress}Domain/${id}`),
      domain
    );    
  }

  Delete(id: string) : Promise<Domain> {
    return this.httpClientUtil.PatchRequest(this.tokenService,
      this.httpClientUtil.GetAccountBaseAddress()
      .then(baseAddress => `${baseAddress}Domain/${id}/Deleted`),
      { "Deleted": "true" }
    );  
  }

  UnDelete(id: string) : Promise<Domain> {
    return this.httpClientUtil.PatchRequest(this.tokenService,
      this.httpClientUtil.GetAccountBaseAddress()
      .then(baseAddress => `${baseAddress}Domain/${id}/Deleted`),
      { "Deleted": "false" }
    );  
  }
}
