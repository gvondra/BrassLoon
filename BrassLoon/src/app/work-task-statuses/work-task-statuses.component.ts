import { Component, Input, OnInit } from '@angular/core';
import { WorkTaskType } from '../models/work-task-type';
import { WorkTaskStatus } from '../models/work-task-status';
import { WorkTaskStatusService } from '../services/work-task-status.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-work-task-statuses',
  templateUrl: './work-task-statuses.component.html',
  styles: [
  ]
})
export class WorkTaskStatusesComponent implements OnInit {
  private _workTaskType: WorkTaskType | null = null;

  ErrorMessage: string = "";
  WorkTaskStatuses: Array<WorkTaskStatus> | null = null;

  public get WorkTaskType(): WorkTaskType | null { return this._workTaskType; }
  @Input() set WorkTaskType(value: WorkTaskType | null) {
    this._workTaskType = value;
    if (value) {
      this.Load(value);
    }
  }

  constructor(private workTaskStatusService: WorkTaskStatusService,
    private router: Router) { }

  ngOnInit(): void {
    this.ErrorMessage = "";
    this.WorkTaskStatuses = null;
  }

  private Load(workTaskType: WorkTaskType): void {
    this.workTaskStatusService.GetAll(workTaskType.DomainId, workTaskType.WorkTaskTypeId)
    .then(statuses => {
      this.WorkTaskStatuses = statuses;
    })
    .catch(err => {
      console.error(err);
      this.ErrorMessage = err.message || "Unexpected Error"
    });
  }

  OnNewStatus() {
    this.router.navigate(['/d', this.WorkTaskType.DomainId, 'WTType', this.WorkTaskType.WorkTaskTypeId, "Status"]);
  }
}
