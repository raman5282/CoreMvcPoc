using System;
namespace ServiceManager
{
	public class ResponseData<T>
	{
		#region Properties

		public bool Success { get; set; }
		public string Message { get; set; }
		public T ResponseObject { get; set; }
		public bool IsError	{ get;set; }

		#endregion Properties
	}
}

