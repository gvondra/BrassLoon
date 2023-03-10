namespace BrassLoon.CommonAPI
{
    public static class Constants
    {
        public const string AUTH_SCHEME_BRASSLOON = "BrassLoonAuthentication";
        public const string AUTH_SCHEME_GOOGLE = "GoogleAuthentication";
        public const string POLICY_CREATE_TOKEN = "Create:Token"; // can exchange google user token for access token
        public const string POLICY_BL_AUTH = "BL:AUTH"; // ensures the requestor used a brass loon token
        public const string POLICY_SYS_ADMIN = "SysAdmin";
    }
}
