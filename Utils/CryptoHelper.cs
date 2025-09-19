using System;
using System.Security.Cryptography;
using System.Text;

namespace RapasIMCEEApi.Utils
{
    public static class CryptoHelper
    {
        // La key attendue : base64 d'une clef 32 bytes (AES-256)
        public static string EncryptString(string plainText, string base64Key)
        {
            var key = Convert.FromBase64String(base64Key); // 32 bytes pour AES-256
            var plainBytes = Encoding.UTF8.GetBytes(plainText);

            var nonce = new byte[12];
            RandomNumberGenerator.Fill(nonce);

            var cipher = new byte[plainBytes.Length];
            var tag = new byte[16];

            using (var aes = new AesGcm(key))
            {
                aes.Encrypt(nonce, plainBytes, cipher, tag);
            }

            // concat nonce + tag + cipher puis base64
            var outb = new byte[nonce.Length + tag.Length + cipher.Length];
            Buffer.BlockCopy(nonce, 0, outb, 0, nonce.Length);
            Buffer.BlockCopy(tag, 0, outb, nonce.Length, tag.Length);
            Buffer.BlockCopy(cipher, 0, outb, nonce.Length + tag.Length, cipher.Length);
            return Convert.ToBase64String(outb);
        }

        public static string DecryptString(string base64Input, string base64Key)
        {
            var key = Convert.FromBase64String(base64Key);
            var all = Convert.FromBase64String(base64Input);

            var nonce = new byte[12];
            var tag = new byte[16];
            var cipher = new byte[all.Length - nonce.Length - tag.Length];

            Buffer.BlockCopy(all, 0, nonce, 0, nonce.Length);
            Buffer.BlockCopy(all, nonce.Length, tag, 0, tag.Length);
            Buffer.BlockCopy(all, nonce.Length + tag.Length, cipher, 0, cipher.Length);

            var plain = new byte[cipher.Length];
            using (var aes = new AesGcm(key))
            {
                aes.Decrypt(nonce, cipher, tag, plain);
            }
            return Encoding.UTF8.GetString(plain);
        }
    }

}
