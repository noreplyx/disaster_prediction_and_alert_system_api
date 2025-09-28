using System.Threading.Tasks;
using Main.Modules.DisasterPredictionModule.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Main.Modules.DisasterPredictionModule
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisasterPredictionRestController : ControllerBase
    {
        public DisasterPredictionRestController(
        )
        {
            
        }
    }
}
