using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ApiExpandContract.Controllers
{
    [Route("api/v{version:apiVersion}/payments")]
    [ApiController]
    [ApiVersion("2.0")]
    //[Consumes(MediaTypeNames.Application.Json)]
    //[Produces(MediaTypeNames.Application.Json)]
    public class PaymentsV2Controller : ControllerBase
    {
        [HttpPost("")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateAsync([FromBody]CashPayment payment)
        {
            return NoContent();
        }

        [HttpGet("")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> IndexAsync([FromBody]CashPayment payment)
        {
            return Ok();
        }
    }
}