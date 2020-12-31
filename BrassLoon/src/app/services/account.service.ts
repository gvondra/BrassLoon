import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { HttpClientUtilService } from '../http-client-util.service';
import { Account } from '../models/account';
import { TokenService } from './token.service';
import { Domain } from '../models/domain';
import { Client } from '../models/client';
import { User } from '../models/user';

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

  Search(emailAddress: string) : Promise<Array<Account>> {  
    let params: HttpParams = new HttpParams()
    .append("emailAddress", emailAddress);
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.get(`${this.httpClientUtil.GetAccountBaseAddress()}Account`, { headers: headers, params: params }).toPromise()
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
    return this.GetDomainsByDeleted(id, false);
  }

  GetDomainsByDeleted(id: string, deleted: boolean) : Promise<Array<Domain>> {
    let params: HttpParams = new HttpParams()
    .append("deleted", String(deleted));
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.get(`${this.httpClientUtil.GetAccountBaseAddress()}Account/${id}/Domain`, {headers: headers, params: params}).toPromise()
        .then(res => res as Array<Domain>);
    });      
  }

  GetClients(id: string) : Promise<Array<Client>> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.get(`${this.httpClientUtil.GetAccountBaseAddress()}Account/${id}/Client`, {headers: headers}).toPromise()
        .then(res => res as Array<Client>);
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

  UpdateLock(id: string, locked: Boolean) : Promise<Account> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.patch(`${this.httpClientUtil.GetAccountBaseAddress()}Account/${id}/Locked`, { "Locked": String(locked) }, {headers: headers}).toPromise()
        .then(res => res as Account);
    });      
  }

  GetUsers(accountId: string) : Promise<Array<User>> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.get(`${this.httpClientUtil.GetAccountBaseAddress()}Account/${accountId}/User`, {headers: headers}).toPromise()
        .then(res => res as Array<User>);
    });    
  }

  RemoveUser(accountId: string, userId: string) : Promise<Object> {    
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.delete(`${this.httpClientUtil.GetAccountBaseAddress()}Account/${accountId}/User/${userId}`, {headers: headers}).toPromise();
    });  
  }
}
