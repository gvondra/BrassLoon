import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AccountService } from '../services/account.service';
import { Account } from '../models/account';
import { UserInvitationService } from '../services/user-invitation.service';
import { UserInvitation } from '../models/user-invitation';

@Component({
  selector: 'app-user-invitation',
  templateUrl: './user-invitation.component.html',
  styles: [
  ]
})
export class UserInvitationComponent implements OnInit {

  ErrorMessage: string = null;
  Account: Account = null;
  Invitation: UserInvitation = null;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private accountService: AccountService,
    private invitationService: UserInvitationService) { }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(params => {
      this.Account = null;
      this.ErrorMessage = null;
      this.Invitation = null;
      if (params["accountId"] && params["id"]) {
        this.accountService.Get(params["accountId"])
        .then(account => this.Account = account)
        .catch(err => {
          console.error(err);
          this.ErrorMessage = err.message || "Unexpected Error"
        }); 
        this.Load(params["id"]);
      }
      else {
        this.router.navigate(['/']);
      }
    });
  }

  private Load(id: string) {
    this.invitationService.Get(id)
    .then(invitation => {
      this.Invitation = invitation;
    })
    .catch(err => {
      console.error(err);
      this.ErrorMessage = err.message || "Unexpected Error"
    }); 
  }

  FormatDate(value: string) : string {
    let result : string = value;
    if (result && result != '') {
      let dt: Date = new Date(Date.parse(value));
      result = dt.toLocaleDateString();
    }
    return result;
  }

  Cancel() {
    this.Invitation.Status = -1;
    this.invitationService.Update(this.Invitation.UserInvitationId, this.Invitation)
    .then(res => this.router.navigate(['/a', this.Account.AccountId]))
    .catch(err => {
      console.error(err);
      this.ErrorMessage = err.message || "Unexpected Error"
    }); 
  }
}
