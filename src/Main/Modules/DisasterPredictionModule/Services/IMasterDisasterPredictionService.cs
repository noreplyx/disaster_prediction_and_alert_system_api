using System;
using Main.Modules.DisasterPredictionModule.Models.Requests;
using Main.Modules.DisasterPredictionModule.Models;
using Main.Modules.DisasterPredictionModule.Enums;

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
}
