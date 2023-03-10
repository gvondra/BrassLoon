import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { HttpClientUtilService } from '../http-client-util.service';
import { User } from '../models/user';
import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private tokenService: TokenService) { }

  Search(emailAddress: string) : Promise<User[]> {     
    let params: HttpParams = new HttpParams()
    if (emailAddress && emailAddress !== '') {      
      params = params.append("emailAddress", emailAddress);
    }
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetAccountBaseAddress()
      .then(baseAddress => `${baseAddress}User`),
      params
    ); 
  }

  Get(userId: string) : Promise<User> {
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetAccountBaseAddress()
      .then(baseAddress => `${baseAddress}User/${userId}`)
    );   
  }

  GetRoles(userId: string) : Promise<string[]> {
    return this.httpClientUtil.GetRequest(this.tokenService,
      this.httpClientUtil.GetAccountBaseAddress()
      .then(baseAddress => `${baseAddress}User/${userId}/Role`)
    );    
  }

  SaveRoles(userId: string, roles: string[]) : Promise<any> {
    return this.httpClientUtil.PutRequest(this.tokenService,
      this.httpClientUtil.GetAccountBaseAddress()
      .then(baseAddress => `${baseAddress}User/${userId}/Role`),
      roles
    );      
  }
}
