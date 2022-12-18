using ExchangeRateServer.Interfaces;
using ExchangeRateServer.Models;
using System.Text.Json;

namespace ExchangeRateServer.Services
{
    public class JsonCacheService : IJsonCacheService
    {
        private const string fileName = "Cache.json";

        private ICollection<CurrencyCacheModel> cacheModels;

        public ICollection<CurrencyCacheModel> Cache
        {
            get
            {
                if (cacheModels == null)
                {
                    cacheModels = new List<CurrencyCacheModel>();

                    if (File.Exists(fileName))
                    {
                        string json = File.ReadAllText(fileName);

                        if (!string.IsNullOrEmpty(json))
                            cacheModels = JsonSerializer.Deserialize<ICollection<CurrencyCacheModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                }

                return cacheModels;
            }
        }

        public void AddToCache(CurrencyCacheModel cacheModel)
        {
            cacheModels.Add(cacheModel);
            string json = JsonSerializer.Serialize<ICollection<CurrencyCacheModel>>(cacheModels);

            File.WriteAllText(fileName, json);
        }
    }
}
