using Dapr.Actors;

namespace Dapr.AppointmentActor.Models;


public interface IAppointmentActor : IActor
{
    Task<ScheduleAppointment> ScheduleAppointment(ScheduleAppointment appointment);
}