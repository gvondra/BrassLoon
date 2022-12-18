import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { HttpClientUtilService } from '../http-client-util.service';
import { TokenService } from './token.service';
import { SigningKey } from '../models/signing-key';

@Injectable({
  providedIn: 'root'
})
export class SigningKeyService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private httpClient: HttpClient,
    private tokenService: TokenService) { }
  
    GetByDomainId(domainId: string) : Promise<SigningKey[]> {
      return this.httpClientUtil.CreateAuthHeader(this.tokenService)
      .then(headers => {
          return this.httpClient.get<SigningKey[]>(`${this.httpClientUtil.GetAuthorizationBaseAddress()}SigningKey/${domainId}`, {headers: headers}).toPromise();
      });    
    }
  
    Create(signingKey: SigningKey): Promise<SigningKey> {
      return this.httpClientUtil.CreateAuthHeader(this.tokenService)
      .then(headers => {
          return this.httpClient.post<SigningKey>(`${this.httpClientUtil.GetAuthorizationBaseAddress()}SigningKey/${signingKey.DomainId}`, signingKey, {headers: headers}).toPromise();
      });    
    }
  
    Update(signingKey: SigningKey): Promise<SigningKey> {
      return this.httpClientUtil.CreateAuthHeader(this.tokenService)
      .then(headers => {
          return this.httpClient.put<SigningKey>(`${this.httpClientUtil.GetAuthorizationBaseAddress()}SigningKey/${signingKey.DomainId}/${signingKey.SigningKeyId}`, signingKey, {headers: headers}).toPromise();
      });
    }
}
