namespace BrassLoon.Account.Framework
{
    public interface ISecretProcessor
    {
        bool Verify(string secret, byte[] hash);
    }
}
