namespace Dapr.Patient.Dto;

public class PatientState
{
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public PatientDto? Patient { get; set; }
}