import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { HttpClientUtilService } from '../http-client-util.service';
import { TokenService } from './token.service';
import { DomainClient } from '../models/domain-client';
import { firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DomainClientService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private httpClient: HttpClient,
    private tokenService: TokenService) { }

  Get(domainId: string, id: string) : Promise<DomainClient> {
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetAuthorizationBaseAddress()
      .then(baseAddress => `${baseAddress}Client/${domainId}/${id}`)
    );  
  }

  GetClientCredentialSecret() : Promise<string> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => this.httpClientUtil.GetAuthorizationBaseAddress()
      .then(baseAddress => firstValueFrom(this.httpClient.get(`${baseAddress}ClientCredentialSecret`, {headers: headers, responseType: "text"})))
    );   
  }
  
  GetByDomainId(domainId: string) : Promise<DomainClient[]> {
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetAuthorizationBaseAddress()
      .then(baseAddress => `${baseAddress}Client/${domainId}`)
    ); 
  }

  Create(client: DomainClient): Promise<DomainClient> {
    return this.httpClientUtil.PostRequest(this.tokenService,
      this.httpClientUtil.GetAuthorizationBaseAddress()
      .then(baseAddress => `${baseAddress}Client/${client.DomainId}`),
      client
    ); 
  }

  Update(client: DomainClient): Promise<DomainClient> {
    return this.httpClientUtil.PutRequest(this.tokenService,
      this.httpClientUtil.GetAuthorizationBaseAddress()
      .then(baseAddress => `${baseAddress}Client/${client.DomainId}/${client.ClientId}`),
      client
    ); 
  }
}
