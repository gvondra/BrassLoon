import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Domain } from '../models/domain';
import { Exception } from '../models/exception';
import { DomainService } from '../services/domain.service';
import { ExceptionService } from '../services/exception.service';

@Component({
  selector: 'app-exception',
  templateUrl: './exception.component.html',
  styles: [
  ]
})
export class ExceptionComponent implements OnInit {

  ErrorMessage: string = null;
  Domain: Domain = null;
  Exception: Exception = null;
  InnerExceptions: Array<Exception> = null;

  constructor(private activatedRoute: ActivatedRoute,
    private domainService: DomainService,
    private exceptionService: ExceptionService) { }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(params => {
      this.Domain = null;
      this.Exception = null;
      this.ErrorMessage = null;
      if (params["domainId"] && params["id"]) {
        this.domainService.Get(params["domainId"])     
        .then(domain => {
          this.Domain = domain;          
        })
        .catch(err => {
          console.error(err);
          this.ErrorMessage = err.message || "Unexpected Error"
        });   
        this.exceptionService.Get(params["domainId"], params["id"])
        .then(exception => {
          this.Exception = exception;
          this.LoadInnerExcpetions(exception);
        })
        .catch(err => {
          console.error(err);
          this.ErrorMessage = err.message || "Unexpected Error"
        });   
      }
    });
  }

  private LoadInnerExcpetions(exception: Exception) {
    if (exception.InnerException) {
      if (!this.InnerExceptions) {
        this.InnerExceptions = [];
      }
      this.InnerExceptions.push(exception.InnerException);
      this.LoadInnerExcpetions(exception.InnerException);
    }
  }

}
