namespace ExchangeRateServer.Models
{
    public class CurrencyCacheModel
    {
        public string? Currency { get; set; }
        public DateTime Date { get; set; }
        public decimal? Value { get; set; }
        public int Ammount { get; set; }
    }
}
