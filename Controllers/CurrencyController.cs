using ExchangeRateServer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeRateServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyController : Controller
    {
        private readonly ICurrencyService currencyService;
        public CurrencyController(ICurrencyService currencyService)
        {
            this.currencyService = currencyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRatesAsync(string abbreviation, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(abbreviation))
                throw new Exception("Не указана валюта");

            if (endDate < startDate)
                throw new Exception("Конечная дата не может быть меньше чем стартовая");

            var result = await currencyService.GetRatesAsync(abbreviation, startDate, endDate, cancellationToken);
            return Ok(result);
        }
    }
}
