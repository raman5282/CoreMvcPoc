using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;

namespace ServiceManager
{
	public class WebServiceManager
	{
		readonly HttpClient client;
		private long maxResponseBufferSize = 256000;
		private double timeoutInSecs = 120;
		private string requestHeaderMediaTypeJson = "application/json";

		public long MaxResponseBufferSize
		{
			set {
				maxResponseBufferSize = value;
				SetSesponseBuffer();
			}
		}

		public WebServiceManager()
		{
			this.client = new HttpClient();
			SetSesponseBuffer();
			this.client.Timeout = TimeSpan.FromSeconds(timeoutInSecs);
		}
        public WebServiceManager(INetworkHandler handler, string fiddlerBaseUrl) : base()
        {
            if(handler == null)
            {
                this.client = new HttpClient() { BaseAddress = new Uri(fiddlerBaseUrl) };
            }
            else
            {
                var handle = handler.GetNetworkHandler();
                this.client = new HttpClient(handle) { BaseAddress = new Uri(fiddlerBaseUrl) };
            }

            SetSesponseBuffer();

        }

        public WebServiceManager(INetworkHandler handler, string fiddlerBaseUrl,long maxResponseContentBufferSize, double timeoutInSeconds, string requestHeaderMediaType) : this()
        {
            if (handler == null)
            {
                this.client = new HttpClient() { BaseAddress = new Uri(fiddlerBaseUrl) };
            }
            else
            {
                var handle = handler.GetNetworkHandler();
                this.client = new HttpClient(handle) { BaseAddress = new Uri(fiddlerBaseUrl) };
            }
            this.client.MaxResponseContentBufferSize = maxResponseContentBufferSize;
            this.client.Timeout = TimeSpan.FromSeconds(timeoutInSeconds);
            this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(requestHeaderMediaType));
        }

        private void SetSesponseBuffer()
		{ 
			this.client.MaxResponseContentBufferSize = maxResponseBufferSize;
		}

