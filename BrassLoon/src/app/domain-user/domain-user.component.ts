import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { RoleService } from '../services/role.service';
import { Role } from '../models/role';
import { DomainUserService } from '../services/domain-user.service';
import { DomainUser } from '../models/domain-user';

@Component({
  selector: 'app-domain-user',
  templateUrl: './domain-user.component.html',
  styles: [
  ]
})
export class DomainUserComponent implements OnInit {

  private _user: DomainUser = null;

  get User(): DomainUser { return this._user; }
  @Input() set User(value: DomainUser) { 
    this.ErrorMessage = null;
    this.NotificationMessage = null;
    this.ShowBusy = false;
    this.Roles = null;
    this._user = value; 
    if (value) {
      this.LoadRoles(value.DomainId);
    }
  }

  @Output() OnSave = new EventEmitter<DomainUser>();

  LabelColumnClass: string = "col-md-2";
  InputColumnClass: string = "col-md-8";
  ErrorMessage: string = null;
  NotificationMessage: string = null;
  ShowBusy: boolean = false;
  Roles: Array<Role> = null;

  constructor(private userService: DomainUserService,
    private roleService: RoleService) { }

  ngOnInit(): void {
    this.ErrorMessage = null;
    this.NotificationMessage = null;
    this.ShowBusy = false;
  }

  private LoadRoles(domainId: string) {
    this.Roles = null;
    this.roleService.GetByDomainId(domainId)
    .then(roles => this.Roles = roles)
    .catch(err => this.CatchWebAPIError(err))
    ;
  }

  private FindUserRoleIndex(user: DomainUser, role: Role): number {
    let i = 0;
    let found = false;    
    if (user && user.Roles) {
      while (!found && i < user.Roles.length) {
        if (user.Roles[i].PolicyName === role.PolicyName) {
          found = true;
        }
        else {
          i += 1;
        }
      }
    }
    if (!found) {
      i = -1;
    }
    return i;
  }

  IsActiveRole(role: Role): boolean {
    const i = this.FindUserRoleIndex(this._user, role);
    return (i >= 0);
  }

  OnChangeActiveRole(role: Role, event) {    
    const user: DomainUser = this._user;
    const i = this.FindUserRoleIndex(user, role);
    if (event.target.checked && i < 0) {
      if (!user.Roles) {
        user.Roles = [];
      }
      user.Roles.push({ "PolicyName": role.PolicyName,  "Name": role.Name});
    }
    if (!event.target.checked && i >= 0) {
      user.Roles.splice(i, 1);
    }
  }

  Save() {
    this.ErrorMessage = null;
    this.NotificationMessage = null;
    this.ShowBusy = true;
    if (this.User.UserId) {
      this.userService.Update(this.User)
      .then(c => {
        this.OnSave.emit(c);
        this.NotificationMessage = "Save complete";
      })
      .catch(err => this.CatchWebAPIError(err))
      .finally(() => this.ShowBusy = false)
      ;
    }
  }

  private CatchWebAPIError(err) {
    console.error(err);
    if (err.error && typeof err.error === "string") {
      this.ErrorMessage = err.error  
    }
    else {
      this.ErrorMessage = err.message || "Unexpected Error"
    }
    
  }
}
