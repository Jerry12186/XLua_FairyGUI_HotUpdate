using System.IO;
using System.Security.Cryptography;

namespace PackageSys
{
    public class MD5Tool : Singleton<MD5Tool>
    {
        public string GetMd5(string filePath)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            using (FileStream fs = File.OpenRead(filePath))
            {
                byte[] bytValue = new byte[fs.Length];
                int total = fs.Read(bytValue, 0, bytValue.Length);
                byte[] bytHash = md5.ComputeHash(bytValue);
                md5.Clear();
                string md5code = "";
                for (int i = 0; i < bytHash.Length; i++)
                {
                    md5code += bytHash[i].ToString("X").PadLeft(2, '0');
                }
                return md5code.ToLower();
            }
        }
    }
}