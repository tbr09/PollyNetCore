using Client.Contracts;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client.HttpClients
{
    public class CustomerClient
    {
        private readonly HttpClient client;

        public CustomerClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<T> GetAsync<T>(string uri)
        {
            var response = await client.GetAsync(uri);
            if (!response.IsSuccessStatusCode)
            {
                return default(T);
            }

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}
