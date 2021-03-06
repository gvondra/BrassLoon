import { Component, OnInit } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';

@Component({
  selector: 'app-auto-login',
  templateUrl: './auto-login.component.html',
  styles: [
  ]
})
export class AutoLoginComponent implements OnInit {

  constructor(public oidcSecurityService: OidcSecurityService) {}

  ngOnInit(): void {
    this.oidcSecurityService.checkAuth().subscribe(() => this.oidcSecurityService.authorize());
  }

}
