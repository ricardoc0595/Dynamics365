﻿using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Runtime.Caching;
namespace Logic.CrmAboxApi.Classes.Helper
{
    /// <summary>
    ///Custom HTTP message handler that uses OAuth authentication through ADAL.
    /// </summary>
    public class OAuthMessageHandler : DelegatingHandler
    {
        private AuthenticationHeaderValue authHeader;
        private AuthenticationParameters ap;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        
        public OAuthMessageHandler(string serviceUrl, string clientId, string redirectUrl, string username, string password,
                HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            ObjectCache cache = MemoryCache.Default;
            var token = cache["AuthToken"] as string;
            // Obtain the Azure Active Directory Authentication Library (ADAL) authentication context.
            //AuthenticationParameters ap = AuthenticationParameters.CreateFromResourceUrlAsync(
            //        new Uri(serviceUrl + "/api/data/")).Result;

            //this.DiscoveryAuthority2(serviceUrl).Wait();

            //Task<AuthenticationParameters> t = AuthenticationParameters.CreateFromResourceUrlAsync(
            //        new Uri(serviceUrl + "/api/data/"));
            if (token==null)
            {
                var task = Task.Run(async () => { await this.DiscoveryAuthority2(serviceUrl); });
                task.Wait();
                AuthenticationParameters ap = this.ap;

                AuthenticationContext authContext = new AuthenticationContext(ap.Authority, false);
                //Note that an Azure AD access token has finite lifetime, default expiration is 60 minutes.
                AuthenticationResult authResult;
                if (username != string.Empty && password != string.Empty)
                {
                    UserCredential cred = new UserCredential(username, password);
                    authResult = authContext.AcquireToken(serviceUrl, clientId, cred);
                }
                else
                {
                    authResult = authContext.AcquireToken(serviceUrl, clientId, new Uri(redirectUrl), PromptBehavior.Auto);
                }

                CacheItemPolicy policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(50);
                cache.Set("AuthToken", authResult.AccessToken, policy);

                authHeader = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);


            }
            else
            {
                authHeader = new AuthenticationHeaderValue("Bearer", token);
            }
           

            
            //Logger.Debug($"AuthorizationToken: {authResult.AccessToken}");
        }

        private async Task DiscoveryAuthority2(string serviceUrl)
        {
            try
            {
                //AuthenticationParameters t = await AuthenticationParameters.CreateFromResourceUrlAsync(
                //    new Uri(serviceUrl + "/api/data/"));
                this.ap = await AuthenticationParameters.CreateFromResourceUrlAsync(
                    new Uri(serviceUrl + "/api/data/")).ConfigureAwait(false);
                string s = "";
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        protected override Task<HttpResponseMessage> SendAsync(
                  HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            request.Headers.Authorization = authHeader;
            return base.SendAsync(request, cancellationToken);
        }
    }
}