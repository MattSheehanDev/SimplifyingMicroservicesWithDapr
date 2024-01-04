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

        var state = await _daprClient.GetStateEntryAsync<AppointmentState>(Constants.StateStore, appointment.AppointmentId.ToString());
        state.Value ??= new AppointmentState { CreatedOn = DateTime.UtcNow };

        state.Value.UpdatedOn = DateTime.UtcNow;
        state.Value.Appointment = appointment;
        await state.SaveAsync();

        return appointment;
    }

}
