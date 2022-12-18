using ExchangeRateServer.Models;

namespace ExchangeRateServer.Interfaces
{
    public interface IJsonCacheService
    {
        ICollection<CurrencyCacheModel> Cache { get; }
        void AddToCache(CurrencyCacheModel cacheModel);
    }
}
