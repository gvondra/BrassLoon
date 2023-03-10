import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { HttpClientUtilService } from '../http-client-util.service';
import { DomainUser } from '../models/domain-user';
import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root'
})
export class DomainUserService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private tokenService: TokenService) { }

    Get(domainId: string, id: string) : Promise<DomainUser> {
      return this.httpClientUtil.GetRequest(this.tokenService,
        this.httpClientUtil.GetAuthorizationBaseAddress()
        .then(baseAddress => `${baseAddress}User/${domainId}/${id}`)
      );   
    }
  
    Search(domainId: string, emailAddress: string) : Promise<DomainUser[]> {
      let params: HttpParams = new HttpParams();
      if (emailAddress && emailAddress !== "") {
        params = params.append("emailAddress", emailAddress);
      }
      return this.httpClientUtil.GetRequest(this.tokenService,
        this.httpClientUtil.GetAuthorizationBaseAddress()
        .then(baseAddress => `${baseAddress}User/${domainId}`),
        params
      );   
    }
  
    Update(user: DomainUser): Promise<DomainUser> {
      return this.httpClientUtil.PutRequest(this.tokenService,
        this.httpClientUtil.GetAuthorizationBaseAddress()
        .then(baseAddress => `${baseAddress}User/${user.DomainId}/${user.UserId}`),
        user
      ); 
    }
}
