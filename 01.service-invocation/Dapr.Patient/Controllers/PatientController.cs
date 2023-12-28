using Dapr.Client;
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

        // Generate patient placeholder image
        var req = daprClient.CreateInvokeMethodRequest(HttpMethod.Get, "robohash", patient.Id.ToString());
        _ = await daprClient.InvokeMethodWithResponseAsync(req);

        return patient;
    }

}