import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { UserService } from '../services/user.service';
import { User } from '../models/user';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styles: [
  ]
})
export class UserComponent implements OnInit {

  ErrorMessage: string = null;
  UserMessage: string = null;
  User: User = null;
  IsAccountAdmin: boolean = false;
  IsSystemAdmin: boolean = false;
  ShowBusy: boolean = false;
  
  constructor(private activatedRoute: ActivatedRoute,
    private userService: UserService) { }

  ngOnInit(): void {
    this.ResetMemberVariables();
    this.activatedRoute.params.subscribe(params => {
      this.ResetMemberVariables();
      const userId = params["userId"];
      if (userId && userId != '') {
        this.ShowBusy = true;
        this.userService.Get(userId)
        .then(user => this.User = user)
        .catch(err => {
          console.error(err);
          this.ErrorMessage = err.message || "Unexpected Error"
        })
        .finally(() => this.ShowBusy = false)
        ;
        this.userService.GetRoles(userId)
        .then(roles => this.SetRoles(roles))
        .catch(err => {
          console.error(err);
          this.ErrorMessage = err.message || "Unexpected Error"
        })
      }
      else {
        this.ErrorMessage = "No user"
      }
    });
  }

  private SetRoles(roles: string[]) : void {
    this.IsAccountAdmin = false;
    this.IsSystemAdmin = false;
    for (let role of roles) {
      if (role === "actadmin") {
        this.IsAccountAdmin = true;
      }
      if (role === "sysadmin") {
        this.IsSystemAdmin = true;
      }
    }
  }

  private ResetMemberVariables() : void {
    this.ErrorMessage = null;
    this.UserMessage = null;
    this.User = null;
    this.ShowBusy = false;
    this.IsAccountAdmin = false;
    this.IsSystemAdmin = false;
  }

  Save() {
    this.ErrorMessage = null;
    this.UserMessage = null;
    const roles = [];
    if (this.IsAccountAdmin) {
      roles.push("actadmin")
    }
    if (this.IsSystemAdmin) {
      roles.push("sysadmin")
    }
    this.ShowBusy = true;
    this.userService.SaveRoles(this.User.UserId, roles)
    .then(() => this.UserMessage = "Roles Saved")
    .catch(err => {
      console.error(err);
      if (err.status && err.status == 401) {
        this.ErrorMessage = err.statusText || "Unauthorized"
      }
      else {
        this.ErrorMessage = err.message || "Unexpected Error"
      }
    })
    .finally(() => this.ShowBusy = false);
  }
}
