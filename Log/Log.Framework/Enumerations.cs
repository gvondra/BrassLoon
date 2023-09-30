namespace BrassLoon.Log.Framework.Enumerations
{
    public enum PurgeWorkerStatus : short
    {
        Error = -1,
        Ready = 0,
        InProgress = 1,
        Complete = 0xFF
    }
}
