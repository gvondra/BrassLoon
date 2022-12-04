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
}
