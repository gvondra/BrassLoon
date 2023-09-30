namespace BrassLoon.Interface.Authorization.Models
{
    public class AppliedRole
    {
        public string Name { get; set; }
        public string PolicyName { get; set; }

        internal static AppliedRole Create(Protos.AppliedRole role)
        {
            return new AppliedRole
            {
                Name = role.Name,
                PolicyName = role.PolicyName
            };
        }
    }
}
