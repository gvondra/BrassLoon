namespace BrassLoon.Authorization.Framework
{
    public interface ISettings : BrassLoon.CommonCore.ISettings
    {
        string SigningKeyVaultAddress { get; }
        string ClientSecretVaultAddress { get; }
    }
}
