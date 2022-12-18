import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { HttpClientUtilService } from '../http-client-util.service';
import { TokenService } from './token.service';
import { DomainUser } from '../models/domain-user';

@Injectable({
  providedIn: 'root'
})
export class DomainUserService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private httpClient: HttpClient,
    private tokenService: TokenService) { }

    Get(domainId: string, id: string) : Promise<DomainUser> {
      return this.httpClientUtil.CreateAuthHeader(this.tokenService)
      .then(headers => {
          return this.httpClient.get<DomainUser>(`${this.httpClientUtil.GetAuthorizationBaseAddress()}User/${domainId}/${id}`, {headers: headers}).toPromise();
      });    
    }
  
    Search(domainId: string, emailAddress: string) : Promise<DomainUser[]> {
      let params: HttpParams = new HttpParams();
      if (emailAddress && emailAddress !== "") {
        params = params.append("emailAddress", emailAddress);
      }
      return this.httpClientUtil.CreateAuthHeader(this.tokenService)
      .then(headers => {
          return this.httpClient.get<DomainUser[]>(`${this.httpClientUtil.GetAuthorizationBaseAddress()}User/${domainId}`, {headers: headers, params: params}).toPromise();
      });    
    }
  
    Create(user: DomainUser): Promise<DomainUser> {
      return this.httpClientUtil.CreateAuthHeader(this.tokenService)
      .then(headers => {
          return this.httpClient.post(`${this.httpClientUtil.GetAuthorizationBaseAddress()}User/${user.DomainId}`, user, {headers: headers}).toPromise()
          .then(res => res as DomainUser);
      });    
    }
  
    Update(user: DomainUser): Promise<DomainUser> {
      return this.httpClientUtil.CreateAuthHeader(this.tokenService)
      .then(headers => {
          return this.httpClient.put(`${this.httpClientUtil.GetAuthorizationBaseAddress()}User/${user.DomainId}/${user.UserId}`, user, {headers: headers}).toPromise()
          .then(res => res as DomainUser);
      });
    }
}
