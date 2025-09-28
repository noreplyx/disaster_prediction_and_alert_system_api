using System;
using Main.Modules.DisasterPredictionModule.Models.Requests;
using Main.Modules.DisasterPredictionModule.Models;
using Main.Modules.DisasterPredictionModule.Enums;
using Main.Modules.DisasterPredictionModule.Models.Responses;

namespace Main.Modules.DisasterPredictionModule.Services;

public interface IMasterDisasterPredictionService
{
    Task<(bool isSuccess, string message)> AddOrUpdateRegionAsync(
        AddOrUpdateRegionRequest addOrUpdateRegionRequest
    );
    Task<(bool isSuccess, string message)> ConfigureRegionAlertSettingAsync(
        AddOrUpdateAlertSettingRequest addOrUpdateAlertSettingRequest
    );
    Task<IEnumerable<DisasterRiskReport>> GetDisasterRiskReportsAsync();
    Task EmailAlertAsync(
        RiskLevel minimumAlertRiskLevel = RiskLevel.High
    );
    Task<PaginationResponse<AlertDataResponse>> GetRecentAlertListAsync(
        int page,
        int pageSize,
        string searchTerm
    );
}
