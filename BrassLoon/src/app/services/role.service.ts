import { Injectable } from '@angular/core';
import { HttpClientUtilService } from '../http-client-util.service';
import { Role } from '../models/role';
import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root'
})
export class RoleService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private tokenService: TokenService) { }

  GetByDomainId(domainId: string) : Promise<Role[]> {
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetAuthorizationBaseAddress()
      .then(baseAddress => `${baseAddress}Role/${domainId}`)
    ); 
  }

  Create(domainId: string, role: Role): Promise<Role> {
    return this.httpClientUtil.PostRequest(this.tokenService,
      this.httpClientUtil.GetAuthorizationBaseAddress()
      .then(baseAddress => `${baseAddress}Role/${domainId}`),
      role
    );  
  }

  Update(role: Role): Promise<Role> {
    return this.httpClientUtil.PutRequest(this.tokenService,
      this.httpClientUtil.GetAuthorizationBaseAddress()
      .then(baseAddress => `${baseAddress}Role/${role.DomainId}/${role.RoleId}`),
      role
    ); 
  }
}
