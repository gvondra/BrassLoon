import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { HttpClientUtilService } from '../http-client-util.service';
import { TokenService } from './token.service';
import { DomainClient } from '../models/domain-client';

@Injectable({
  providedIn: 'root'
})
export class DomainClientService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private httpClient: HttpClient,
    private tokenService: TokenService) { }

  Get(domainId: string, id: string) : Promise<DomainClient> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.get<DomainClient>(`${this.httpClientUtil.GetAuthorizationBaseAddress()}Client/${domainId}/${id}`, {headers: headers}).toPromise();
    });    
  }

  GetClientCredentialSecret() : Promise<string> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.get(`${this.httpClientUtil.GetAuthorizationBaseAddress()}ClientCredentialSecret`, {headers: headers, responseType: "text"}).toPromise()
        .then(s => {
          return s as string;
        })
    });   
  }
  
  GetByDomainId(domainId: string) : Promise<DomainClient[]> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.get(`${this.httpClientUtil.GetAuthorizationBaseAddress()}Client/${domainId}`, {headers: headers}).toPromise()
        .then(res => res as Array<DomainClient>);
    });    
  }

  Create(client: DomainClient): Promise<DomainClient> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.post(`${this.httpClientUtil.GetAuthorizationBaseAddress()}Client/${client.DomainId}`, client, {headers: headers}).toPromise()
        .then(res => res as DomainClient);
    });    
  }

  Update(client: DomainClient): Promise<DomainClient> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.put(`${this.httpClientUtil.GetAuthorizationBaseAddress()}Client/${client.DomainId}/${client.ClientId}`, client, {headers: headers}).toPromise()
        .then(res => res as DomainClient);
    });
  }
}
