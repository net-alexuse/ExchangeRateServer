using System.Net;
using System.Text.Json;

namespace ExchangeRateServer.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                string responseText = error.Message;

                var response = context.Response;
                response.ContentType = "application/json";

                response.StatusCode = StatusCodes.Status400BadRequest;

                switch (error)
                {
                    case TaskCanceledException:
                        responseText = "Сервер НБРБ не отвечал более 10 секунд. Проверьте соединение сервера с интернетом.";
                        break;
                    case HttpRequestException e:
                        if (e.StatusCode == HttpStatusCode.NotFound)
                            responseText = "Введена неправильная аббривеатура валюты.";
                        else if (e.StatusCode == HttpStatusCode.InternalServerError)
                            responseText = "НБРБ апи не может обработать запрос.";
                        break;
                }

                ResponseContent responseContent = new ResponseContent()
                {
                    Message = responseText
                };

                string jsonResult = JsonSerializer.Serialize(responseContent);

                await response.WriteAsync(jsonResult);
            }
        }

        public class ResponseContent
        {
            public string? Message { get; set; }
        }
    }
}
