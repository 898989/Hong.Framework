using System;
using System.Security.Cryptography;
using System.Text;

namespace Hong.Common.Extendsion
{
    public class Security
    {
        static readonly MD5 md5 = MD5.Create();

        public static string GetMD516(string str)
        {
            return BitConverter.ToString(
                md5.ComputeHash(Encoding.UTF8.GetBytes(str)),
                4, 8)
                .Replace("-", "");
        }

        public static string GetMD532(string str)
        {
            var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(str));

            StringBuilder sb = new StringBuilder();
            foreach (var b in bytes)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                sb.Append(b.ToString("X"));
            }

            str = sb.ToString();
            sb = null;

            return str;
        }
    }
}
