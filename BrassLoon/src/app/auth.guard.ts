import { Injectable } from '@angular/core';
import { map, flatMap } from 'rxjs/operators';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { Observable } from 'rxjs';
import { HttpClientUtilService } from './http-client-util.service';
import { TokenService } from './services/token.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

constructor (private oidcSecurityService: OidcSecurityService,
  private tokenService: TokenService,
  private httpClientUtil: HttpClientUtilService) {}

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
      return this.oidcSecurityService.isAuthenticated$.pipe(
        flatMap(isAuthenticated => {
          if (isAuthenticated && next && next.data && next.data.Role && next.data.Role != "") {
            return this.RoleCheck(next.data.Role);
          }
          else {
            return Promise.resolve(isAuthenticated);
          }          
        })
      );
  }

  private RoleCheck(expectedRole: string) : Promise<boolean> {
    return this.httpClientUtil.GetRoles(this.tokenService)
      .then(roles => {
        if (roles && roles.length > 0) {
          return roles.some(r => r === expectedRole);
        }
        else {
          return false;
        }
      })
  }
  
}
