using Dapr.Actors.Runtime;
using Dapr.AppointmentActor.Models;
using Dapr.Client;
using Dapr.Common;
using Dapr.Patient.Dto;

namespace Dapr.AppointmentActor;

public class AppointmentActor : Actor, IAppointmentActor
{
    private readonly DaprClient _daprClient;

    public AppointmentActor(ActorHost host, DaprClient daprClient)
        : base(host)
    {
        _daprClient = daprClient;
    }

    public async Task<ScheduleAppointment> ScheduleAppointment(ScheduleAppointment appointment)
    {
        var patient = await _daprClient.InvokeMethodAsync<PatientDto, PatientDto>(HttpMethod.Post, "patient-service", "patient", new PatientDto { Id = appointment.PatientId, FirstName = appointment.PatientFirstName, LastName = appointment.PatientLastName });

        appointment.PatientId = patient.Id;
        appointment.AppointmentId ??= Guid.NewGuid();

        var state = await StateManager.TryGetStateAsync<AppointmentState>(appointment.AppointmentId.ToString());
        
        var apptState = state.HasValue ? state.Value : new AppointmentState { CreatedOn = DateTime.UtcNow };
        apptState.UpdatedOn = DateTime.UtcNow;
        apptState.Appointment = appointment;
        
        await StateManager.SetStateAsync<AppointmentState>(appointment.AppointmentId.ToString(), apptState);

        return appointment;
    }

}
