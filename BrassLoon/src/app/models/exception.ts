export class Exception {
    ExceptionId : number;
    DomainId : string;
    Message : string;
    TypeName : string;
    Source : string;
    AppDomain : string;
    TargetSite : string;
    StackTrace : string;
    Data : any;
    CreateTimestamp : string;
    InnerException : Exception;
}
