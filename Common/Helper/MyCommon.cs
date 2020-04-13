using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using Common.Emums;

namespace Common.Helper
{
    public sealed class MyCommon
    {
        public static string ConvertNullToEmpty(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? string.Empty : s;
            //return s ?? string.Empty;
        }

        /// <summary>
        /// Example: GenerateRgba("#ff7700", 0.5)
        /// </summary>
        /// <param name="backgroundColor"></param>
        /// <param name="backgroundOpacity"></param>
        /// <returns></returns>
        public static string GenerateRgba(string backgroundColor, double backgroundOpacity)
        {
            var color = ColorTranslator.FromHtml(backgroundColor);
            int r = Convert.ToInt16(color.R);
            int g = Convert.ToInt16(color.G);
            int b = Convert.ToInt16(color.B);
            return string.Format("rgba({0}, {1}, {2}, {3});", r, g, b, backgroundOpacity.ToString(CultureInfo.InvariantCulture));
        }
        public static string FormatFileSize(decimal filesize)
        {
            const decimal oneKiloByte = 1024;
            const decimal oneMegaByte = 1048576;
            const decimal oneGigaByte = 1073741824;

            if (filesize >= oneGigaByte)
            {
                return (filesize / oneGigaByte).ToString("0.00", CultureInfo.InvariantCulture) + " GB";
            }

            if (filesize >= oneMegaByte)
            {
                return (filesize / oneMegaByte).ToString("0.00", CultureInfo.InvariantCulture) + " MB";
            }

            if (filesize >= oneKiloByte)
            {
                return (filesize / oneKiloByte).ToString("0", CultureInfo.InvariantCulture) + " KB";
            }

            return filesize + " bytes";
        }

        public static string ClientIp()
        {
            try
            {
                var szRemoteAddr = HttpContext.Current.Request.UserHostAddress;
                var szXForwardedFor = HttpContext.Current.Request.ServerVariables["X_FORWARDED_FOR"];
                string szIp;

                if (szXForwardedFor == null)
                {
                    szIp = szRemoteAddr;
                }
                else
                {
                    szIp = szXForwardedFor;
                    if (szIp.IndexOf(",", StringComparison.Ordinal) <= 0) return szIp;
                    var arIPs = szIp.Split(',');

                    foreach (var item in arIPs)
                    {
                        if (!IsPrivateIpAddress(item))
                        {
                            return item;
                        }

                        var strHostName = Dns.GetHostName();
                        var clientIpAddress = Dns.GetHostAddresses(strHostName).GetValue(1).ToString();

                        return clientIpAddress;
                    }
                }

                return szIp;
            }
            catch (Exception)
            {
                // Always return all zeroes for any failure (my calling code expects it)
                return "0.0.0.0";
            }
        }

        public static bool IsLocalIp(string hostNameOrAddress)
        {
            if (string.IsNullOrEmpty(hostNameOrAddress))
                return false;

            try
            {
                // get host IP addresses
                var hostIPs = Dns.GetHostAddresses(hostNameOrAddress);
                // get local IP addresses
                var localIPs = Dns.GetHostAddresses(Dns.GetHostName());
                // test if any host IP is a loopback IP or is equal to any local IP
                return hostIPs.Any(hostIp => IPAddress.IsLoopback(hostIp) || localIPs.Contains(hostIp));
            }
            catch
            {
                return false;
            }
        }

