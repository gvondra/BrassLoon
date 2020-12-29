import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { UserInvitationService } from '../services/user-invitation.service';
import { UserInvitation } from '../models/user-invitation';
import { HttpClientUtilService } from '../http-client-util.service';

@Component({
  selector: 'app-accept-invitation',
  templateUrl: './accept-invitation.component.html',
  styles: [
  ]
})
export class AcceptInvitationComponent implements OnInit {

  ErrorMessage: string = null;
  Message: string = null;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private invitationService: UserInvitationService,
    private httpClientUtil: HttpClientUtilService) { }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(params => {
      this.Message = "Updating";
      this.ErrorMessage = null;
      if (params["accountId"] && params["id"]) {
        this.invitationService.Get(params["id"])
        .then(invitation => {
          if (invitation) {
            if (invitation.Status == -1) {
              this.Message = "Invitation Cancelled"
            }
            else if (invitation.Status > 0) {
              this.router.navigate(['/a', params["accountId"]])
            }
            else if (new Date(Date.parse(invitation.ExpirationTimestamp)) <= new Date()) {
              this.Message = "Invitation Expired"
            }
            else {
              this.Update(invitation);
            }
          }  
          else {
            this.Message = "Error looking up invitation"
          }        
        })
        .catch(err => {
          console.error(err);
          this.ErrorMessage = err.message || "Unexpected Error"
        }); 
      }
    });
  }

  private Update(invitation: UserInvitation) {
    invitation.Status = 255;
    this.invitationService.Update(invitation.UserInvitationId, invitation)
    .then(res => {
      this.httpClientUtil.DropCache();
      this.router.navigate(['/']);
    })
    .catch(err => {
      console.error(err);
      this.ErrorMessage = err.message || "Unexpected Error"
    }); 
  }

}
