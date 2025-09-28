using System;

namespace Main.Common.Models;

public class AppSettingModel
{
    public OpenWeatherConfiguration OpenWeatherConfiguration { get; set; }
    public ConnectionString ConnectionStrings { get; set; }
    public SendGridConfiguration SendGridConfiguration { get; set; }
}

public class ConnectionString
{
    public string PostgreSQL { get; set; }
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

public class SendGridConfiguration
{
    public string ApiKey { get; set; }
    public string EmailFrom { get; set; }
    public string EmailTo { get; set; }

}