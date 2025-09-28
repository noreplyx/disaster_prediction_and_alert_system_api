using System;

namespace Main.Modules.DisasterPredictionModule.Services.DisasterPrediction;

public interface IRegionDisasterPredictionService
{
    void PredictRegion(
        string regionName,
        DateTimeOffset dateTime
    );
}
