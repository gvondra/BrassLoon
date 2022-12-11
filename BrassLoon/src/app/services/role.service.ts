import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { HttpClientUtilService } from '../http-client-util.service';
import { TokenService } from './token.service';
import { Role } from '../models/role';

@Injectable({
  providedIn: 'root'
})
export class RoleService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private httpClient: HttpClient,
    private tokenService: TokenService) { }

  GetByDomainId(domainId: string) : Promise<Role[]> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.get(`${this.httpClientUtil.GetAuthorizationBaseAddress()}Role/${domainId}`, {headers: headers}).toPromise()
        .then(res => res as Array<Role>);
    });    
  }

  Create(domainId: string, role: Role): Promise<Role> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.post(`${this.httpClientUtil.GetAuthorizationBaseAddress()}Role/${domainId}`, role, {headers: headers}).toPromise()
        .then(res => res as Role);
    });    
  }

  Update(role: Role): Promise<Role> {
    return this.httpClientUtil.CreateAuthHeader(this.tokenService)
    .then(headers => {
        return this.httpClient.put(`${this.httpClientUtil.GetAuthorizationBaseAddress()}Role/${role.DomainId}/${role.RoleId}`, role, {headers: headers}).toPromise()
        .then(res => res as Role);
    });
  }
}
