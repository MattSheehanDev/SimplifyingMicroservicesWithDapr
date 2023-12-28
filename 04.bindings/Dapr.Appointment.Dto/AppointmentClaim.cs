namespace Dapr.Appointment.Dto;

public class AppointmentClaim
{
    public Guid? PatientId { get; set; }
    public Guid? AppointmentId { get; set; }
    public decimal ClaimAmount { get; set; }

}
