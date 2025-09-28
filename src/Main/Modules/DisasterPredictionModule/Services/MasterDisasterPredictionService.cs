using System;
using Main.Common.Persistence.ApiClient;
using Main.Common.Persistence.DatabaseContext;
using Main.Modules.DisasterPredictionModule.Entities;
using Main.Modules.DisasterPredictionModule.Models.Requests;
using Main.Modules.DisasterPredictionModule.Models;
using Main.Modules.DisasterPredictionModule.Services.RiskCalculator;
using Microsoft.EntityFrameworkCore;
using Main.Modules.DisasterPredictionModule.Models;
using Main.Modules.DisasterPredictionModule.Enums;
using Main.Modules.DisasterPredictionModule.Services.AlertService;
using Microsoft.Extensions.Options;
using Main.Common.Models;

namespace Main.Modules.DisasterPredictionModule.Services;

public class MasterDisasterPredictionService : IMasterDisasterPredictionService
{
    private readonly PostgreSqlDbContext _postgreSqlDbContext;
    private readonly IOpenWeatherClient _openWeatherClient;
    private readonly IRiskCalculatorService _riskCalculatorService;
    private readonly IAlertService _alertService;
    private readonly IOptionsMonitor<AppSettingModel> _appSettingModel;
    public MasterDisasterPredictionService(
        PostgreSqlDbContext postgreSqlDbContext,
        IOpenWeatherClient openWeatherClient,
        IRiskCalculatorService riskCalculatorService,
        IAlertService alertService,
        IOptionsMonitor<AppSettingModel> appSettingModel
    )
    {
        _postgreSqlDbContext = postgreSqlDbContext;
        _openWeatherClient = openWeatherClient;
        _riskCalculatorService = riskCalculatorService;
        _alertService = alertService;
        _appSettingModel = appSettingModel;
    }
    public async Task<(bool isSuccess, string message)> AddOrUpdateRegionAsync(
        AddOrUpdateRegionRequest addOrUpdateRegionRequest
    )
    {
        var isNoDisasterType = addOrUpdateRegionRequest.DisasterTypes == null ||
        !addOrUpdateRegionRequest.DisasterTypes.Any();
        if (isNoDisasterType) return (false, "No Disaster type");

        var deduplicateDisasterType = addOrUpdateRegionRequest.DisasterTypes
        .Distinct()
        .ToList();

        var oldRegionId = await _postgreSqlDbContext.Regions
        .Where(region =>
            region.Name == addOrUpdateRegionRequest.RegionId
        )
        .Select(r => r.Id)
        .SingleOrDefaultAsync();
        var isOldRegion = oldRegionId > 0;

        var region = new Region
        {
            Id = isOldRegion ? oldRegionId : 0,
            Name = addOrUpdateRegionRequest.RegionId,
            Latitude = addOrUpdateRegionRequest.LocationCoordinates.Latitude,
            Longitude = addOrUpdateRegionRequest.LocationCoordinates.Longitude,
        };
        if (isOldRegion)
        {
            _postgreSqlDbContext.Regions.Update(region);
        }
        else
        {
            _postgreSqlDbContext.Regions.Add(region);
        }

        var oldDisasterTypes = await _postgreSqlDbContext.DisasterTypes
        .AsNoTracking()
        .ToListAsync();

        #region duplciate and new disaster type
        foreach (var disasterType in deduplicateDisasterType)
        {
            var oldDisasterType = oldDisasterTypes.SingleOrDefault(dt => dt.Name == disasterType);
            var isInOldDisasterType = oldDisasterType != null;
            if (isInOldDisasterType) continue;

            var newRegionDisasterConfig = new RegionDisasterConfiguration
            {
                Region = region,
                DisasterType = new DisasterType
                {
                    Name = disasterType
                },
            };
            _postgreSqlDbContext.Add(newRegionDisasterConfig);
        }
        #endregion

        #region removed disaster type
        foreach (var oldDisasterType in oldDisasterTypes)
        {
            var newDisasterType = deduplicateDisasterType
            .SingleOrDefault(dt => dt == oldDisasterType.Name);

            var isInOldDisasterType = newDisasterType != null;
            if (isInOldDisasterType) continue;

            _postgreSqlDbContext.Remove(oldDisasterType);
        }
        #endregion
        var rowAffected = await _postgreSqlDbContext.SaveChangesAsync();
        return (true, String.Empty);
    }

    public async Task<(bool isSuccess, string message)> ConfigureRegionAlertSettingAsync(
        AddOrUpdateAlertSettingRequest addOrUpdateAlertSettingRequest
    )
    {
        var regionId = await _postgreSqlDbContext.Regions
        .Where
        (r =>
            r.Name == addOrUpdateAlertSettingRequest.RegionId
        )
        .Select(r => r.Id)
        .SingleOrDefaultAsync();
        var isHasRegion = regionId != 0;
        if (!isHasRegion) return (false, "Not Found Region");

        var regionDisasterConfiguration = await _postgreSqlDbContext.RegionDisasterConfigurations
        .SingleOrDefaultAsync(rdc =>
            rdc.RegionId == regionId &&
            rdc.DisasterType.Name == addOrUpdateAlertSettingRequest.DisasterType
        );
        var isNotFoundConfiguration = regionDisasterConfiguration == null;
        if (isNotFoundConfiguration)
        {
            return (false, "Not found Disaster Type");
        }
        else
        {
            regionDisasterConfiguration.Threshold = addOrUpdateAlertSettingRequest.ThresholdScore;
        }
        var rowAffected = await _postgreSqlDbContext.SaveChangesAsync();
        return (true, String.Empty);
    }

