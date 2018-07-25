using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /**
    * 接口签名生成规则：
    * 
    * <ol>
    * <li>请求参数排序（包括公共参数和业务参数,除去winc_sign参数和byte[]类型的参数）,根据参数名称的ASCII码表的顺序排序.如：foo=1, bar=2, foo_bar=3,
    * foobar=4排序后的顺序是bar=2, foo=1, foo_bar=3, foobar=4.</li>
    * <li>将排序好的参数名和参数值拼装在一起，根据上面的示例得到的结果为：bar2foo1foo_bar3foobar4.</li>
    * <li>
    * 把拼装好的字符串采用UTF-8编码,使用签名算法对编码后的字节流进行摘要.如果使用MD5算法，则需要在拼装的字符串前后加上clientSecret后, 再进行摘要;
    * 如果使用HMAC_MD5算法,则需要用clientSecret初始化摘要算法后，再进行摘要.</li>
    * <li>将摘要得到的字节流结果使用十六进制表示，如：E349C7D7F5D33CB398C1A0CAEB9C4F7A</li>
    * </ol>
    */
    public class SignUtils
    {

        /** UTF-8字符集 **/
        public static String CHARSET_UTF8 = "UTF-8";
        /** MD5签名方式 */
        public static String SIGN_METHOD_MD5 = "md5";
        /** HMAC签名方式 */
        public static String SIGN_METHOD_HMAC = "hmac";

        public static String SignWcopRequest(Dictionary<String, String> pars, String secret, String signMethod)
        {

            var keys = pars.Keys.OrderBy(x => x);

            // 把所有参数名和参数值串在一起
            StringBuilder query = new StringBuilder();
            if (SIGN_METHOD_MD5.Equals(signMethod))
            {
                query.Append(secret);
            }

            String noSignParams = null;
            pars.TryGetValue("no_sign_params", out noSignParams);
            foreach (String key in keys)
            {
                if ("winc_sign".Equals(key) || "no_sign_params".Equals(key)) continue;
                if (CheckNoSignParams(noSignParams, key)) continue;
                String value = pars[key];
                if (!String.IsNullOrEmpty(key) && !String.IsNullOrEmpty(value))
                {
                    query.Append(key).Append(value);
                }
            }

            // 使用MD5/HMAC加密
            byte[] bytes;
            if (SIGN_METHOD_HMAC.Equals(signMethod))
            {
                bytes = EncryptHMAC(query.ToString(), secret);
            }
            else
            {
                query.Append(secret);
                bytes = EncryptMD5(query.ToString());
            }
            return Byte2hex(bytes);
        }
        private static bool CheckNoSignParams(string noSignParams, string key)
        {
            if (!String.IsNullOrEmpty(noSignParams))
            {
                String[] ps = noSignParams.Trim().Split(',');
                foreach (String p in ps)
                {
                    if (p.Equals(key))
                        return true;
                }
            }
            return false;
        }


        public static byte[] EncryptHMAC(String data, String secret)
        {
            byte[]
            bytes = null;
            try
            {
                HMACMD5 hmac = new HMACMD5(Encoding.GetEncoding(CHARSET_UTF8).GetBytes(secret));
                bytes = hmac.ComputeHash(System.Text.Encoding.GetEncoding(CHARSET_UTF8).GetBytes(data));

            }
            catch (Exception)
            {
                throw;
            }
            return bytes;
        }

        public static byte[] EncryptMD5(String data)
        {
            return EncryptMD5(System.Text.Encoding.GetEncoding(CHARSET_UTF8).GetBytes(data));
        }

        public static byte[] EncryptMD5(byte[] data)
        {
            MD5 md5 = MD5.Create();

            return md5.ComputeHash(data);
        }

        /**
         * 把二进制转化为大写的十六进制.
         * 
         * @param bytes
         * @return
         */
        public static String Byte2hex(byte[] bytes)
        {
            StringBuilder sign = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                String hex = bytes[i].ToString("X2");
                if (hex.Length == 1)
                {
                    sign.Append("0");
                }
                sign.Append(hex.ToUpper());
            }
            return sign.ToString();
        }
        public static long CurrentTimeMillis
        {
            get
            {
                return Convert.ToInt64(DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds);
            }

        }

    }
}
