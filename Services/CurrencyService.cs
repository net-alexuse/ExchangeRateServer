using ExchangeRateServer.Interfaces;
using ExchangeRateServer.Models;

namespace ExchangeRateServer.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IBankClient bankClient;
        private readonly IJsonCacheService jsonCacheService;
        public CurrencyService(IBankClient bankClient, IJsonCacheService jsonCacheService)
        {
            this.bankClient = bankClient;
            this.jsonCacheService = jsonCacheService;
        }

        public async Task<ICollection<RateShort>> GetRatesAsync(string abbreviation, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            ICollection<Rate> rates = GetRatesFromCache(jsonCacheService.Cache, abbreviation, startDate, endDate, out ICollection<DateTime> missedRatesDates);

            if (missedRatesDates.Count > 0)
            {
                foreach (var missedRateDate in missedRatesDates)
                {
                    Rate missedRate = await GetRateFromApiAsync(abbreviation, missedRateDate, cancellationToken);
                    if (missedRate != null)
                    {
                        rates.Add(missedRate);

                        jsonCacheService.AddToCache(new CurrencyCacheModel
                        {
                            Ammount = missedRate.Cur_Scale,
                            Currency = missedRate.Cur_Abbreviation,
                            Date = missedRate.Date,
                            Value = missedRate.Cur_OfficialRate
                        });
                    }
                }
            }

            int maxScale = rates.Max(x => x.Cur_Scale);

            List<RateShort> result = rates.Select(x => new RateShort
            {
                Cur_OfficialRate = x.Cur_OfficialRate / (maxScale / x.Cur_Scale),
                Date = x.Date
            }).ToList();

            return result;
        }

        private async Task<Rate> GetRateFromApiAsync(string abbreviation, DateTime onDate, CancellationToken cancellationToken)
        {
            Rate rate = await bankClient.GetRateAsync(abbreviation, onDate, cancellationToken);
            return rate;
        }

        private ICollection<Rate> GetRatesFromCache(ICollection<CurrencyCacheModel> cachedItems, string abbreviation, DateTime startDate, DateTime endDate, out ICollection<DateTime> missedRates)
        {
            missedRates = new List<DateTime>();
            List<Rate> result = new List<Rate>();
            DateTime currentSearchingDate = startDate;

            do
            {
                CurrencyCacheModel cacheModel = cachedItems.FirstOrDefault(x => x.Date == currentSearchingDate && x.Currency.ToUpper() == abbreviation.ToUpper());
                if (cacheModel == null)
                    missedRates.Add(currentSearchingDate);
                else
                    result.Add(new Rate
                    {
                        Cur_OfficialRate = cacheModel.Value,
                        Cur_Scale = cacheModel.Ammount,
                        Date = currentSearchingDate
                    });

                currentSearchingDate = currentSearchingDate.AddDays(1);

            } while (currentSearchingDate <= endDate);

            return result;
        }
    }
}
