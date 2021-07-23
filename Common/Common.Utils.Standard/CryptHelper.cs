using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Common.Utils.Standard
{
    public static class CryptHelper
    {
        private static string _password = Config.GetAppSettings("CryptHelper.Password");// "10fa2984609246sdfsfsf3eb42c484d3289cbddd3e2b02fe4eab9f4331973cedbc83593e23a6ace9a728cdb42c9a0bf59375b65957dda412cf7bff143e3bf7c7cd67f47e79280f9ff962d7b4cd7865c5ca162b7a5490fc4b68bc49743dc9fd8d178ad5b0f5d0907b607908442f7bf4dcfa31702d51673f1a618f543467eaeb3454118a8abb413bc28d920d14a4b97d5026cc0e9ca1c";

        public static string GetHash(string s)
        {
            var sBytes = Encoding.UTF8.GetBytes(s);
            var hashBytes = SHA256.Create().ComputeHash(sBytes);

            return Convert.ToBase64String(hashBytes);
        }

        public static string Encrypt(string input, string password = null)
        {
            string salt = null;
            if (string.IsNullOrWhiteSpace(salt))
                salt = GetRandomString(16);

            byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(input);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password ?? _password);
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes, saltBytes);

            string result = Convert.ToBase64String(bytesEncrypted);

            return result + "." + salt;
        }

        public static string Decrypt(string input, string password = null)
        {
            if (input == null)
                return null;

            string salt = null;
            var inputArr = input.Split('.');
            if (inputArr.Length > 1)
            {
                input = inputArr[0];
                salt = inputArr[1];
            }

            byte[] bytesToBeDecrypted = Convert.FromBase64String(input);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password ?? _password);
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);

            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes, saltBytes);

            string result = Encoding.UTF8.GetString(bytesDecrypted);

            return result;
        }

        public static string TryDecrypt(string input, string password = null)
        {
            try
            {
                return Decrypt(input, password);
            }
            catch (Exception ex)
            {
                Config.Logger?.Error(ex);
            }

            return null;
        }

        public static string GetRandomString(short length)
        {
            if (length <= 0)
                return string.Empty;

            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
            {
                byte[] tokenData = new byte[length];
                rng.GetBytes(tokenData);

                string token = Convert.ToBase64String(tokenData);

                return token;
            }
        }

        public static string GetRandomStringAlphaNumeric(short length)
        {
            int doubleLength = length * 2;
            if (doubleLength > short.MaxValue)
                throw new Exception($"Max legal length is {((short)(short.MaxValue / 2))}");

            var sb = new StringBuilder();
            var s = GetRandomString((short)(length * 2));

            foreach (var c in s)
            {
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                    if (sb.Length >= length)
                        break;
                }
            }

            return sb.ToString();
        }

        private static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes, byte[] saltBytes)
        {
            byte[] encryptedBytes = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        private static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes, byte[] saltBytes)
        {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }
    }
}
