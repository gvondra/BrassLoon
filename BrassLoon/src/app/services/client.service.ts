import { Injectable } from '@angular/core';
import { HttpClientUtilService } from '../http-client-util.service';
import { Client } from '../models/client';
import { ClientCredentialRequest } from '../models/client-credential-request';
import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root'
})
export class ClientService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private tokenService: TokenService) { }

  CreateClientSecret() : Promise<string> {
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetAccountBaseAddress()
      .then(baseAddress => `${baseAddress}ClientSecret`)
    )
    .then(res => res["Secret"]);
  }  

  Get(id: string) : Promise<Client> {
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetAccountBaseAddress()
      .then(baseAddress => `${baseAddress}Client/${id}`)
    ); 
  }
  
  Create(clientRequest: ClientCredentialRequest) : Promise<Client> {
    return this.httpClientUtil.PostRequest(this.tokenService,
      this.httpClientUtil.GetAccountBaseAddress()
      .then(baseAddress => `${baseAddress}Client`),
      clientRequest
    ); 
  }

  Update(id: string, clientRequest: ClientCredentialRequest) : Promise<Client> {
    return this.httpClientUtil.PutRequest(this.tokenService,
      this.httpClientUtil.GetAccountBaseAddress()
      .then(baseAddress => `${baseAddress}Client/${id}`),
      clientRequest
    ); 
  }
}
