namespace BrassLoon.Account.Framework
{
    public interface ISettings : BrassLoon.CommonCore.ISettings
    {
        string ClientSecretVaultAddress { get; }
    }
}
