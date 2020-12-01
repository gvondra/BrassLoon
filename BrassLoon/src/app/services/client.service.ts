import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { HttpClientUtilService } from '../http-client-util.service';
import { TokenService } from './token.service';
import { Client } from '../models/client';
import { ClientCredentialRequest } from '../models/client-credential-request';

@Injectable({
  providedIn: 'root'
})
export class ClientService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private httpClient: HttpClient,
    private tokenService: TokenService) { }

  CreateClientSecret() : Promise<string> {
    return this.httpClient.get(`${this.httpClientUtil.GetAccountBaseAddress()}ClientSecret`).toPromise()
      .then(res => res["Secret"]);
  }  

  Get(id: string) : Promise<Client> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.get(`${this.httpClientUtil.GetAccountBaseAddress()}Client/${id}`, {headers: headers}).toPromise()
        .then(res => res as Client);
    });      
  }
  
  Create(clientRequest: ClientCredentialRequest) : Promise<Client> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.post(`${this.httpClientUtil.GetAccountBaseAddress()}Client`, clientRequest, {headers: headers}).toPromise()
        .then(res => res as Client);
    });      
  }

  Update(id: string, clientRequest: ClientCredentialRequest) : Promise<Client> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.put(`${this.httpClientUtil.GetAccountBaseAddress()}Client/${id}`, clientRequest, {headers: headers}).toPromise()
        .then(res => res as Client);
    });      
  }
}
