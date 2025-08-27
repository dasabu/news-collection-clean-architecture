using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsCollection.Application.Dtos;
using NewsCollection.Application.Interfaces;
using System.Threading.Tasks;

namespace NewsCollection.Api.Controllers;

[Authorize]
[Route("api/subscriptions")]
[ApiController]
public class SubscriptionController(ISubscriptionService service) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<SubscriptionDto>> Subscribe([FromBody] CreateSubscriptionDto request)
    {
        var result = await service.SubscribeAsync(request);
        return result != null ? Ok(result) : BadRequest("Invalid category or already subscribed");
    }

    [HttpDelete("{categoryId}")]
    public async Task<ActionResult> Unsubscribe(int categoryId)
    {
        var result = await service.UnsubscribeAsync(categoryId);
        return result ? Ok() : BadRequest("Invalid category or not subscribed");
    }

    [HttpGet]
    public async Task<ActionResult<List<SubscriptionDto>>> GetSubscriptions() =>
        Ok(await service.GetUserSubscriptionsAsync());

    [HttpPut("frequency")]
    public async Task<ActionResult> UpdateFrequency([FromBody] UpdateUserFrequencyDto request)
    {
        var result = await service.UpdateUserFrequencyAsync(request);
        return result ? Ok() : BadRequest("Invalid frequency");
    }
}