import { Component, OnInit } from '@angular/core';
import { Account } from '../models/account';
import { Router, ActivatedRoute } from '@angular/router';
import { AccountService } from '../services/account.service';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styles: [
  ]
})
export class AccountComponent implements OnInit {

  ErrorMessage: string = null;
  Account: Account = null;
  AccountId: string = null;
  IsNew: boolean = false;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private accountService: AccountService) { }

  ngOnInit(): void {        
    this.activatedRoute.params.subscribe(params => {
      this.ErrorMessage = null;
      this.Account = null;
      this.AccountId = null;
      if (params["id"]) {
        this.IsNew = false;
        this.AccountId = params["id"];
        this.accountService.Get(params["id"])
        .then(account => this.Account = account)
        .catch(err => {
          console.error(err);
          this.ErrorMessage = err.message || "Unexpected Error"
        });    
      }
      else {
        this.IsNew = true;
        this.Account= new Account();
      }
    });
  }

  Save() {
    if (this.IsNew) {
      this.accountService.Create(this.Account)
      .then(account => this.router.navigate(["/a", account.AccountId]))
      .catch(err => {
        console.error(err);
        this.ErrorMessage = err.message || "Unexpected Error"
      });    
    }
    else {
      this.accountService.Update(this.AccountId, this.Account)
      .then(account => this.Account = account)
      .catch(err => {
        console.error(err);
        this.ErrorMessage = err.message || "Unexpected Error"
      });    
    }
  }
}
