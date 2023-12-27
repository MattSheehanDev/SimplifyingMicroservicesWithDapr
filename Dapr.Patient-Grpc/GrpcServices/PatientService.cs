using Dapr.AppCallback.Autogen.Grpc.v1;
using Dapr.Client;
using Dapr.Client.Autogen.Grpc.v1;
using Dapr.Common;
using Dapr.Patient.Dto;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Patient_Grpc;

public class PatientService : AppCallback.AppCallbackBase
{
    private readonly ILogger<PatientService> _logger;
    private readonly DaprClient _daprClient;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="daprClient"></param>
    /// <param name="logger"></param>
    public PatientService(DaprClient daprClient, ILogger<PatientService> logger)
    {
        _daprClient = daprClient;
        _logger = logger;
    }

    public override async Task<InvokeResponse> OnInvoke(InvokeRequest request, ServerCallContext context)
    {
        Console.WriteLine($"Method {request.Method}");

        var response = new InvokeResponse();
        switch (request.Method)
        {
            case "patient":
                var patientReq = request.Data.Unpack<Dapr.Patient_Grpc.Generated.Item>();
                patientReq.Id = string.IsNullOrEmpty(patientReq.Id) ? Guid.NewGuid().ToString() : patientReq.Id;

                var state = await _daprClient.GetStateEntryAsync<PatientState>(Constants.StateStore, patientReq.Id.ToString());
                state.Value ??= new PatientState { CreatedOn = DateTime.UtcNow };

                state.Value.UpdatedOn = DateTime.UtcNow;
                state.Value.Patient = new PatientDto
                {
                    FirstName = patientReq.FirstName,
                    LastName = patientReq.LastName,
                    Id = Guid.Parse(patientReq.Id)
                };
                await state.SaveAsync();

                var output = await Task.FromResult<Dapr.Patient_Grpc.Generated.Item>(new Dapr.Patient_Grpc.Generated.Item { FirstName = patientReq.FirstName, LastName = patientReq.LastName, Id = patientReq.Id });

                response.Data = Any.Pack(output);
                break;
            default:
                Console.WriteLine("Method not supported");
                break;
        }
        return response;
    }

    /// <inheritdoc/>
    public override Task<ListInputBindingsResponse> ListInputBindings(Empty request, ServerCallContext context)
    {
        return Task.FromResult(new ListInputBindingsResponse());
    }

    /// <inheritdoc/>
    public override Task<ListTopicSubscriptionsResponse> ListTopicSubscriptions(Empty request, ServerCallContext context)
    {
        return Task.FromResult(new ListTopicSubscriptionsResponse());
    }

    public override Task<TopicEventResponse> OnTopicEvent(TopicEventRequest request, ServerCallContext context)
    {
        return Task.FromResult(new TopicEventResponse());
    }

    public override Task<BindingEventResponse> OnBindingEvent(BindingEventRequest request, ServerCallContext context)
    {
        return Task.FromResult(new BindingEventResponse());
    }
}