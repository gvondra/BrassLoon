import { Injectable } from '@angular/core';
import { AppSettingsService } from './app-settings.service';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { HttpHeaders } from '@angular/common/http';
import { TokenService } from './services/token.service';

@Injectable({
  providedIn: 'root'
})
export class HttpClientUtilService {

  constructor(private appSettings: AppSettingsService,
    private oidcSecurityService: OidcSecurityService) { }

  GetAccountBaseAddress() : string {
    return this.appSettings.GetSettings().AccountBaseAddress;
  }

  CreateUserTokenAuthHeader() : HttpHeaders {
    let tkn: string = this.oidcSecurityService.getIdToken().trim();
    return new HttpHeaders({"Authorization": `bearer ${tkn}`});    
  }

  CreateAuthHeader(tokenService: TokenService) : Promise<HttpHeaders> {
    return tokenService.GetToken()
    .then(tkn => new HttpHeaders({"Authorization": `bearer ${tkn}`}))
    ;
  }
}
