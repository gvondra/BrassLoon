namespace BrassLoon.Account.Framework
{
    public interface ISecretProcessor
    {
        // this is an older method. Phasing this out
        bool Verify(string secret, byte[] hash);
        byte[] CreateSalt();
        byte[] HashSecretArgon2i(string value, byte[] salt);
    }
}
