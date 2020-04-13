using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Common.Helper
{
    public sealed class Encryptor
    {
        public static string SecretKey
        {
            get
            {
                var secretKey = ConfigurationManager.AppSettings["SecretKey"] ?? ConfigurationManager.AppSettings["secret-key"];
                return !string.IsNullOrEmpty(secretKey) ? secretKey : "proview";
            }
        }

        public static byte[] Encrypt(byte[] input, string password)
        {
            try
            {
                var service = new TripleDESCryptoServiceProvider();
                var md5 = new MD5CryptoServiceProvider();

                var key = md5.ComputeHash(Encoding.ASCII.GetBytes(password));
                var iv = md5.ComputeHash(Encoding.ASCII.GetBytes(password));

                return Transform(input, service.CreateEncryptor(key, iv));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.ToString());
            }
        }

        public static byte[] Decrypt(byte[] input, string password)
        {
            try
            {
                var service = new TripleDESCryptoServiceProvider();
                var md5 = new MD5CryptoServiceProvider();

                var key = md5.ComputeHash(Encoding.ASCII.GetBytes(password));
                var iv = md5.ComputeHash(Encoding.ASCII.GetBytes(password));

                return Transform(input, service.CreateDecryptor(key, iv));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.ToString());
            }
        }

        public static string Encrypt(string text, string password)
        {
            var input = Encoding.UTF8.GetBytes(text);
            var output = Encrypt(input, password);
            return Convert.ToBase64String(output);
        }

        public static string Decrypt(string text, string password)
        {
            var input = Convert.FromBase64String(text);
            var output = Decrypt(input, password);
            return Encoding.UTF8.GetString(output);
        }

        private static byte[] Transform(byte[] input, ICryptoTransform cryptoTransform)
        {
            var memStream = new MemoryStream();
            var cryptStream = new CryptoStream(memStream, cryptoTransform, CryptoStreamMode.Write);

            cryptStream.Write(input, 0, input.Length);
            cryptStream.FlushFinalBlock();

            memStream.Position = 0;
            var result = new byte[Convert.ToInt32(memStream.Length)];
            memStream.Read(result, 0, Convert.ToInt32(result.Length));

            memStream.Close();
            cryptStream.Close();

            return result;
        }

        /// <summary>
        ///     Encrypt the given value with the Rijndael algorithm.
        /// </summary>
        /// <param name="encryptValue">Value to encrypt</param>
        /// <returns>Encrypted value. </returns>
        public static string Encrypt(string encryptValue)
        {
            try
            {
                if (!string.IsNullOrEmpty(encryptValue))
                {
                    return Encrypt(encryptValue, SecretKey);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
            return null;
        }


        /// <summary>
        ///     Decrypt the given value with the Rijndael algorithm.
        /// </summary>
        /// <param name="decryptValue">Value to decrypt</param>
        /// <returns>Decrypted value. </returns>
        public static string Decrypt(string decryptValue)
        {
            try
            {
                if (!string.IsNullOrEmpty(decryptValue))
                {
                    return Decrypt(decryptValue, SecretKey);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
            return "";
        }


        public static string EncryptPassword(string userName, string password)
        {
            var pwd = userName.ToLower() + "@" + password;
            return Encrypt(pwd);
        }

        public static string Password(string userName, string password)
        {
            var pwd = userName.ToLower() + "@" + password;
            return Encrypt(pwd, password);
        }

        #region Encode - Decode

        private static readonly char[] Key = "@42d4e309add544f9bcd7d3b7f8a44266".ToCharArray();

        #region UseRandomKeyEncypt

        private static bool UseRandomKeyEncypt
        {
            get { return Convert.ToBoolean(ConfigurationManager.AppSettings["UseRandomKeyEncypt"]); }
        }

        #endregion

        public static string Base64Encode(object input)
        {
            try
            {
                if (input == null || string.IsNullOrEmpty(input.ToString()))
                    return "";

                var b = (new UTF8Encoding()).GetBytes(input.ToString());
                return Convert.ToBase64String(b);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode " + ex.Message);
            }
        }

        public static string Base64Decode(object input)
        {
            try
            {
                if (input == null || string.IsNullOrEmpty(input.ToString()))
                    return "";

                var b = Convert.FromBase64String(input.ToString());
                return (new UTF8Encoding()).GetString(b);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Decode" + ex.Message);
            }
        }

        public static string Encode(object input)
        {
            if (input == null || string.IsNullOrEmpty(input.ToString())) return "";

            var ca = Base64Encode(input).ToCharArray();

            StringBuilder sb;

            if (UseRandomKeyEncypt)
            {
                var k = new Random().Next(255);

                sb = k > 15
                    ? new StringBuilder("" + Convert.ToString(k, 16))
                    : new StringBuilder("0" + Convert.ToString(k, 16));
            }
            else
            {
                sb = new StringBuilder(Convert.ToString(118, 16));
            }

            var m = Convert.ToInt32(sb.ToString(), 16);
            for (var i = 0; i < ca.Length; i++)
            {
                if ((ca[i] + m) > 255)
                {
                    m = ((ca[i] + m) - 255) ^ Key[i%13];
                }
                else
                {
                    m = (ca[i] + m) ^ Key[i%13];
                }
                if (m > 15)
                {
                    sb.Append(Convert.ToString(m, 16));
                }
                else
                {
                    sb.Append("0" + Convert.ToString(m, 16));
                }
            }
            return sb.ToString().ToUpper();
        }


        public static string Decode(object input)
        {
            try
            {
                if (input == null || string.IsNullOrEmpty(input.ToString())) return "";

                var sb = new StringBuilder();
                var ca = input.ToString().ToCharArray();
                for (var i = 0; i < (ca.Length - 2); i += 2)
                {
                    var s1 = "" + ca[i + 2] + ca[i + 3];
                    var s2 = "" + Convert.ToString((byte) Key[(i/2)%13], 16);
                    var s3 = "" + ca[i] + ca[i + 1];
                    var m = (Convert.ToInt32(s1, 16) ^ Convert.ToInt32(s2, 16)) - Convert.ToInt32(s3, 16);
                    if (m < 0)
                    {
                        m += 255;
                    }
                    sb.Append((char) ((short) m));
                }
                return Base64Decode(sb);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        #region Encode - Decode With Unicode

        public static string Encode(object input, bool unicode)
        {
            if (unicode)
                return Encode(Base64Encode(input));
            return Encode(input);
        }

        public static string Decode(object input, bool unicode)
        {
            if (unicode)
                return Base64Decode(Decode(input));
            return Decode(input);
        }

        #endregion

        #endregion
    }
}