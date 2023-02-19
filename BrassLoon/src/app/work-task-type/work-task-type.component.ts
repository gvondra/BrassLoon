import { Component, OnInit } from '@angular/core';
import { Domain } from '../models/domain';
import { DomainService } from '../services/domain.service';
import { WorkTaskType } from '../models/work-task-type';
import { WorkTaskTypeService } from '../services/work-task-type.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-work-task-type',
  templateUrl: './work-task-type.component.html',
  styles: [
  ]
})
export class WorkTaskTypeComponent implements OnInit {

  ShowBusy: boolean = false;
  Saving: boolean = false;
  ErrorMessage: string = null;
  Domain: Domain = null;
  WorkTaskType: WorkTaskType = null;

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
          if (params["id"]) {
            this.LoadTaskType(domain.DomainId, params["id"]);
          }
          else {
            this.NewTaskType(domain.DomainId);
          }
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
    this.Saving = false;
    this.ErrorMessage = null;
    this.Domain = null;
    this.WorkTaskType = null;
  }

  private LoadTaskType(domainId, id) {
    this.workTaskTypeService.Get(domainId, id).toPromise()
    .then(taskType => this.WorkTaskType = taskType)
    .catch(err => {
      console.error(err);
      this.ErrorMessage = err.message || "Unexpected Error"
    })
    .finally(() => this.ShowBusy = false)
    ;
  }

  private NewTaskType(domainId) {
    const workTaskType: WorkTaskType = new WorkTaskType();
    workTaskType.DomainId = domainId;
    workTaskType.Title = "New Type";
    workTaskType.WorkTaskCount = 0;
    this.WorkTaskType = workTaskType;    
    this.ShowBusy = false
  }

  Save() {
    this.Saving = true;
    if (this.WorkTaskType.WorkTaskTypeId) {
      this.workTaskTypeService.Update(this.Domain.DomainId, this.WorkTaskType).toPromise()
      .then(workTaskType => this.WorkTaskType = workTaskType)
      .catch(err => {
        console.error(err);
        this.ErrorMessage = err.message || "Unexpected Error"
      })
      .finally(() => this.Saving = false)
      ;
    }
    else {
      this.workTaskTypeService.Create(this.Domain.DomainId, this.WorkTaskType).toPromise()
      .then(workTaskType => this.router.navigate(['/d', this.Domain.DomainId, 'WTType', workTaskType.WorkTaskTypeId]))
      .catch(err => {
        console.error(err);
        this.ErrorMessage = err.message || "Unexpected Error"
      })
      .finally(() => this.Saving = false)
      ;
    }
  }
}
