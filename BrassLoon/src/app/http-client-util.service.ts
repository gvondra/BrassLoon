import { Injectable } from '@angular/core';
import { AppSettingsService } from './app-settings.service';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { TokenService } from './services/token.service';
import jwt_decode from 'jwt-decode';
import { firstValueFrom, Observable, of } from 'rxjs';
import { map, tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class HttpClientUtilService {

  constructor(private appSettings: AppSettingsService,
    private oidcSecurityService: OidcSecurityService,
    private httpClient: HttpClient) { }

  GetAccountBaseAddress() : Promise<string> {
    return this.appSettings.GetSettings()
    .then(appSettings => appSettings.AccountBaseAddress);
  }
  GetLogBaseAddress() : Promise<string> {
    return this.appSettings.GetSettings()
    .then(appSettings => appSettings.LogBaseAddress);
  }
  GetConfigBaseAddress() : Promise<string> {
    return this.appSettings.GetSettings()
    .then(appSettings => appSettings.ConfigBaseAddress);
  }
  GetAuthorizationBaseAddress() : Promise<string> {
    return this.appSettings.GetSettings()
    .then(appSettings => appSettings.AuthoriaztionBaseAddress);
  }
  GetWorkTaskBaseAddress() : Promise<string> {
    return this.appSettings.GetSettings()
    .then(appSettings => appSettings.WorkTaskBaseAddress);
  }

  CreateUserTokenAuthHeader() : Observable<HttpHeaders> {
    return this.oidcSecurityService.getIdToken()
    .pipe(map(tkn => new HttpHeaders({"Authorization": `bearer ${tkn.trim()}`})))
    ;
  }

  CreateAuthHeader(tokenService: TokenService) : Promise<HttpHeaders> {
    return this.GetToken(tokenService)
    .then(tkn => this.CreateAuthorizationHeader(tkn))
    ;
  }

  CreateAuthHeader2(tokenService: TokenService) : Observable<HttpHeaders> {
    return this.GetToken2(tokenService)
    .pipe(
      map(tkn => this.CreateAuthorizationHeader(tkn))      
      );
  }

  GetToken(tokenService: TokenService) : Promise<string> {
    if (this.IsCachedTokenAvailable()) {
      return Promise.resolve(this.GetCachedToken());
    }
    else {
      return tokenService.GetToken()
      .then(tkn => {
        this.TapAccessToken(tkn);
        return tkn;
      });
    }
  }

  GetToken2(tokenService: TokenService) : Observable<string> {
    if (this.IsCachedTokenAvailable()) {
      return of(this.GetCachedToken());
    }
    else {
      return tokenService.GetToken2()
      .pipe(
        tap(tkn => this.TapAccessToken(tkn))
      );
    }
  }

  private TapAccessToken(tkn: string): void {
    sessionStorage.setItem("AccessToken", tkn);
    let expiration: Date = new Date();
    expiration.setMinutes(expiration.getMinutes() + 59);
    sessionStorage.setItem("AccessToknExpiration", expiration.valueOf().toString());
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

  GetRequest<T>(tokenService: TokenService, getAddress: Promise<string>, params: HttpParams | null = null) : Promise<T> {
    return this.CreateAuthHeader(tokenService)
    .then(headers => getAddress
      .then(address => firstValueFrom(this.httpClient.get<T>(address, {headers: headers, params: params}))
      ));
  }

  DeleteRequest<T>(tokenService: TokenService, getAddress: Promise<string>, params: HttpParams | null = null) : Promise<T> {
    return this.CreateAuthHeader(tokenService)
    .then(headers => getAddress
      .then(address => firstValueFrom(this.httpClient.delete<T>(address, {headers: headers, params: params}))
      ));
  }

  PostRequest<T>(tokenService: TokenService, getAddress: Promise<string>, value: any | null = null, params: HttpParams | null = null) : Promise<T> {
    return this.CreateAuthHeader(tokenService)
    .then(headers => getAddress
      .then(address => firstValueFrom(this.httpClient.post<T>(address, value, {headers: headers, params: params}))
      ));
  }

  PutRequest<T>(tokenService: TokenService, getAddress: Promise<string>, value: any | null = null, params: HttpParams | null = null) : Promise<T> {
    return this.CreateAuthHeader(tokenService)
    .then(headers => getAddress
      .then(address => firstValueFrom(this.httpClient.put<T>(address, value, {headers: headers, params: params}))
      ));
  }

  PatchRequest<T>(tokenService: TokenService, getAddress: Promise<string>, value: any | null = null, params: HttpParams | null = null) : Promise<T> {
    return this.CreateAuthHeader(tokenService)
    .then(headers => getAddress
      .then(address => firstValueFrom(this.httpClient.patch<T>(address, value, {headers: headers, params: params}))
      ));
  }
}
