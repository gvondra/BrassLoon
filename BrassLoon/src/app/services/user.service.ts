import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { HttpClientUtilService } from '../http-client-util.service';
import { TokenService } from './token.service';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private httpClient: HttpClient,
    private tokenService: TokenService) { }

  Search(emailAddress: string) : Promise<User[]> {     
    let params: HttpParams = new HttpParams()
    if (emailAddress && emailAddress !== '') {      
      params = params.append("emailAddress", emailAddress);
    }
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.get(`${this.httpClientUtil.GetAccountBaseAddress()}User`, { headers: headers, params: params }).toPromise()
        .then(res => res as Array<User>);
    });      
  }

  Get(userId: string) : Promise<User> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.get(`${this.httpClientUtil.GetAccountBaseAddress()}User/${userId}`, { headers: headers }).toPromise()
        .then(res => res as User);
    });      
  }

  GetRoles(userId: string) : Promise<string[]> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.get(`${this.httpClientUtil.GetAccountBaseAddress()}User/${userId}/Role`, { headers: headers }).toPromise()
        .then(res => res as Array<string>);
    });      
  }

  SaveRoles(userId: string, roles: string[]) : Promise<any> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.put(`${this.httpClientUtil.GetAccountBaseAddress()}User/${userId}/Role`, roles, { headers: headers }).toPromise();
    });      
  }
}