    public async Task<IEnumerable<DisasterRiskReport>> GetDisasterRiskReportsAsync()
    {
        var utcNow = DateTimeOffset.UtcNow;
        var regions = await _postgreSqlDbContext.Regions
        .AsNoTracking()
        .ToListAsync();
        var isNoRegion = !regions.Any();
        if (isNoRegion) return Enumerable.Empty<DisasterRiskReport>();

        var riskReports = new List<DisasterRiskReport>();
        foreach (var region in regions)
        {
            var regionDisasterTypes = await _postgreSqlDbContext.RegionDisasterConfigurations
            .AsNoTracking()
            .Include(rdc => rdc.DisasterType)
            .Where(rdc =>
                rdc.RegionId == region.Id &&
                rdc.Threshold > 0
            )
            .ToListAsync();
            var weatherData = await _openWeatherClient.CallWeatherDataTimestamp(
                region.Latitude,
                region.Longitude,
                utcNow
            );
            foreach (var regionDisasterType in regionDisasterTypes)
            {
                if (regionDisasterType.DisasterType.Name.ToLower() == "wildfire")
                {
                    var wildfireRiskResult = _riskCalculatorService.CalculateWildfire(new WildfireCalculateMaterial
                    {
                        Pressure = weatherData.Data.First().Pressure,
                        WindDegree = weatherData.Data.First().WindDeg,
                        WindSpeed = weatherData.Data.First().WindSpeed
                    });
                    riskReports.Add(new DisasterRiskReport
                    {
                        Region = region,
                        DisasterType = regionDisasterType.DisasterType,
                        RiskLevel = wildfireRiskResult.riskLevel,
                        RiskScore = wildfireRiskResult.RiskScore,
                        AlertTriggered = wildfireRiskResult.RiskScore > regionDisasterType.Threshold
                    });
                }
                else if (regionDisasterType.DisasterType.Name.ToLower() == "earthquake")
                {
                    var earthquakeRiskResult = _riskCalculatorService.CalculateEarthquake(new EarthquakeCalculateMaterial
                    {
                        WindSpeed = weatherData.Data.First().WindSpeed,
                        Humidity = weatherData.Data.First().Humidity
                    });
                    riskReports.Add(new DisasterRiskReport
                    {
                        Region = region,
                        DisasterType = regionDisasterType.DisasterType,
                        RiskLevel = earthquakeRiskResult.riskLevel,
                        RiskScore = earthquakeRiskResult.RiskScore,
                        AlertTriggered = earthquakeRiskResult.RiskScore > regionDisasterType.Threshold
                    });
                }
                else if (regionDisasterType.DisasterType.Name.ToLower() == "flood")
                {
                    var floodRiskResult = _riskCalculatorService.CalculateFlood(new FloodRiskCalculateMaterial
                    {
                        WindSpeed = weatherData.Data.First().WindSpeed,
                        WindGust = weatherData.Data.First().WindGust
                    });
                    riskReports.Add(new DisasterRiskReport
                    {
                        Region = region,
                        DisasterType = regionDisasterType.DisasterType,
                        RiskLevel = floodRiskResult.riskLevel,
                        RiskScore = floodRiskResult.RiskScore,
                        AlertTriggered = floodRiskResult.RiskScore > regionDisasterType.Threshold
                    });
                }
            }

        }
        return riskReports;
    }
    public async Task EmailAlertAsync(
        RiskLevel minimumAlertRiskLevel = RiskLevel.High
    )
    {
        var disasterRiskReports = await GetDisasterRiskReportsAsync();
        var needAlertDisasterReports = disasterRiskReports.Where(drr =>
            drr.RiskLevel >= minimumAlertRiskLevel
        )
        .ToList();
        var isNoNeedAlert = !needAlertDisasterReports.Any();
        if (isNoNeedAlert) return;

        var utcNow = DateTimeOffset.UtcNow;
        var newRegionAlertRecords = new List<RegionAlertRecord>();
        foreach (var riskReport in needAlertDisasterReports)
        {
            var newRegionAlertRecord = new RegionAlertRecord
            {
                AlertType = AlertType.Email,
                CreateDate = utcNow,
                RiskLevel = riskReport.RiskLevel,
                RiskScore = riskReport.RiskScore,
                Threshold = riskReport.Threshold,
                DisasterTypeId = riskReport.DisasterType.Id,
                RegionId = riskReport.Region.Id,
                Content = $"""
                    Region: {riskReport.Region.Name}
                    DisasterType: {riskReport.DisasterType.Name}
                    Level: {riskReport.RiskLevel}
                    Score: {riskReport.RiskScore}
                    Time: {utcNow}
                """,
                UpdateDate = utcNow,
                Status = AlertStatus.Initial,
            };
            newRegionAlertRecords.Add(newRegionAlertRecord);
            _postgreSqlDbContext.Add(newRegionAlertRecord);
        }
        var rowAffected = await _postgreSqlDbContext.SaveChangesAsync();

        //* can seperate to queue/worker
        var sendEmailTasks = new List<Task<SendGrid.Response>>();
        foreach (var newRegionAlertRecord in newRegionAlertRecords)
        {
            var alertEmailRes = _alertService.AlertEmailAsync(
                new List<string> { _appSettingModel.CurrentValue.SendGridConfiguration.EmailTo },
                newRegionAlertRecord.Content
            );
            sendEmailTasks.Add(alertEmailRes.response);
        }
        await Task.WhenAll(sendEmailTasks);
    }
    public void GetRecentListAlert()
    {
        // _postgreSqlDbContext
    }
}
