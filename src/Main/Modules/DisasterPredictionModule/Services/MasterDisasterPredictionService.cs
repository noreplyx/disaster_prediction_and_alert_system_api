using System;
using Main.Common.Persistence.DatabaseContext;
using Main.Modules.DisasterPredictionModule.Entities;
using Main.Modules.DisasterPredictionModule.Models.Requests;
using Microsoft.EntityFrameworkCore;

namespace Main.Modules.DisasterPredictionModule.Services;

public class MasterDisasterPredictionService : IMasterDisasterPredictionService
{
    private readonly PostgreSqlDbContext _postgreSqlDbContext;
    public MasterDisasterPredictionService(
        PostgreSqlDbContext postgreSqlDbContext
    )
    {
        _postgreSqlDbContext = postgreSqlDbContext;
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

    public void GetDisasterRisksAsync()
    {
        throw new NotImplementedException();
    }
}
