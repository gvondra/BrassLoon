namespace BrassLoon.WorkTask.Framework
{
    public interface IWorkTaskStatusFactory
    {
        IWorkTaskStatus Create(IWorkTaskType workTaskType, string code);
    }
}
