using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Pablo.Gallery.Logic.Converters
{
	public class ConvertInfo
	{
		public Models.Pack Pack { get; set; }

		public Func<string, Task> ExtractFile { get; set; }

		public string FileName { get; set; }

		public string InputFormat { get; set; }

		public string InputType { get; set; }

		public string OutFileName { get; set; }

		public NameValueCollection Properties { get; set; }

		public T GetProperty<T>(string key)
		{
			var val = Properties[key];
			return string.IsNullOrEmpty(val) ? default(T) : (T)Convert.ChangeType(val, Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T));
		}
	}

	public abstract class Converter
	{
		public abstract bool CanConvert(ConvertInfo info);

		public abstract Task<Stream> Convert(ConvertInfo info);

		public virtual void Prepare(ConvertInfo info)
		{
		}
	}
}