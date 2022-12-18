import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Domain } from '../models/domain';
import { DomainService } from '../services/domain.service';
import { DomainUserService } from '../services/domain-user.service';
import { DomainUser } from '../models/domain-user';

@Component({
  selector: 'app-domain-users',
  templateUrl: './domain-users.component.html',
  styles: [
  ]
})
export class DomainUsersComponent implements OnInit {

  ShowBusy: boolean = false;
  ErrorMessage: string = null;
  Domain: Domain = null;
  Users: Array<DomainUser> = null;
  SelectedUser: DomainUser = null;
  SearchText: string = null;

  constructor(private activatedRoute: ActivatedRoute,
    private domainService: DomainService,
    private userService: DomainUserService) { }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(params => {
      this.InitializeMemberVariables();
      if (params["domainId"]) { 
        this.ShowBusy = true;
        this.domainService.Get(params["domainId"])     
        .then(domain => {
          this.Domain = domain;
          this.ShowBusy = false;
        })
        .catch(err => {
          console.error(err);
          this.ErrorMessage = err.message || "Unexpected Error"
        }); 
      }
    });
  }

  private LoadUsers(domainId: string, emailAddress: string) {
    this.userService.Search(domainId, emailAddress)
    .then(users => {
      this.Users = users;
      if (users && users.length > 0) {
        this.SelectedUser = users[0];
      }
    })
    .catch(err => {
      console.error(err);
      this.ErrorMessage = err.message || "Unexpected Error"
    });
  }

  private InitializeMemberVariables() {
    this.ErrorMessage = null;
    this.Domain = null;
    this.Users = null;
    this.SelectedUser = null;
    this.SearchText = null;
    this.ShowBusy = false;
  }

  private FindUserIndex(users: Array<DomainUser>, user: DomainUser) : number {
    let i : number = 0;
    let found: boolean = false;
    while (!found && i < users.length) {
      if (users[i].UserId === user.UserId) {
        found = true;
      }
      else {
        i += 1;
      }
    }
    if (found) { return i; }
    else return -1;
  }

  OnSaveUser(user: DomainUser) { 
    const i: number = this.FindUserIndex(this.Users, user);
    if (i >= 0) { this.Users[i] = user; }
  }

  Find() {
    this.ErrorMessage = null;
    this.Users = null;
    if (this.SearchText && this.SearchText !== ""){
      this.LoadUsers(this.Domain.DomainId, this.SearchText);
    }
  }

}
