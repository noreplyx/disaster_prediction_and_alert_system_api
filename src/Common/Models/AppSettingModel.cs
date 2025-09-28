using System;

namespace Common.Models;

public class AppSettingModel
{
    public OpenWeatherConfiguration OpenWeatherConfiguration { get; set; }
}

public class OpenWeatherConfiguration
{
    public string BaseUrl { get; set; }
    public string ApiKey { get; set; }
    public OpenWeatherConfigurationPath Path { get; set; }
}
public class OpenWeatherConfigurationPath
{
    public string Overview { get; set; }
    public string WeatherDataTimestamp { get; set; }
    public string DailyAggregation { get; set; }
    public string CurrentForecastWeather { get; set; }
}
