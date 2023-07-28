export class DomainClient {
    ClientId: string | null = null;
    DomainId: string | null = null;
    Name: string = "";
    IsActive: boolean = true;
    UserEmailAddress: string = "";
    UserName: string = "";
    Secret: string = "";
    Roles: Array<any> = [];
    CreateTimestamp: string = "";
    UpdateTimestamp: string = "";
}
