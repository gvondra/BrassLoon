import { Component, OnInit } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { HttpClientUtilService } from '../http-client-util.service';
import { TokenService } from '../services/token.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styles: [
  ]
})
export class HomeComponent implements OnInit {

  ShowSysAdmin: boolean = false;
  ShowActAdmin: boolean = false;

  constructor(private oidcSecurityService: OidcSecurityService,
    private tokenService: TokenService,
    private httpClientUtil: HttpClientUtilService) { }

  ngOnInit(): void {
    this.ShowSysAdmin = false;
    this.ShowActAdmin = false;
    this.oidcSecurityService.isAuthenticated$.subscribe(isAuthenticated => {
      this.ShowSysAdmin = false;
      if (isAuthenticated.isAuthenticated) {
        this.httpClientUtil.GetRoles(this.tokenService)
        .then(role => {
          if (role && role.length > 0 && role.some(r => r === 'sysadmin')) {
            this.ShowSysAdmin = true;
          }
          if (role && role.length > 0 && role.some(r => r === 'actadmin')) {
            this.ShowActAdmin = true;
          }
        })
        .catch(err => {
          console.error(err);
        });   
      }
    });
  }

}
