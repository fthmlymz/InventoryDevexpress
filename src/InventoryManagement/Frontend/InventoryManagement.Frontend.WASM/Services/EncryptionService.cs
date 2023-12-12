using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.Text;

namespace InventoryManagement.Frontend.Services
{
    public class EncryptionService
    {
        private static readonly byte[] Key = new byte[] { 0x73, 0x65, 0x63, 0x72, 0x65, 0x74, 0x31, 0x73, 0x65, 0x63, 0x72, 0x65, 0x74, 0x31, 0x73, 0x65 };
        private static readonly byte[] IV = new byte[] { 0x49, 0x76, 0x32, 0x30, 0x31, 0x39, 0x20, 0x49, 0x6E, 0x69, 0x74, 0x56, 0x65, 0x63, 0x74, 0x6F };

        static EncryptionService()
        {
            IV = new byte[] { 0x49, 0x76, 0x32, 0x30, 0x31, 0x39, 0x20, 0x49, 0x6E, 0x69, 0x74, 0x56, 0x65, 0x63, 0x74, 0x6F };
        }
        public static string Encrypt(string data)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentException("Data cannot be null or empty.");

            var plaintext = Encoding.UTF8.GetBytes(data);
            var cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7Padding");

            cipher.Init(true, new ParametersWithIV(ParameterUtilities.CreateKeyParameter("AES", Key), IV));

            var ciphertext = cipher.DoFinal(plaintext);

            return Convert.ToBase64String(ciphertext);
        }
        public static string Decrypt(string encryptedData)
        {
            if (string.IsNullOrEmpty(encryptedData))
                return string.Empty;
                //throw new ArgumentException("Encrypted data cannot be null or empty.");

            var ciphertext = Convert.FromBase64String(encryptedData);
            var cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7Padding");

            cipher.Init(false, new ParametersWithIV(ParameterUtilities.CreateKeyParameter("AES", Key), IV));

            var plaintext = cipher.DoFinal(ciphertext);

            return Encoding.UTF8.GetString(plaintext);
        }
    }
}
