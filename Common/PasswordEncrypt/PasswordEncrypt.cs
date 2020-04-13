using System;
using System.Security.Cryptography;
using System.Text;

namespace Common.PasswordEncrypt
{
    public static class PasswordEncrypt
    {
        public static string GetHash(string input)
        {
            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();

            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }
        //public static string ToAbsoluteUrl(this string relativeUrl) //Use absolute URL instead of adding phycal path for CSS, JS and Images     
        //{
        //    if (string.IsNullOrEmpty(relativeUrl)) return relativeUrl;
        //    if (HttpContext.Current == null) return relativeUrl;
        //    if (relativeUrl.StartsWith("/")) relativeUrl = relativeUrl.Insert(0, "~");
        //    if (!relativeUrl.StartsWith("~/")) relativeUrl = relativeUrl.Insert(0, "~/");
        //    var url = HttpContext.Current.Request.Url;
        //    var port = url.Port != 80 ? (":" + url.Port) : String.Empty;
        //    return String.Format("{0}://{1}{2}{3}", url.Scheme, url.Host, port, VirtualPathUtility.ToAbsolute(relativeUrl));
        //}
        public static string GeneratePassword(int length) //length of salt    
        {
            const string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            var randNum = new Random();
            var chars = new char[length];
            for (var i = 0; i <= length - 1; i++)
            {
                chars[i] = allowedChars[Convert.ToInt32((allowedChars.Length) * randNum.NextDouble())];
            }
            return new string(chars);
        }

        public static string EncodePassword(string pass, string salt) //encrypt password    
        {
            var bytes = Encoding.Unicode.GetBytes(pass);
            var src = Encoding.Unicode.GetBytes(salt);
            var dst = new byte[src.Length + bytes.Length];
            Buffer.BlockCopy(src, 0, dst, 0, src.Length);
            Buffer.BlockCopy(bytes, 0, dst, src.Length, bytes.Length);
            var algorithm = HashAlgorithm.Create("SHA1");
            var inArray = algorithm.ComputeHash(dst);
            //return Convert.ToBase64String(inArray);    
            return EncodePasswordMd5(Convert.ToBase64String(inArray));
        }

        public static string EncodePasswordMd5(string pass) //Encrypt using MD5    
        {
            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)    
            var md5 = new MD5CryptoServiceProvider();
            var originalBytes = Encoding.Default.GetBytes(pass);
            var encodedBytes = md5.ComputeHash(originalBytes);
            //Convert encoded bytes back to a 'readable' string    
            return BitConverter.ToString(encodedBytes);
        }
        public static string Base64Encode(string sData) // Encode    
        {
            try
            {
                var encDataByte = Encoding.UTF8.GetBytes(sData);
                var encodedData = Convert.ToBase64String(encDataByte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }
        public static string Base64Decode(string sData) //Decode    
        {
            try
            {
                var encoder = new System.Text.UTF8Encoding();
                var utf8Decode = encoder.GetDecoder();
                var todecodeByte = Convert.FromBase64String(sData);
                var charCount = utf8Decode.GetCharCount(todecodeByte, 0, todecodeByte.Length);
                var decodedChar = new char[charCount];
                utf8Decode.GetChars(todecodeByte, 0, todecodeByte.Length, decodedChar, 0);
                var result = new String(decodedChar);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Decode" + ex.Message);
            }
        }
    }
}