		public WebServiceManager(long maxResponseContentBufferSize, double timeoutInSeconds, string requestHeaderMediaType):this()
		{
			this.client.MaxResponseContentBufferSize = maxResponseContentBufferSize;
			this.client.Timeout = TimeSpan.FromSeconds(timeoutInSeconds);
			this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(requestHeaderMediaType));
		}

		private void AddHeaders(List<RequestHeaderItem> headers)
		{
			client.DefaultRequestHeaders.Clear();
			this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(requestHeaderMediaTypeJson));
			
			if (headers != null && headers.Count > 0)
			{
				foreach (var header in headers)
				{
					if (header.HeaderItemType == HeaderItemType.Authorization)
					{
						client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(header.Key
																														   , header.Value);
					}
					else
					{
						client.DefaultRequestHeaders.Add(header.Key, header.Value);
					}
				}
			}
		}

		public async Task<ResponseData<T>> Get<T>(string requestUrl, List<RequestHeaderItem> headers = null)
		{
			ResponseData<T> returnValue = new ResponseData<T>();
			var uri = new Uri(requestUrl);
			try
			{
				AddHeaders(headers);
				var response = await client.GetAsync(uri);
				if (response != null)
				{
					returnValue.Success = response.IsSuccessStatusCode;
					if (returnValue.Success)
					{
						var responseString = await response.Content.ReadAsStringAsync();
						returnValue.ResponseObject = JsonHelper.DeSerialize<T>(responseString);
					}
					else
					{
						returnValue.Message = response.ReasonPhrase;
					}
				}
			}
			catch (Exception ex)
			{
				returnValue.Success = false;
				returnValue.IsError = true;
				returnValue.Message = ex.Message;
			}
			return returnValue;
		}

		public async Task<ResponseData<T>> Post<T, P>(string requestUrl, P postData, List<RequestHeaderItem> headers = null)
		{
			ResponseData<T> returnValue = new ResponseData<T>();
			var uri = new Uri(requestUrl);
			try
			{
				var json = JsonHelper.Serialize(postData);
				AddHeaders(headers);
				var content = new StringContent(json, Encoding.UTF8, requestHeaderMediaTypeJson);
				var response = await client.PostAsync(uri, content);
				if (response != null)
				{
					returnValue.Success = response.IsSuccessStatusCode;
					if (returnValue.Success)
					{
						var responseString = await response.Content.ReadAsStringAsync();
						returnValue.ResponseObject = JsonHelper.DeSerialize<T>(responseString);
					}
					else
					{
						returnValue.Message = response.ReasonPhrase;
					}
				}
			}
			catch (Exception ex)
			{
				returnValue.Success = false;
				returnValue.IsError = true;
				returnValue.Message = ex.Message;
			}
			return returnValue;
		}

		public async Task<ResponseData<T>> Post<T>(string requestUrl, List<RequestHeaderItem> headers = null)
		{
			ResponseData<T> returnValue = new ResponseData<T>();
			var uri = new Uri(requestUrl);
			try
			{
				AddHeaders(headers);
				var response = await client.PostAsync(uri, null);
				if (response != null)
				{
					returnValue.Success = response.IsSuccessStatusCode;
					if (returnValue.Success)
					{
						returnValue.ResponseObject = JsonHelper.DeSerialize<T>(await response.Content.ReadAsStringAsync());
					}
					else
					{
						returnValue.Message = response.ReasonPhrase;
					}
				}
			}
			catch (Exception ex)
			{
				returnValue.Success = false;
				returnValue.IsError = true;
				returnValue.Message = ex.Message;
			}
			return returnValue;
		}

		public async Task<ResponseData<T>> Put<T, P>(string requestUrl, P postData, List<RequestHeaderItem> headers = null)
		{
			ResponseData<T> returnValue = new ResponseData<T>();
			var uri = new Uri(requestUrl);
			try
			{
				var json = JsonHelper.Serialize(postData);
				var content = new StringContent(json, Encoding.UTF8, requestHeaderMediaTypeJson);
				AddHeaders(headers);
				var response = await client.PutAsync(uri, content);
				if (response != null)
				{
					returnValue.Success = response.IsSuccessStatusCode;
					if (returnValue.Success)
					{
						var responseString = await response.Content.ReadAsStringAsync();
						returnValue.ResponseObject = JsonHelper.DeSerialize<T>(responseString);
					}
					else
					{
						returnValue.Message = response.ReasonPhrase;
					}
				}
			}
			catch (Exception ex)
			{
				returnValue.Success = false;
				returnValue.IsError = true;
				returnValue.Message = ex.Message;
			}
			return returnValue;
		}

		public async Task<ResponseData<T>> Put<T>(string requestUrl, List<RequestHeaderItem> headers = null)
		{
			ResponseData<T> returnValue = new ResponseData<T>();
			var uri = new Uri(requestUrl);
			try
			{
				AddHeaders(headers);
				var response = await client.PutAsync(uri, null);
				if (response != null)
				{
					returnValue.Success = response.IsSuccessStatusCode;
					if (returnValue.Success)
					{
						var responseString = await response.Content.ReadAsStringAsync();
						returnValue.ResponseObject = JsonHelper.DeSerialize<T>(responseString);
					}
					else
					{
						returnValue.Message = response.ReasonPhrase;
					}
				}
			}
			catch (Exception ex)
			{
				returnValue.Success = false;
				returnValue.IsError = true;
				returnValue.Message = ex.Message;
			}
			return returnValue;
		}

		public async Task<ResponseData<bool>> Delete(string requestUrl, List<RequestHeaderItem> headers = null)
		{
			ResponseData<bool> returnValue = new ResponseData<bool>();
			var uri = new Uri(requestUrl);
			try
			{
				client.BaseAddress = uri;
				AddHeaders(headers);
				var response = await client.DeleteAsync(requestUrl);
				if (response != null)
				{
					returnValue.Success = response.IsSuccessStatusCode;
					if (returnValue.Success)
					{
						returnValue.ResponseObject = true;
					}
					else
					{
						returnValue.Message = response.ReasonPhrase;
					}
				}
			}
			catch (Exception ex)
			{
				returnValue.Success = false;
				returnValue.IsError = true;
				returnValue.ResponseObject = false;
				returnValue.Message = ex.Message;
			}
			return returnValue;
		}


	}
}

