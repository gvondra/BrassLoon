import { Injectable } from '@angular/core';
import { HttpClientUtilService } from '../http-client-util.service';
import { TokenService } from './token.service';
import { SigningKey } from '../models/signing-key';

@Injectable({
  providedIn: 'root'
})
export class SigningKeyService {

  constructor(private httpClientUtil: HttpClientUtilService,
    private tokenService: TokenService) { }
  
    GetByDomainId(domainId: string) : Promise<SigningKey[]> {
      return this.httpClientUtil.GetRequest(this.tokenService,
        this.httpClientUtil.GetAuthorizationBaseAddress()
        .then(baseAddress => `${baseAddress}SigningKey/${domainId}`)
      ); 
    }
  
    Create(signingKey: SigningKey): Promise<SigningKey> {
      return this.httpClientUtil.PostRequest(this.tokenService,
        this.httpClientUtil.GetAuthorizationBaseAddress()
        .then(baseAddress => `${baseAddress}SigningKey/${signingKey.DomainId}`),
        signingKey
      ); 
    }
  
    Update(signingKey: SigningKey): Promise<SigningKey> {
      return this.httpClientUtil.PutRequest(this.tokenService,
        this.httpClientUtil.GetAuthorizationBaseAddress()
        .then(baseAddress => `${baseAddress}SigningKey/${signingKey.DomainId}/${signingKey.SigningKeyId}`),
        signingKey
      ); 
    }
}
