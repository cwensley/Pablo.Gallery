using System;
using System.Web.Mvc;
using System.Web;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pablo.Gallery
{
	public static class HtmlHelpers
	{
		class ScriptBlock : IDisposable
		{
			const string scriptsKey = "scripts";

			public static List<string> PageScripts
			{
				get
				{
					if (HttpContext.Current.Items[scriptsKey] == null)
						HttpContext.Current.Items[scriptsKey] = new List<string>();
					return (List<string>)HttpContext.Current.Items[scriptsKey];
				}
			}

			readonly WebViewPage webPageBase;

			public ScriptBlock(WebViewPage webPageBase)
			{
				this.webPageBase = webPageBase;
				this.webPageBase.OutputStack.Push(new StringWriter());
			}

			public void Dispose()
			{
				PageScripts.Add(((StringWriter)webPageBase.OutputStack.Pop()).ToString());
			}
		}

		public static IDisposable BeginScripts(this HtmlHelper helper)
		{
			return new ScriptBlock((WebViewPage)helper.ViewDataContainer);
		}

		public static MvcHtmlString PageScripts(this HtmlHelper helper)
		{
			return MvcHtmlString.Create(string.Join(Environment.NewLine, ScriptBlock.PageScripts));
		}
	}
}

