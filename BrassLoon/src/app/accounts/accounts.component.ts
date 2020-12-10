import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { faPlus } from '@fortawesome/free-solid-svg-icons';
import { AccountService } from '../services/account.service';
import { Account } from '../models/account';
import { OidcSecurityService } from 'angular-auth-oidc-client';

@Component({
  selector: 'app-accounts',
  templateUrl: './accounts.component.html',
  styles: [
  ]
})
export class AccountsComponent implements OnInit {

  ErrorMessage: string = null;
  Accounts: Array<Account> = null;
  faPlus = faPlus;

  constructor(private accountService: AccountService,
    private router: Router,
    private oidcSecurityService: OidcSecurityService) { }

  ngOnInit(): void {
    this.oidcSecurityService.isAuthenticated$.subscribe(isAuthenticated => {
      this.Accounts = null;
      if (isAuthenticated) {
        this.LoadAccounts();
      }
    });    
  }

  private LoadAccounts() {
    this.accountService.GetAll()
    .then(res => this.Accounts = res)
    .catch(err => {
      console.error(err);
      this.ErrorMessage = err.message || "Unexpected Error"
    });    
  }

  AddAccount() {
    this.router.navigate(["/a"]);
  }

}
