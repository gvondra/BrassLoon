import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Domain } from '../models/domain';
import { DomainService } from '../services/domain.service';
import { WorkTaskStatus } from '../models/work-task-status';
import { WorkTaskStatusService } from '../services/work-task-status.service';

@Component({
  selector: 'app-work-task-status',
  templateUrl: './work-task-status.component.html',
  styles: [
  ]
})
export class WorkTaskStatusComponent implements OnInit {

  ShowBusy: boolean = false;
  Saving: boolean = false;
  ErrorMessage: string = "";
  Domain: Domain | null = null;
  WorkTaskStatus: WorkTaskStatus | null = null;

  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private domainService: DomainService,
    private workTaskStatusService: WorkTaskStatusService) { }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(params => {
      this.InitializeComponent();
      if (params["domainId"] && params["typeId"]) { 
        this.ShowBusy = true;
        this.domainService.Get(params["domainId"])     
        .then(domain => {
          this.Domain = domain;
          if (params["statusId"]) {
            this.LoadTaskStatus(domain.DomainId, params["typeId"], params["statusId"]);
          }
          else {
            this.NewTaskStatus(domain.DomainId, params["typeId"]);
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
    this.WorkTaskStatus = null;
  }

  private LoadTaskStatus(domainId: string, typeId: string, statusId: string): void {
    this.workTaskStatusService.Get(domainId, typeId, statusId)
    .then(workTaskStatus => this.WorkTaskStatus = workTaskStatus)
    .catch(err => {
      console.error(err);
      this.ErrorMessage = err.message || "Unexpected Error"
    })
    .finally(() => this.ShowBusy = false)
    ;    
  }

  private NewTaskStatus(domainId: string, typeId: string): void {
    const workTaskStatus: WorkTaskStatus = new WorkTaskStatus();
    workTaskStatus.Name = "New Status";
    workTaskStatus.IsClosedStatus = false;
    workTaskStatus.IsDefaultStatus = false;
    workTaskStatus.WorkTaskCount = 0;
    workTaskStatus.Code = "";
    workTaskStatus.Description = "";
    workTaskStatus.DomainId = domainId;
    workTaskStatus.WorkTaskTypeId = typeId;
    this.WorkTaskStatus = workTaskStatus;
    this.ShowBusy = false;
  }

  CanEditCode(): string | null {
    if (this.WorkTaskStatus.WorkTaskStatusId) {
      return "true";
    }
    else {
      return null;
    }
  }

  Save() {
    this.ErrorMessage = "";
    this.Saving = true;
    if (this.WorkTaskStatus.WorkTaskStatusId) {
      this.workTaskStatusService.Update(this.WorkTaskStatus.DomainId, this.WorkTaskStatus.WorkTaskTypeId, this.WorkTaskStatus)
      .then(status => this.WorkTaskStatus = status)
      .catch(err => {
        console.error(err);
        this.ErrorMessage = err.message || "Unexpected Error"
      })
      .finally(() => this.Saving = false)
      ;
    }
    else {
      this.workTaskStatusService.Create(this.WorkTaskStatus.DomainId, this.WorkTaskStatus.WorkTaskTypeId, this.WorkTaskStatus)
      .then(status => this.router.navigate(["/d", status.DomainId, "WTType", status.WorkTaskTypeId, "Status", status.WorkTaskStatusId]))
      .catch(err => {
        console.error(err);
        this.ErrorMessage = err.message || "Unexpected Error"
      })
      .finally(() => this.Saving = false)
      ;
    }
  }

}
