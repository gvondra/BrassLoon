import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { RoleService } from '../services/role.service';
import { Role } from '../models/role';
@Component({
  selector: 'app-role',
  templateUrl: './role.component.html',
  styles: [
  ]
})
export class RoleComponent implements OnInit {

  private _role: Role = null;

  get Role() : Role { return this._role; }
  @Input() set Role(value: Role) { 
    this._role = value; 
    this.ErrorMessage = null;
    this.NotificationMessage = null;
  }

  @Output() OnSave = new EventEmitter<Role>();

  LabelColumnClass: string = "col-md-2";
  InputColumnClass: string = "col-md-4";
  ErrorMessage: string = null;
  NotificationMessage: string = null;

  constructor(private roleService: RoleService) { }

  ngOnInit(): void {
    this.ErrorMessage = null;
    this.NotificationMessage = null;
  }

  Save() {
    this.ErrorMessage = null;
    this.NotificationMessage = null;
    if (this.Role) {
      if (this.Role.RoleId && this.Role.RoleId !== "") {
        this.roleService.Update(this.Role)
        .then(role => this.AfterSave(role))
        .catch(err => this.CatchWebAPIError(err)); 
      }
      else {
        this.roleService.Create(this.Role.DomainId, this.Role)
        .then(role => this.AfterSave(role))
        .catch(err => this.CatchWebAPIError(err)); 
      }
    }    
  }

  private CatchWebAPIError(err) {
    console.error(err);
    this.ErrorMessage = err.error || err.message || "Unexpected Error"
  }

  private AfterSave(role: Role) {
    this.NotificationMessage = "Role saved"; 
    this._role = role;
    this.OnSave.emit(role);
  }

  IsPolicyReadOnly() {
    if (this.Role.RoleId && this.Role.RoleId != null && this.Role.RoleId !== "") {
      return true;
    }
    else {
      return null;
    }
  }
}
