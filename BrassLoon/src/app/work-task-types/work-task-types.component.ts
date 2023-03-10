import { Component, OnInit } from '@angular/core';
import { Domain } from '../models/domain';
import { DomainService } from '../services/domain.service';
import { WorkTaskType } from '../models/work-task-type';
import { WorkTaskTypeService } from '../services/work-task-type.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-work-task-types',
  templateUrl: './work-task-types.component.html',
  styles: [
  ]
})
export class WorkTaskTypesComponent implements OnInit {
  
  ShowBusy: boolean = false;
  ErrorMessage: string = null;
  Domain: Domain = null;
  WorkTaskTypes: Array<WorkTaskType> = null;

  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private domainService: DomainService,
    private workTaskTypeService: WorkTaskTypeService) { }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(params => {
      this.InitializeComponent();
      if (params["domainId"]) { 
        this.ShowBusy = true;
        this.domainService.Get(params["domainId"])     
        .then(domain => {
          this.Domain = domain;
          this.LoadTaskTypes(domain.DomainId);
        })
        .catch(err => {
          console.error(err);
          this.ErrorMessage = err.message || "Unexpected Error"
        }); 
      }
    });
  }

  private InitializeComponent() {
    this.ShowBusy = false;
    this.ErrorMessage = null;
    this.Domain = null;
    this.WorkTaskTypes = null;
  }

  private LoadTaskTypes(domainId) {
    this.workTaskTypeService.GetAll(domainId)
    .then(taskTypes => this.WorkTaskTypes = taskTypes)
    .catch(err => {
      console.error(err);
      this.ErrorMessage = err.message || "Unexpected Error"
    })
    .finally(() => this.ShowBusy = false)
    ;
  }

  OnNewType() {
    this.router.navigate(['/d', this.Domain.DomainId, 'WTType']);
  }
}
