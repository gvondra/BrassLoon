export class WorkTaskStatus {
    WorkTaskStatusId: string;
    WorkTaskTypeId: string;
    DomainId: string;
    Code: string;
    Name: string;
    Description: string;
    IsDefaultStatus: boolean | null;
    IsClosedStatus: boolean | null;
    CreateTimestamp: string;
    UpdateTimestamp: string;
    WorkTaskCount: number | null;
}
