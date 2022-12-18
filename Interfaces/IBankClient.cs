using ExchangeRateServer.Models;

namespace ExchangeRateServer.Interfaces
{
    public interface IBankClient
    {
        Task<Rate> GetRateAsync(string abbreviation, DateTime date, CancellationToken cancellationToken);
    }
}
