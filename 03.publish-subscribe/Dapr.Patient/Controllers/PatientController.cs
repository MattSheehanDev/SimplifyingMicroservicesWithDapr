using Dapr.Client;
using Dapr.Common;
using Dapr.Patient.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Dapr.Appointment.Controllers;

[ApiController]
public class PatientController : ControllerBase
{

    [HttpPost("patient")]
    public async Task<ActionResult<PatientDto>> Patient(PatientDto patient, [FromServices] DaprClient daprClient)
    {
        // Generate patient id if doesn't exist
        patient.Id ??= Guid.NewGuid();

        var state = await daprClient.GetStateEntryAsync<PatientState>(Constants.StateStore, patient.Id.ToString());
        state.Value ??= new PatientState { CreatedOn = DateTime.UtcNow };

        state.Value.UpdatedOn = DateTime.UtcNow;
        state.Value.Patient = patient;
        await state.SaveAsync();

        // Generate patient placeholder image
        var req = daprClient.CreateInvokeMethodRequest(HttpMethod.Get, "robohash", patient.Id.ToString());
        _ = await daprClient.InvokeMethodWithResponseAsync(req);

        return patient;
    }

}