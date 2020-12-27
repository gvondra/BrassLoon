import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Account } from '../models/account';
import { AccountService } from '../services/account.service';

@Component({
  selector: 'app-account-search',
  templateUrl: './account-search.component.html',
  styles: [
  ]
})
export class AccountSearchComponent implements OnInit {

  ErrorMessage: string = null;
  Address: string = null;
  Accounts: Array<Account> = null;
  ShowBusy: boolean = false;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private accountService: AccountService) { }

  ngOnInit(): void {
    this.ErrorMessage = null;
    this.Address = null;
    this.Accounts = null;
    this.ShowBusy = false;
    this.activatedRoute.queryParams.subscribe(params => {
      this.ErrorMessage = null;
      this.Address = null;      
      this.Accounts = null;
      this.ShowBusy = false;
      if (params["address"] && params["address"] != '') {
        this.ShowBusy = true;
        this.Address = params["address"];
        this.accountService.Search(params["address"])
        .then(accounts => this.Accounts = accounts)        
        .catch(err => {
          console.error(err);
          this.ErrorMessage = err.message || "Unexpected Error"
        })
        .finally(() => this.ShowBusy = false)
        ;   
      }
    });
  }

  Load() {
    if (this.Address && this.Address != '') {
      this.router.navigate(['/aa'], { queryParams: { 'address': this.Address } });
    }    
  }

}