        private static bool IsPrivateIpAddress(string ipAddress)
        {
            // http://en.wikipedia.org/wiki/Private_network
            // Private IP Addresses are:
            //  24-bit block: 10.0.0.0 through 10.255.255.255
            //  20-bit block: 172.16.0.0 through 172.31.255.255
            //  16-bit block: 192.168.0.0 through 192.168.255.255
            //  Link-local addresses: 169.254.0.0 through 169.254.255.255 (http://en.wikipedia.org/wiki/Link-local_address)

            var ip = IPAddress.Parse(ipAddress);
            var octets = ip.GetAddressBytes();

            var is24BitBlock = octets[0] == 10;
            if (is24BitBlock)
            {
                return true; // Return to prevent further processing
            }

            var is20BitBlock = octets[0] == 172 && octets[1] >= 16 && octets[1] <= 31;
            if (is20BitBlock)
            {
                return true; // Return to prevent further processing
            }

            var is16BitBlock = octets[0] == 192 && octets[1] == 168;
            if (is16BitBlock)
            {
                return true; // Return to prevent further processing
            }

            var isLinkLocalAddress = octets[0] == 169 && octets[1] == 254;
            return isLinkLocalAddress;
        }

        public static string UrlRoot
        {
            get
            {
                var sRet = HttpContext.Current.Request.ApplicationPath;
                if (sRet != null && !sRet.EndsWith("/"))
                {
                    sRet = sRet + "/";
                }
                return sRet;
            }
        }

        //public static string SiteName
        //{
        //    get
        //    {
        //        var sRet = Config.SiteName;
        //        if (!string.IsNullOrEmpty(sRet)) return sRet;
        //        var sDomain = HttpContext.Current.Request.Url.Host.Trim().ToLower();
        //        sRet = sDomain.IndexOf("localhost", StringComparison.Ordinal) < 0 ? sDomain.Replace("." + Config.DomainName, "") : "www";
        //        return sRet;
        //    }
        //}

        public static string GetDomain(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return string.Empty;

            var host = new Uri(url).Host;

            var idx = host.IndexOf('.');

            if (idx < 0) return host;

            idx += 1;
            return host.Substring(idx, host.Length - idx);
        }


        public static string HenryDecrypt(string value)
        {
            var rijndaelEnhanced = new RijndaelEnhanced("HenryDo", "@1B2c3D4e5F6g7H8");
            return rijndaelEnhanced.Decrypt(value);
        }

        public static string HenryEndcrypt(string value)
        {
            var rijndaelEnhanced = new RijndaelEnhanced("HenryDo", "@1B2c3D4e5F6g7H8");
            return rijndaelEnhanced.Encrypt(value);
        }

        public static string HenryDecrypt(string value, string key)
        {
            var rijndaelEnhanced = new RijndaelEnhanced(key, "@1B2c3D4e5F6g7H8");
            return rijndaelEnhanced.Decrypt(value);
        }

        public static string HenryEndcrypt(string value, string key)
        {
            var rijndaelEnhanced = new RijndaelEnhanced(key, "@1B2c3D4e5F6g7H8");
            return rijndaelEnhanced.Encrypt(value);
        }

        public static bool CheckEmailFormat(string email)
        {
            String ZipRegex = @"^([a-zA-Z0-9_\.]+)@((\[[0-9]{1,3}" + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\"
                              + @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

            if (!Regex.IsMatch(email, ZipRegex))
            {
                return false;
            }

            return true;
        }

