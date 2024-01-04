namespace Dapr.AppointmentActor.Models;

public class AppointmentState
{
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public ScheduleAppointment? Appointment { get; set; }
}