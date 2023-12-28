using Dapr.Appointment.Models;
using Dapr.Appointment.State;
using Dapr.Client;
using Dapr.Patient.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Dapr.Appointment.Controllers;

[ApiController]
public class AppointmentController : ControllerBase
{

    [HttpPost("v1/schedule")]
    public async Task<ActionResult<AppointmentState>> ScheduleAppointmentV1(ScheduleAppointment appointment, [FromServices] DaprClient daprClient)
    {
        // Get/Create patient
        var patient = await daprClient.InvokeMethodAsync<PatientDto, PatientDto>(HttpMethod.Post, "patient-service", "patient", new PatientDto { Id = appointment.PatientId, FirstName = appointment.PatientFirstName, LastName = appointment.PatientLastName });

        appointment.PatientId = patient.Id;
        appointment.AppointmentId ??= Guid.NewGuid();

        var state = new AppointmentState {
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            Appointment = appointment
        };
        return state;
    }

    [HttpPost("v2/schedule")]
    public async Task<ActionResult<AppointmentState>> ScheduleAppointmentV2(ScheduleAppointment appointment, [FromServices] DaprClient daprClient)
    {
        var data = new Patient_Grpc.Generated.Item { Id = appointment.PatientId.HasValue ? appointment.PatientId.ToString() : string.Empty, FirstName = appointment.PatientFirstName, LastName = appointment.PatientLastName };
        var patient = await daprClient.InvokeMethodGrpcAsync<Patient_Grpc.Generated.Item, Patient_Grpc.Generated.Item>("patient-service", "patient", data);

        appointment.PatientId = Guid.Parse(patient.Id);
        appointment.AppointmentId ??= Guid.NewGuid();

        var state = new AppointmentState {
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            Appointment = appointment
        };
        return state;
    }

}