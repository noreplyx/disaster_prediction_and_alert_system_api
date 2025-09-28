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
    public async Task<(bool isSuccess, string message)> AddRegionAsync(
        NewRegionRequest newRegionRequest
    )
    {
        var isNoDisasterType = newRegionRequest.DisasterTypes == null || !newRegionRequest.DisasterTypes.Any();
        if (isNoDisasterType) return (false, "No Disaster type");

        var isDuplicatedRegion = await _postgreSqlDbContext.Regions.AnyAsync(region =>
            region.Name == newRegionRequest.RegionId
        );
        if (isDuplicatedRegion) return (false, $"{newRegionRequest.RegionId} is duplicated");

        var newRegion = new Region
        {
            Name = newRegionRequest.RegionId,
            Latitude = newRegionRequest.LocationCoordinates.Latitude,
            Longitude = newRegionRequest.LocationCoordinates.Longitude,
        };

        var oldDisasterTypes = await _postgreSqlDbContext.DisasterTypes
        .AsNoTracking()
        .Where(dt =>
            newRegionRequest.DisasterTypes.Contains(dt.Name)
        )
        .ToListAsync();

        foreach (var disasterType in newRegionRequest.DisasterTypes)
        {
            var oldDisasterType = oldDisasterTypes.SingleOrDefault(dt => dt.Name == disasterType);
            var isInOldDisasterType = oldDisasterType != null;

            var newRegionDisasterConfig = new RegionDisasterConfiguration
            {
                Region = newRegion,
                DisasterTypeId = isInOldDisasterType ? oldDisasterType.Id : 0,
                DisasterType = isInOldDisasterType ? null : new DisasterType
                {
                    Name = disasterType
                },
            };
            _postgreSqlDbContext.Add(newRegionDisasterConfig);
        }
        var rowAffected = await _postgreSqlDbContext.SaveChangesAsync();
        return (true, String.Empty);
    }

    public void ConfigureRegionAlertSettingAsync()
    {
        throw new NotImplementedException();
    }

    public void GetDisasterRisksAsync()
    {
        throw new NotImplementedException();
    }
}
