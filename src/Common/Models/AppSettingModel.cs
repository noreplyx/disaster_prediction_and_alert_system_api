using System;

namespace Common.Models;

public class AppSettingModel
{
    public OpenWeather OpenWeather { get; set; }
}

public class OpenWeather
{
    public string BaseUrl { get; set; }
    public string ApiKey { get; set; }
}
