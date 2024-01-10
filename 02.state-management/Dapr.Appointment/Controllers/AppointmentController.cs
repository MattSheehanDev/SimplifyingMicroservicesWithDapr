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
        var patient = await daprClient.InvokeMethodAsync<PatientDto, PatientDto>(
            HttpMethod.Post,
            "patient-service",
            "patient",
            new PatientDto { Id = appointment.PatientId, FirstName = appointment.PatientFirstName, LastName = appointment.PatientLastName });

        appointment.PatientId = patient.Id;
        appointment.AppointmentId ??= Guid.NewGuid();

        var state = await daprClient.GetStateEntryAsync<AppointmentState>(Constants.StateStore, appointment.AppointmentId.ToString());
        state.Value ??= new AppointmentState { CreatedOn = DateTime.UtcNow };

        state.Value.UpdatedOn = DateTime.UtcNow;
        state.Value.Appointment = appointment;
        await state.SaveAsync();

        return appointment;
    }

}