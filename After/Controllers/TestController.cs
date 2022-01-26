using After.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace After.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly IMediator mediator;

        public TestController(ILogger<TestController> logger, IMediator mediator)
        {
            _logger = logger;
            this.mediator = mediator;
        }

        [HttpGet("calculate-offer-value")]
        public IActionResult Get(string email, string offerType)
        {
            //$"/calculate-offer-value?email={member.Email}&offerType={offerType.Name}",
            return Ok(1);
        }

        [HttpPost("AssignOffer")]
        public async Task<IActionResult> AssignOffer(AssignOfferRequest assignOfferRequest)
        {
            try
            {
                await mediator.Send(assignOfferRequest);
                return Ok();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}