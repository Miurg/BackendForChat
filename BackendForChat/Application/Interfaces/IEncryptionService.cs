using BackendForChat.Models.Entities;

namespace BackendForChat.Application.Interfaces
{
    public interface IEncryptionService
    {
        public string Encrypt(string plainText);
        public string Decrypt(string cipherText);
    }
}
