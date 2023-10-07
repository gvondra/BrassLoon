using System;
using System.Globalization;

namespace BrassLoon.Interface.WorkTask.Models
{
    public class ClaimWorkTaskResponse
    {
        public bool IsAssigned { get; set; }
        public string Message { get; set; }
        public string AssignedToUserId { get; set; }
        public DateTime? AssignedDate { get; set; }

        internal static ClaimWorkTaskResponse Create(Protos.ClaimWorkTaskResponse response)
        {
            return new ClaimWorkTaskResponse
            {
                AssignedDate = !string.IsNullOrEmpty(response.AssignedDate) ? DateTime.Parse(response.AssignedDate, CultureInfo.InvariantCulture) : default,
                Message = response.Message,
                AssignedToUserId = response.AssignedToUserId,
                IsAssigned = response.IsAssigned
            };
        }
    }
}
