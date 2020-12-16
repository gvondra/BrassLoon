import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-domain-nav',
  templateUrl: './domain-nav.component.html',
  styles: [
  ]
})
export class DomainNavComponent implements OnInit {

  @Input() CurrentTab: string = 'Detail';
  @Input() DomainId: string;

  constructor() { }

  ngOnInit(): void {
  }

  IsActive(tab: string) : boolean {
    if (tab.toLowerCase() === this.CurrentTab.toLowerCase()) {
      return true;
    }
    else {
      return false;
    }
  }

}
