import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-head',
  templateUrl: './head.component.html',
  styles: [
  ]
})
export class HeadComponent implements OnInit {

  constructor(private router: Router) { }

  ngOnInit(): void {
  }

  NavigateHome()
  {
    this.router.navigate(['/']);
  }
}
