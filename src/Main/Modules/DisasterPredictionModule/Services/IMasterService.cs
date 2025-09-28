using System;
using Main.Modules.DisasterPredictionModule.Models.Requests;

namespace Main.Modules.DisasterPredictionModule.Services;

public interface IMasterDisasterPredictionService
{
    Task<(bool isSuccess, string message)> AddRegionAsync(NewRegionRequest newRegionRequest);
    void ConfigureRegionAlertSettingAsync();
    void GetDisasterRisksAsync();
}
