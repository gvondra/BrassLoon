import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../services/user.service';
import { User } from '../models/user';

@Component({
  selector: 'app-user-search',
  templateUrl: './user-search.component.html',
  styles: [
  ]
})
export class UserSearchComponent implements OnInit {

  ErrorMessage: string = null;
  Address: string = null;
  Users: Array<User> = null;
  ShowBusy: boolean = false;

  constructor(private router: Router,
    private activatedRoute: ActivatedRoute,
    private userService: UserService) { }

  ngOnInit(): void {
    this.ResetMemberVariables();
    this.activatedRoute.queryParams.subscribe(params => {
      this.ResetMemberVariables();
      const address = params["address"];
      if (address && address != '') {
        this.ShowBusy = true;
        this.Address = address;
        this.userService.Search(address)
        .then(users => this.Users = users)
        .catch(err => {
          console.error(err);
          this.ErrorMessage = err.message || "Unexpected Error"
        })
        .finally(() => this.ShowBusy = false)
        ;
      }
    });
  }

  private ResetMemberVariables() : void {
    this.ErrorMessage = null;
    this.Address = null;
    this.Users = null;
    this.ShowBusy = false;
  }

  Load() : void {
    this.router.navigate(['/sa/Users'], { queryParams: { "address": this.Address }});
  }
}
