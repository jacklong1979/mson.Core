using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace mson.Core.Client.Test
{
    class Program
    {
        // 从元数据中发现客户端
       
        static string secretString = "25Z$%^&*(_)?><nbc6";
        static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();
        static async Task<DiscoveryResponse> GetDiscoveryClient()
        {
            return await DiscoveryClient.GetAsync("http://localhost:5000");
        }
        private static async Task MainAsync()
        {


            var disco =await GetDiscoveryClient();

            // 请求令牌
            var tokenClient = new TokenClient(disco.TokenEndpoint, "client1", secretString);
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");//AllowedGrantTypes=GrantTypes.ClientCredentials, // 没有交互性用户，使用 clientid/secret 实现认证。

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            // 调用api
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:5001/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }
           await PasswordApiTests();
           
        }
        public static async Task PasswordApiTests()
        {
            var disco = await GetDiscoveryClient();
            var tokenClient = new TokenClient(disco.TokenEndpoint, "ro.client", secretString);
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("abc", "a123", "api1");

            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);//add bearer with access_token
            var response = await client.GetAsync("http://localhost:5001/api/Values");//call API with access_token
            var apiResult = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(apiResult);
            Console.ReadKey();
        }
        private async Task<TokenResponse> GetToken(string clientId, string clientSecret, string grantType, string userName, string password, string scope)
        {
            var client = new DiscoveryClient($"http://localhost:5000");
            client.Policy.RequireHttps = false;
            var disco = await client.GetAsync();
            var tokenClient = new TokenClient(disco.TokenEndpoint, clientId, clientSecret);
            return await tokenClient.RequestResourceOwnerPasswordAsync(userName, password, scope);
        }

        private async Task<TokenResponse> GetRefreshToken(string clientId, string clientSecret, string grantType, string refreshToken)
        {
            var client = new DiscoveryClient($"http://localhost:5000");
            client.Policy.RequireHttps = false;
            var disco = await client.GetAsync();
            var tokenClient = new TokenClient(disco.TokenEndpoint, clientId, clientSecret);
            return await tokenClient.RequestRefreshTokenAsync(refreshToken);
        }

    }
}
