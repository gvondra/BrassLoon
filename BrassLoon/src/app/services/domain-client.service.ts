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
        return this.httpClient.get(`${this.httpClientUtil.GetAuthorizationBaseAddress()}Client/${domainId}/${id}`, {headers: headers}).toPromise()
        .then(res => res as DomainClient);
    });    
  }
  
  GetByDomainId(domainId: string) : Promise<DomainClient[]> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.get(`${this.httpClientUtil.GetAuthorizationBaseAddress()}Client/${domainId}`, {headers: headers}).toPromise()
        .then(res => res as Array<DomainClient>);
    });    
  }

  Create(domainId: string, client: DomainClient): Promise<DomainClient> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.post(`${this.httpClientUtil.GetAuthorizationBaseAddress()}Client/${domainId}`, client, {headers: headers}).toPromise()
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
