using GithubGraberLib;
using Macrodeek.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorkerRole;

namespace ExtractBase.RestApi
{
    public class HttpException : Exception
    {
        private HttpStatusCode statusCode = 0;
        private string msg = null;

        public HttpStatusCode StatusCode
        {
            get
            {
                return statusCode;
            }

            set
            {
                statusCode = value;
            }
        }

        public string Msg
        {
            get
            {
                return msg;
            }

            set
            {
                msg = value;
            }
        }

        public HttpException(HttpStatusCode statusCode, string msg)
        {
            this.StatusCode = statusCode;
            this.Msg = msg;
        }
    }
    public class RestApiCaller<T> where T : class
    {
        private string baseUrl;
        public RestApiCaller(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }
        private AccountResource accountResource;

        public AccountResource AccountResource
        {
            get
            {
                return accountResource;
            }

            set
            {
                accountResource = value;
            }
        }

        string accessToken = null;
        public T Get(string urlParameters, bool retryUntil)
        {
            return this.CallApi("Get", urlParameters, retryUntil);
        }

        public T CallApi(string method, string urlParameters, bool retryForever = true)
        {
            if (retryForever)
            {
                return this.RetryForOutOfResource(() =>
                {
                    return CallRun(method, urlParameters);
                }, () =>
                {
                    this.accessToken = accountResource.GetNextAccessToken();
                });
            }
            else
            {
                return CallRun(method, urlParameters);
            }
        }

        private T RetryForOutOfResource(Func<T> action, Action outOfResourceFallback)
        {
            T t = null;
            while (true)
            {
                try
                {
                    t = action();
                    break;
                }
                catch (TalentGraberException e)
                {
                    Trace.TraceError(e.Message);
                    if (e.StatusCode == HttpStatusCode.Forbidden
                        || e.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        outOfResourceFallback();
                        Thread.Sleep(30000);
                    }
                    if (e.StatusCode == HttpStatusCode.NotFound)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.ToString());
                    return null;
                }
            }
            return t;
        }

        private T CallRun(string method, string urlParameters)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(this.baseUrl);
            if (accountResource != null)
            {
                accessToken = accountResource.GetCurrentAccessToken();
                client.DefaultRequestHeaders.Add(AuthorizeUtil.TokenHeaderName, "Basic " + accessToken);
            }
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");

            // Add an Accept header for JSON format.
            HttpResponseMessage response = null;
            T result = null;
            switch (method.ToLower())
            {
                case "get":
                    Task<HttpResponseMessage> responseTask = client.GetAsync(urlParameters);
                    responseTask.Wait();
                    response = responseTask.Result;
                    if (response.StatusCode == HttpStatusCode.Accepted
                        || response.StatusCode == HttpStatusCode.OK)
                    {
                        Task<T> task = response.Content.ReadAsAsync<T>();
                        task.Wait();
                        result = task.Result;
                        return result;
                    }
                    else
                    {
                        string headerMessage = string.Empty;
                        foreach (var header in response.Headers)
                        {
                            headerMessage += string.Format("Key:{0},Value:{1}", header.Key, header.Value);
                        }
                        throw new TalentGraberException(response.StatusCode, headerMessage);
                    }
                default:
                    throw new NotImplementedException();
            }
        }

    }
}
