using System.Security.Cryptography;
using System.Text;

namespace Hms.WebApp.Helper
{
    public class UrlEncryptionExtension
    {
        public static IConfiguration? Configuration { get; set; }

        public static string EncryptUrl(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            try
            {
                string key = Configuration?["EncryptionKey"] ?? "MyDefaultSecretKey1234567890ab";
                byte[] keyBytes = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.GenerateIV();

                    using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    using (var ms = new MemoryStream())
                    {
                        ms.Write(aes.IV, 0, aes.IV.Length);
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            cs.Write(plainBytes, 0, plainBytes.Length);
                        }
                        return Convert.ToBase64String(ms.ToArray())
                            .Replace("+", "-")
                            .Replace("/", "_")
                            .Replace("=", "");
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string DecryptUrl(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
                return string.Empty;

            try
            {
                string key = Configuration?["EncryptionKey"] ?? "MyDefaultSecretKey1234567890ab";
                byte[] keyBytes = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));

                string base64 = encryptedText
                    .Replace("-", "+")
                    .Replace("_", "/");

                int padding = (4 - base64.Length % 4) % 4;
                base64 += new string('=', padding);

                byte[] cipherBytes = Convert.FromBase64String(base64);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    byte[] iv = new byte[16];
                    Array.Copy(cipherBytes, 0, iv, 0, 16);
                    aes.IV = iv;

                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    using (var ms = new MemoryStream(cipherBytes, 16, cipherBytes.Length - 16))
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    using (var reader = new StreamReader(cs))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}