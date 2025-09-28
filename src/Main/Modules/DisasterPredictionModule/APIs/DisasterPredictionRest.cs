using System.Threading.Tasks;
using Main.Modules.DisasterPredictionModule.Models.Requests;
using Main.Modules.DisasterPredictionModule.Models.Responses;
using Main.Modules.DisasterPredictionModule.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Main.Modules.DisasterPredictionModule
{
    [Route("api")]
    [ApiController]
    public class DisasterPredictionRestController : ControllerBase
    {
        private readonly IMasterDisasterPredictionService _masterDisasterPredictionService;
        public DisasterPredictionRestController(
            IMasterDisasterPredictionService masterDisasterPredictionService
        )
        {
            _masterDisasterPredictionService = masterDisasterPredictionService;
        }

        [HttpPost("regions")]
        public async Task<ActionResult> AddOrUpdateRegionAsync(
            [FromBody] AddOrUpdateRegionRequest newRegionRequest
        )
        {
            var result = await _masterDisasterPredictionService.AddOrUpdateRegionAsync(newRegionRequest);
            return Ok(result);
        }
        [HttpPost("alert-settings")]
        public async Task<ActionResult> ConfigureRegionAlertSettingAsync(
            [FromBody] AddOrUpdateAlertSettingRequest addOrUpdateAlertSettingRequest
        )
        {
            var result = await _masterDisasterPredictionService.ConfigureRegionAlertSettingAsync(addOrUpdateAlertSettingRequest);
            return Ok(result);
        }

        [HttpGet("disaster-risks")]
        public async Task<ActionResult<IEnumerable<DisasterRiskReportResponse>>> GetDisasterRiskReportsAsync()
        {
            var serviceResult = await _masterDisasterPredictionService.GetDisasterRiskReportsAsync();
            var apiResult = serviceResult.Select(x => new DisasterRiskReportResponse
            {
                AlertTriggered = x.AlertTriggered,
                DisasterType = x.DisasterType.Name,
                RegionId = x.Region.Name,
                RiskScore = x.RiskScore,
                RiskLevel = x.RiskLevel
            });
            return Ok(apiResult);
        }

        [HttpPost("alerts/send")]
        public async Task<ActionResult<IEnumerable<DisasterRiskReportResponse>>> EmailAlertAsync()
        {
            await _masterDisasterPredictionService.EmailAlertAsync();
            return Ok();
        }

        [HttpGet("alerts")]
        public async Task<ActionResult<PaginationResponse<DisasterRiskReportResponse>>> GetRecentAlertListAsync(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string searchTerm = null
        )
        {
            var recentAlertListPagination = await _masterDisasterPredictionService.GetRecentAlertListAsync(
                page,
                pageSize,
                searchTerm
            );
            return Ok(recentAlertListPagination);
        }
    }
}
