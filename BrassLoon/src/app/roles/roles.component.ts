import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Domain } from '../models/domain';
import { DomainService } from '../services/domain.service';
import { RoleService } from '../services/role.service';
import { Role } from '../models/role';

@Component({
  selector: 'app-roles',
  templateUrl: './roles.component.html',
  styles: [
  ]
})
export class RolesComponent implements OnInit {

  ShowBusy: boolean = false;
  ErrorMessage: string = null;
  Domain: Domain = null;
  Roles: Array<Role> = null;
  SelectedRole: Role = null;

  constructor(private activatedRoute: ActivatedRoute,
    private domainService: DomainService,
    private roleService: RoleService) { }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(params => {
      this.InitializeMemberVariables();
      if (params["domainId"]) { 
        this.ShowBusy = true;
        this.domainService.Get(params["domainId"])     
        .then(domain => {
          this.Domain = domain;
          this.LoadRoles(domain.DomainId);
        })
        .catch(err => {
          console.error(err);
          this.ErrorMessage = err.message || "Unexpected Error"
        }); 
      }
    });
  }

  private LoadRoles(domainId: string) {
    this.roleService.GetByDomainId(domainId)
    .then(roles => {
      this.Roles = roles;
      if (roles && roles.length > 0) {
        this.SelectedRole = roles[0];
      }
      if (this.SelectedRole == null) {
        this.SelectedRole = this.NewRole();
      }
      this.ShowBusy = false;
    })
  }

  private NewRole() : Role {
    const role = new Role();
    role.Name = "New Role";
    role.PolicyName = "Set:Policy"
    role.DomainId = this.Domain.DomainId;
    return role;
  }

  OnNewRole() {
    this.SelectedRole = this.NewRole();
  }

  private InitializeMemberVariables() {
    this.ErrorMessage = null;
    this.Domain = null;
    this.Roles = null;
    this.SelectedRole = null;
    this.ShowBusy = false;
  }

  private FindRoleIndex(roles: Array<Role>, role: Role) : number {
    let i : number = 0;
    let found: boolean = false;
    while (!found && i < roles.length) {
      if (roles[i].RoleId === role.RoleId) {
        found = true;
      }
      else {
        i += 1;
      }
    }
    if (found) { return i; }
    else return -1;
  }

  OnSaveRole(role: Role) { 
    const i: number = this.FindRoleIndex(this.Roles, role);
    if (i >= 0) { this.Roles[i] = role; }
    else { this.Roles.push(role); }
  }

}
