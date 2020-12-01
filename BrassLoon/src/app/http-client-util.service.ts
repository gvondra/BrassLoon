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
    if (this.IsCachedTokenAvailable()) {
      return Promise.resolve(this.CreateHeader(this.GetCachedToken()));
    }
    else {
      return tokenService.GetToken()
      .then(tkn => {
        sessionStorage.setItem("AccessToken", tkn);
        let expiration: Date = new Date();
        expiration.setMinutes(expiration.getMinutes() + 59);
        sessionStorage.setItem("AccessToknExpiration", expiration.valueOf().toString());
        return this.CreateHeader(tkn);
      });
    }
  }

  private CreateHeader(tkn: string) : HttpHeaders {
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
}
