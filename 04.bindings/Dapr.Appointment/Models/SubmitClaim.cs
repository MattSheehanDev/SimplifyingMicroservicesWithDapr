namespace Dapr.Appointment;

public class SubmitClaim
{
    public Guid? AppointmentId { get; set; }
    public decimal Amount { get; set; }
}
