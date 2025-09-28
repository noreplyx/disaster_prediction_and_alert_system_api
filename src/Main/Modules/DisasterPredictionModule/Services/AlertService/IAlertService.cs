using System;
using SendGrid;

namespace Main.Modules.DisasterPredictionModule.Services.AlertService;

public interface IAlertService
{
    (
        Task<Response> response,
        CancellationTokenSource cancellationTokenSource
    ) AlertEmailAsync(
        IEnumerable<string> emailTo,
        string content
    );
}
