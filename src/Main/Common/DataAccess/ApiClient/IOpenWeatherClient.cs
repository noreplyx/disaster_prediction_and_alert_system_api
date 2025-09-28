using System;
using Main.Common.Models.OpenWeather.ResponseModels;

namespace Main.Common.Persistence.ApiClient;

public interface IOpenWeatherClient
{
    Task<CurrentForecastWeatherResponse> CallCurrentForecastWeatherAsync(
        double lat,
        double lon,
        IEnumerable<string> excludes
    );
    Task<WeatherDataTimestampResponse> CallWeatherDataTimestamp(
        double lat,
        double lon,
        DateTimeOffset dateTime
    );
}
