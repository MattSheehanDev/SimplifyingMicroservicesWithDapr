using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Appointment.Dto;
using Dapr.AppointmentActor.Models;
using Dapr.Client;
using Dapr.Common;
using Microsoft.AspNetCore.Mvc;

namespace Dapr.Appointment.Controllers;

[ApiController]
public class AppointmentController : ControllerBase
{

    [HttpPost("schedule")]
    public async Task<ActionResult<ScheduleAppointment>> ScheduleAppointment(ScheduleAppointment appointment, [FromServices] DaprClient daprClient)
    {
        var proxy = ActorProxy.Create<IAppointmentActor>(new ActorId(appointment.Practitioner), "AppointmentActor");
        appointment = await proxy.ScheduleAppointment(appointment);
        return appointment;
    }

    [HttpPost("submitclaim")]
    public async Task<ActionResult<ScheduleAppointment>> SubmitAppointmentClaim(SubmitClaim claim, [FromServices] DaprClient daprClient)
    {
        var state = await daprClient.GetStateAsync<AppointmentState>(Constants.StateStore, claim.AppointmentId.ToString());

        if (state is null) return NotFound();
        if (state.Appointment!.Closed) return Ok();

        var metadata = new Dictionary<string, string>
        {
            { "cloudevent.type", "balance.v1" }
        };
        await daprClient.PublishEventAsync<AppointmentClaim>(Constants.PubSub, Topics.OnClaimSubmitted, new AppointmentClaim { AppointmentId = claim.AppointmentId, PatientId = state.Appointment!.PatientId, ClaimAmount = claim.Amount }, metadata);

        state.Appointment.Closed = true;
        await daprClient.SaveStateAsync<AppointmentState>(Constants.StateStore, state.Appointment.AppointmentId.ToString(), state);

        return state.Appointment;
    }

}