using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laika.Web
{
	public static class Base64
	{
		public static string ToBase64String(byte[] data)
		{
			return Convert.ToBase64String(data);
		}

		public static byte[] FromBase64String(string data)
		{
			return Convert.FromBase64String(data);
		}
	}
}
