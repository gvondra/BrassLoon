import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { HttpClientUtilService } from '../http-client-util.service';
import { TokenService } from './token.service';
import { UserInvitation } from '../models/user-invitation';

@Injectable({
  providedIn: 'root'
})
export class UserInvitationService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private httpClient: HttpClient,
    private tokenService: TokenService) { }

  GetByAccountId(accountId: string) : Promise<Array<UserInvitation>> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.get(`${this.httpClientUtil.GetAccountBaseAddress()}Account/${accountId}/Invitation`, {headers: headers}).toPromise()
        .then(res => res as UserInvitation[]);
    });   
  }

  Get(id: string) : Promise<UserInvitation> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.get(`${this.httpClientUtil.GetAccountBaseAddress()}UserInvitation/${id}`, {headers: headers}).toPromise()
        .then(res => res as UserInvitation);
    }); 
  }

  Create(accountId: string, invitation: UserInvitation) :Promise<UserInvitation> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.post(`${this.httpClientUtil.GetAccountBaseAddress()}Account/${accountId}/Invitation`, invitation, {headers: headers}).toPromise()
        .then(res => res as UserInvitation);
    });   
  }

  Update(id: string, invitation: UserInvitation) : Promise<UserInvitation> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.put(`${this.httpClientUtil.GetAccountBaseAddress()}UserInvitation/${id}`, invitation, {headers: headers}).toPromise()
        .then(res => res as UserInvitation);
    }); 
  }
}
