import { Component, Input, OnInit } from '@angular/core';
import { Exception } from '../models/exception';

@Component({
  selector: 'app-exception-data',
  templateUrl: './exception-data.component.html',
  styles: [
  ]
})
export class ExceptionDataComponent implements OnInit {

  @Input() Exception: Exception;

  constructor() { }

  ngOnInit(): void {
  }

  GetDataKeys() {
    if (this.Exception && this.Exception.Data) {
      return Object.keys(this.Exception.Data);
    }
    else {
      return null;
    }
  }

}
