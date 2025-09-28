using System.Threading.Tasks;
using Main.Modules.DisasterPredictionModule.Models.Requests;
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
        public async Task<ActionResult> AddNewRegionAsync(
            [FromBody] NewRegionRequest newRegionRequest
        )
        {
            var result = await _masterDisasterPredictionService.AddRegionAsync(newRegionRequest);
            return Ok(result);
        }
    }
}
