using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using SevenDigital.Api.Wrapper.EndpointResolution;

namespace SevenDigital.Api.Wrapper.Utility.Http
{
	public class HttpClient : IHttpClient
	{
		public IResponse Get(IRequest request)
		{
			var webRequest = MakeWebRequest(request);

			WebResponse webResponse;
			try
			{
				webResponse = webRequest.GetResponse();
			}
			catch (WebException ex)
			{
				if (ex.Response == null)
				{
					throw;
				}
				webResponse = ex.Response;
			}

			return MakeResponse(webResponse);
		}

		public void GetAsync(IRequest request, Action<IResponse> callback)
		{
			var webRequest = MakeWebRequest(request);
			webRequest.BeginGetResponse(iar => callback(GetAsyncResponse(iar)), webRequest);
		}

		private IResponse GetAsyncResponse(IAsyncResult iar)
		{
			var webRequest = (WebRequest)iar.AsyncState;

			WebResponse webResponse;
			try
			{
				webResponse = webRequest.EndGetResponse(iar);
			}
			catch (WebException ex)
			{
				if (ex.Response == null)
				{
					throw;
				}
				webResponse = ex.Response;
			}

			return MakeResponse(webResponse);
		}

		public IResponse Post(IRequest request)
		{
			var webRequest = MakePostRequest(request);

			WebResponse webResponse;
			try
			{
				webResponse = webRequest.GetResponse();
			}
			catch (WebException ex)
			{
				if (ex.Response == null)
				{
					throw;
				}
				webResponse = ex.Response;
			}

			return MakeResponse(webResponse);
		}

		public void PostAsync(IRequest request, Action<IResponse> callback)
		{
			var webRequest = MakePostRequest(request);

			webRequest.BeginGetResponse(iar => callback(GetAsyncResponse(iar)), webRequest);
		}

		private static HttpWebRequest MakeWebRequest(IRequest request)
		{
			var webRequest = (HttpWebRequest)WebRequest.Create(request.Url);
			webRequest.Method = "GET";
			webRequest.UserAgent = "7digital .Net Api Wrapper";

			foreach (var header in request.Headers)
			{
				webRequest.Headers.Add(header.Key, header.Value);
			}
			return webRequest;
		}

		private IResponse MakeResponse(WebResponse webResponse)
		{

			string output;
			using (var sr = new StreamReader(webResponse.GetResponseStream()))
			{
				output = sr.ReadToEnd();
			}

			var response = new Response
			{
				StatusCode = ReadStatusCode(webResponse),
				Headers = MapHeaders(webResponse.Headers),
				Body = output
			};

			webResponse.Close();

			return response;
		}

		private static HttpWebRequest MakePostRequest(IRequest request)
		{
			var webRequest = MakeWebRequest(request);
			webRequest.Method = "POST";
			webRequest.ContentType = "application/x-www-form-urlencoded";

			var postData = request.Parameters.ToQueryString();
			var postBytes = Encoding.UTF8.GetBytes(postData);
			webRequest.ContentLength = postBytes.Length;

			using (Stream dataStream = webRequest.GetRequestStream())
			{
				dataStream.Write(postBytes, 0, postBytes.Length);
			}
			return webRequest;
		}

		public Dictionary<string, string> MapHeaders(WebHeaderCollection headerCollection)
		{
			var headers = new Dictionary<string, string>();

			for (var i = 0; i < headerCollection.Count; i++)
			{
				headers.Add(headerCollection.GetKey(i), string.Join(",", headerCollection.GetValues(i)));
			}

			return headers;
		}

		private static HttpStatusCode ReadStatusCode(WebResponse webResponse)
		{
			HttpWebResponse httpResponse = webResponse as HttpWebResponse;
			if (httpResponse == null)
			{
				return HttpStatusCode.NoContent;
			}

			return httpResponse.StatusCode;
		}
	}
}