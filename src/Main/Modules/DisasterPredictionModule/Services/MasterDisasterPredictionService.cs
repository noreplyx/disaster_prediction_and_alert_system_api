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
using Main.Modules.DisasterPredictionModule.Models.Responses;
using Microsoft.Extensions.Caching.Distributed;
using Serilog;

namespace Main.Modules.DisasterPredictionModule.Services;

public class MasterDisasterPredictionService : IMasterDisasterPredictionService
{
    private readonly ILogger<MasterDisasterPredictionService> _logger;
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
        IOptionsMonitor<AppSettingModel> appSettingModel,
        ILogger<MasterDisasterPredictionService> logger
    )
    {
        _postgreSqlDbContext = postgreSqlDbContext;
        _openWeatherClient = openWeatherClient;
        _riskCalculatorService = riskCalculatorService;
        _alertService = alertService;
        _appSettingModel = appSettingModel;
        _logger = logger;
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
        .Where(dt =>
            deduplicateDisasterType.Contains(dt.Name)
        )
        .ToListAsync();
        var oldDisasterTypeConfigurations = await _postgreSqlDbContext.RegionDisasterConfigurations
        .Include(rdc=> rdc.DisasterType)
        .Where(rdc=>
            rdc.RegionId == region.Id
        )
        .AsNoTracking()
        .ToListAsync();

        #region duplciate and new disaster type configuration
        foreach (var disasterType in deduplicateDisasterType)
        {
            var oldDisasterTypeConfiguration = oldDisasterTypeConfigurations
            .SingleOrDefault(rdc=>
                rdc.DisasterType.Name == disasterType
            );
            var isInOldDisasterType = oldDisasterTypeConfiguration != null;
            if (isInOldDisasterType) continue;

            var oldDisasterType = oldDisasterTypes.SingleOrDefault(dt=> dt.Name == disasterType);
            var isOldDisasterType = oldDisasterType != null;
            var newRegionDisasterConfig = new RegionDisasterConfiguration
            {
                Region = region,
                DisasterTypeId = isOldDisasterType ? oldDisasterType.Id : 0,
                DisasterType = isOldDisasterType ? null : new DisasterType
                {
                    Name = disasterType
                },
            };
            _postgreSqlDbContext.Add(newRegionDisasterConfig);
        }
        #endregion

        #region removed disaster type configuration
        foreach (var oldDisasterTypeConfiguration in oldDisasterTypeConfigurations)
        {
            var newDisasterType = deduplicateDisasterType
            .SingleOrDefault(dt => dt == oldDisasterTypeConfiguration.DisasterType.Name);

            var isInOldDisasterType = newDisasterType != null;
            if (isInOldDisasterType) continue;

            _postgreSqlDbContext.Remove(oldDisasterTypeConfiguration);
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
            var weatherData = await _openWeatherClient.CallWeatherDataTimestampAsync(
                region.Latitude,
                region.Longitude,
                utcNow
            );
            foreach (var regionDisasterType in regionDisasterTypes)
            {
                if (regionDisasterType.DisasterType.Name.ToLower() == "wildfire")
                {
                    var wildfireRiskResult = await _riskCalculatorService.CalculateWildfireAsync(new WildfireCalculateMaterial
                    {
                        Pressure = weatherData.Data.First().Pressure,
                        WindDegree = weatherData.Data.First().WindDeg,
                        WindSpeed = weatherData.Data.First().WindSpeed
                    });
                    riskReports.Add(new DisasterRiskReport
                    {
                        Region = region,
                        DisasterType = regionDisasterType.DisasterType,
                        RiskLevel = wildfireRiskResult.RiskLevel,
                        RiskScore = wildfireRiskResult.RiskScore,
                        AlertTriggered = wildfireRiskResult.RiskScore > regionDisasterType.Threshold
                    });
                }
                else if (regionDisasterType.DisasterType.Name.ToLower() == "earthquake")
                {
                    var earthquakeRiskResult = await _riskCalculatorService.CalculateEarthquakeAsync(new EarthquakeCalculateMaterial
                    {
                        WindSpeed = weatherData.Data.First().WindSpeed,
                        Humidity = weatherData.Data.First().Humidity
                    });
                    riskReports.Add(new DisasterRiskReport
                    {
                        Region = region,
                        DisasterType = regionDisasterType.DisasterType,
                        RiskLevel = earthquakeRiskResult.RiskLevel,
                        RiskScore = earthquakeRiskResult.RiskScore,
                        AlertTriggered = earthquakeRiskResult.RiskScore > regionDisasterType.Threshold
                    });
                }
                else if (regionDisasterType.DisasterType.Name.ToLower() == "flood")
                {
                    var floodRiskResult = await _riskCalculatorService.CalculateFloodAsync(new FloodRiskCalculateMaterial
                    {
                        WindSpeed = weatherData.Data.First().WindSpeed,
                        WindGust = weatherData.Data.First().WindGust
                    });
                    riskReports.Add(new DisasterRiskReport
                    {
                        Region = region,
                        DisasterType = regionDisasterType.DisasterType,
                        RiskLevel = floodRiskResult.RiskLevel,
                        RiskScore = floodRiskResult.RiskScore,
                        AlertTriggered = floodRiskResult.RiskScore > regionDisasterType.Threshold
                    });
                }
            }

        }
        return riskReports;
    }
    public async Task<(
        bool isSuccess,
        string message
    )> EmailAlertAsync(
        RiskLevel minimumAlertRiskLevel = RiskLevel.High
    )
    {
        var disasterRiskReports = await GetDisasterRiskReportsAsync();
        var needAlertDisasterReports = disasterRiskReports.Where(drr =>
            drr.RiskLevel >= minimumAlertRiskLevel
        )
        .ToList();
        var isNoNeedAlert = !needAlertDisasterReports.Any();
        if (isNoNeedAlert) return (false, "Not Need to Alert");

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
                "Disaster Alert",
                newRegionAlertRecord.Content
            );
            sendEmailTasks.Add(alertEmailRes.response);
        }
        await Task.WhenAll(sendEmailTasks);
        return (true, String.Empty);
    }
    public async Task<PaginationResponse<AlertDataResponse>> GetRecentAlertListAsync(
        int page,
        int pageSize,
        string searchTerm
    )
    {
        var isPaging = pageSize > 0;
        var isHasSearchTerm = !String.IsNullOrWhiteSpace(searchTerm) && !String.IsNullOrEmpty(searchTerm);
        var query = _postgreSqlDbContext.RegionAlertRecords
        .AsNoTracking()
        .OrderByDescending(rar =>
            rar.CreateDate
        )
        .AsQueryable();

        if (isHasSearchTerm)
        {
            query = query.Where(rar =>
                rar.Region.Name.Contains(searchTerm)
            );
        }

        var totalRecord = await query.CountAsync();
        if (isPaging)
        {
            var skipSize = (page - 1) * pageSize;
            query = query.Skip(skipSize).Take(pageSize);
        }

        var alertDataRecords = await query.Select(rar => new AlertDataResponse
        {
            AlertMessage = rar.Content,
            DisasterType = rar.DisasterType.Name,
            RegionId = rar.Region.Name,
            RiskLevel = rar.RiskLevel,
            Timestamp = rar.CreateDate,
            Type = rar.AlertType
        })
        .ToListAsync();

        var totalPage = Convert.ToInt32(
            Math.Ceiling((decimal)totalRecord / (decimal)pageSize)
        );
        var paginationResponse = new PaginationResponse<AlertDataResponse>
        {
            Data = alertDataRecords,
            Page = page,
            PageSize = pageSize,
            TotalRecord = totalRecord,
            TotalPage = totalPage
        };
        return paginationResponse;
    }
}
