using Dapr.Appointment.Dto;
using Dapr.Appointment.Models;
using Dapr.Appointment.State;
using Dapr.Client;
using Dapr.Common;
using Dapr.Patient.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Dapr.Appointment.Controllers;

[ApiController]
public class AppointmentController : ControllerBase
{

    [HttpPost("schedule")]
    public async Task<ActionResult<ScheduleAppointment>> ScheduleAppointment(ScheduleAppointment appointment, [FromServices] DaprClient daprClient)
    {
        var patient = await daprClient.InvokeMethodAsync<PatientDto, PatientDto>(HttpMethod.Post, "patient-service", "patient", new PatientDto { Id = appointment.PatientId, FirstName = appointment.PatientFirstName, LastName = appointment.PatientLastName });

        appointment.PatientId = patient.Id;
        appointment.AppointmentId ??= Guid.NewGuid();

        var state = await daprClient.GetStateEntryAsync<AppointmentState>(Constants.StateStore, appointment.AppointmentId.ToString());
        state.Value ??= new AppointmentState { CreatedOn = DateTime.UtcNow };

        state.Value.UpdatedOn = DateTime.UtcNow;
        state.Value.Appointment = appointment;
        await state.SaveAsync();

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