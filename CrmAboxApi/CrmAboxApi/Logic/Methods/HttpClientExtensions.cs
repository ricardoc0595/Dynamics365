using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace CrmAboxApi.Logic.Methods
{
    public static class HttpClientExtensions
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public async static Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri, HttpContent content)
        {
            var method = new HttpMethod("PATCH");


            
            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = content
            };

            return await client.SendAsync(request).ConfigureAwait(false);
        }

        public async static Task<HttpResponseMessage> PatchAsync(this HttpClient client, Uri requestUri, HttpContent content)
        {
            var method = new HttpMethod("PATCH");

           
            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = content
            };

            return await client.SendAsync(request);
        }

        public async static Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            var method = new HttpMethod("PATCH");
           
            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = content
            };

            return await client.SendAsync(request, cancellationToken);
        }

        public async static Task<HttpResponseMessage> PatchAsync(this HttpClient client, Uri requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            var method = new HttpMethod("PATCH");
            
            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = content
            };

            return await client.SendAsync(request, cancellationToken);
        }
    }

}