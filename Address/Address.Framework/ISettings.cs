namespace BrassLoon.Address.Framework
{
    public interface ISettings : CommonCore.ISettings
    {
        string KeyVaultAddress { get; }
    }
}
