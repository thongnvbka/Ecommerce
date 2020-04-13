using System;
using System.IO;
using System.Text;
using System.Web;

namespace Common.Host
{
    public class OutputLog
    {
        public static void WriteOutputLog(Exception ex)
        {
            WriteOutputLog(HttpContext.Current, ex, string.Empty);
        }

        public static void WriteOutputLog(Exception ex, string text)
        {
            WriteOutputLog(HttpContext.Current, ex, text);
        }

        public static void WriteOutputLog(HttpContext context, Exception ex)
        {
            WriteOutputLog(HttpContext.Current, ex, string.Empty);
        }
        public static void WriteOutputLog(string text)
        {
            var context = HttpContext.Current;
            var dt = DateTime.Now;
            string lDir;
            string lFile;
            var sbLog = new StringBuilder();
            sbLog.AppendFormat("--------- {0:HH:mm:ss} ---------", dt).AppendLine();
            if (context != null)
            {
                lDir = context.Request.MapPath(string.Format("/Log"));
                lFile = context.Request.MapPath(string.Format("/Log/Log_{0:yyyyMMdd}.txt", dt));

                sbLog.AppendFormat("Ref URL: {0}",
                                   (context.Request.UrlReferrer == null)
                                       ? ""
                                       : context.Request.UrlReferrer.ToString()).AppendLine();
                sbLog.AppendFormat("Host: {0}", context.Request.Url.Authority).AppendLine();
                sbLog.AppendFormat("URL: {0}", context.Request.RawUrl).AppendLine();
                sbLog.AppendFormat("IP: {0}", context.Request.UserHostAddress).AppendLine();
            }
            else
            {
                lDir = System.Web.Hosting.HostingEnvironment.MapPath(string.Format("/Log"));
                lFile = System.Web.Hosting.HostingEnvironment.MapPath(string.Format("/Log/Log_{0:yyyyMMdd}.txt", dt));
            }
            if (lDir == null || lFile == null) return;
            if (!Directory.Exists(lDir))
                Directory.CreateDirectory(lDir);
            if (!string.IsNullOrEmpty(text))
                sbLog.AppendFormat("Text: {0}", text).AppendLine();
            File.AppendAllText(lFile, sbLog.ToString());
        }
        public static void WriteOutputLog(HttpContext context, Exception ex, string text)
        {
            var dt = DateTime.Now;
            string lDir;
            string lFile;
            var sbLog = new StringBuilder();
            sbLog.AppendFormat("--------- {0:HH:mm:ss} ---------", dt).AppendLine();
            if (context != null)
            {
                lDir = context.Request.MapPath(string.Format("/Log"));
                lFile = context.Request.MapPath(string.Format("/Log/Log_{0:yyyyMMdd}.txt", dt));

                sbLog.AppendFormat("Ref URL: {0}",
                                   (context.Request.UrlReferrer == null)
                                       ? ""
                                       : context.Request.UrlReferrer.ToString()).AppendLine();
                sbLog.AppendFormat("Host: {0}", context.Request.Url.Authority).AppendLine();
                sbLog.AppendFormat("URL: {0}", context.Request.RawUrl).AppendLine();
                sbLog.AppendFormat("IP: {0}", context.Request.UserHostAddress).AppendLine();
            }
            else
            {
                lDir = System.Web.Hosting.HostingEnvironment.MapPath(string.Format("/Log"));
                lFile = System.Web.Hosting.HostingEnvironment.MapPath(string.Format("/Log/Log_{0:yyyyMMdd}.txt", dt));
            }
            if (lDir == null || lFile == null) return;
            if (!Directory.Exists(lDir))
                Directory.CreateDirectory(lDir);
            sbLog.AppendFormat("Error: {0}", ex.Message).AppendLine(); ;
            if (!string.IsNullOrEmpty(text))
                sbLog.AppendFormat("Text: {0}", text).AppendLine();
            sbLog.AppendFormat("Source: {0}", ex.Source).AppendLine();
            sbLog.AppendFormat("Trace: {0}", ex.StackTrace).AppendLine();
            sbLog.AppendFormat("InnerException : {0}", ex.InnerException).AppendLine();
            sbLog.AppendFormat("GetBaseException : {0}", ex.GetBaseException()).AppendLine();
            sbLog.AppendFormat("TargetSite : {0}", ex.TargetSite).AppendLine();
            File.AppendAllText(lFile, sbLog.ToString());
        }
    }
}
