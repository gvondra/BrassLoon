namespace BrassLoon.Interface.WorkTask.Models
{
    public class ClaimWorkTaskResponse
    {
        public bool IsAssigned { get; set; }
        public string Message { get; set; }
        public string AssignedToUserId { get; set; }
    }
}
