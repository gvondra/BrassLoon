import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { HttpClientUtilService } from '../http-client-util.service';
import { Account } from '../models/account';
import { Domain } from '../models/domain';
import { Client } from '../models/client';
import { User } from '../models/user';
import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private tokenService: TokenService) { }

  GetAll() : Promise<Array<Account>> { 
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetAccountBaseAddress()
      .then(baseAddress => `${baseAddress}Account`)
    );  
  }

  Search(emailAddress: string) : Promise<Array<Account>> {  
    let params: HttpParams = new HttpParams()
    .append("emailAddress", emailAddress);
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetAccountBaseAddress()
      .then(baseAddress => `${baseAddress}Account`),
      params
    );   
  }

  Get(id: string) : Promise<Account> {
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetAccountBaseAddress()
      .then(baseAddress => `${baseAddress}Account/${id}`)
    );     
  }

  GetDomains(id: string) : Promise<Array<Domain>> {
    return this.GetDomainsByDeleted(id, false);
  }

  GetDomainsByDeleted(id: string, deleted: boolean) : Promise<Array<Domain>> {
    let params: HttpParams = new HttpParams()
    .append("deleted", String(deleted));
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetAccountBaseAddress()
      .then(baseAddress => `${baseAddress}Account/${id}/Domain`),
      params
    );      
  }

  GetClients(id: string) : Promise<Array<Client>> {
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetAccountBaseAddress()
      .then(baseAddress => `${baseAddress}Account/${id}/Client`)
    );   
  }

  Create(account: Account) : Promise<Account> {
    return this.httpClientUtil.PostRequest(this.tokenService,
      this.httpClientUtil.GetAccountBaseAddress()
      .then(baseAddress => `${baseAddress}Account`),
      account
    );   
  }

  Update(id: string, account: Account) : Promise<Account> {
    return this.httpClientUtil.PutRequest(this.tokenService,
      this.httpClientUtil.GetAccountBaseAddress()
      .then(baseAddress => `${baseAddress}Account/${id}`),
      account
    );    
  }

  UpdateLock(id: string, locked: Boolean) : Promise<Account> {
    return this.httpClientUtil.PatchRequest(this.tokenService,
      this.httpClientUtil.GetAccountBaseAddress()
      .then(baseAddress => `${baseAddress}Account/${id}/Locked`),
      { "Locked": String(locked) }
    ); 
  }

  GetUsers(accountId: string) : Promise<Array<User>> {
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetAccountBaseAddress()
      .then(baseAddress => `${baseAddress}Account/${accountId}/User`)
    );   
  }

  RemoveUser(accountId: string, userId: string) : Promise<Object> { 
    return this.httpClientUtil.DeleteRequest(this.tokenService,
      this.httpClientUtil.GetAccountBaseAddress()
      .then(baseAddress => `${baseAddress}Account/${accountId}/User/${userId}`)
    );  
  }
}
