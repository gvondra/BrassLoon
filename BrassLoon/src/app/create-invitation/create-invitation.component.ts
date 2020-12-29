import { Component, OnInit } from '@angular/core';
import { Location, LocationStrategy, PathLocationStrategy } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { AccountService } from '../services/account.service';
import { Account } from '../models/account';
import { UserInvitationService } from '../services/user-invitation.service';
import { UserInvitation } from '../models/user-invitation';

@Component({
  selector: 'app-create-invitation',
  templateUrl: './create-invitation.component.html',
  providers: [Location, {provide: LocationStrategy, useClass: PathLocationStrategy}],
  styles: [
  ]
})
export class CreateInvitationComponent implements OnInit {

  ErrorMessage: string = null;
  Account: Account = null;
  Invitation: UserInvitation = null;
  InvitationAddress: string = null;
  SampleInvitation: string = null;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private accountService: AccountService,
    private invitationService: UserInvitationService,
    private location: Location) { }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(params => {
      this.Account = null;
      this.ErrorMessage = null;
      this.Invitation = null;
      this.InvitationAddress = null;
      if (params["accountId"]) {
        this.accountService.Get(params["accountId"])
        .then(account => {
          this.Account = account;
          this.InitializeUserInvitation();
        })
        .catch(err => {
          console.error(err);
          this.ErrorMessage = err.message || "Unexpected Error"
        }); 
      }
      else {
        this.router.navigate(['/']);
      }
    });
  }

  private InitializeUserInvitation() { 
    let expiration: Date = new Date();
    expiration.setDate(expiration.getDate() + 7);
    let invitation: UserInvitation = new UserInvitation();
    invitation.EmailAddress = '';
    invitation.ExpirationTimestamp = expiration.toISOString().split('T')[0];
    this.Invitation = invitation;
  }

  Save() {
    let time: string = new Date().toISOString().split('T')[1];
    let expiration: Date = new Date(Date.parse(this.Invitation.ExpirationTimestamp + 'T' + time));
    let invitation: UserInvitation = {
      "EmailAddress": this.Invitation.EmailAddress,
      "ExpirationTimestamp": expiration.toISOString(),
      "Status": 0,
      "CreateTimestamp": null,
      "UpdateTimestamp": null,
      "UserInvitationId": null
    };
    this.invitationService.Create(this.Account.AccountId, invitation)
    .then(res => {
      this.Invitation = res;
      this.InvitationAddress = location.origin + this.location.prepareExternalUrl(this.router.createUrlTree(['/a', this.Account.AccountId, 'Invitation', res.UserInvitationId, 'Accept']).toString());
      this.SetSample();
    })
    .catch(err => {
      console.error(err);
      this.ErrorMessage = err.message || "Unexpected Error"
    }); 
  }

  private SetSample() {
    this.SampleInvitation = `Here is your invitation to ${this.Account.Name} on Brass Loon\nTo accept this invitation, please go to ${this.InvitationAddress}`;
  }

  CreateMail() : string {  
    let subject: string = `Brass Loon ${this.Account.Name} Account Invitation`;
    let result: string = `mailto:${this.Invitation.EmailAddress}?subject=${encodeURIComponent(subject)}&body=${encodeURIComponent(this.SampleInvitation)}`;
    return result;
  }
}
