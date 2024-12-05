
using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;

namespace clientLibrary.Helpers
{
    public class GetHttpClient (IHttpClientFactory httpClientFactory,LocalStorageService localStorageService )
    {
        private const string HeaderKey = "authorization";
        public async Task<HttpClient> GetPrivateHttpclient()
        {
            var client = httpClientFactory.CreateClient("SystemApiClient");
            var stringToken=await  localStorageService.GetToken();
            if(string.IsNullOrEmpty(stringToken)) return client;

            var deserializeToken=Serilizations.DeserilizeJsonString<UserSession>(stringToken); 
            if(deserializeToken == null) return client;

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", deserializeToken.Token);
            return client;
        }
        public HttpClient GetPublicHttpclient()
        {
            var client = httpClientFactory.CreateClient("SystemApiClient");
                client.DefaultRequestHeaders.Remove(HeaderKey);
            return client;
        }
    }
}
