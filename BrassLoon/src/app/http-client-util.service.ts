import { Injectable } from '@angular/core';
import { AppSettingsService } from './app-settings.service';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { HttpHeaders } from '@angular/common/http';
import { TokenService } from './services/token.service';
import jwt_decode from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class HttpClientUtilService {

  constructor(private appSettings: AppSettingsService,
    private oidcSecurityService: OidcSecurityService) { }

  GetAccountBaseAddress() : string {
    return this.appSettings.GetSettings().AccountBaseAddress;
  }
  GetLogBaseAddress() : string {
    return this.appSettings.GetSettings().LogBaseAddress;
  }

  CreateUserTokenAuthHeader() : HttpHeaders {
    let tkn: string = this.oidcSecurityService.getIdToken().trim();
    return new HttpHeaders({"Authorization": `bearer ${tkn}`});    
  }

  CreateAuthHeader(tokenService: TokenService) : Promise<HttpHeaders> {
    return this.GetToken(tokenService)
    .then(tkn => this.CreateAuthorizationHeader(tkn))
    ;
  }

  GetToken(tokenService: TokenService) : Promise<string> {
    if (this.IsCachedTokenAvailable()) {
      return Promise.resolve(this.GetCachedToken());
    }
    else {
      return tokenService.GetToken()
      .then(tkn => {
        sessionStorage.setItem("AccessToken", tkn);
        let expiration: Date = new Date();
        expiration.setMinutes(expiration.getMinutes() + 59);
        sessionStorage.setItem("AccessToknExpiration", expiration.valueOf().toString());
        return tkn;
      });
    }
  }

  GetRoles(tokenService: TokenService) : Promise<Array<string>> {
    return this.GetToken(tokenService)
    .then(tkn => {
      const decoded : any = jwt_decode(tkn);
      if (decoded && decoded.role && Array.isArray(decoded.role)) 
      {
        return decoded.role;
      }
      else if (decoded && decoded.role) {
        return [ decoded.role ];
      }
      else {
        return [];
      }      
    });
  }

  private CreateAuthorizationHeader(tkn: string) : HttpHeaders {
    return new HttpHeaders({"Authorization": `bearer ${tkn}`})
  }

  private IsCachedTokenAvailable() : boolean {
    const tkn = sessionStorage.getItem("AccessToken");
    const expiration = sessionStorage.getItem("AccessToknExpiration");
    let result: boolean = false;
    if (tkn && expiration) {
      if (Date.now() < Number(expiration)) {
        result = true;
      }
      let dt: Date = new Date(Number(expiration));
    }
    return result;
  }

  private GetCachedToken() : string {
    return sessionStorage.getItem("AccessToken");
  }

  DropCache() : void {
    sessionStorage.removeItem("AccessToken");
    sessionStorage.removeItem("AccessToknExpiration");
  }
}
