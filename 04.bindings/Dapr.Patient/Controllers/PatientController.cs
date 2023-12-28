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
        var res = await daprClient.InvokeMethodWithResponseAsync(req);

        var content = await res.Content.ReadAsStreamAsync();

        using var ms = new MemoryStream();
        await content.CopyToAsync(ms);

        var metadata = new Dictionary<string, string>
            {
                { "blobName", $"{patient.Id}.png" }
            };
        await daprClient.InvokeBindingAsync("blobstorage", "create", data: Convert.ToBase64String(ms.ToArray()), metadata: metadata);

        return patient;
    }

}