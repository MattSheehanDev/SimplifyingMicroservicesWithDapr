using Dapr.Appointment.Models;

namespace Dapr.Appointment.State;

public class AppointmentState
{
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public ScheduleAppointment? Appointment { get; set; }
}