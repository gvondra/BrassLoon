namespace BrassLoon.Account.Framework
{
    public interface ISettings : CommonCore.ISettings
    {
        string ClientSecretVaultAddress { get; }
    }
}
