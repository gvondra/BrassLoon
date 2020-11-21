import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { faPlus } from '@fortawesome/free-solid-svg-icons';
import { AccountService } from '../services/account.service';
import { Account } from '../models/account';

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
    private router: Router) { }

  ngOnInit(): void {
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
