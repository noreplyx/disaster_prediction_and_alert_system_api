using System;
using Main.Common.Models;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Main.Modules.DisasterPredictionModule.Services.AlertService;

public class AlertService : IAlertService
{
    private readonly ISendGridClient _sendGridClient;
    private readonly IOptionsMonitor<AppSettingModel> _appSettingModel;

    private SendGridConfiguration SendGridConfiguration
    {
        get
        {
            return _appSettingModel.CurrentValue.SendGridConfiguration;
        }
    }
    public AlertService(
        ISendGridClient sendGridClient,
        IOptionsMonitor<AppSettingModel> appSettingModel
    )
    {
        _sendGridClient = sendGridClient;
        _appSettingModel = appSettingModel;
    }
    public (
        Task<Response> response,
        CancellationTokenSource cancellationTokenSource
    ) AlertEmailAsync(
        IEnumerable<string> emailTo,
        string subject,
        string content
    )
    {
        var sendGridMessage = new SendGridMessage
        {
            From = new EmailAddress(SendGridConfiguration.EmailFrom),
            Subject = subject
        };
        foreach (var email in emailTo)
        {
            sendGridMessage.AddTo(email);
        }
        sendGridMessage.AddContent(MimeType.Text, content);
        var cancelToken = new CancellationTokenSource();
        var sendEmailTask = _sendGridClient.SendEmailAsync(sendGridMessage, cancelToken.Token);
        return (
            sendEmailTask,
            cancelToken
        );
    }
}
