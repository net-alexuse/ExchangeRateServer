using ExchangeRateServer.Models;

namespace ExchangeRateServer.Interfaces
{
    public interface ICurrencyService
    {
        Task<ICollection<RateShort>> GetRatesAsync(string abbreviation, DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
    }
}
