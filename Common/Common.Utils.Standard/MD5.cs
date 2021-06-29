using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Common.Utils.Standard
{
    public class MD5
    {
        private const string Key = "rdtvjiepel2006";
        private MD5CryptoServiceProvider md5;
        private byte[] hash;
        private UnicodeEncoding uEncode;
        private string outputString;
        public MD5()
        {
            md5 = new MD5CryptoServiceProvider();
            uEncode = new UnicodeEncoding();
        }

        public void CryptData(string sData)
        {
            hash = HashString(sData);
        }

        public bool Compare(string sData, string sHashedData)
        {
            bool result = true;
            byte[] hashOriginal = uEncode.GetBytes(sHashedData);
            string strHashForCompare = GenerateHashDigest(sData);
            byte[] hashForCompare = uEncode.GetBytes(strHashForCompare);

            if (hashOriginal.Length > 0 && hashOriginal.Length == hashForCompare.Length)
            {
                for (int i = 0; i < hashOriginal.Length; i++)
                {
                    if (hashOriginal[i] != hashForCompare[i])
                    {
                        result = false;
                        outputString = "Data has been corrupted!";
                        break;
                    }
                    else
                    {

                        outputString = "Comparision Successful!";
                    }
                }
            }
            else
            {
                result = false;
                outputString = "Data has been corrupted!";
            }

            return result;
        }

        private byte[] HashString(string inString)
        {
            byte[] outBytes;
            byte[] inBytes = System.Text.Encoding.Default.GetBytes(inString + Key);
            outBytes = md5.ComputeHash(inBytes);
            return outBytes;
        }

        private string GenerateHashDigest(string inStr)
        {
            byte[] bytes = System.Text.Encoding.Default.GetBytes(inStr + Key);
            hash = md5.ComputeHash(bytes);
            string ret = "";
            foreach (byte a in hash)
            {
                if (a < 16)
                    ret += "0" + a.ToString("x");
                else
                    ret += a.ToString("x");
            }
            return ret;
        }

        public string ReturnHashedData()
        {
            if (hash.Length > 0)
            {
                //return Convert.ToBase64String(hash);
                string ret = "";
                foreach (byte a in hash)
                {
                    if (a < 16)
                        ret += "0" + a.ToString("x");
                    else
                        ret += a.ToString("x");
                }
                return ret;
            }
            else
            {
                return "there is no hashed data";
            }
        }


        public string Result
        {
            get { return outputString; }
        }
    }
}
