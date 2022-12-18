using System.ComponentModel.DataAnnotations;

namespace ExchangeRateServer.Models
{
    public class RateShort
    {
        public int Cur_ID { get; set; }
        [Key]
        public DateTime Date { get; set; }
        public decimal? Cur_OfficialRate { get; set; }
    }
}
