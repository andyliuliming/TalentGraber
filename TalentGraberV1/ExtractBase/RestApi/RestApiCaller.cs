using GithubGraberLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ExtractBase.RestApi
{
    public class RestApiCaller<T> where T : class
    {
        private string baseUrl;
        public RestApiCaller(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public T CallApi(string method, string accessToken, string urlParameters, T body)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(this.baseUrl);

            if (!string.IsNullOrEmpty(accessToken))
            {
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
                    response = client.GetAsync(urlParameters).Result;
                    result = response.Content.ReadAsAsync<T>().Result;
                    return result;
                case "post":
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        client.DefaultRequestHeaders.Add(AuthorizeUtil.TokenHeaderName, accessToken);
                    }
                    response = client.PostAsJsonAsync<T>(urlParameters, body).Result;
                    result = response.Content.ReadAsAsync<T>().Result;
                    return result;
                default:
                    throw new NotImplementedException();
            }
        }

        public T Post(string urlParameters, string accessToken, T body)
        {
            return this.CallApi("Post", accessToken, urlParameters, body);
        }

        public T Get(string urlParameters, string accessToken)
        {
            return this.CallApi("Get", accessToken, urlParameters, null);
        }
    }
}
