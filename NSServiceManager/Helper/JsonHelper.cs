using System;
using Newtonsoft.Json;

namespace ServiceManager
{
	public static class JsonHelper
	{
		public static string Serialize<T>(T obj)
		{
			return JsonConvert.SerializeObject(obj);
		}

		public static T DeSerialize<T>(string value)
		{
			return JsonConvert.DeserializeObject<T>(value);
		}
	}
}

