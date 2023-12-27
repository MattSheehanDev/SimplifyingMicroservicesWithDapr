using Dapr.Appointment.Dto;

namespace Dapr.Balance;

public class BalanceState
{
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public Guid? PatientId { get; set; }
    public List<AppointmentClaim> Claims { get; set; } = new();
}
