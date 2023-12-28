namespace Dapr.Appointment.Models;

public class ScheduleAppointment
{
    public Guid? AppointmentId { get; set; }
    public DateTime AppointmentDateTime { get; set; }
    public bool Closed { get; set; } = false;
    public Guid? PatientId { get; set; }
    public string? PatientFirstName { get; set; }
    public string? PatientLastName { get; set; }
}