using System;
using System.Text.Json;
using System.Threading.Tasks;
using Main.Common.Models;
using Main.Common.Models.OpenWeather.ResponseModels;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Main.Common.Persistence.ApiClient;

public class OpenWeatherClient : IOpenWeatherClient
{
    private readonly IOptionsMonitor<AppSettingModel> _appSetting;
    private readonly IRestClient _restClient;
    private readonly JsonSerializerOptions _jsonSerializerOption;

    private OpenWeatherConfiguration OpenWeatherConfiguration
    {
        get
        {
            return _appSetting.CurrentValue.OpenWeatherConfiguration;
        }
    }

    public OpenWeatherClient(
        IOptionsMonitor<AppSettingModel> appSetting
    )
    {
        _appSetting = appSetting;
        var options = new RestClientOptions(OpenWeatherConfiguration.BaseUrl);
        _restClient = new RestClient(options);
        _restClient.AddDefaultQueryParameter("appid", OpenWeatherConfiguration.ApiKey);
        _jsonSerializerOption = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
    }

    public async Task<CurrentForecastWeatherResponse> CallCurrentForecastWeatherAsync(
        double lat,
        double lon,
        IEnumerable<string> excludes
    )
    {
        var isHasExclude = excludes != null && excludes.Any();
        var restRequest = new RestRequest(OpenWeatherConfiguration.Path.CurrentForecastWeather);
        restRequest.AddQueryParameter("lat", lat);
        restRequest.AddQueryParameter("lon", lon);
        if (isHasExclude)
        {
            restRequest.AddQueryParameter("exclude", String.Join(',', excludes));
        }
        var result = await _restClient.GetAsync(restRequest);
        var content = JsonSerializer.Deserialize<CurrentForecastWeatherResponse>(result.Content, _jsonSerializerOption);
        return content;
    }

    public async Task<WeatherDataTimestampResponse> CallWeatherDataTimestamp(double lat, double lon, DateTimeOffset dateTime)
    {
        var restRequest = new RestRequest(OpenWeatherConfiguration.Path.WeatherDataTimestamp);
        restRequest.AddQueryParameter("lat", lat);
        restRequest.AddQueryParameter("lon", lon);
        restRequest.AddQueryParameter("dt", dateTime.Ticks);
        var result = await _restClient.GetAsync(restRequest);
        var content = JsonSerializer.Deserialize<WeatherDataTimestampResponse>(result.Content, _jsonSerializerOption);
        return content;
    }
}
