using System;
using System.Text.Json.Serialization;

namespace Common.Models.OpenWeather.ResponseModels;

public class CurrentForecastWeatherResponse
{
    public int Lat { get; set; }
    public int Lon { get; set; }
    public string Timezone { get; set; }
    public int TimezoneOffset { get; set; }
    public Current Current { get; set; }
    public List<Hourly> Hourly { get; set; }
    public List<Daily> Daily { get; set; }
}
public class Current
{
    public int Dt { get; set; }
    public double Temp { get; set; }
    public double FeelsLike { get; set; }
    public int Pressure { get; set; }
    public int Humidity { get; set; }
    public double DewPoint { get; set; }
    public int Uvi { get; set; }
    public int Clouds { get; set; }
    public int Visibility { get; set; }
    public double WindSpeed { get; set; }
    public int WindDeg { get; set; }
    public double WindGust { get; set; }
    public List<Weather> Weather { get; set; }
    public Snow Snow { get; set; }
}

public class Daily
{
    public int Dt { get; set; }
    public int Sunrise { get; set; }
    public int Sunset { get; set; }
    public int Moonrise { get; set; }
    public int Moonset { get; set; }
    public double MoonPhase { get; set; }
    public string Summary { get; set; }
    public Temp Temp { get; set; }
    public FeelsLike FeelsLike { get; set; }
    public int Pressure { get; set; }
    public int Humidity { get; set; }
    public double DewPoint { get; set; }
    public double WindSpeed { get; set; }
    public int WindDeg { get; set; }
    public double WindGust { get; set; }
    public List<Weather> Weather { get; set; }
    public int Clouds { get; set; }
    public double Pop { get; set; }
    public double Snow { get; set; }
    public int Uvi { get; set; }
}

public class FeelsLike
{
    public double Day { get; set; }
    public double Night { get; set; }
    public double Eve { get; set; }
    public double Morn { get; set; }
}

public class Hourly
{
    public int Dt { get; set; }
    public double Temp { get; set; }
    public double FeelsLike { get; set; }
    public int Pressure { get; set; }
    public int Humidity { get; set; }
    public double DewPoint { get; set; }
    public int Uvi { get; set; }
    public int Clouds { get; set; }
    public int Visibility { get; set; }
    public double WindSpeed { get; set; }
    public int WindDeg { get; set; }
    public double WindGust { get; set; }
    public List<Weather> Weather { get; set; }
    public double Pop { get; set; }
    public Snow Snow { get; set; }
}

public class Snow
{
    [JsonPropertyName("1h")]
    public double _1h { get; set; }
}

public class Temp
{
    public double Day { get; set; }
    public double Min { get; set; }
    public double Max { get; set; }
    public double Night { get; set; }
    public double Eve { get; set; }
    public double Morn { get; set; }
}