namespace BrassLoon.Authorization.Framework
{
    public interface ISettings : CommonCore.ISettings
    {
        string SigningKeyVaultAddress { get; }
        string ClientSecretVaultAddress { get; }
    }
}
