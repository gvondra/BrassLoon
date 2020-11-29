import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { HttpClientUtilService } from '../http-client-util.service';
import { Account } from '../models/account';
import { TokenService } from './token.service';
import { Domain } from '../models/domain';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private httpClient: HttpClient,
    private tokenService: TokenService) { }

  GetAll() : Promise<Array<Account>> {  
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.get(`${this.httpClientUtil.GetAccountBaseAddress()}Account`, {headers: headers}).toPromise()
        .then(res => res as Array<Account>);
    });      
  }

  Get(id: string) : Promise<Account> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.get(`${this.httpClientUtil.GetAccountBaseAddress()}Account/${id}`, {headers: headers}).toPromise()
        .then(res => res as Account);
    });      
  }

  GetDomains(id: string) : Promise<Array<Domain>> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.get(`${this.httpClientUtil.GetAccountBaseAddress()}Account/${id}/Domain`, {headers: headers}).toPromise()
        .then(res => res as Array<Domain>);
    });      
  }

  Create(account: Account) : Promise<Account> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.post(`${this.httpClientUtil.GetAccountBaseAddress()}Account`, account, {headers: headers}).toPromise()
        .then(res => res as Account);
    });      
  }

  Update(id: string, account: Account) : Promise<Account> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.put(`${this.httpClientUtil.GetAccountBaseAddress()}Account/${id}`, account, {headers: headers}).toPromise()
        .then(res => res as Account);
    });      
  }
}
