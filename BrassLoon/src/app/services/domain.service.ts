import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { HttpClientUtilService } from '../http-client-util.service';
import { TokenService } from './token.service';
import { Domain } from '../models/domain';

@Injectable({
  providedIn: 'root'
})
export class DomainService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private httpClient: HttpClient,
    private tokenService: TokenService) { }

  Get(id: string) : Promise<Domain> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.get(`${this.httpClientUtil.GetAccountBaseAddress()}Domain/${id}`, {headers: headers}).toPromise()
        .then(res => res as Domain);
    });   
  }

  Create(domain: Domain) : Promise<Domain> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.post(`${this.httpClientUtil.GetAccountBaseAddress()}Domain`, domain, {headers: headers}).toPromise()
        .then(res => res as Domain);
    });      
  }

  Update(id: string, domain: Domain) : Promise<Domain> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.put(`${this.httpClientUtil.GetAccountBaseAddress()}Domain/${id}`, domain, {headers: headers}).toPromise()
        .then(res => res as Domain);
    });      
  }

  Delete(id: string) : Promise<Domain> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.patch(`${this.httpClientUtil.GetAccountBaseAddress()}Domain/${id}/Deleted`, { "Deleted": "true" }, {headers: headers}).toPromise()
        .then(res => res as Domain);
    });      
  }
}
