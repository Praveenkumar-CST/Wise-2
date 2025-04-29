using System.Security.Cryptography;
using System.Text;

namespace WiseHRServer.Services
{
    public class EncryptionService
    {
        private readonly byte[] _key;

        //public EncryptionService(string base64Key)
        //{
        //    _key = Convert.FromBase64String(base64Key);
        //    if (_key.Length != 32) // 256 bits
        //        throw new ArgumentException("Key must be 256 bits (32 bytes).");
        //}


        public EncryptionService(string base64Key)
        {
            Console.WriteLine($"Base64 Key: {base64Key}");
            _key = Convert.FromBase64String(base64Key);
            Console.WriteLine($"Decoded Key Length: {_key.Length} bytes");
            if (_key.Length != 32) // 256 bits
                throw new ArgumentException("Key must be 256 bits (32 bytes).");
        }



        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.GenerateIV();
            using var encryptor = aes.CreateEncryptor();
            using var ms = new MemoryStream();
            ms.Write(aes.IV, 0, aes.IV.Length); // Prepend IV
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using var sw = new StreamWriter(cs);
            sw.Write(plainText);
            sw.Close();
            return Convert.ToBase64String(ms.ToArray());
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using var aes = Aes.Create();
            aes.Key = _key;
            byte[] iv = cipherBytes.Take(16).ToArray(); // Extract IV (first 16 bytes)
            byte[] actualCipher = cipherBytes.Skip(16).ToArray();
            aes.IV = iv;
            using var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(actualCipher);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
    }
}