        public static string Md5Endcoding(string password)
        {
            return BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(password))).Replace("-", string.Empty);
        }

        public static string CreateRandomCode(int codeCount)
        {
            const string allChar =
                "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";
            string[] allCharArray = allChar.Split(',');
            string randomCode = "";
            int temp = -1;

            var rand = new Random();
            for (int i = 0; i < codeCount; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(36);
                if (temp != -1 && temp == t)
                {
                    return CreateRandomCode(codeCount);
                }
                temp = t;
                randomCode += allCharArray[t];
            }

            return randomCode;
        }

        public string XssFilter(string sValue)
        {
            const string sTemp = "?=:/._-0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var sOut = "";
            for (var i = 0; i < sValue.Length; i++)
            {
                if (sTemp.IndexOf(sValue[i]) >= 0)
                {
                    sOut += sValue[i];
                }
            }

            return sOut;
        }

        public static String Ucs2Convert(String sContent)
        {
            if (string.IsNullOrWhiteSpace(sContent))
            {
                return "";
            }

            sContent = sContent.Replace("\\", string.Empty);

            sContent = sContent.Trim();
            const string sUtf8Lower = "a|á|à|ả|ã|ạ|ă|ắ|ằ|ẳ|ẵ|ặ|â|ấ|ầ|ẩ|ẫ|ậ|đ|e|é|è|ẻ|ẽ|ẹ|ê|ế|ề|ể|ễ|ệ|i|í|ì|ỉ|ĩ|ị|o|ó|ò|ỏ|õ|ọ|ô|ố|ồ|ổ|ỗ|ộ|ơ|ớ|ờ|ở|ỡ|ợ|u|ú|ù|ủ|ũ|ụ|ư|ứ|ừ|ử|ữ|ự|y|ý|ỳ|ỷ|ỹ|ỵ";

            const string sUtf8Upper = "A|Á|À|Ả|Ã|Ạ|Ă|Ắ|Ằ|Ẳ|Ẵ|Ặ|Â|Ấ|Ầ|Ẩ|Ẫ|Ậ|Đ|E|É|È|Ẻ|Ẽ|Ẹ|Ê|Ế|Ề|Ể|Ễ|Ệ|I|Í|Ì|Ỉ|Ĩ|Ị|O|Ó|Ò|Ỏ|Õ|Ọ|Ô|Ố|Ồ|Ổ|Ỗ|Ộ|Ơ|Ớ|Ờ|Ở|Ỡ|Ợ|U|Ú|Ù|Ủ|Ũ|Ụ|Ư|Ứ|Ừ|Ử|Ữ|Ự|Y|Ý|Ỳ|Ỷ|Ỹ|Ỵ";

            const string sUcs2Lower = "a|a|a|a|a|a|a|a|a|a|a|a|a|a|a|a|a|a|d|e|e|e|e|e|e|e|e|e|e|e|e|i|i|i|i|i|i|o|o|o|o|o|o|o|o|o|o|o|o|o|o|o|o|o|o|u|u|u|u|u|u|u|u|u|u|u|u|y|y|y|y|y|y";

            const string sUcs2Upper = "A|A|A|A|A|A|A|A|A|A|A|A|A|A|A|A|A|A|D|E|E|E|E|E|E|E|E|E|E|E|E|I|I|I|I|I|I|O|O|O|O|O|O|O|O|O|O|O|O|O|O|O|O|O|O|U|U|U|U|U|U|U|U|U|U|U|U|Y|Y|Y|Y|Y|Y";

            var aUtf8Lower = sUtf8Lower.Split('|');

            var aUtf8Upper = sUtf8Upper.Split('|');

            var aUcs2Lower = sUcs2Lower.Split('|');

            var aUcs2Upper = sUcs2Upper.Split('|');

            var nLimitChar = aUtf8Lower.GetUpperBound(0);

            for (int i = 1; i <= nLimitChar; i++)
            {
                sContent = sContent.Replace(aUtf8Lower[i], aUcs2Lower[i]);

                sContent = sContent.Replace(aUtf8Upper[i], aUcs2Upper[i]);
            }
            const string sUcs2Regex = @"[.A-Za-z0-9- _@#$]";
            var sEscaped =
                new Regex(sUcs2Regex, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture)
                    .Replace(sContent, string.Empty);

            if (string.IsNullOrEmpty(sEscaped))
            {
                return sContent;
            }
            sEscaped = sEscaped.Replace("[", "\\[");
            sEscaped = sEscaped.Replace("]", "\\]");
            sEscaped = sEscaped.Replace("^", "\\^");
            var sEscapedregex = @"[" + sEscaped + "]";
            return
                new Regex(
                    sEscapedregex,
                    RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture).Replace(
                        sContent,
                        string.Empty);
        }

        public static string StripHtml(string html, string tagHtml)
        {
            //Stripts the <tagHtml> tags from the Html
            string tagBegin = @"<" + tagHtml + @"[\s\S]*?>";
            var tagStripBegin = new Regex(
                tagBegin,
                RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
            string strBegin = tagStripBegin.Replace(html, string.Empty);

            string tagEnd = @"</" + tagHtml + ">";
            var tagStripEnd = new Regex(
                tagEnd,
                RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
            string strEnd = tagStripEnd.Replace(strBegin, string.Empty);

            return strEnd;
        }

        public static string FixHtml(string html)
        {
            var strOutput = html;
            //Stripts the <p> tags from the Html
            const string pregex = @"<p[^>.]*>&nbsp;</p>";
            var p = new Regex(pregex, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
            strOutput = p.Replace(strOutput, "<br>");

            //Stripts the <link> tags from the Html
            const string linkregex = @"<link[\s\S]*?>";
            var link = new Regex(
                linkregex,
                RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
            strOutput = link.Replace(strOutput, string.Empty);

            //Stripts the <style> tags from the Html
            const string styleregex = @"<style[^>.]*>[\s\S]*?</style>";
            var styles = new Regex(
                styleregex,
                RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
            strOutput = styles.Replace(strOutput, string.Empty);

            //Stripts the [if tags from the Html
            const string ifregex = @"<!--[^>.]*>[\s\S]*?<![^>.]*-->";
            var iftag = new Regex(
                ifregex,
                RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
            strOutput = iftag.Replace(strOutput, string.Empty);
            return strOutput;
        }

        public static string SubString(string sSource, int length)
        {
            if (string.IsNullOrEmpty(sSource))
            {
                return string.Empty;
            }
            if (sSource.Length <= length)
            {
                return sSource;
            }

            var mSource = sSource;
            var nLength = length;

            while (nLength > 0 && mSource[nLength].ToString() != " ")
            {
                nLength--;
            }
            mSource = mSource.Substring(0, nLength);
            return mSource + "...";
        }

        /// <summary>
        /// Đọc file video theo 1 công cụ đọc file truyền vào
        /// </summary>
        /// <param name="toolFile"></param>
        /// <param name="videoFile"></param>
        /// <returns>chuỗ thông tin video đọc được</returns>
        public static string ReadMediaFile(string toolFile, string videoFile)
        {
            var ffmpeg = new System.Diagnostics.Process();

            String result; // temp variable holding a string representation of our video's duration
            StreamReader errorreader; // StringWriter to hold output from ffmpeg

            // we want to execute the process without opening a shell
            ffmpeg.StartInfo.UseShellExecute = false;
            ffmpeg.StartInfo.ErrorDialog = false;

            // redirect StandardError so we can parse it
            // for some reason the output comes through over StandardError
            ffmpeg.StartInfo.RedirectStandardError = true;

            // set the file name of our process, including the full path
            // (as well as quotes, as if you were calling it from the command-line)
            ffmpeg.StartInfo.FileName = toolFile;

            // set the command-line arguments of our process, including full paths of any files
            // (as well as quotes, as if you were passing these arguments on the command-line)
            ffmpeg.StartInfo.Arguments = @"-i " + videoFile;

            // start the process
            ffmpeg.Start();

            // now that the process is started, we can redirect output to the StreamReader we defined
            errorreader = ffmpeg.StandardError;

            // wait until ffmpeg comes back
            ffmpeg.WaitForExit(); //[time_to_wait_in_milliseconds]

            // read the output from ffmpeg, which for some reason is found in Process.StandardError
            result = errorreader.ReadToEnd();

            // a little convoluded, this string manipulation...
            // working from the inside out, it:
            // takes a substring of result, starting from the end of the "Duration: " label contained within,
            // (execute "ffmpeg.exe -i somevideofile" on the command-line to verify for yourself that it is there)
            // and going the full length of the timestamp.
            // The resulting substring is of the form "HH:MM:SS.UU"

            return result;
        }

        /// <summary>
        /// Lấy thời gian của file video theo định dạng HH:MM:SS.UU
        /// </summary>
        /// <param name="toolFile"></param>
        /// <param name="videoFile"></param>
        /// <returns></returns>
        public static string GetDurationStringOfMediaFile(string toolFile, string videoFile)
        {
            // a little convoluded, this string manipulation...
            // working from the inside out, it:
            // takes a substring of result, starting from the end of the "Duration: " label contained within,
            // (execute "ffmpeg.exe -i somevideofile" on the command-line to verify for yourself that it is there)
            // and going the full length of the timestamp.
            // The resulting substring is of the form "HH:MM:SS.UU"
            var result = ReadMediaFile(toolFile, videoFile);
            var duration = result.Substring(result.IndexOf("Duration: ", StringComparison.Ordinal) + ("Duration: ").Length, 11);
            //("00:00:00.00").Length

            return duration;
        }

        /// <summary>
        /// Lấy thời gian tính bằng giây của file video
        /// </summary>
        /// <param name="toolFile"></param>
        /// <param name="videoFile"></param>
        /// <returns></returns>
        public static long GetDurationInSecondOfMediaFile(string toolFile, string videoFile)
        {
            long secondsOfMediFile;
            var duration = GetDurationStringOfMediaFile(toolFile, videoFile);
            //Log.Debug("GetDurationInSecondOfMediaFile.duration=" + duration);
            try
            {
                var h = Convert.ToInt32(duration.Substring(0, 2));
                var m = Convert.ToInt32(duration.Substring(3, 2));
                var s = Convert.ToInt32(duration.Substring(6, 2));
                secondsOfMediFile = h * 3600 + m * 60 + s;
            }
            catch (Exception)
            {
                secondsOfMediFile = 0;
            }
            return secondsOfMediFile;
        }

        public static string ConvertVni2Unicode(string strInput)
        {
            if (string.IsNullOrEmpty(strInput))
            {
                return null;
            }

            var maAcii = new[]
                               {
                                   7845, 7847, 7849, 7851, 7853, 226, 7843, 227, 7841, 7855, 7857, 7859, 7861, 7863, 259,
                                   250, 249, 7911, 361, 7909, 7913, 7915, 7917, 7919, 7921, 432, 7871, 7873, 7875, 7877,
                                   7879, 234, 233, 232, 7867, 7869, 7865, 7889, 7891, 7893, 7895, 7897, 7887, 245, 7885,
                                   7899, 7901, 7903, 7905, 7907, 417, 237, 236, 7881, 297, 7883, 253, 7923, 7927, 7929,
                                   7925, 273, 7844, 7846, 7848, 7850, 7852, 194, 7842, 195, 7840, 7854, 7856, 7858, 7860,
                                   7862, 258, 218, 217, 7910, 360, 7908, 7912, 7914, 7916, 7918, 7920, 431, 7870, 7872,
                                   7874, 7876, 7878, 202, 201, 200, 7866, 7868, 7864, 7888, 7890, 7892, 7894, 7896, 7886,
                                   213, 7884, 7898, 7900, 7902, 7904, 7906, 416, 205, 204, 7880, 296, 7882, 221, 7922, 7926
                                   , 7928, 7924, 272, 225, 224, 244, 243, 242, 193, 192, 212, 211, 210
                               };
            var vni = new[]
                               {
                                   "aá", "aà", "aå", "aã", "aä", "aâ", "aû", "aõ", "aï", "aé", "aè",
                                   "aú", "aü", "aë", "aê", "uù", "uø", "uû", "uõ", "uï", "öù", "öø", "öû", "öõ",
                                   "öï", "ö", "eá", "eà", "eå", "eã", "eä", "eâ", "eù", "eø", "eû", "eõ", "eï",
                                   "oá", "oà", "oå", "oã", "oä", "oû", "oõ", "oï", "ôù", "ôø",
                                   "ôû", "ôõ", "ôï", "ô", "í", "ì", "æ", "ó", "ò", "yù", "yø", "yû", "yõ", "î",
                                   "ñ", "AÁ", "AÀ", "AÅ", "AÃ", "AÄ", "AÂ", "AÛ", "AÕ",
                                   "AÏ", "AÉ", "AÈ", "AÚ", "AÜ", "AË", "AÊ", "UÙ", "UØ", "UÛ", "UÕ",
                                   "UÏ", "ÖÙ", "ÖØ", "ÖÛ", "ÖÕ", "ÖÏ", "Ö", "EÁ", "EÀ", "EÅ",
                                   "EÃ", "EÄ", "EÂ", "EÙ", "EØ", "EÛ", "EÕ", "EÏ", "OÁ", "OÀ", "OÅ",
                                   "OÃ", "OÄ", "OÛ", "OÕ", "OÏ", "ÔÙ", "ÔØ", "ÔÛ",
                                   "ÔÕ", "ÔÏ", "Ô", "Í", "Ì", "Æ", "Ó", "Ò", "YÙ", "YØ", "YÛ", "YÕ",
                                   "Î", "Ñ", "aù", "aø", "oâ", "où", "oø", "AÙ", "AØ", "OÂ", "OÙ", "OØ"
                               };

            var result = strInput;
            for (var i = 0; i < 134; i++)
            {
                result = result.Replace(vni[i], Convert.ToChar(maAcii[i]).ToString());
            }

            return result;
        }

        // Convert Unicode Font To Vni Font
        public static string ConvertUnicode2Vni(string strInput)
        {
            if (string.IsNullOrEmpty(strInput))
            {
                return null;
            }

            var maAcii = new[]
                               {
                                   7845, 7847, 7849, 7851, 7853, 226, 7843, 227, 7841, 7855, 7857, 7859, 7861, 7863, 259,
                                   250, 249, 7911, 361, 7909, 7913, 7915, 7917, 7919, 7921, 432, 7871, 7873, 7875, 7877,
                                   7879, 234, 233, 232, 7867, 7869, 7865, 7889, 7891, 7893, 7895, 7897, 7887, 245, 7885,
                                   7899, 7901, 7903, 7905, 7907, 417, 237, 236, 7881, 297, 7883, 253, 7923, 7927, 7929,
                                   7925, 273, 7844, 7846, 7848, 7850, 7852, 194, 7842, 195, 7840, 7854, 7856, 7858, 7860,
                                   7862, 258, 218, 217, 7910, 360, 7908, 7912, 7914, 7916, 7918, 7920, 431, 7870, 7872,
                                   7874, 7876, 7878, 202, 201, 200, 7866, 7868, 7864, 7888, 7890, 7892, 7894, 7896, 7886,
                                   213, 7884, 7898, 7900, 7902, 7904, 7906, 416, 205, 204, 7880, 296, 7882, 221, 7922, 7926
                                   , 7928, 7924, 272, 225, 224, 244, 243, 242, 193, 192, 212, 211, 210
                               };
            var vni = new[]
                               {
                                   "aá", "aà", "aå", "aã", "aä", "aâ", "aû", "aõ", "aï", "aé", "aè",
                                   "aú", "aü", "aë", "aê", "uù", "uø", "uû", "uõ", "uï", "öù", "öø", "öû", "öõ",
                                   "öï", "ö", "eá", "eà", "eå", "eã", "eä", "eâ", "eù", "eø", "eû", "eõ", "eï",
                                   "oá", "oà", "oå", "oã", "oä", "oû", "oõ", "oï", "ôù", "ôø",
                                   "ôû", "ôõ", "ôï", "ô", "í", "ì", "æ", "ó", "ò", "yù", "yø", "yû", "yõ", "î",
                                   "ñ", "AÁ", "AÀ", "AÅ", "AÃ", "AÄ", "AÂ", "AÛ", "AÕ",
                                   "AÏ", "AÉ", "AÈ", "AÚ", "AÜ", "AË", "AÊ", "UÙ", "UØ", "UÛ", "UÕ",
                                   "UÏ", "ÖÙ", "ÖØ", "ÖÛ", "ÖÕ", "ÖÏ", "Ö", "EÁ", "EÀ", "EÅ",
                                   "EÃ", "EÄ", "EÂ", "EÙ", "EØ", "EÛ", "EÕ", "EÏ", "OÁ", "OÀ", "OÅ",
                                   "OÃ", "OÄ", "OÛ", "OÕ", "OÏ", "ÔÙ", "ÔØ", "ÔÛ",
                                   "ÔÕ", "ÔÏ", "Ô", "Í", "Ì", "Æ", "Ó", "Ò", "YÙ", "YØ", "YÛ", "YÕ",
                                   "Î", "Ñ", "aù", "aø", "oâ", "où", "oø", "AÙ", "AØ", "OÂ", "OÙ", "OØ"
                               };

            var result = strInput;
            for (var i = 0; i < 134; i++)
            {
                result = result.Replace(Convert.ToChar(maAcii[i]).ToString(), vni[i]);
            }
            return result;
        }

        /// <summary>
        /// Convert String to URL Format
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ConvertToUrlString(string str)
        {
            str = (new Regex(@"[-]{2}")).Replace((new Regex(@"[^\w]")).Replace(Ucs2Convert(str.ToLower()), "-"), "-");

            return str;
        }

        public static void CheckExistsAndCreateFolder(string folder)
        {
            if (!new DirectoryInfo(folder).Exists)
            {
                new DirectoryInfo(folder).Create();
            }
        }

        public static int WeeksInYear(DateTime date)
        {
            GregorianCalendar cal = new GregorianCalendar(GregorianCalendarTypes.Localized);
            return cal.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }

        public static int GetQuater(DateTime date)
        {
            return (date.Month - 1) / 3 + 1;
        }

        public static string ToHashTag(string tag)
        {
            return Ucs2Convert(tag).Replace(" ", "").Replace("-", "").Replace(".", "").ToLower();
        }

        public static void GetStartDateEndDateOfWeek(DateTime date, out DateTime startOfWeek, out DateTime endOfWeek, DayOfWeek dayOfWeek)
        {
            int offset = date.DayOfWeek - dayOfWeek;
            startOfWeek = date.AddDays(-offset);
            endOfWeek = startOfWeek.AddDays(6);
        }

        public static void GenerateAvatar(string firstName, string lastName, string pathName)
        {
            var backgroundColours = new List<string> { "3C79B2", "FF8F88", "6FB9FF", "C0CC44", "AFB28C", "E8740C", "0CE8E3", "BA1AE8", "85E807", "E8A907", "449CE8", "FF0097" };
            string avatarString;

            if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName))
            {
                throw new Exception("\"FirstName\" and \"LastName\" is null");
            }

            if (string.IsNullOrWhiteSpace(pathName))
            {
                throw new Exception("\"PathName\" is null");
            }

            if (!string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName))
            {
                avatarString = firstName[0].ToString().ToLower();
            }
            else if (string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
            {
                avatarString = lastName[0].ToString().ToLower();
            }
            else
            {
                avatarString = string.Format("{0}{1}", firstName[0], lastName[0]).ToUpper();
            }

            var randomIndex = new Random().Next(0, backgroundColours.Count - 1);
            var bgColour = backgroundColours[randomIndex];

            var bmp = new Bitmap(192, 192);
            var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            var font = new Font("Arial", 70, FontStyle.Bold, GraphicsUnit.Pixel);
            var graphics = Graphics.FromImage(bmp);

            graphics.Clear((Color)new ColorConverter().ConvertFromString("#" + bgColour));
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            graphics.DrawString(avatarString, font, new SolidBrush(Color.WhiteSmoke), new RectangleF(0, 0, 192, 192), sf);
            graphics.Flush();

            bmp.Save(pathName, ImageFormat.Png);
        }

        /// <summary>
        /// Convert list object to Xml
        /// </summary>
        /// <typeparam name="T">Kiểu object</typeparam>
        /// <param name="listItem">List object</param>
        /// <param name="elementName">Tên thẻ</param>
        /// <returns></returns>
        public static string ConvertToXml<T>(List<T> listItem, string elementName)
        {
            if (listItem == null || listItem.Count <= 0) return string.Empty;

            var xDoc = new XDocument();
            var xElement = new XElement("Root");
            xDoc.Add(xElement);

            listItem.ForEach(x =>
            {
                var listProp = new List<object>();
                x.GetType().GetProperties().ToList().ForEach(prop =>
                {
                    listProp.Add(new XElement(prop.Name, prop.GetValue(x)));
                });

                var xItem = new XElement(elementName, listProp);
                xElement.Add(xItem);
            });

            return xDoc.ToString();
        }

        public static string ToRoman(int number)
        {
            if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900); //EDIT: i've typed 400 instead 900
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            throw new ArgumentOutOfRangeException("something bad happened");
        }

        public static string ReturnCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return code;
            }

            if (code.Contains("-"))
            {
                return code;
            }

            var type = byte.Parse(code.Substring(code.Length - 1, 1));
            if (type == (byte)OrderType.Order)
                return "ORD" + code;
            if (type == (byte)OrderType.Deposit)
                return "DEP" + code;
            if (type == (byte)OrderType.Commerce)
                return "COM" + code;
            if (type == (byte)OrderType.Source)
                return "SOU" + code;
            if (type == (byte)OrderType.StockQuotes)
                return "STO" + code;
            return code;
        }

        /// <summary>
        /// Hàm sinh mã khách hàng dạng bắt đầu bằng chữ cái theo số tự tự của khách hàng
        /// mã dạng: [A-Z]-STT
        /// STT từ 1 -> 999
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string GenCode(long number)
        {
            if(number < 0)
                throw new Exception("Requirements input must be greater than 0");

            var str = "ABCDEFGHIJKLMNPQRSTUVWXYZ";

            var maxNum = 999;

            int idx = (int)((decimal)number / maxNum);

            if (number % maxNum == 0)
            {
                idx -= 1;
            }
            if (idx > str.Length - 1)
                idx = str.Length - 1;

            return $"{str[idx]}{number - (idx * maxNum)}";
        }

        /// <summary>
        /// Lấy các kí tự đầu tiên của mỗi từ trong một đoạn string
        /// VD: "ví Dụ" -> "VD"
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        public static string FirstCharOfWords(string words)
        {
            if (string.IsNullOrWhiteSpace(words))
                return "";

            words = Ucs2Convert(words).Replace("  ", " ");

            words = string.Join("", words.Split(' ').Select(x => x[0]));

            var rgx = new Regex("[^a-zA-Z]");

            words = rgx.Replace(words, "").ToUpper();

            return words;
        }

        public static string RemoveCode(string code)
        {
            return code.Trim().ToUpper().Replace("ORD", "").Replace("DEP", "").Replace("COM", "").Replace("SOU", "").Replace("STO", "");
        }

        public static string RemoveHash(string str, string hash)
        {
            if (string.IsNullOrWhiteSpace(str) || string.IsNullOrWhiteSpace(hash))
                return str;

            var firstIdx = str.IndexOf(hash, StringComparison.OrdinalIgnoreCase);
            var lastIdx = str.LastIndexOf(hash, StringComparison.OrdinalIgnoreCase);

            if (firstIdx < 0 || lastIdx < 0 || firstIdx == lastIdx) return str;

            var begin = lastIdx + hash.Length;
            str = str.Substring(0, firstIdx) + str.Substring(begin, str.Length - begin);

            if (string.IsNullOrWhiteSpace(str))
                return str;

            return str.Replace("  ", " ");
        }
    }
}
