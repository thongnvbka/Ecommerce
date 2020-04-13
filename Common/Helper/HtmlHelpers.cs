using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Common.Helper
{
    public static class HtmlHelpers
    {
        private class ScriptBlock : IDisposable
        {
            private const string ScriptsKey = "scripts";
            public static List<string> PageScripts
            {
                get
                {
                    if (HttpContext.Current.Items[ScriptsKey] == null)
                        HttpContext.Current.Items[ScriptsKey] = new List<string>();
                    return (List<string>)HttpContext.Current.Items[ScriptsKey];
                }
            }

            readonly WebViewPage _webPageBase;

            public ScriptBlock(WebViewPage webPageBase)
            {
                _webPageBase = webPageBase;
                _webPageBase.OutputStack.Push(new StringWriter());
            }

            public void Dispose()
            {
                PageScripts.Add(((StringWriter)_webPageBase.OutputStack.Pop()).ToString());
            }
        }

        public static IDisposable BeginScripts(this HtmlHelper helper)
        {
            return new ScriptBlock((WebViewPage)helper.ViewDataContainer);
        }

        public static MvcHtmlString PageScripts(this HtmlHelper helper)
        {
            return MvcHtmlString.Create(string.Join(Environment.NewLine, ScriptBlock.PageScripts.Select(s => s.ToString())));
        }
    }
}