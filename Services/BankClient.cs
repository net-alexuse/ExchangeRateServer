using ExchangeRateServer.Interfaces;
using ExchangeRateServer.Models;
using System.Collections.Specialized;
using System.Text.Json;
using System.Web;

namespace ExchangeRateServer.Services
{
    public class BankClient : IBankClient
    {
        private readonly HttpClient httpClient;

        public BankClient(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        public async Task<Rate> GetRateAsync(string abbreviation, DateTime date, CancellationToken cancellationToken)
        {
            var uriBuilder = new UriBuilder
            {
                Scheme = Uri.UriSchemeHttps,
                Host = "www.nbrb.by",
                Path = $"api/exrates/rates/{abbreviation}"
            };

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query["parammode"] = "2";
            query["ondate"] = date.ToString("yyyy-MM-d");

            uriBuilder.Query = query.ToString();

            HttpResponseMessage response = await httpClient.GetAsync(uriBuilder.Uri, cancellationToken).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            var result = JsonSerializer.Deserialize<Rate>(content);

            return result;
        }

    }
}
