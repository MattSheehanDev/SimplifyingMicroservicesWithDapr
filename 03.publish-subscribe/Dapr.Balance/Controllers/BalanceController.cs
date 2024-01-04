using Dapr.Appointment.Dto;
using Dapr.Client;
using Dapr.Common;
using Microsoft.AspNetCore.Mvc;

namespace Dapr.Balance;

[ApiController]
public class BalanceController : ControllerBase
{

    [Topic(Constants.PubSub, Topics.OnClaimSubmitted, "event.type ==\"balance.v1\"", 1)]
    [HttpPost("v1/balance")]
    public async Task<ActionResult> AddBalanceV1(AppointmentClaim claim, [FromServices] DaprClient daprClient)
    {
        var state = await daprClient.GetStateEntryAsync<BalanceState>(Constants.StateStore, claim.PatientId.ToString());
        state.Value ??= new BalanceState {CreatedOn = DateTime.UtcNow};

        state.Value.UpdatedOn = DateTime.UtcNow;
        state.Value.Claims.Add(claim);
        state.Value.PatientId = claim.PatientId;
        await state.SaveAsync();

        return Ok();
    }

    [Topic(Constants.PubSub, Topics.OnClaimSubmitted)]
    [HttpPost("balance")]
    public async Task<ActionResult> AddBalance(AppointmentClaim claim, [FromServices] DaprClient daprClient)
    {
        var state = await daprClient.GetStateEntryAsync<BalanceState>(Constants.StateStore, claim.PatientId.ToString());
        state.Value ??= new BalanceState {CreatedOn = DateTime.UtcNow};

        state.Value.UpdatedOn = DateTime.UtcNow;
        state.Value.Claims.Add(claim);
        state.Value.PatientId = claim.PatientId;
        await state.SaveAsync();

        return Ok();
    }

}
